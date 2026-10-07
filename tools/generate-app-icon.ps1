# Original two-card mark. Rebuilds PNG assets from geometry using Windows GDI+.
param([string]$OutputDirectory = (Join-Path $PSScriptRoot '../game/Assets/WordDeduction/UI/Artwork'))
Add-Type -AssemblyName System.Drawing
[IO.Directory]::CreateDirectory([IO.Path]::GetFullPath($OutputDirectory)) | Out-Null
$bitmap = [Drawing.Bitmap]::new(1024,1024)
$graphics = [Drawing.Graphics]::FromImage($bitmap)
$graphics.SmoothingMode = [Drawing.Drawing2D.SmoothingMode]::AntiAlias
$graphics.TextRenderingHint = [Drawing.Text.TextRenderingHint]::AntiAliasGridFit
$paper = [Drawing.ColorTranslator]::FromHtml('#FAF7F0')
$graphics.Clear($paper)
function Card([single]$x,[single]$y,[single]$width,[single]$height,[single]$rotation,[string]$color,[bool]$question) {
    $state=$graphics.Save()
    $graphics.TranslateTransform($x+$width/2,$y+$height/2)
    $graphics.RotateTransform($rotation)
    $path=[Drawing.Drawing2D.GraphicsPath]::new()
    $left=-$width/2; $top=-$height/2; $diameter=112
    $path.AddArc($left,$top,$diameter,$diameter,180,90)
    $path.AddArc($left+$width-$diameter,$top,$diameter,$diameter,270,90)
    $path.AddArc($left+$width-$diameter,$top+$height-$diameter,$diameter,$diameter,0,90)
    $path.AddArc($left,$top+$height-$diameter,$diameter,$diameter,90,90)
    $path.CloseFigure()
    $brush=[Drawing.SolidBrush]::new([Drawing.ColorTranslator]::FromHtml($color))
    $graphics.FillPath($brush,$path)
    if($question) {
        $font=[Drawing.Font]::new('Arial',250,[Drawing.FontStyle]::Bold,[Drawing.GraphicsUnit]::Pixel)
        $format=[Drawing.StringFormat]::new(); $format.Alignment=[Drawing.StringAlignment]::Center; $format.LineAlignment=[Drawing.StringAlignment]::Center
        $graphics.DrawString('?',$font,[Drawing.Brushes]::White,[Drawing.RectangleF]::new($left,$top-10,$width,$height),$format)
        $font.Dispose(); $format.Dispose()
    }
    $path.Dispose(); $brush.Dispose(); $graphics.Restore($state)
}
Card 216 204 330 440 -13 '#B5CDB3' $false
Card 402 316 330 440 10 '#6A48C8' $true
$bitmap.Save((Join-Path $OutputDirectory 'AppIcon.png'),[Drawing.Imaging.ImageFormat]::Png)
$graphics.Dispose(); $bitmap.Dispose()