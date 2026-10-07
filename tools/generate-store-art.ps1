$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$releaseArtRoot = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'generate-app-icon.ps1') -OutputDirectory (Join-Path $releaseArtRoot 'docs/release/store') -Size 512
$releaseFonts=[Drawing.Text.PrivateFontCollection]::new()
$releaseFonts.AddFontFile("$releaseArtRoot/game/Assets/WordDeduction/UI/Fonts/Inter-Regular.ttf")
function RoundCard($g,[single]$cx,[single]$cy,[single]$w,[single]$h,[single]$rotation,$color,$question) {
    $state=$g.Save();$g.TranslateTransform($cx,$cy);$g.RotateTransform($rotation)
    $path=[Drawing.Drawing2D.GraphicsPath]::new();$x=-$w/2;$y=-$h/2;$d=56
    $path.AddArc($x,$y,$d,$d,180,90);$path.AddArc($x+$w-$d,$y,$d,$d,270,90)
    $path.AddArc($x+$w-$d,$y+$h-$d,$d,$d,0,90);$path.AddArc($x,$y+$h-$d,$d,$d,90,90);$path.CloseFigure()
    $brush=[Drawing.SolidBrush]::new([Drawing.ColorTranslator]::FromHtml($color));$g.FillPath($brush,$path)
    if($question) {
        $font=[Drawing.Font]::new($releaseFonts.Families[0],156,[Drawing.FontStyle]::Regular,[Drawing.GraphicsUnit]::Pixel)
        $format=[Drawing.StringFormat]::new();$format.Alignment='Center';$format.LineAlignment='Center'
        $g.DrawString('?',$font,[Drawing.Brushes]::White,[Drawing.RectangleF]::new($x,$y-8,$w,$h),$format)
        $font.Dispose();$format.Dispose()
    }
    $brush.Dispose();$path.Dispose();$g.Restore($state)
}
foreach($locale in @('en','de')) {
    $bitmap=[Drawing.Bitmap]::new(1024,500,[Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $g=[Drawing.Graphics]::FromImage($bitmap);$g.SmoothingMode='AntiAlias';$g.TextRenderingHint='AntiAliasGridFit'
    $g.Clear([Drawing.ColorTranslator]::FromHtml('#FAF7F0'))
    $ink=[Drawing.SolidBrush]::new([Drawing.ColorTranslator]::FromHtml('#262034'))
    $muted=[Drawing.SolidBrush]::new([Drawing.ColorTranslator]::FromHtml('#665D71'))
    $heading=[Drawing.Font]::new($releaseFonts.Families[0],82,[Drawing.FontStyle]::Regular,[Drawing.GraphicsUnit]::Pixel)
    $caption=[Drawing.Font]::new($releaseFonts.Families[0],27,[Drawing.FontStyle]::Regular,[Drawing.GraphicsUnit]::Pixel)
    $g.DrawString("Word`nDeduction",$heading,$ink,[Drawing.PointF]::new(70,94))
    $text=if($locale -eq 'de'){'Ein Handy. Eure Runde.'}else{"One phone. Everyone's in."}
    $g.DrawString($text,$caption,$muted,[Drawing.PointF]::new(75,316))
    RoundCard $g 702 219 224 286 -14 '#B5CDB3' $false
    RoundCard $g 803 280 224 286 10 '#6A48C8' $true
    $accent=[Drawing.SolidBrush]::new([Drawing.ColorTranslator]::FromHtml('#D99B86'))
    $g.FillEllipse($accent,875,71,18,18);$g.FillEllipse($accent,603,392,10,10)
    $bitmap.Save("$releaseArtRoot/docs/release/store/feature-$locale.png",[Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose();$bitmap.Dispose();$ink.Dispose();$muted.Dispose();$heading.Dispose();$caption.Dispose();$accent.Dispose()
}
$releaseFonts.Dispose()
