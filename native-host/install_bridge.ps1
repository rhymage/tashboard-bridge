param(
    [Parameter(Mandatory = $true)]
    [string]$ExtensionId,

    [string]$HostName = "com.dashboard_zen.bridge",
    [string]$ExePath = ""
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

if ([string]::IsNullOrWhiteSpace($ExePath)) {
    $ExePath = Join-Path $ScriptDir "publish\DashboardZenBridge.exe"
}

if (-not (Test-Path -LiteralPath $ExePath)) {
    throw "Bridge executable not found: $ExePath. Run native-host\build_bridge.ps1 first."
}

$ManifestPath = Join-Path $ScriptDir "$HostName.json"
$Manifest = [ordered]@{
    name = $HostName
    description = "TASHBOARD Native Messaging Bridge"
    path = (Resolve-Path -LiteralPath $ExePath).Path
    type = "stdio"
    allowed_origins = @("chrome-extension://$ExtensionId/")
}

$ManifestJson = $Manifest | ConvertTo-Json -Depth 5
$Utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText($ManifestPath, $ManifestJson, $Utf8NoBom)

$RegPath = "HKCU:\Software\Google\Chrome\NativeMessagingHosts\$HostName"
New-Item -Path $RegPath -Force | Out-Null
Set-Item -Path $RegPath -Value $ManifestPath

Write-Host "TASHBOARD bridge registered."
Write-Host "Host: $HostName"
Write-Host "Manifest: $ManifestPath"
Write-Host "Extension: chrome-extension://$ExtensionId/"
