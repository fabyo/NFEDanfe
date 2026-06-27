using System;
using PdfSharp.Drawing;
using NFEDanfe.Builder;
using NFEDanfe.Domain.Models;
using NFEDanfe.Domain.Formatting;

namespace NFEDanfe.Blocks;

/// <summary>Bloco de transporte do DANFE.</summary>
internal sealed class TransportBlock
{
    private static double Mm(double mm) => mm * 2.834645;

    private readonly DanfeModel _model;

    internal TransportBlock(DanfeModel model)
    {
        _model = model;
    }

    /// <summary>Desenha o bloco de transporte. Retorna a altura consumida.</summary>
    internal double Draw(XGraphics gfx, DanfeStyleCatalog styles, double x, double y, double width, bool isLandscape = false)
    {
        double titleH = 12.0;
        double fieldH = isLandscape ? 18.0 : DanfeField.DefaultHeight(styles);
        double height = titleH + fieldH * 3.0;
        var valueFontOverride = isLandscape ? new XFont(DanfeFontResolver.FamilyName, 7.0, XFontStyleEx.Regular) : null;

        // 1. Título "TRANSPORTADOR / VOLUMES TRANSPORTADOS" com fundo cinza claro e borda
        var grayBrush = new XSolidBrush(XColor.FromArgb(224, 224, 224));
        gfx.DrawRectangle(styles.BorderPen, grayBrush, x, y, width, titleH);

        var titleFormat = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Center };
        var titleFont = new XFont(DanfeFontResolver.FamilyName, 6.0, XFontStyleEx.Bold);
        gfx.DrawString("TRANSPORTADOR / VOLUMES TRANSPORTADOS", titleFont, styles.TextBrush,
            new XRect(x + 4, y, width - 8, titleH), titleFormat);
        y += titleH;

        var transp = _model.Transportador;

        // 2. Linha 1: RAZÃO SOCIAL (4/12), FRETE POR CONTA (3/12), CÓDIGO ANTT (1/12), PLACA (1/12), UF (1/12), CNPJ/CPF (2/12)
        double wRazao = width * 4.0 / 12.0;
        double wFrete = width * 3.0 / 12.0;
        double wAntt = width * 1.0 / 12.0;
        double wPlaca = width * 1.3 / 12.0;
        double wUfPlaca = width * 0.7 / 12.0;
        double wCnpj = width * 2.0 / 12.0;

        double currentX = x;
        new DanfeField("RAZÃO SOCIAL / NOME", transp?.RazaoSocial.ToUpper() ?? "", 33.33)
            .Draw(gfx, styles, currentX, y, wRazao, fieldH, valueFontOverride: valueFontOverride);
        currentX += wRazao;

        new DanfeField("FRETE POR CONTA", transp?.FretePorConta.ToUpper() ?? "", 25.0)
            .Draw(gfx, styles, currentX, y, wFrete, fieldH, valueFontOverride: valueFontOverride);
        currentX += wFrete;

        new DanfeField("CÓDIGO ANTT", transp?.CodigoAntt.ToUpper() ?? "", 8.33)
            .Draw(gfx, styles, currentX, y, wAntt, fieldH, valueFontOverride: valueFontOverride);
        currentX += wAntt;

        new DanfeField("PLACA DO VEÍCULO", transp?.PlacaVeiculo.ToUpper() ?? "", 10.83)
            .Draw(gfx, styles, currentX, y, wPlaca, fieldH, valueFontOverride: valueFontOverride);
        currentX += wPlaca;

        new DanfeField("UF", transp?.UfPlaca.ToUpper() ?? "", 5.83)
            .Draw(gfx, styles, currentX, y, wUfPlaca, fieldH, valueFontOverride: valueFontOverride);
        currentX += wUfPlaca;

        new DanfeField("CNPJ / CPF", DocumentFormatter.CnpjCpf(transp?.Documento), 16.67)
            .Draw(gfx, styles, currentX, y, wCnpj, fieldH, valueFontOverride: valueFontOverride);

