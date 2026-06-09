param(
    [Parameter(Mandatory = $true)]
    [string]$ExtensionId,

    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$NativeHostDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $NativeHostDir
$InstallerDir = Join-Path $NativeHostDir "installer"
$PayloadDir = Join-Path $InstallerDir "payload"
$PublishDir = Join-Path $NativeHostDir "publish"
$DistDir = Join-Path $RepoRoot "dist"
$PayloadZip = Join-Path $PayloadDir "bridge-payload.zip"
$StagingDir = Join-Path ([System.IO.Path]::GetTempPath()) ("dashboard-zen-bridge-payload-" + [System.Guid]::NewGuid().ToString("N"))

if ($ExtensionId -notmatch '^[a-p]{32}$') {
    Write-Warning "ExtensionId does not look like a Chrome extension ID: $ExtensionId"
}

& (Join-Path $NativeHostDir "build_bridge.ps1") -Configuration $Configuration

if (-not (Test-Path -LiteralPath (Join-Path $PublishDir "DashboardZenBridge.exe"))) {
    throw "Bridge publish output is missing DashboardZenBridge.exe"
}

New-Item -ItemType Directory -Force -Path $PayloadDir, $DistDir | Out-Null
if (Test-Path -LiteralPath $PayloadZip) {
    Remove-Item -LiteralPath $PayloadZip -Force
}

Start-Sleep -Milliseconds 500
New-Item -ItemType Directory -Force -Path $StagingDir | Out-Null
try {
    $copied = $false
    for ($attempt = 1; $attempt -le 5 -and -not $copied; $attempt++) {
        try {
            Copy-Item -Path (Join-Path $PublishDir "*") -Destination $StagingDir -Recurse -Force
            $copied = $true
        } catch {
            if ($attempt -eq 5) { throw }
            Start-Sleep -Milliseconds (500 * $attempt)
        }
    }

    Compress-Archive -Path (Join-Path $StagingDir "*") -DestinationPath $PayloadZip -Force
} finally {
    if (Test-Path -LiteralPath $StagingDir) {
        Remove-Item -LiteralPath $StagingDir -Recurse -Force
    }
}

$InstallerPublish = Join-Path $InstallerDir "publish"
dotnet publish (Join-Path $InstallerDir "DashboardZenBridgeSetup.csproj") `
    -c $Configuration `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -p:BridgeExtensionId=$ExtensionId `
    -o $InstallerPublish

if ($LASTEXITCODE -ne 0) {
    throw "Setup installer publish failed."
}

$SetupExe = Join-Path $InstallerPublish "TashboardBridgeSetup.exe"
if (-not (Test-Path -LiteralPath $SetupExe)) {
    throw "Installer publish did not produce TashboardBridgeSetup.exe"
}

Copy-Item -LiteralPath $SetupExe -Destination (Join-Path $DistDir "TashboardBridgeSetup.exe") -Force
Write-Host "Setup ready: $(Join-Path $DistDir "TashboardBridgeSetup.exe")"
