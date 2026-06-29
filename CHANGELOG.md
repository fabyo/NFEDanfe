# Histórico de alterações

## [2.3.0](https://github.com/fabyo/NFEDanfe/compare/v2.2.1...v2.3.0) (2026-06-29)


### Features

* Add CustomXmlEncoding to DanfeOptions to override XML encoding declaration ([2d07bf1](https://github.com/fabyo/NFEDanfe/commit/2d07bf1cee0c7227b9143ac2c4c40a9bc6172e03))
* add support for local de entrega (&lt;entrega&gt;) XML parsing and layout rendering with page size optimizations ([27f76e4](https://github.com/fabyo/NFEDanfe/commit/27f76e43235531b9a83276d6ae3323c8cdaae9a5))
* Add watermark for canceled NFe and bump version to v0.1.11 ([a94220c](https://github.com/fabyo/NFEDanfe/commit/a94220c535653e66e402fe17beb01054a80194fb))
* adicionar marca d'agua para homologacao ([eca3c86](https://github.com/fabyo/NFEDanfe/commit/eca3c8611f32295ec874c426de29d5af4d2d09a8))
* automate releases and harden package validation ([#17](https://github.com/fabyo/NFEDanfe/issues/17)) ([7553520](https://github.com/fabyo/NFEDanfe/commit/7553520b7d02614965d1b313ab2739f4d4152eec))
* configure NuGet properties, fix and expand tests, and setup GitHub Actions CI/CD ([595d2c1](https://github.com/fabyo/NFEDanfe/commit/595d2c16e94a541e5978e54d9b9ea949ed69369c))
* expor opcao --font e limitar CLI a gerar apenas um PDF por padrao ([499f09a](https://github.com/fabyo/NFEDanfe/commit/499f09a3241d4884eb80ef02f3afbddc8b0d52b6))
* extend products table to fill remaining page height and push footer to bottom ([d03f87e](https://github.com/fabyo/NFEDanfe/commit/d03f87e523b7bc542dcee2f0aac139a8b16808ad))
* first release ([efa0e94](https://github.com/fabyo/NFEDanfe/commit/efa0e9419c9399bef8a528b07a92b41e5e426a38))
* first release ([bd82338](https://github.com/fabyo/NFEDanfe/commit/bd823383ac1c6784e3d25422ac2c634c0756d515))
* highlight email addresses in bold in INFORMAÇÕES COMPLEMENTARES ([7dcf381](https://github.com/fabyo/NFEDanfe/commit/7dcf3815a40585d7541eb624e78b474b1c23547b))
* split semicolons as line breaks in RESERVADO AO FISCO block ([d99ad72](https://github.com/fabyo/NFEDanfe/commit/d99ad72c26d0faf376d0a978e3e506d8b327ddc2))
* v0.1.10 - vertical lines only on items, compact table, remove gap above products header ([3401c77](https://github.com/fabyo/NFEDanfe/commit/3401c7790a5288602d152754bec29d087125d380))


### Bug Fixes

* add .NET 10.0 SDK to NuGet publish workflow ([17c935d](https://github.com/fabyo/NFEDanfe/commit/17c935dc8858eb558e60c5246884f1419d60fd59))
* replace deleted real NF-e XMLs with anonymous test fixtures in testdata/ ([083cbd8](https://github.com/fabyo/NFEDanfe/commit/083cbd8d6b6f04d6429ec6fd00b83978e78224a6))
* run required checks on automated release PRs ([#19](https://github.com/fabyo/NFEDanfe/issues/19)) ([0d7db96](https://github.com/fabyo/NFEDanfe/commit/0d7db9690306b97a9d91afec5f386f1a3f0e63d0))
* set repository for release workflow dispatch ([#20](https://github.com/fabyo/NFEDanfe/issues/20)) ([90f3b41](https://github.com/fabyo/NFEDanfe/commit/90f3b413c2e8b7be5879c68c550b6fa4a95bc9ec))

## 2.2.1 (2026-06-29)

### Correções

- Corrigida a orientação do DANFE para respeitar automaticamente `<tpImp>2</tpImp>` como paisagem, sem exigir configuração externa.
- Corrigida a detecção de cancelamento por evento autorizado `110111`, garantindo a marca d'água de nota cancelada.
- Adicionado teste de regressão para garantir que a orientação informada no XML seja aplicada ao PDF.

### Exemplos

- Adicionados PDFs de exemplo para notas normal, cancelada, em homologação e em paisagem.

## [2.2.0](https://github.com/fabyo/NFEDanfe/compare/v2.1.7...v2.2.0) (2026-06-28)


### Features

* Add CustomXmlEncoding to DanfeOptions to override XML encoding declaration ([2d07bf1](https://github.com/fabyo/NFEDanfe/commit/2d07bf1cee0c7227b9143ac2c4c40a9bc6172e03))
* add support for local de entrega (&lt;entrega&gt;) XML parsing and layout rendering with page size optimizations ([27f76e4](https://github.com/fabyo/NFEDanfe/commit/27f76e43235531b9a83276d6ae3323c8cdaae9a5))
* Add watermark for canceled NFe and bump version to v0.1.11 ([a94220c](https://github.com/fabyo/NFEDanfe/commit/a94220c535653e66e402fe17beb01054a80194fb))
* adicionar marca d'agua para homologacao ([eca3c86](https://github.com/fabyo/NFEDanfe/commit/eca3c8611f32295ec874c426de29d5af4d2d09a8))
* automate releases and harden package validation ([#17](https://github.com/fabyo/NFEDanfe/issues/17)) ([7553520](https://github.com/fabyo/NFEDanfe/commit/7553520b7d02614965d1b313ab2739f4d4152eec))
* configure NuGet properties, fix and expand tests, and setup GitHub Actions CI/CD ([595d2c1](https://github.com/fabyo/NFEDanfe/commit/595d2c16e94a541e5978e54d9b9ea949ed69369c))
* expor opcao --font e limitar CLI a gerar apenas um PDF por padrao ([499f09a](https://github.com/fabyo/NFEDanfe/commit/499f09a3241d4884eb80ef02f3afbddc8b0d52b6))
* extend products table to fill remaining page height and push footer to bottom ([d03f87e](https://github.com/fabyo/NFEDanfe/commit/d03f87e523b7bc542dcee2f0aac139a8b16808ad))
* first release ([efa0e94](https://github.com/fabyo/NFEDanfe/commit/efa0e9419c9399bef8a528b07a92b41e5e426a38))
* first release ([bd82338](https://github.com/fabyo/NFEDanfe/commit/bd823383ac1c6784e3d25422ac2c634c0756d515))
* highlight email addresses in bold in INFORMAÇÕES COMPLEMENTARES ([7dcf381](https://github.com/fabyo/NFEDanfe/commit/7dcf3815a40585d7541eb624e78b474b1c23547b))
* split semicolons as line breaks in RESERVADO AO FISCO block ([d99ad72](https://github.com/fabyo/NFEDanfe/commit/d99ad72c26d0faf376d0a978e3e506d8b327ddc2))
* v0.1.10 - vertical lines only on items, compact table, remove gap above products header ([3401c77](https://github.com/fabyo/NFEDanfe/commit/3401c7790a5288602d152754bec29d087125d380))


### Bug Fixes

* add .NET 10.0 SDK to NuGet publish workflow ([17c935d](https://github.com/fabyo/NFEDanfe/commit/17c935dc8858eb558e60c5246884f1419d60fd59))
* replace deleted real NF-e XMLs with anonymous test fixtures in testdata/ ([083cbd8](https://github.com/fabyo/NFEDanfe/commit/083cbd8d6b6f04d6429ec6fd00b83978e78224a6))
* run required checks on automated release PRs ([#19](https://github.com/fabyo/NFEDanfe/issues/19)) ([0d7db96](https://github.com/fabyo/NFEDanfe/commit/0d7db9690306b97a9d91afec5f386f1a3f0e63d0))
* set repository for release workflow dispatch ([#20](https://github.com/fabyo/NFEDanfe/issues/20)) ([90f3b41](https://github.com/fabyo/NFEDanfe/commit/90f3b413c2e8b7be5879c68c550b6fa4a95bc9ec))

## 2.1.7
- Removidos workflows de segurança incompatíveis com o projeto, destinados a imagens Docker e aplicativos móveis.
- Mantidos apenas os workflows aplicáveis ao projeto .NET, com GitHub Actions fixadas por hashes imutáveis.

## 2.1.6
- Empacotamento automático da pasta de fontes (`fonts/`) dentro do pacote NuGet, com cópia automática para a pasta de build dos projetos que referenciam a biblioteca.
- Correção e aplicação de negrito (faux bold via overstrike) na label e no valor do número do Pedido em Informações Complementares.
- Correção do nome no rodapé da DANFE de PDFHK para NFEDanfe.
- Remoção do `justfile` e do diretório `scripts/` do repositório.
- Criação e integração da política de segurança (`SECURITY.md`).

## 2.1.3
- Configuração do pacote NuGet para incluir e renderizar o `README.md` e o logotipo oficial (`logo.png` / `logo-200.png`) diretamente na página do pacote.
- Adicionado guia de contribuição detalhado em português (`CONTRIBUTING.md`).

## 2.1.1
- Ajuste na quebra de linha da Razão Social do Emitente no cabeçalho para evitar estouro horizontal.
- Correção na leitura de imagem do logotipo a partir de stream de bytes no PDFsharp (evitando exceções com buffers ocultos de `MemoryStream`).

## 2.1.0
- Transição completa da engine de layout: deixamos de usar o QuestPDF e passamos a usar um motor de renderização vetorial próprio, leve e nativo construído sobre o PDFsharp.
- Eliminação total de dependências com licenças comerciais restritivas no motor de PDF, tornando a biblioteca ideal para qualquer tipo de uso corporativo ou pessoal.
- Implementação de algoritmo inteligente de paginação real de itens do DANFE com cabeçalhos de continuação.
- Suporte a simulação de negrito (faux bold via overstrike) garantindo legibilidade perfeita mesmo em ambientes restritos a fontes regulares.
- Suporte a carregamento flexível de logotipos corporativos por fluxo de bytes ou arquivo físico.
- Suporte nativo a multi-targeting direcionando .NET 8 e .NET 10.

## Motor de Renderização

Esta biblioteca utiliza o **[PDFsharp](https://github.com/empira/PDFsharp)** como motor de geração de PDF, desenvolvido e mantido pela empira Software sob licença **MIT pura**, sem restrições de faturamento ou volume de uso.

### Por que não o QuestPDF?

O QuestPDF é uma excelente biblioteca de propósito geral, mas a partir da versão 2023.x adotou um modelo de licenciamento comercial que vincula o uso gratuito ao faturamento anual da empresa. Para projetos em crescimento ou operações de contabilidade e emissão fiscal em escala, isso representa um custo variável e imprevisível.

Além disso, o QuestPDF foi projetado para layouts de fluxo livre — o que traz complexidade desnecessária para o DANFE, cujo layout é uma grade fiscal rígida e bem definida pelo MOC da SEFAZ. Usar uma engine genérica para um documento específico significa lutar contra abstrações que não se encaixam no problema.

### A decisão pelo PDFsharp

O PDFsharp é **100% C# managed**, sem binários nativos, sem dependências de sistema operacional, e roda em Linux/Docker sem nenhuma configuração adicional. A engine de layout desta biblioteca foi construída diretamente sobre a API de baixo nível do PDFsharp, com um motor de grade, DSL fluente e sistema de paginação desenhados especificamente para o padrão DANFE NF-e modelo 55.

O resultado é uma biblioteca:

- **Mais rápida** — sem camadas de abstração genéricas desnecessárias
- **Mais previsível** — cada pixel do DANFE é controlado explicitamente
- **Sem limite de uso** — MIT irrestrita, independente de faturamento ou volume
- **Cross-platform real** — Windows, Linux e Docker sem asterisco
- **Sem dependência de futuras mudanças de licença** de terceiros

A troca não foi uma restrição — foi uma escolha de arquitetura deliberada que resultou em um motor mais adequado ao problema fiscal brasileiro.

## 0.1.27
- Reforçadas as permissões mínimas dos workflows do GitHub Actions.
- Fixadas as Actions por hash imutável e adicionados lockfiles do NuGet para builds reproduzíveis.
- Adicionada análise estática de segurança com CodeQL.

## 0.1.26
- Alinhados ao fundo os valores monetários da seção `CÁLCULO DO IMPOSTO`, mantendo o tamanho original das células.
- Removidas as linhas tracejadas vazias em `DADOS DO PRODUTO / SERVIÇOS`, desenhando a grade apenas para produtos existentes.
- Atualizado o XML de exemplo de 5 produtos para ambiente de produção (`tpAmb = 1`), evitando marca d'água de homologação na amostra.

## 0.1.25
- Removida a linha em branco antes de `Pedido: <xPed>` em `DADOS ADICIONAIS`, mantendo apenas a quebra simples quando houver informações complementares anteriores.

## 0.1.24
- Corrigida a leitura do pedido de compra para usar a tag `<compra><xPed>...</xPed></compra>` da NF-e, mantendo fallback para `xPed` informado nos produtos.

## 0.1.23
- Ajustadas as larguras da tabela `DADOS DO PRODUTO / SERVIÇOS`, ampliando `CÓDIGO PRODUTO` e dando mais espaço para `DESCRIÇÃO DO PRODUTO/SERVIÇO`.
- Reduzidas as colunas `BC ICMS`, `VALOR DESC`, `CST` e `CFOP` para melhorar o aproveitamento horizontal da grade de produtos.
- Adicionada leitura da tag `<xPed>` dos produtos e impressão de `Pedido: <xPed>` em `DADOS ADICIONAIS`, com o valor do pedido em negrito quando existir.

## 0.1.22
- Melhorada a estabilidade do layout de produtos para XMLs grandes, preenchendo a grade até a altura-alvo quando necessário.
- Adicionados testes de paginação para cenários grandes em retrato e paisagem.
- Incluída configuração de encoding (`.editorconfig` e `.gitattributes`) para evitar regressões de texto UTF-8.

## 0.1.21
- Adicionada marca d'água `NOTA FISCAL EM HOMOLOGAÇÃO` no DANFE quando o XML NF-e possuir `tpAmb = 2`, nos layouts retrato e paisagem.

## 0.1.20
- Adicionado parâmetro `--font` (`-f`) para seleção de fonte na CLI.
- Limitada a CLI para gerar apenas 1 PDF por nota por padrão (evitando a geração automática de 5 PDFs com todas as fontes).

## 0.1.19
- Ajustes de formatação de código e estilização no parser e componentes de cobrança.
- Atualização da dependência do `xunit.runner.visualstudio` para resolver aviso de build `NU1603`.

## 0.1.18
- Atualização da política de segurança e adição de automação (Scorecard).
- Melhorias na documentação pública (README.md) com adição de projetos relacionados e diagramas.
