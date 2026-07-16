![Logo](https://raw.githubusercontent.com/fabyo/NFEDanfe/main/logo-200.png)

[![CI](https://github.com/fabyo/NFEDanfe/actions/workflows/ci.yml/badge.svg)](https://github.com/fabyo/NFEDanfe/actions/workflows/ci.yml)
[![CodeQL](https://github.com/fabyo/NFEDanfe/actions/workflows/codeql.yml/badge.svg)](https://github.com/fabyo/NFEDanfe/actions/workflows/codeql.yml)
[![OpenSSF Scorecard](https://api.securityscorecards.dev/projects/github.com/fabyo/NFEDanfe/badge)](https://securityscorecards.dev/viewer/?uri=github.com/fabyo/NFEDanfe)
[![OpenSSF Best Practices](https://www.bestpractices.dev/projects/13399/badge)](https://www.bestpractices.dev/projects/13399)
[![Dependabot](https://img.shields.io/badge/Dependabot-enabled-brightgreen?logo=dependabot)](https://github.com/fabyo/NFEDanfe/security/dependabot)
[![NuGet](https://img.shields.io/nuget/v/NFEDanfe.svg)](https://www.nuget.org/packages/NFEDanfe)
[![Downloads](https://img.shields.io/nuget/dt/NFEDanfe.svg)](https://www.nuget.org/packages/NFEDanfe)
[![License](https://img.shields.io/github/license/fabyo/NFEDanfe.svg)](https://github.com/fabyo/NFEDanfe/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%2010.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![GitHub Stars](https://img.shields.io/github/stars/fabyo/NFEDanfe)](https://github.com/fabyo/NFEDanfe)

Biblioteca .NET para gerar DANFE em PDF a partir de XML de NF-e autorizada.

O projeto tem dois formatos de uso:

- `NFEDanfe`: biblioteca para integração em sistemas .NET, APIs, workers e ERPs.
- `NFEDanfe.Cli`: ferramenta de linha de comando para uso operacional e scripts.

## Motivos da Troca do QuestPDF

Para entender as motivações de arquitetura e licenciamento que levaram à substituição do QuestPDF pelo PDFsharp nesta nova versão, acesse a página detalhada dos [Motivos da Troca do QuestPDF para o PDFsharp](MOTIVOS_TROCA_QUESTPDF.md).

## Recursos

- Compatível com .NET 8 e .NET 10 (multi-targeting).
- Geração de DANFE em PDF 100% nativa, rápida e vetorial baseada em PDFsharp (livre de licenças comerciais restritivas).
- DANFE em modo retrato e paisagem.
- Seleção automática pelo campo `tpImp` do XML NF-e.
- Override manual de orientação via `DanfeOptions.TipoImpressaoOverride`.
- Parser seguro de XML NF-e com DTD proibido.
- Validação de consistência de totais de produtos, descontos e valor da nota.
- Paginação real de itens do DANFE com cabeçalhos de continuação automáticos.
- Simulação de negrito inteligente (overstrike) que garante formatação perfeita mesmo em ambientes restritos a fontes regulares.
- API pública simples com `DanfeGenerator` e `DanfeOptions` compatível com a API de referência.
- Snapshot textual para regressão funcional.

## Instalação Como Biblioteca

Quando publicado no NuGet:

```powershell
dotnet add package NFEDanfe
```

Uso básico:

```csharp
using NFEDanfe;

await using FileStream output = File.Create("danfe.pdf");
DanfeGenerator.GenerateFromXml("nota-procNFe.xml", output);
```

Sem configuração adicional, o DANFE usa a `logo.png` padrão incorporada ao pacote.

Com logo personalizado:

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
| `LogoPath` | `string?` | Caminho de um logotipo personalizado (PNG ou JPEG). Se for inválido, usa o logotipo padrão. | `null` |
| `LogoBytes` | `byte[]?` | Bytes de um logotipo personalizado (PNG ou JPEG). Se forem inválidos, usa o logotipo padrão. | `null` |
| `UseDefaultLogo` | `bool` | Usa a `logo.png` incorporada quando nenhum logotipo personalizado válido estiver disponível. | `true` |
| `ValidateBeforeGenerate` | `bool` | Se `true`, valida as regras de negócio e integridade da nota antes de gerar. | `true` |
| `EmitFooter` | `bool` | Se `true`, exibe a informação de rodapé "NFEDanfe - impresso em...". | `true` |
| `TipoImpressaoOverride` | `int?` | Sobrescreve a orientação definida no XML (`1` = Retrato, `2` = Paisagem). Se `null`, respeita o XML. | `null` |
| `Font` | `DanfeFont` | Enum para escolher a fonte (`Arial`, `Inter`, `Roboto`, `IbmPlexSans`). As fontes alternativas selecionam automaticamente seus arquivos Regular e Bold. | `DanfeFont.Arial` |
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

Caso queira usar uma fonte específica instalada no sistema operacional, use a propriedade `CustomFontName`:

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
> A fonte padrão é **Arial**, quando instalada no sistema. Em ambientes sem Arial, o projeto usa Roboto como fallback multiplataforma. Inter, Roboto e IBM Plex Sans somente são selecionadas explicitamente por `Font` ou `FontConfig`.



## Instalação Como CLI

Durante desenvolvimento:

```powershell
dotnet run --project .\NFEDanfe.Cli\NFEDanfe.Cli.csproj --framework net10.0 -- .\samples\nota-exemplo.xml
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

Gerar com arquivos de fonte específicos:

```powershell
nfedanfe .\samples\nota-exemplo.xml --font-reg .\fonts\Regular.ttf --font-bold .\fonts\Bold.ttf --output .\out
```

Gerar com logo por caminho explícito:

```powershell
nfedanfe .\nota-procNFe.xml --logo-path .\minha-logo.png --output .\out
```

## Logo Na CLI

Sem `--logo-path`, a CLI usa automaticamente o logotipo padrão incorporado ao pacote. Para personalizar, informe um caminho explícito:

```powershell
nfedanfe .\nota-procNFe.xml --logo-path .\assets\logo.png
```

Se o arquivo não existir ou não for uma imagem válida, a geração continua usando o logotipo padrão.

## Linux e Docker

O projeto é compatível com Linux porque usa .NET, PDFsharp e Barcoder sem `System.Drawing`.

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
- `Barcode`: gerador de códigos de barras (Code 128 / QR Code).
- `Blocks`: blocos visuais de desenho em baixo nível (canhoto, emitente, destinatário, etc.).
- `Builder`: construtor fluente do layout do DANFE.
- `Domain/Parser`: parser XML seguro.
- `Layout`: orquestração gráfica do documento.
- `Options`: opções de configuração do documento.
- `Pagination`: paginação de grade de itens e divisão em múltiplas folhas.

- [Histórico de alterações](CHANGELOG.md)

## 🔗 Projetos relacionados

| Projeto | Descrição |
|---|---|
| [NFeSchemaDownloader](https://github.com/fabyo/NFeSchemaDownloader) | Mantém os Schemas XML (XSD) da SEFAZ sempre atualizados automaticamente |
| [NFEEmissor](https://github.com/fabyo/NFEEmissor) | Gera, assina e autoriza NF-e em homologação ou produção, com API stateless, CLI e pacotes NuGet |
| [NFEConsulta](https://github.com/fabyo/NFEConsulta) | Consulta NF-e, valida XML e verifica status oficial da SEFAZ |

### Ferramentas CLI

- **NFEConsulta.Cli** → Consulta NF-e pela linha de comando.
- **NFeSchemaDownloader.Cli** → Automação de download de Schemas.

### Fluxo recomendado

```text
NFeSchemaDownloader (Mantém XSDs atualizados)
   │
   ▼
NFEEmissor (Gera, assina e autoriza a NF-e)
   │
   ▼
NF-e XML autorizado
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
