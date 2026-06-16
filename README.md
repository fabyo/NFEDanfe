# NFEDanfe

Biblioteca .NET para gerar DANFE em PDF a partir de XML de NF-e autorizada.

<img src="logo.png" alt="Gordon Watcher Logo" width="200"/>

O projeto tem dois formatos de uso:

- `NFEDanfe`: biblioteca para integração em sistemas .NET, APIs, workers e ERPs.
- `NFEDanfe.Cli`: ferramenta de linha de comando para uso operacional e scripts.

## Recursos

- Compatível com .NET 8 e .NET 10.
- Geração de DANFE em PDF com QuestPDF.
- DANFE em modo retrato e paisagem.
- Seleção automática pelo campo `tpImp` do XML NF-e.
- Override manual de orientação via `DanfeOptions.TipoImpressaoOverride`.
- Parser seguro de XML NF-e com DTD proibido.
- Validação fail-fast de domínio.
- Validação de CPF, CNPJ numérico e CNPJ alfanumérico.
- Validação de chave de acesso NF-e, incluindo modelo `55` e dígito verificador.
- Validação de consistência de totais de produtos, descontos e valor da nota.
- Paginação real no DANFE via QuestPDF.
- API pública simples com `DanfeGenerator` e `DanfeOptions`.
- Snapshot textual para regressão funcional.

## Instalação Como Biblioteca

Quando publicado no NuGet:

```powershell
dotnet add package NFEDanfe
```

Uso básico:

```csharp
using NFEDanfe;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

await using FileStream output = File.Create("danfe.pdf");
DanfeGenerator.GenerateFromXml("nota-procNFe.xml", output);
```

Com logo:

```csharp
using NFEDanfe;

byte[] logo = await File.ReadAllBytesAsync("logo.png");

DanfeOptions options = new()
{
    LogoBytes = logo
};

await using FileStream output = File.Create("danfe.pdf");
DanfeGenerator.GenerateFromXml("nota-procNFe.xml", output, options);
```

Forçar modo paisagem:

```csharp
using NFEDanfe;

DanfeOptions options = new()
{
    TipoImpressaoOverride = 2 // 1 = retrato, 2 = paisagem
};

await using FileStream output = File.Create("danfe-paisagem.pdf");
DanfeGenerator.GenerateFromXml("nota-procNFe.xml", output, options);
```

Forçar modo retrato:

```csharp
using NFEDanfe;

DanfeOptions options = new()
{
    TipoImpressaoOverride = 1 // 1 = retrato, 2 = paisagem
};

await using FileStream output = File.Create("danfe-retrato.pdf");
DanfeGenerator.GenerateFromXml("nota-procNFe.xml", output, options);
```

Carregar modelo e gerar snapshot textual:

```csharp
using NFEDanfe;

var model = DanfeGenerator.LoadFromXml("nota-procNFe.xml");
string snapshot = DanfeSnapshot.CreateText(model);
```

## Instalação Como CLI

Durante desenvolvimento:

```powershell
dotnet run --project .\NFEDanfe.Cli\NFEDanfe.Cli.csproj -- .\samples\nota-exemplo-procNFe.xml
```

Como ferramenta local a partir do pacote gerado:

```powershell
dotnet pack .\NFEDanfe.Cli\NFEDanfe.Cli.csproj -c Release
dotnet tool install --global --add-source .\NFEDanfe.Cli\bin\Release NFEDanfe.Cli
```

Depois de instalada:

```powershell
nfedanfe .\samples\nota-exemplo-procNFe.xml --output .\out
```

Gerar com logo por caminho explícito:

```powershell
nfedanfe .\nota-procNFe.xml --logo-path .\minha-logo.png --output .\out
```

Gerar com busca automática por `logo.png`:

```powershell
nfedanfe .\nota-procNFe.xml --logo
```

Gerar snapshot textual junto com o PDF:

```powershell
nfedanfe .\nota-procNFe.xml --snapshot
```

Gerar DANFE mock de demonstração:

```powershell
nfedanfe --mock
```

## Logo Na CLI

A opção recomendada é `--logo-path`, porque é explícita:

```powershell
nfedanfe .\nota-procNFe.xml --logo-path .\assets\logo.png
```

A opção `--logo` também existe e procura automaticamente um arquivo chamado `logo.png`.

Locais verificados pela CLI:

- Diretório onde o comando foi executado.
- Diretório do binário da ferramenta.
- Diretórios pais desses caminhos, subindo alguns níveis.

Se o arquivo não for encontrado, o DANFE é gerado sem logo.

## Linux e Docker

O projeto é compatível com Linux porque usa .NET, QuestPDF e Barcoder sem `System.Drawing`.

Exemplo Linux:

```bash
dotnet run --project ./NFEDanfe.Cli/NFEDanfe.Cli.csproj -- ./samples/nota-exemplo-procNFe.xml --output ./out
```

Exemplo de publicação:

```bash
dotnet publish ./NFEDanfe.Cli/NFEDanfe.Cli.csproj -c Release -o ./publish
dotnet ./publish/NFEDanfe.Cli.dll ./samples/nota-exemplo-procNFe.xml --output ./out
```

## Samples

A pasta `samples/` contém XML público sanitizado para demonstração.

A pasta `xml_testes/` é ignorada pelo Git e deve ser usada apenas para XMLs fiscais reais locais.

## QuestPDF License

Este projeto usa QuestPDF. Antes de usar em produção, valide o tipo de licença exigido pelo QuestPDF para o seu cenário de uso.

No exemplo/CLI é usada a configuração:

```csharp
QuestPDF.Settings.License = LicenseType.Community;
```

## Empacotar

Gerar pacote NuGet da biblioteca:

```powershell
dotnet pack .\NFEDanfe\NFEDanfe.csproj -c Release
```

Gerar pacote da CLI como `dotnet tool`:

```powershell
dotnet pack .\NFEDanfe.Cli\NFEDanfe.Cli.csproj -c Release
```

## Estrutura

- `NFEDanfe`: biblioteca reutilizável.
- `NFEDanfe.Cli`: CLI e exemplo real de consumo.
- `samples`: exemplos públicos sanitizados.
- `Domain/Parser`: parser XML seguro.
- `Domain/Validation`: validações fiscais e de domínio.
- `Layout`: documentos QuestPDF.
- `Layout/Components`: blocos visuais do DANFE.

## 👨‍💻 Autor

Fabyo Guimarães Oliveira

- LinkedIn: [https://www.linkedin.com/in/fabyo-guimaraes/](https://www.linkedin.com/in/fabyo-guimaraes/)
- GitHub: https://github.com/fabyo

## Licença

MIT.
