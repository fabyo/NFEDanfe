# Motivos da troca do QuestPDF para o PDFsharp

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
