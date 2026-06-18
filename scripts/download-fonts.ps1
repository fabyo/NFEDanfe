$fontsDir = Join-Path $PSScriptRoot "..\fonts"
if (-not (Test-Path $fontsDir)) {
    New-Item -ItemType Directory -Path $fontsDir | Out-Null
}

$tempDir = Join-Path $PSScriptRoot "temp_fonts"
if (Test-Path $tempDir) {
    Remove-Item -Recurse -Force $tempDir
}
New-Item -ItemType Directory -Path $tempDir | Out-Null

$fontsToDownload = @{
    "Inter" = "https://fonts.google.com/download?family=Inter"
    "Roboto" = "https://fonts.google.com/download?family=Roboto"
    "IBMPlexSans" = "https://fonts.google.com/download?family=IBM%20Plex%20Sans"
}

foreach ($fontName in $fontsToDownload.Keys) {
    $url = $fontsToDownload[$fontName]
    Write-Host "Baixando $fontName de $url..."
    
    $zipPath = Join-Path $tempDir "$fontName.zip"
    $extractPath = Join-Path $tempDir $fontName
    
    # Download the font zip file
    Invoke-WebRequest -Uri $url -OutFile $zipPath
    
    # Extract the zip file
    Expand-Archive -Path $zipPath -DestinationPath $extractPath -Force
    
    # Search for any Regular.ttf file (or fallback to any .ttf)
    $ttfFile = Get-ChildItem -Path $extractPath -Filter "*Regular.ttf" -Recurse | Select-Object -First 1
    if (-not $ttfFile) {
        $ttfFile = Get-ChildItem -Path $extractPath -Filter "*.ttf" -Recurse | Select-Object -First 1
    }
    
    if ($ttfFile) {
        $destFile = Join-Path $fontsDir "$fontName-Regular.ttf"
        Copy-Item -Path $ttfFile.FullName -Destination $destFile -Force
        Write-Host "Salvo $fontName em $destFile"
    } else {
        Write-Error "Não foi possível encontrar arquivo .ttf para $fontName"
    }
}

# Clean up temp files
Remove-Item -Recurse -Force $tempDir
Write-Host "Downloads concluídos com sucesso!"
