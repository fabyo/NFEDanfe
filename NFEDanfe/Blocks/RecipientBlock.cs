using System;
using PdfSharp.Drawing;
using NFEDanfe.Builder;
using NFEDanfe.Domain.Models;
using NFEDanfe.Domain.Formatting;

namespace NFEDanfe.Blocks;

/// <summary>Bloco do destinatário/remetente.</summary>
internal sealed class RecipientBlock
{
    private static double Mm(double mm) => mm * 2.834645;

    private readonly DanfeModel _model;

    internal RecipientBlock(DanfeModel model)
    {
        _model = model;
    }

    /// <summary>Desenha o bloco do destinatário. Retorna a altura consumida.</summary>
    internal double Draw(XGraphics gfx, DanfeStyleCatalog styles, double x, double y, double width, bool isLandscape = false)
    {
        // Altura do título + 3 linhas de campos
        double titleH = 12.0;
        double fieldH = isLandscape ? 18.0 : DanfeField.DefaultHeight(styles);
        double height = titleH + fieldH * 3.0;
        var valueFontOverride = isLandscape ? new XFont(DanfeFontResolver.FamilyName, 7.0, XFontStyleEx.Regular) : null;

        // 1. Título "DESTINATÁRIO / REMETENTE" com fundo cinza claro e borda
        var grayBrush = new XSolidBrush(XColor.FromArgb(224, 224, 224));
        gfx.DrawRectangle(styles.BorderPen, grayBrush, x, y, width, titleH);

        var titleFormat = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Center };
        var titleFont = new XFont(DanfeFontResolver.FamilyName, 6.0, XFontStyleEx.Bold);
        gfx.DrawString("DESTINATÁRIO / REMETENTE", titleFont, styles.TextBrush,
            new XRect(x + 4, y, width - 8, titleH), titleFormat);
        y += titleH;

        // 2. Linha 1: Nome/Razão Social (7/12), CNPJ/CPF (3/12), Data Emissão (2/12)
        double wRazao = width * 7.0 / 12.0;
        double wCnpj = width * 3.0 / 12.0;
        double wDataEmi = width * 2.0 / 12.0;

        double currentX = x;
        new DanfeField("NOME / RAZÃO SOCIAL", _model.Destinatario.RazaoSocial.ToUpper(), 58.33)
            .Draw(gfx, styles, currentX, y, wRazao, fieldH, valueFontOverride: valueFontOverride);
        currentX += wRazao;

        new DanfeField("CNPJ / CPF", DocumentFormatter.CnpjCpf(_model.Destinatario.Documento), 25.0)
            .Draw(gfx, styles, currentX, y, wCnpj, fieldH, valueFontOverride: valueFontOverride);
        currentX += wCnpj;

        new DanfeField("DATA DA EMISSÃO", _model.DadosDanfe.DataEmissao.ToString("dd/MM/yyyy"), 16.67)
            .Draw(gfx, styles, currentX, y, wDataEmi, fieldH, valueFontOverride: valueFontOverride);

        y += fieldH;

        // 3. Linha 2: Endereço (6/12), Bairro (2/12), CEP (2/12), Data Entrada/Saída (2/12)
        double wAddr = width * 6.0 / 12.0;
        double wBairro = width * 2.0 / 12.0;
        double wCep = width * 2.0 / 12.0;
        double wDataEnt = width * 2.0 / 12.0;

        currentX = x;
        string enderecoVal = DocumentFormatter.Address(_model.Destinatario.Endereco).ToUpper();
        new DanfeField("ENDEREÇO", enderecoVal, 50.0)
            .Draw(gfx, styles, currentX, y, wAddr, fieldH, valueFontOverride: valueFontOverride);
        currentX += wAddr;

        new DanfeField("BAIRRO / DISTRITO", _model.Destinatario.Endereco.Bairro.ToUpper(), 16.67)
            .Draw(gfx, styles, currentX, y, wBairro, fieldH, valueFontOverride: valueFontOverride);
        currentX += wBairro;

        new DanfeField("CEP", DocumentFormatter.Cep(_model.Destinatario.Endereco.Cep), 16.67)
            .Draw(gfx, styles, currentX, y, wCep, fieldH, valueFontOverride: valueFontOverride);
        currentX += wCep;

        string dtEntSai = _model.DadosDanfe.DataEntradaSaida?.ToString("dd/MM/yyyy") ?? "";
        new DanfeField("DATA DA ENTRADA / SAÍDA", dtEntSai, 16.66)
            .Draw(gfx, styles, currentX, y, wDataEnt, fieldH, valueFontOverride: valueFontOverride);

        y += fieldH;

        // 4. Linha 3: Município (5/12), Fone/Fax (2/12), UF (1/12), Inscrição Estadual (2/12), Hora Entrada/Saída (2/12)
        double wMun = width * 5.0 / 12.0;
        double wFone = width * 2.0 / 12.0;
        double wUf = width * 1.0 / 12.0;
        double wIe = width * 2.0 / 12.0;
        double wHoraEnt = width * 2.0 / 12.0;

        currentX = x;
        new DanfeField("MUNICÍPIO", _model.Destinatario.Endereco.Municipio.ToUpper(), 41.67)
            .Draw(gfx, styles, currentX, y, wMun, fieldH, valueFontOverride: valueFontOverride);
        currentX += wMun;

        new DanfeField("FONE / FAX", DocumentFormatter.Phone(_model.Destinatario.Telefone), 16.67)
            .Draw(gfx, styles, currentX, y, wFone, fieldH, valueFontOverride: valueFontOverride);
        currentX += wFone;

        new DanfeField("UF", _model.Destinatario.Endereco.Uf.ToUpper(), 8.33)
            .Draw(gfx, styles, currentX, y, wUf, fieldH, valueFontOverride: valueFontOverride);
        currentX += wUf;

        new DanfeField("INSCRIÇÃO ESTADUAL", _model.Destinatario.InscricaoEstadual.ToUpper(), 16.67)
            .Draw(gfx, styles, currentX, y, wIe, fieldH, valueFontOverride: valueFontOverride);
        currentX += wIe;

        string hrEntSai = _model.DadosDanfe.DataEntradaSaida?.ToString("HH:mm:ss") ?? "";
        new DanfeField("HORA DA SAÍDA", hrEntSai, 16.66)
            .Draw(gfx, styles, currentX, y, wHoraEnt, fieldH, valueFontOverride: valueFontOverride);

        return height;
    }
}
