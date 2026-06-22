![Logo](https://raw.githubusercontent.com/fabyo/NFEDanfe/main/logo-200.png)

[![NuGet](https://img.shields.io/nuget/v/NFEDanfe.svg)](https://www.nuget.org/packages/NFEDanfe)
[![Downloads](https://img.shields.io/nuget/dt/NFEDanfe.svg)](https://www.nuget.org/packages/NFEDanfe)
[![Build](https://github.com/fabyo/NFEDanfe/actions/workflows/ci.yml/badge.svg)](https://github.com/fabyo/NFEDanfe/actions/workflows/ci.yml)
[![Publish](https://github.com/fabyo/NFEDanfe/actions/workflows/publish.yml/badge.svg)](https://github.com/fabyo/NFEDanfe/actions/workflows/publish.yml)
[![Scorecard](https://github.com/fabyo/NFEDanfe/actions/workflows/scorecard.yml/badge.svg)](https://github.com/fabyo/NFEDanfe/actions/workflows/scorecard.yml)
[![GitHub stars](https://img.shields.io/github/stars/fabyo/NFEDanfe)](https://github.com/fabyo/NFEDanfe)
[![License](https://img.shields.io/github/license/fabyo/NFEDanfe)](https://github.com/fabyo/NFEDanfe)

Biblioteca .NET para gerar DANFE em PDF a partir de XML de NF-e autorizada.

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

Gerar a partir de conteúdo XML em memória (String / Banco de Dados):

Se você possui o conteúdo XML salvo no banco de dados como uma `string`, você pode gerá-lo diretamente em memória sem precisar criar um arquivo físico:

```csharp
using NFEDanfe;

string xmlContent = ObterXmlDoBancoDeDados(nfeId);

await using FileStream output = File.Create("danfe.pdf");
DanfeGenerator.GenerateFromXmlContent(xmlContent, output);
```


## Referência da API

### Classe `DanfeGenerator` (Estática)
Responsável por carregar o modelo de dados e gerar o arquivo PDF.

| Método | Descrição |
| --- | --- |
| `GenerateFromXml(string xmlPath, Stream output, DanfeOptions? options = null)` | Gera o DANFE em PDF a partir do caminho de um arquivo XML. |
| `GenerateFromXml(Stream xmlStream, Stream output, DanfeOptions? options = null)` | Gera o DANFE em PDF a partir de um Stream contendo o XML. |
| `GenerateFromXmlContent(string xmlContent, Stream output, DanfeOptions? options = null)` | Gera o DANFE em PDF a partir de uma string contendo o conteúdo XML cru. |
| `LoadFromXml(string xmlPath, DanfeOptions? options = null)` | Carrega e valida o modelo `DanfeModel` a partir do caminho de um arquivo XML. |
| `LoadFromXml(Stream xmlStream, DanfeOptions? options = null)` | Carrega e valida o modelo `DanfeModel` a partir de um Stream contendo o XML. |
| `LoadFromXmlContent(string xmlContent, DanfeOptions? options = null)` | Carrega e valida o modelo `DanfeModel` a partir de uma string contendo o conteúdo XML cru. |
| `Generate(DanfeModel model, Stream output, DanfeOptions? options = null)` | Gera o DANFE em PDF a partir de um objeto `DanfeModel` previamente carregado. |

### Classe `DanfeOptions`
Configurações opcionais para a geração do documento.

| Propriedade | Tipo | Descrição | Valor Padrão |
| --- | --- | --- | --- |
| `LogoBytes` | `byte[]?` | Vetor de bytes contendo o logotipo da empresa emitente (PNG ou JPEG). | `null` |
| `ValidateBeforeGenerate` | `bool` | Se `true`, valida as regras de negócio e integridade da nota antes de gerar. | `true` |
| `EmitFooter` | `bool` | Se `true`, exibe a informação de rodapé "NFEDanfe - impresso em...". | `true` |
| `TipoImpressaoOverride` | `int?` | Sobrescreve a orientação definida no XML (`1` = Retrato, `2` = Paisagem). Se `null`, respeita o XML. | `null` |
| `Font` | `DanfeFont` | Enum para escolher uma das fontes pré-configuradas (`Arial`, `Inter`, `Roboto`, `IbmPlexSans`). | `DanfeFont.Arial` |
| `CustomFontName` | `string?` | Sobrescreve o enum para utilizar o nome de qualquer fonte do sistema ou registrada sob demanda. | `null` |
| `CustomXmlEncoding` | `System.Text.Encoding?` | Força a leitura do XML com um Encoding específico (ex: `Encoding.UTF8`), ignorando o cabeçalho original. Apenas para métodos que recebem `Stream` ou path. | `null` |

### Customização de Fonte e Recomendações

Você pode escolher a fonte tipográfica utilizada para a renderização do DANFE passando a configuração via `DanfeOptions`:

```csharp
var options = new DanfeOptions
{
    Font = DanfeFont.Inter // Opções: Arial, Inter, Roboto, IbmPlexSans
};
```

Caso queira usar uma fonte específica instalada no sistema operacional ou previamente registrada no QuestPDF, use a propriedade `CustomFontName`:

```csharp
var options = new DanfeOptions
{
    CustomFontName = "Liberation Sans"
};
```

> [!WARNING]
> **Recomendação Legal (MOC/SEFAZ)**:
> O Manual de Orientação do Contribuinte (MOC) da Nota Fiscal Eletrônica (NF-e) estabelece que a fonte padrão recomendada para a impressão do DANFE é a **Arial** (ou Courier/Times New Roman em caso de impressão de caracteres).
>
> Manter a fonte padrão **Arial** (que é o padrão `DanfeFont.Arial` configurado por omissão) garante total conformidade legal e visual com o layout padrão homologado junto à SEFAZ, evitando riscos de descaracterização em auditorias ou quebras de texto indesejadas. Utilize fontes alternativas como `Inter`, `Roboto` ou `IBM Plex Sans` sob sua própria responsabilidade ou para fins não-fiscais.



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
// Requer o namespace 'QuestPDF.Infrastructure':
using QuestPDF.Infrastructure;
QuestPDF.Settings.License = LicenseType.Community;

// Ou de forma totalmente qualificada (sem necessidade de using):
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
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

## 🔗 Projetos relacionados

| Projeto | Descrição |
|---|---|
| [NFeSchemaDownloader](https://github.com/fabyo/NFeSchemaDownloader) | Mantém os Schemas XML (XSD) da SEFAZ sempre atualizados automaticamente |
| [NFEConsulta](https://github.com/fabyo/NFEConsulta) | Consulta NF-e, valida XML e verifica status oficial da SEFAZ |

### Ferramentas CLI

- **NFEConsulta.Cli** → Consulta NF-e pela linha de comando.
- **NFeSchemaDownloader.Cli** → Automação de download de Schemas.

### Fluxo recomendado

```text
NFeSchemaDownloader (Mantém XSDs atualizados)
   │
   ▼
NF-e XML
   │
   ▼
NFEConsulta (Valida XML via XSD e consulta SEFAZ)
   │
   ▼
NFEDanfe (Gera o PDF final)
```

## 👨‍💻 Autor

Fabyo Guimarães Oliveira

- LinkedIn: [https://www.linkedin.com/in/fabyo-guimaraes/](https://www.linkedin.com/in/fabyo-guimaraes/)
- GitHub: https://github.com/fabyo

## Licença

MIT.
