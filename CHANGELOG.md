# Histórico de alterações

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
