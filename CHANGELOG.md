# Histórico de alterações

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
