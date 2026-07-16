# Guia de Contribuição

Obrigado por seu interesse em contribuir para o **NFEDanfe**! Este documento serve para orientar o processo de contribuição, ajudando a manter o projeto organizado, seguro e de alta qualidade.

---

## 🛠️ Como Contribuir

### 1. Relatando Bugs ou Sugerindo Recursos
Antes de abrir uma Issue, verifique se já não existe uma parecida aberta ou resolvida. Ao abrir uma Issue de bug, tente incluir:
* Passos claros para reproduzir o problema.
* O comportamento esperado e o comportamento obtido.
* Detalhes do ambiente (versão do .NET, Sistema Operacional, etc.).
* O arquivo XML da NF-e (com dados sensíveis/pessoais removidos ou alterados).

### 2. Desenvolvimento Local

#### Pré-requisitos
* **.NET 8 SDK** e **.NET 10 SDK** (instalados localmente).
* Um editor de código de sua preferência (Visual Studio, VS Code, JetBrains Rider).

#### Fluxo de Trabalho
1. Faça um **Fork** do repositório.
2. Crie uma branch para a sua alteração:
   ```bash
   git checkout -b feat/minha-nova-funcionalidade
   # ou
   git checkout -b fix/correcao-de-bug
   ```
3. Realize suas modificações no código.
4. Garanta que o projeto esteja compilando perfeitamente:
   ```bash
   dotnet restore NFEDanfe.slnx --locked-mode
   dotnet format NFEDanfe.slnx --verify-no-changes --no-restore
   dotnet build NFEDanfe.slnx --configuration Release --no-restore
   dotnet test NFEDanfe.Tests/NFEDanfe.Tests.csproj --configuration Release --no-build --no-restore
   ```
5. Comite e envie as alterações para o seu fork:
   ```bash
   git add .
   git commit -m "feat: adiciona suporte a novo campo do DANFE"
   git push origin feat/minha-nova-funcionalidade
   ```
6. Abra um **Pull Request (PR)** detalhado contra a branch `main` do repositório original.

---

## 📐 Diretrizes de Desenvolvimento

### Arquitetura do Projeto
O projeto está dividido de forma simples e modularizada para facilitar a manutenção:
* **`NFEDanfe`**: Biblioteca core contendo o motor gráfico baseado em PDFsharp.
  * **`Barcode`**: Desenho e codificação de barras (Code 128 e QR Code).
  * **`Blocks`**: Primitivas visuais e posicionamento de elementos de baixo nível (canhoto, cabeçalhos, etc.).
  * **`Builder`**: API fluente interna de montagem do layout.
  * **`Domain`**: Modelos de dados e parser de XML.
  * **`Layout`**: Orquestrador e compositor das páginas.
  * **`Pagination`**: Lógica de quebra de página automática baseada nas margens e tabelas.
* **`NFEDanfe.Cli`**: Ferramenta utilitária de linha de comando (`nfedanfe`).

### Boas Práticas de Código
* **Nomes em Português / Termos Fiscais**: Mantemos os nomes fiscais em conformidade com o Manual de Orientação do Contribuinte (MOC) da SEFAZ (ex: *emitente*, *destinatário*, *duplicata*, *canhoto*).
* **Ausência de dependências proprietárias**: Mantemos a biblioteca livre de dependências que exijam licenças comerciais restritivas ou faturamento. Toda a renderização gráfica deve ser feita via primitivas nativas do PDFsharp.
* **Layout Responsivo**: O layout do DANFE (Retrato ou Paisagem) deve se ajustar perfeitamente às margens e larguras úteis dinâmicas calculadas pelo `DanfeEngine`. Evite fixar dimensões absolutas de largura sem usar percentuais ou cálculo relativo.
* **Validação de PDF**: A suíte usa PdfPig para extrair e auditar o texto dos PDFs em memória. Não instale nem dependa de executáveis nativos como `pdftotext` para validar conteúdo.
* **Testes**: A suíte usa xUnit v3 e desabilita paralelismo entre coleções porque o registro de fontes do PDFsharp é compartilhado durante o processo.

---

## 💬 Mensagens de Commit

Utilizamos padrões semânticos simples para mensagens de commit baseados em [Conventional Commits](https://www.conventionalcommits.org/):
* `feat:` para novas funcionalidades ou novos campos visuais.
* `fix:` para correções de bugs, estouros de texto ou quebras de layout.
* `docs:` para atualizações na documentação ou README.
* `chore:` para manutenção de dependências e automações.

Use `!` ou um rodapé `BREAKING CHANGE:` quando houver incompatibilidade de API. O tipo do commit define a próxima versão:

* `fix:` gera uma versão patch (`2.1.7` → `2.1.8`).
* `feat:` gera uma versão minor (`2.1.7` → `2.2.0`).
* `feat!:`/`fix!:` gera uma versão major (`2.1.7` → `3.0.0`).

## Processo de release

A versão é definida explicitamente em `Directory.Build.props` e `version.txt`, e a alteração deve ser revisada por Pull Request junto com o `CHANGELOG.md`.

Quando o PR é mesclado em `main`, o workflow de publicação é executado automaticamente e:

1. valida se `Directory.Build.props` e `version.txt` possuem a mesma versão;
2. restaura dependências com lockfiles, compila e executa os testes;
3. gera e valida os pacotes `NFEDanfe` e `NFEDanfe.Cli`;
4. publica os pacotes no NuGet.

O workflow não calcula, incrementa ou altera versões e não cria Pull Requests. Quando a versão já existir no NuGet, `--skip-duplicate` evita falha e mantém o pacote existente. O acionamento manual fica disponível apenas para recuperação.

---

## 📜 Licença

Ao contribuir para este projeto, você concorda que suas contribuições serão licenciadas sob a licença **MIT** do projeto.
