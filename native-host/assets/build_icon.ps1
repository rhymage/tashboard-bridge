param(
    [string]$OutputDirectory = $PSScriptRoot
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$pngPath = Join-Path $OutputDirectory "tashboard-bridge-256.png"
$icoPath = Join-Path $OutputDirectory "tashboard-bridge.ico"

$size = 256
$bitmap = New-Object System.Drawing.Bitmap($size, $size)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$graphics.Clear([System.Drawing.Color]::Transparent)

$background = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
    (New-Object System.Drawing.Point(32, 20)),
    (New-Object System.Drawing.Point(224, 236)),
    [System.Drawing.Color]::FromArgb(31, 41, 55),
    [System.Drawing.Color]::FromArgb(17, 24, 39)
)
$outline = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(51, 65, 85), 6)
$accentLeft = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(94, 234, 212))
$accentRight = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(59, 130, 246))
$white = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(248, 250, 252))
$accentPen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(56, 189, 248), 8)
$bridgePen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(82, 180, 230), 16)
$bridgePen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
$bridgePen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round

$path = New-Object System.Drawing.Drawing2D.GraphicsPath
$radius = 56
$path.AddArc(3, 3, $radius, $radius, 180, 90)
$path.AddArc($size - $radius - 3, 3, $radius, $radius, 270, 90)
$path.AddArc($size - $radius - 3, $size - $radius - 3, $radius, $radius, 0, 90)
$path.AddArc(3, $size - $radius - 3, $radius, $radius, 90, 90)
$path.CloseFigure()
$graphics.FillPath($background, $path)
$graphics.DrawPath($outline, $path)

$tPath = New-Object System.Drawing.Drawing2D.GraphicsPath
$tPath.AddPolygon([System.Drawing.Point[]]@(
    (New-Object System.Drawing.Point(56, 48)),
    (New-Object System.Drawing.Point(200, 48)),
    (New-Object System.Drawing.Point(200, 86)),
    (New-Object System.Drawing.Point(150, 86)),
    (New-Object System.Drawing.Point(150, 160)),
    (New-Object System.Drawing.Point(106, 160)),
    (New-Object System.Drawing.Point(106, 86)),
    (New-Object System.Drawing.Point(56, 86))
))
$graphics.FillPath($white, $tPath)

$graphics.DrawLine($bridgePen, 50, 190, 206, 190)
$graphics.FillEllipse($accentLeft, 34, 174, 32, 32)
$graphics.FillEllipse($white, 108, 170, 40, 40)
$graphics.DrawEllipse($accentPen, 108, 170, 40, 40)
$graphics.FillEllipse($accentRight, 190, 174, 32, 32)

$bitmap.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)
$graphics.Dispose()
$bitmap.Dispose()

$pngBytes = [System.IO.File]::ReadAllBytes($pngPath)
$stream = [System.IO.File]::Create($icoPath)
$writer = New-Object System.IO.BinaryWriter($stream)
try {
    $writer.Write([uint16]0)
    $writer.Write([uint16]1)
    $writer.Write([uint16]1)
    $writer.Write([byte]0)
    $writer.Write([byte]0)
    $writer.Write([byte]0)
    $writer.Write([byte]0)
    $writer.Write([uint16]1)
    $writer.Write([uint16]32)
    $writer.Write([uint32]$pngBytes.Length)
    $writer.Write([uint32]22)
    $writer.Write($pngBytes)
} finally {
    $writer.Dispose()
    $stream.Dispose()
}

Write-Host "Bridge icon ready: $icoPath"
