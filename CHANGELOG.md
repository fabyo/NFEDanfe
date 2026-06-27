# Histórico de alterações

## 2.0.0
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