        y += fieldH;

        // 3. Linha 2: ENDEREÇO (5/12), MUNICÍPIO (4.3/12), UF (0.7/12), INSCRIÇÃO ESTADUAL (2/12)
        double wAddr = width * 5.0 / 12.0;
        double wMun = width * 4.3 / 12.0;
        double wUf = width * 0.7 / 12.0;
        double wIe = width * 2.0 / 12.0;

        currentX = x;
        new DanfeField("ENDEREÇO", transp?.EnderecoCompleto.ToUpper() ?? "", 41.67)
            .Draw(gfx, styles, currentX, y, wAddr, fieldH, valueFontOverride: valueFontOverride);
        currentX += wAddr;

        new DanfeField("MUNICÍPIO", transp?.Municipio.ToUpper() ?? "", 35.83)
            .Draw(gfx, styles, currentX, y, wMun, fieldH, valueFontOverride: valueFontOverride);
        currentX += wMun;

        new DanfeField("UF", transp?.Uf.ToUpper() ?? "", 5.83)
            .Draw(gfx, styles, currentX, y, wUf, fieldH, valueFontOverride: valueFontOverride);
        currentX += wUf;

        new DanfeField("INSCRIÇÃO ESTADUAL", transp?.InscricaoEstadual.ToUpper() ?? "", 16.67)
            .Draw(gfx, styles, currentX, y, wIe, fieldH, valueFontOverride: valueFontOverride);

        y += fieldH;

        // 4. Linha 3: QUANTIDADE (1.5/12), ESPÉCIE (2/12), MARCA (2/12), NUMERAÇÃO (1.5/12), PESO BRUTO (2.5/12), PESO LÍQUIDO (2.5/12)
        double wQtd = width * 1.5 / 12.0;
        double wEsp = width * 2.0 / 12.0;
        double wMarca = width * 2.0 / 12.0;
        double wNum = width * 1.5 / 12.0;
        double wBruto = width * 2.5 / 12.0;
        double wLiq = width * 2.5 / 12.0;

        currentX = x;
        string qtdVal = transp?.QuantidadeVolumes != null ? DocumentFormatter.Decimal(transp.QuantidadeVolumes) : "";
        new DanfeField("QUANTIDADE", qtdVal, 12.5)
            .Draw(gfx, styles, currentX, y, wQtd, fieldH, valueFontOverride: valueFontOverride);
        currentX += wQtd;

        new DanfeField("ESPÉCIE", transp?.Especie.ToUpper() ?? "", 16.67)
            .Draw(gfx, styles, currentX, y, wEsp, fieldH, valueFontOverride: valueFontOverride);
        currentX += wEsp;

        new DanfeField("MARCA", transp?.Marca.ToUpper() ?? "", 16.67)
            .Draw(gfx, styles, currentX, y, wMarca, fieldH, valueFontOverride: valueFontOverride);
        currentX += wMarca;

        new DanfeField("NUMERAÇÃO", transp?.Numeracao.ToUpper() ?? "", 12.5)
            .Draw(gfx, styles, currentX, y, wNum, fieldH, valueFontOverride: valueFontOverride);
        currentX += wNum;

        string brutoVal = transp?.PesoBruto != null ? DocumentFormatter.Decimal(transp.PesoBruto) : "";
        new DanfeField("PESO BRUTO", brutoVal, 20.83)
            .Draw(gfx, styles, currentX, y, wBruto, fieldH, valueFontOverride: valueFontOverride);
        currentX += wBruto;

        string liqVal = transp?.PesoLiquido != null ? DocumentFormatter.Decimal(transp.PesoLiquido) : "";
        new DanfeField("PESO LÍQUIDO", liqVal, 20.83)
            .Draw(gfx, styles, currentX, y, wLiq, fieldH, valueFontOverride: valueFontOverride);

        return height;
    }
}
