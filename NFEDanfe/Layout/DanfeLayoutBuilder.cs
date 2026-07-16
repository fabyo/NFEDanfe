using System;
using System.IO;
using System.Linq;
using PdfSharp.Drawing;
using NFEDanfe.Blocks;
using NFEDanfe.Grid;
using NFEDanfe.Options;
using NFEDanfe.Pagination;
using NFEDanfe.Barcode;
using NFEDanfe.Domain.Models;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Builder;

namespace NFEDanfe.Layout;

/// <summary>Orquestrador do layout do DANFE compatível com NFEDanfe.</summary>
public sealed class DanfeLayoutBuilder
{
    private static double Mm(double mm) => mm * 2.834645;

    /// <summary>Gera o DANFE e retorna os bytes do PDF.</summary>
    public byte[] Build(DanfeModel model, DanfeOptions options)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(options);

        var mode = ResolvePageMode(model, options);
        using var engine = new DanfeEngine(options.FontConfig, options.Margins, mode);
        RenderAll(engine, model, options);
        return engine.Build();
    }

    /// <summary>Gera o DANFE e salva no stream.</summary>
    public void Build(DanfeModel model, DanfeOptions options, Stream outputStream)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(outputStream);

        var mode = ResolvePageMode(model, options);
        using var engine = new DanfeEngine(options.FontConfig, options.Margins, mode);
        RenderAll(engine, model, options);
        engine.Build(outputStream);
    }

    /// <summary>Compatibilidade com a classe DanfeData original.</summary>
    internal byte[] Build(DanfeData data, DanfeOptions options)
    {
        var model = ConvertToModel(data);
        return Build(model, options);
    }

    /// <summary>Compatibilidade com a classe DanfeData original.</summary>
    internal void Build(DanfeData data, DanfeOptions options, Stream outputStream)
    {
        var model = ConvertToModel(data);
        Build(model, options, outputStream);
    }

    private void RenderAll(DanfeEngine engine, DanfeModel model, DanfeOptions options)
    {
        var canhoto = new CanhotoBlock(model);
        var emitente = new EmitentBlock(model, options.LogoPath, options.LogoBytes, options.UseDefaultLogo, 1, 1);
        var recipientBlock = new RecipientBlock(model);
        var invoiceBlock = new InvoiceBlock(model);
        var transportBlock = new TransportBlock(model);
        var taxBlock = new TaxBlock(model);
        var additionalBlock = new AdditionalDataBlock(model);

        var isLandscape = ResolvePageMode(model, options) == DanfePageMode.Landscape;

        // Medidas e dimensões
        double titleBarH = 10.0; // "DADOS DO PRODUTO / SERVIÇOS"
        double fieldH = DanfeField.DefaultHeight(engine.Styles);

        double canhotoH = isLandscape ? 0 : 74.0;
        double emitenteH = isLandscape ? 50.0 : 90.0;
        double row3H = isLandscape ? 18.0 : fieldH;
        double row4H = isLandscape ? 18.0 : fieldH;
        var valueFontOverride = isLandscape ? new XFont(DanfeFontResolver.FamilyName, 7.0, XFontStyleEx.Regular) : null;
        double destinatarioH = 12.0 + (isLandscape ? 18.0 : fieldH) * 3.0;
        double localEntregaH = model.LocalEntrega != null ? 12.0 + fieldH * 2.0 : 0;
        double faturaH = invoiceBlock.ShouldRender ? 12.0 + 28.0 : 0;
        double impostosH = 12.0 + (isLandscape ? 18.0 : fieldH) * 2.0;
        double transporteH = 12.0 + (isLandscape ? 18.0 : fieldH) * 3.0;

        // Rodapé de todas as páginas (Dados Adicionais + Linha de Rodapé)
        // O bloco de Dados Adicionais (82 pt) fica no limite inferior da página (0 pt gap inferior).
        // Deixamos um gap de 6.0 pt entre a grade de produtos e o rodapé.
        double footerH = 6.0 + 82.0 + 0.0; // 88.0 pt

        var columns = DanfeItemColumn.GetDefaultColumns(isLandscape);
        double totalConstantWidth = columns.Where(c => c.WidthPt > 0).Sum(c => c.WidthPt);
        double outerContentW = engine.UsableWidth;
        if (isLandscape) outerContentW -= Mm(28) + 6;
        double descColumnWidth = outerContentW - totalConstantWidth;

        // Cálculo das Zonas Fixas (Page 1 vs Continuação) - usando espaçamento padrão de 6.0 pt acima dos itens
        double fixedUpperPage1 = canhotoH + emitenteH + row3H + row4H + 6.0 + destinatarioH + (model.LocalEntrega != null ? 6.0 + localEntregaH : 0) + (invoiceBlock.ShouldRender ? 6.0 + faturaH : 0) + 6.0 + impostosH + 6.0 + transporteH + 6.0 + titleBarH;
        double fixedUpperContinuation = emitenteH + row3H + row4H + 6.0 + titleBarH;

        double availableHeightPage1 = engine.UsableHeight - fixedUpperPage1 - footerH - 11.0;
        double availableHeightContinuation = engine.UsableHeight - fixedUpperContinuation - footerH - 11.0;

        // Mapeamento dos produtos para a grade (15 colunas)
        var items = (model.Produtos ?? Array.Empty<ProdutoModel>()).Select(p => new DanfeItemRow(
            CodigoProduto: p.Codigo,
            Descricao: p.Descricao,
            Ncm: p.Ncm,
            Cst: p.CstCsosn,
            Cfop: p.Cfop,
            Unidade: p.Unidade,
            Quantidade: p.Quantidade.ToString("N4", DocumentFormatter.Brazil),
            ValorUnitario: p.ValorUnitario % 0.01m != 0
                ? p.ValorUnitario.ToString("N4", DocumentFormatter.Brazil)
                : p.ValorUnitario.ToString("N2", DocumentFormatter.Brazil),
            ValorTotal: DocumentFormatter.Money(p.ValorTotal),
            ValorDesconto: DocumentFormatter.Money(p.ValorDesconto),
            BaseIcms: DocumentFormatter.Money(p.BaseCalculoIcms),
            ValorIcms: DocumentFormatter.Money(p.ValorIcms),
            ValorIpi: DocumentFormatter.Money(p.ValorIpi),
            AliquotaIcms: p.AliquotaIcms.ToString("N2", DocumentFormatter.Brazil),
            AliquotaIpi: p.AliquotaIpi.ToString("N2", DocumentFormatter.Brazil)
        )).ToList();

        // Paginação com alturas dinâmicas baseado nas descrições de produtos
        var valueFont = new XFont(DanfeFontResolver.FamilyName, 5.0, XFontStyleEx.Regular);

        // Criar um documento e XGraphics temporário para medições precisas de fontes
        var tempDoc = new PdfSharp.Pdf.PdfDocument();
        var tempPage = tempDoc.AddPage();
        var rowHeights = new List<double>();
        using (var measureGfx = XGraphics.FromPdfPage(tempPage))
        {
            foreach (var item in items)
            {
                // Subtrai os 2.0 pt de padding da coluna de descrição para medição 100% fiel
                int lines = GetLinesCount(measureGfx, item.Descricao, valueFont, descColumnWidth - 2.0);
                double h = Math.Max(11.0, lines * 6.5 + 4.5);
                rowHeights.Add(h);
            }
        }

        var plan = DanfePaginator.Calculate(rowHeights, availableHeightPage1, availableHeightContinuation);

        var itemGrid = new DanfeItemGrid();

        // Renderizar todas as páginas planejadas
        for (int p = 0; p < plan.TotalPages; p++)
        {
            using var gfx = engine.BeginPage();
            double currentY = engine.MarginTopPt;

            double contentX = engine.MarginLeftPt;
            double contentW = engine.UsableWidth;

            // No modo Paisagem, o canhoto é desenhado verticalmente no canto esquerdo da página
            if (isLandscape)
            {
                canhoto.DrawLandscape(gfx, engine.Styles, engine.MarginLeftPt, engine.MarginTopPt, engine.UsableHeight);
                contentX += Mm(28) + 6;
                contentW -= Mm(28) + 6;
            }

            if (p == 0)
            {
                // PAGE 1
                // 1. Canhoto (Portrait apenas)
                if (!isLandscape)
                {
                    currentY += canhoto.Draw(gfx, engine.Styles, contentX, currentY, contentW);
                }

                // 2. EmitenteBox / DanfeBox / ChaveAcessoBox
                var pEmitente = new EmitentBlock(model, options.LogoPath, options.LogoBytes, options.UseDefaultLogo, 1, plan.TotalPages);
                currentY += pEmitente.Draw(gfx, engine.Styles, contentX, currentY, contentW, isLandscape);

                // 3. Natureza da Operação / Protocolo
                new DanfeField("NATUREZA DA OPERAÇÃO", model.DadosDanfe.NaturezaOperacao, 66.67)
                    .Draw(gfx, engine.Styles, contentX, currentY, contentW * 0.6667, row3H, valueFontOverride: valueFontOverride);

                string protStr = string.IsNullOrEmpty(model.DadosDanfe.ProtocoloAutorizacao)
                    ? ""
                    : $"{model.DadosDanfe.ProtocoloAutorizacao} - {model.DadosDanfe.DataProtocolo:dd/MM/yyyy HH:mm:ss}";
                new DanfeField("PROTOCOLO DE AUTORIZAÇÃO DE USO", protStr, 33.33)
                    .Draw(gfx, engine.Styles, contentX + contentW * 0.6667, currentY, contentW * 0.3333, row3H, valueFontOverride: valueFontOverride);
                currentY += row3H;

                // 4. IE / IE ST / CNPJ
                new DanfeField("INSCRIÇÃO ESTADUAL", model.Emitente.InscricaoEstadual, 33.33)
                    .Draw(gfx, engine.Styles, contentX, currentY, contentW * 0.3333, row4H, valueFontOverride: valueFontOverride);
                new DanfeField("INSCRIÇÃO ESTADUAL DO SUBST. TRIBUT.", model.Emitente.InscricaoEstadualSt ?? "", 33.33)
                    .Draw(gfx, engine.Styles, contentX + contentW * 0.3333, currentY, contentW * 0.3333, row4H, valueFontOverride: valueFontOverride);
                new DanfeField("CNPJ", DocumentFormatter.CnpjCpf(model.Emitente.Cnpj), 33.34)
                    .Draw(gfx, engine.Styles, contentX + contentW * 0.6667, currentY, contentW * 0.3334, row4H, valueFontOverride: valueFontOverride);
                currentY += row4H;

                // 5. Destinatário Box
                currentY += 6.0;
                currentY += recipientBlock.Draw(gfx, engine.Styles, contentX, currentY, contentW, isLandscape);

                // 6. Local de Entrega (se aplicável)
                if (model.LocalEntrega != null)
                {
                    currentY += 6.0;
                    currentY += DrawLocalEntrega(gfx, engine.Styles, contentX, currentY, contentW, model.LocalEntrega);
                }

                // 7. Cobrança / Fatura
                if (invoiceBlock.ShouldRender)
                {
                    currentY += 6.0;
                    currentY += invoiceBlock.Draw(gfx, engine.Styles, contentX, currentY, contentW);
                }

                // 8. Cálculo de Impostos
                currentY += 6.0;
                currentY += taxBlock.Draw(gfx, engine.Styles, contentX, currentY, contentW, isLandscape);

                // 9. Transportador / Volumes
                currentY += 6.0;
                currentY += transportBlock.Draw(gfx, engine.Styles, contentX, currentY, contentW, isLandscape);
            }
            else
            {
                // PAGES 2+
                // 1. Emitente / Danfe / Chave reduzidos
                var pEmitente = new EmitentBlock(model, options.LogoPath, options.LogoBytes, options.UseDefaultLogo, p + 1, plan.TotalPages);
                currentY += pEmitente.Draw(gfx, engine.Styles, contentX, currentY, contentW, isLandscape);

                // 2. Natureza da Operação / Protocolo
                new DanfeField("NATUREZA DA OPERAÇÃO", model.DadosDanfe.NaturezaOperacao, 66.67)
                    .Draw(gfx, engine.Styles, contentX, currentY, contentW * 0.6667, row3H, valueFontOverride: valueFontOverride);

                string protStr = string.IsNullOrEmpty(model.DadosDanfe.ProtocoloAutorizacao)
                    ? ""
                    : $"{model.DadosDanfe.ProtocoloAutorizacao} - {model.DadosDanfe.DataProtocolo:dd/MM/yyyy HH:mm:ss}";
                new DanfeField("PROTOCOLO DE AUTORIZAÇÃO DE USO", protStr, 33.33)
                    .Draw(gfx, engine.Styles, contentX + contentW * 0.6667, currentY, contentW * 0.3333, row3H, valueFontOverride: valueFontOverride);
                currentY += row3H;

                // 3. IE / IE ST / CNPJ
                new DanfeField("INSCRIÇÃO ESTADUAL", model.Emitente.InscricaoEstadual, 33.33)
                    .Draw(gfx, engine.Styles, contentX, currentY, contentW * 0.3333, row4H, valueFontOverride: valueFontOverride);
                new DanfeField("INSCRIÇÃO ESTADUAL DO SUBST. TRIBUT.", model.Emitente.InscricaoEstadualSt ?? "", 33.33)
                    .Draw(gfx, engine.Styles, contentX + contentW * 0.3333, currentY, contentW * 0.3333, row4H, valueFontOverride: valueFontOverride);
                new DanfeField("CNPJ", DocumentFormatter.CnpjCpf(model.Emitente.Cnpj), 33.34)
                    .Draw(gfx, engine.Styles, contentX + contentW * 0.6667, currentY, contentW * 0.3334, row4H, valueFontOverride: valueFontOverride);
                currentY += row4H;
            }

            // --- TÍTULO DOS ITENS ---
            currentY += 6.0;
            var grayBrush = new XSolidBrush(XColor.FromArgb(224, 224, 224));
            gfx.DrawRectangle(engine.Styles.BorderPen, grayBrush, contentX, currentY, contentW, titleBarH);
            var titleFormat = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Center };
            var titleFont = new XFont(DanfeFontResolver.FamilyName, 6.0, XFontStyleEx.Bold);
            gfx.DrawString("DADOS DO PRODUTO / SERVIÇOS", titleFont, engine.Styles.TextBrush,
                new XRect(contentX + 4, currentY, contentW - 8, titleBarH), titleFormat);
            currentY += titleBarH;

            // --- GRADE DE ITENS ---
            if (p < plan.Slices.Count)
            {
                var slice = plan.Slices[p];
                itemGrid.Draw(gfx, engine.Styles, slice, items, rowHeights, columns,
                    contentX, currentY, contentW, drawColumnHeaders: true);
            }

            // --- RODAPÉ FIXO DA PÁGINA ---
            double footerY = engine.MarginTopPt + engine.UsableHeight - footerH;
            additionalBlock.Draw(gfx, engine.Styles, contentX, footerY + 6.0, contentW);

            // Linha de Copyright / Impresso por
            if (options.EmitFooter)
            {
                var footerTextFont = new XFont(DanfeFontResolver.FamilyName, 5.0, XFontStyleEx.Italic);
                var formatRight = new XStringFormat { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.Far };
                string footerText = $"NFEDanfe  impresso em {DateTime.Now:dd/MM/yyyy HH:mm:ss} - layout NF-e {model.DadosDanfe.VersaoLayout}";
                gfx.DrawString(footerText, footerTextFont, engine.Styles.TextBrush,
                    new XRect(contentX, engine.MarginTopPt + engine.UsableHeight + 2.0, contentW, 10), formatRight);
            }

            // --- MARCA D'ÁGUA ---
            bool isCancelada = options.CanceledOverride ?? model.DadosDanfe.IsCancelada;
            if (isCancelada)
            {
                DrawWatermark(gfx, engine, "NOTA FISCAL CANCELADA");
            }
            else if (options.Ambiente == DanfeAmbiente.Homologacao || model.DadosDanfe.TipoAmbiente == 2)
            {
                DrawWatermark(gfx, engine, "SEM VALOR FISCAL");
            }
        }
    }

    private static DanfePageMode ResolvePageMode(DanfeModel model, DanfeOptions options)
    {
        int tipoImpressao = options.TipoImpressaoOverride ?? model.DadosDanfe.TipoImpressao;
        return tipoImpressao == 2 || options.PageMode == DanfePageMode.Landscape
            ? DanfePageMode.Landscape
            : DanfePageMode.Portrait;
    }

    private static double DrawLocalEntrega(XGraphics gfx, DanfeStyleCatalog styles, double x, double y, double width, LocalEntrega entrega)
    {
        double titleH = 12.0;
        double fieldH = DanfeField.DefaultHeight(styles);
        double height = titleH + fieldH * 2.0;

        var grayBrush = new XSolidBrush(XColor.FromArgb(240, 240, 240));
        gfx.DrawRectangle(styles.BorderPen, grayBrush, x, y, width, titleH);

        var titleFormat = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Center };
        var titleFont = new XFont(DanfeFontResolver.FamilyName, 6.0, XFontStyleEx.Bold);
        gfx.DrawString("INFORMAÇÕES DO LOCAL DE ENTREGA", titleFont, styles.TextBrush,
            new XRect(x + 4, y, width - 8, titleH), titleFormat);
        y += titleH;

        new DanfeField("NOME / RAZÃO SOCIAL", entrega.RazaoSocial ?? "", 50)
            .Draw(gfx, styles, x, y, width * 0.5, fieldH);
        new DanfeField("CNPJ / CPF", DocumentFormatter.CnpjCpf(entrega.Documento), 25)
            .Draw(gfx, styles, x + width * 0.5, y, width * 0.25, fieldH);
        new DanfeField("INSCRIÇÃO ESTADUAL", entrega.InscricaoEstadual ?? "", 25)
            .Draw(gfx, styles, x + width * 0.75, y, width * 0.25, fieldH);

        y += fieldH;

        string end = DocumentFormatter.Address(entrega.Endereco);
        new DanfeField("ENDEREÇO", end, 66.67)
            .Draw(gfx, styles, x, y, width * 0.6667, fieldH);
        new DanfeField("MUNICÍPIO", entrega.Endereco.Municipio, 25.0)
            .Draw(gfx, styles, x + width * 0.6667, y, width * 0.25, fieldH);
        new DanfeField("UF", entrega.Endereco.Uf, 8.33)
            .Draw(gfx, styles, x + width * 0.9167, y, width * 0.0833, fieldH);

        return height;
    }

    private static void DrawWatermark(XGraphics gfx, DanfeEngine engine, string text)
    {
        var state = gfx.Save();

        double centerX = engine.MarginLeftPt + engine.UsableWidth / 2;
        double centerY = engine.MarginTopPt + engine.UsableHeight / 2;

        gfx.TranslateTransform(centerX, centerY);
        gfx.RotateTransform(-45);

        var watermarkFont = new XFont(DanfeFontResolver.FamilyName, 44, XFontStyleEx.Bold);
        var format = new XStringFormat
        {
            Alignment = XStringAlignment.Center,
            LineAlignment = XLineAlignment.Center
        };

        gfx.DrawString(text, watermarkFont, engine.Styles.WatermarkBrush,
            new XRect(-250, -30, 500, 60), format);

        gfx.Restore(state);
    }

    private static DanfeModel ConvertToModel(DanfeData data)
    {
        var emitente = new Emitente(
            RazaoSocial: data.RazaoSocialEmitente,
            NomeFantasia: data.RazaoSocialEmitente,
            Cnpj: data.CnpjEmitente,
            InscricaoEstadual: data.IeEmitente,
            InscricaoEstadualSt: null,
            Endereco: new Endereco(data.EnderecoEmitente, "", "", "", data.MunicipioEmitente, data.UfEmitente, data.CepEmitente),
            Telefone: data.FoneEmitente,
            LogoBytes: null
        );

        string destRazao = data.RazaoSocialDest;
        if (data.Ambiente == DanfeAmbiente.Homologacao)
        {
            destRazao = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL";
        }

        var dest = new Destinatario(
            RazaoSocial: destRazao,
            Documento: data.CnpjCpfDest,
            Endereco: new Endereco(data.EnderecoDest, "", "", data.BairroDest, data.MunicipioDest, data.UfDest, data.CepDest),
            InscricaoEstadual: data.IeDest,
            Telefone: data.FoneDest
        );

        var dados = new DadosDanfe(
            TipoOperacao: int.TryParse(data.TipoOperacao, out int to) ? to : 1,
            NaturezaOperacao: data.NaturezaOperacao,
            ChaveAcesso: data.ChaveAcesso,
            ProtocoloAutorizacao: data.Protocolo,
            DataProtocolo: DateTime.TryParse(data.DataProtocolo, out DateTime dp) ? dp : DateTime.MinValue,
            Numero: int.TryParse(data.Numero, out int num) ? num : 0,
            Serie: int.TryParse(data.Serie, out int ser) ? ser : 1,
            PaginaAtual: 1,
            TotalPaginas: 1,
            DataEmissao: DateTime.TryParse(data.DataEmissao, out DateTime de) ? de : DateTime.MinValue,
            DataEntradaSaida: DateTime.TryParse(data.DataEntradaSaida, out DateTime des) ? des : null,
            VersaoLayout: data.VersaoLayout,
            TipoImpressao: 1,
            TipoAmbiente: data.Ambiente == DanfeAmbiente.Homologacao ? 2 : 1,
            IsCancelada: false
        );

        var impostos = new ImpostosModel(
            BaseCalculoIcms: decimal.TryParse(data.BaseIcms, out decimal bi) ? bi : 0,
            ValorIcms: decimal.TryParse(data.ValorIcms, out decimal vi) ? vi : 0,
            BaseCalculoIcmsSt: decimal.TryParse(data.BaseIcmsSt, out decimal bist) ? bist : 0,
            ValorIcmsSt: decimal.TryParse(data.ValorIcmsSt, out decimal vist) ? vist : 0,
            ValorFcp: 0,
            ValorProdutos: decimal.TryParse(data.ValorProdutos, out decimal vp) ? vp : 0,
            ValorFrete: decimal.TryParse(data.ValorFrete, out decimal vf) ? vf : 0,
            ValorSeguro: decimal.TryParse(data.ValorSeguro, out decimal vs) ? vs : 0,
            ValorDesconto: decimal.TryParse(data.Desconto, out decimal vd) ? vd : 0,
            OutrasDespesas: decimal.TryParse(data.OutrasDespesas, out decimal od) ? od : 0,
            ValorIpi: decimal.TryParse(data.ValorIpi, out decimal vipi) ? vipi : 0,
            ValorIcmsUfDest: 0,
            ValorTotTrib: decimal.TryParse(data.ValorTributos, out decimal vt) ? vt : 0,
            ValorIi: 0,
            ValorIcmsUfRemet: 0,
            ValorPis: decimal.TryParse(data.ValorPis, out decimal vpis) ? vpis : 0,
            ValorCofins: decimal.TryParse(data.ValorCofins, out decimal vcof) ? vcof : 0,
            ValorNota: decimal.TryParse(data.ValorTotal, out decimal vn) ? vn : 0
        );

        var transportador = new TransportadorModel(
            RazaoSocial: data.TransportadoraNome,
            FretePorConta: data.ModalidadeFrete,
            CodigoAntt: "",
            PlacaVeiculo: data.PlacaVeiculo,
            UfPlaca: data.UfVeiculo,
            Documento: data.TransportadoraCnpj,
            EnderecoCompleto: "",
            Municipio: "",
            Uf: "",
            InscricaoEstadual: "",
            QuantidadeVolumes: null,
            Especie: "",
            Marca: "",
            Numeracao: "",
            PesoBruto: null,
            PesoLiquido: null
        );

        var produtos = data.Itens.Select(i => new ProdutoModel(
            Codigo: i.CodigoProduto,
            Descricao: i.Descricao,
            Ncm: i.Ncm,
            CstCsosn: i.Cst,
            Cfop: i.Cfop,
            Unidade: i.Unidade,
            Quantidade: decimal.TryParse(i.Quantidade, out decimal q) ? q : 0,
            ValorUnitario: decimal.TryParse(i.ValorUnitario, out decimal vu) ? vu : 0,
            ValorTotal: decimal.TryParse(i.ValorTotal, out decimal vt) ? vt : 0,
            ValorDesconto: 0,
            BaseCalculoIcms: decimal.TryParse(i.BaseIcms, out decimal bci) ? bci : 0,
            ValorIcms: decimal.TryParse(i.ValorIcms, out decimal vic) ? vic : 0,
            ValorIpi: decimal.TryParse(i.ValorIpi, out decimal vip) ? vip : 0,
            AliquotaIcms: 0,
            AliquotaIpi: 0
        )).ToList();

        var duplicatas = data.Duplicatas.Select(d => new DuplicataModel(
            Numero: d.Numero,
            Vencimento: DateTime.TryParse(d.Vencimento, out DateTime dv) ? dv : DateTime.MinValue,
            Valor: decimal.TryParse(d.Valor, out decimal val) ? val : 0
        )).ToList();

        var cobranca = new CobrancaModel(null, null, null, null, duplicatas);
        var adicionais = new DadosAdicionaisModel(data.InformacoesComplementares, null, null);

        return new DanfeModel(
            Emitente: emitente,
            DadosDanfe: dados,
            Destinatario: dest,
            ValorTotal: impostos.ValorNota,
            Cobranca: cobranca,
            Impostos: impostos,
            Transportador: transportador,
            Produtos: produtos,
            DadosAdicionais: adicionais
        );
    }

    private static int GetLinesCount(XGraphics gfx, string text, XFont font, double maxWidth)
    {
        if (string.IsNullOrEmpty(text)) return 1;

        string[] rawLines = text.Split('\n');
        int totalLines = 0;

        foreach (var rawLine in rawLines)
        {
            string[] words = rawLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0)
            {
                totalLines++;
                continue;
            }

            int lines = 1;
            string currentLine = "";

            foreach (string word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                double width = gfx.MeasureString(testLine, font).Width;
                if (width > maxWidth)
                {
                    lines++;
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }
            totalLines += lines;
        }

        return totalLines;
    }
}
