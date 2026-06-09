param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$ProjectDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$PublishDir = Join-Path $ProjectDir "publish"

dotnet publish (Join-Path $ProjectDir "DashboardZenBridge.csproj") `
    -c $Configuration `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=false `
    -o $PublishDir

if ($LASTEXITCODE -ne 0) {
    throw "Bridge helper publish failed."
}

Write-Host "Bridge helper published to: $PublishDir"
