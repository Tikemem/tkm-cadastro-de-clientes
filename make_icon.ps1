Add-Type -AssemblyName System.Drawing

$assetsDir = Join-Path $PSScriptRoot "Assets"
if (!(Test-Path $assetsDir)) {
    New-Item -ItemType Directory -Path $assetsDir -Force | Out-Null
}

function Create-TkmBitmap([int]$size) {
    $bmp = New-Object System.Drawing.Bitmap($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)

    # Cores
    $navy = [System.Drawing.Color]::FromArgb(255, 13, 27, 42)
    $blue = [System.Drawing.Color]::FromArgb(255, 37, 99, 235)
    $lightBlue = [System.Drawing.Color]::FromArgb(255, 96, 165, 250)
    $white = [System.Drawing.Color]::FromArgb(255, 248, 250, 252)

    # Fundo arredondado
    $margin = [Math]::Max(1, [int]($size * 0.04))
    $boxSize = $size - (2 * $margin)
    $radius = [int]($size * 0.22)
    $rect = New-Object System.Drawing.Rectangle($margin, $margin, $boxSize, $boxSize)

    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $diameter = $radius * 2
    $path.AddArc($rect.X, $rect.Y, $diameter, $diameter, 180, 90)
    $path.AddArc($rect.Right - $diameter, $rect.Y, $diameter, $diameter, 270, 90)
    $path.AddArc($rect.Right - $diameter, $rect.Bottom - $diameter, $diameter, $diameter, 0, 90)
    $path.AddArc($rect.X, $rect.Bottom - $diameter, $diameter, $diameter, 90, 90)
    $path.CloseFigure()

    $brushNavy = New-Object System.Drawing.SolidBrush($navy)
    $g.FillPath($brushNavy, $path)

    $penBlue = New-Object System.Drawing.Pen($blue, [Math]::Max(1.5, $size * 0.035))
    $g.DrawPath($penBlue, $path)

    # Tipografia "TK"
    $fontTkSize = $size * 0.42
    $fontTk = New-Object System.Drawing.Font("Segoe UI", $fontTkSize, [System.Drawing.FontStyle]::Bold)
    $brushWhite = New-Object System.Drawing.SolidBrush($white)
    $tkX = $size * 0.12
    $tkY = $size * 0.12
    $g.DrawString("TK", $fontTk, $brushWhite, $tkX, $tkY)

    # Tipografia "M"
    $fontMSize = $size * 0.28
    $fontM = New-Object System.Drawing.Font("Segoe UI", $fontMSize, [System.Drawing.FontStyle]::Bold)
    $brushLightBlue = New-Object System.Drawing.SolidBrush($lightBlue)
    $mX = $size * 0.62
    $mY = $size * 0.28
    $g.DrawString("M", $fontM, $brushLightBlue, $mX, $mY)

    # Ponto de destaque
    $dotSize = [Math]::Max(3, [int]($size * 0.11))
    $dotX = $size * 0.68
    $dotY = $size * 0.47
    $brushBlue = New-Object System.Drawing.SolidBrush($blue)
    $g.FillEllipse($brushBlue, $dotX, $dotY, $dotSize, $dotSize)

    # Barra inferior
    $barX = $size * 0.22
    $barY = $size * 0.82
    $barW = $size * 0.56
    $barH = [Math]::Max(2, [int]($size * 0.065))
    $g.FillRectangle($brushBlue, $barX, $barY, $barW, $barH)

    $g.Dispose()
    return $bmp
}

# Gera frames PNG em múltiplos tamanhos padrão Windows ICO
$sizes = @(256, 128, 64, 48, 32, 16)
$pngStreams = @()

foreach ($s in $sizes) {
    $bmp = Create-TkmBitmap -size $s
    $ms = New-Object System.IO.MemoryStream
    $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    $pngStreams += @{ Size = $s; Stream = $ms; Bytes = $ms.ToArray() }
}

# Monta o arquivo .ico binário oficial (suporta PNGs no frame)
$icoPath = Join-Path $assetsDir "app.ico"
$fs = [System.IO.File]::Create($icoPath)
$bw = New-Object System.IO.BinaryWriter($fs)

# ICONDIR Header
$bw.Write([UInt16]0) # Reserved
$bw.Write([UInt16]1) # Type 1 = Icon
$bw.Write([UInt16]$pngStreams.Count) # Count

# Header offset: 6 bytes (header) + count * 16 bytes (entries)
$offset = 6 + ($pngStreams.Count * 16)

# ICONDIRENTRY entries
foreach ($p in $pngStreams) {
    $widthByte = if ($p.Size -ge 256) { [byte]0 } else { [byte]$p.Size }
    $heightByte = if ($p.Size -ge 256) { [byte]0 } else { [byte]$p.Size }
    $bw.Write($widthByte)       # Width
    $bw.Write($heightByte)      # Height
    $bw.Write([byte]0)          # ColorCount
    $bw.Write([byte]0)          # Reserved
    $bw.Write([UInt16]1)        # Planes
    $bw.Write([UInt16]32)       # BitCount
    $bw.Write([UInt32]$p.Bytes.Length) # BytesInRes
    $bw.Write([UInt32]$offset)  # ImageOffset
    $offset += $p.Bytes.Length
}

# Image Data
foreach ($p in $pngStreams) {
    $bw.Write($p.Bytes)
    $p.Stream.Dispose()
}

$bw.Flush()
$bw.Close()
$fs.Close()

Write-Host "ICO gerado com sucesso em: $icoPath"
