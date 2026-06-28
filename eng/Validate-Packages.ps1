param(
    [Parameter(Mandatory = $true)]
    [string] $ArtifactsPath,

    [Parameter(Mandatory = $true)]
    [string] $Version
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression.FileSystem

function Assert-Package {
    param(
        [string] $PackageId,
        [string[]] $RequiredPatterns
    )

    $packagePath = Join-Path $ArtifactsPath "$PackageId.$Version.nupkg"
    if (-not (Test-Path -LiteralPath $packagePath)) {
        throw "Pacote esperado não encontrado: $packagePath"
    }

    $archive = [System.IO.Compression.ZipFile]::OpenRead($packagePath)
    try {
        $entries = @($archive.Entries | ForEach-Object FullName)
        foreach ($pattern in $RequiredPatterns) {
            if (-not ($entries | Where-Object { $_ -like $pattern })) {
                throw "O pacote $PackageId não contém uma entrada compatível com '$pattern'."
            }
        }

        $nuspecEntry = $archive.Entries | Where-Object FullName -eq "$PackageId.nuspec"
        if ($null -eq $nuspecEntry) {
            throw "Nuspec não encontrado no pacote $PackageId."
        }

        $reader = [System.IO.StreamReader]::new($nuspecEntry.Open())
        try {
            [xml] $nuspec = $reader.ReadToEnd()
        }
        finally {
            $reader.Dispose()
        }

        if ($nuspec.package.metadata.id -ne $PackageId) {
            throw "PackageId inválido em ${PackageId}: $($nuspec.package.metadata.id)"
        }
        if ($nuspec.package.metadata.version -ne $Version) {
            throw "Versão inválida em ${PackageId}: $($nuspec.package.metadata.version)"
        }
    }
    finally {
        $archive.Dispose()
    }
}

Assert-Package -PackageId 'NFEDanfe' -RequiredPatterns @(
    'lib/net8.0/NFEDanfe.dll',
    'lib/net10.0/NFEDanfe.dll',
    'README.md',
    'logo.png',
    'contentFiles/any/any/fonts/*.ttf'
)

Assert-Package -PackageId 'NFEDanfe.Cli' -RequiredPatterns @(
    'tools/*/any/NFEDanfe.Cli.dll',
    'README.md',
    'logo-200.png'
)

Write-Host "Pacotes NFEDanfe $Version validados com sucesso."
