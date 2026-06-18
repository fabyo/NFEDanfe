$timestamp = Get-Date -Format 'yyyyMMdd_HHmmss'
$has7z = $null -ne (Get-Command 7z -ErrorAction SilentlyContinue)

if ($has7z) {
    $backupFile = "NFEDanfe_backup_$timestamp.7z"
    Write-Host "Compactando arquivos usando 7-Zip para $backupFile..." -ForegroundColor Yellow
    
    # Executa o 7z diretamente com exclusões recursivas sem criar pasta temporária no disco
    & 7z a "$backupFile" * "-xr!.git" "-xr!.github" "-xr!out" "-xr!samples" "-xr!artifacts" "-xr!bin" "-xr!obj" "-xr!TestResults" "-xr!*.zip" "-xr!*.7z" "-xr!.vs" "-xr!.gemini" -mx9 -bd | Out-Null
} else {
    $backupFile = "NFEDanfe_backup_$timestamp.zip"
    Write-Host "7-Zip não encontrado. Compactando usando .NET ZipArchive para $backupFile..." -ForegroundColor Yellow
    
    # Coleta arquivos do projeto respeitando exclusões
    $excludePatterns = @(
        '\\\.git(\\|$)',
        '\\\.github(\\|$)',
        '\\\.vs(\\|$)',
        '\\bin(\\|$)',
        '\\obj(\\|$)',
        '\\out(\\|$)',
        '\\\.gemini(\\|$)',
        '\\TestResults(\\|$)',
        '\\samples(\\|$)',
        '\\artifacts(\\|$)'
    )

    $filesToBackup = Get-ChildItem -Path . -Recurse | Where-Object {
        $path = $_.FullName
        $exclude = $false
        foreach ($pattern in $excludePatterns) {
            if ($path -match $pattern) {
                $exclude = $true
                break
            }
        }
        !$exclude -and !$_.PSIsContainer -and ($_.Name -notmatch '\.(zip|7z)$')
    }

    # Carrega assembly de compressão do .NET e compacta em fluxo de memória/arquivo
    Add-Type -AssemblyName System.IO.Compression

    $zipStream = [System.IO.File]::Create((Join-Path $pwd.Path $backupFile))
    $archive = New-Object System.IO.Compression.ZipArchive($zipStream, [System.IO.Compression.ZipArchiveMode]::Create)

    foreach ($file in $filesToBackup) {
        $relativePath = Resolve-Path -Path $file.FullName -Relative
        $relativePath = $relativePath -replace '^\.\\', ''
        $zipEntryPath = $relativePath -replace '\\', '/'
        
        $entry = $archive.CreateEntry($zipEntryPath, [System.IO.Compression.CompressionLevel]::Optimal)
        $entryStream = $entry.Open()
        $fileStream = [System.IO.File]::OpenRead($file.FullName)
        $fileStream.CopyTo($entryStream)
        
        $fileStream.Close()
        $entryStream.Close()
    }

    $archive.Dispose()
    $zipStream.Close()
}

Write-Host "Backup criado com sucesso: $backupFile" -ForegroundColor Green
