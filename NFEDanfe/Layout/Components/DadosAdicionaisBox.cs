using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class DadosAdicionaisBox : IComponent
{
    private readonly DadosAdicionaisModel _dados;

    public DadosAdicionaisBox(DadosAdicionaisModel dados)
    {
        _dados = dados;
    }

    public void Compose(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Background(Colors.Grey.Lighten3)
                .Border(DanfeTheme.EspessuraBorda)
                .BorderColor(DanfeTheme.CorBorda)
                .PaddingLeft(4)
                .PaddingVertical(1)
                .Text("DADOS ADICIONAIS")
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(DanfeTheme.TamanhoFonteLabel + 1f)
                .Bold();

            column.Item().MinHeight(70).Row(row =>
            {
                row.RelativeItem(8)
                    .BorderLeft(DanfeTheme.EspessuraBorda)
                    .BorderBottom(DanfeTheme.EspessuraBorda)
                    .BorderColor(DanfeTheme.CorBorda)
                    .Padding(4)
                    .Column(c =>
                    {
                        c.Item().Text("INFORMAÇÕES COMPLEMENTARES")
                            .FontFamily(DanfeTheme.FontePadrao)
                            .FontSize(DanfeTheme.TamanhoFonteLabel)
                            .Bold();

                        var infComplRaw = _dados.InformacoesComplementares ?? string.Empty;
                        var infComplLines = infComplRaw.Split(';', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList();

                        var emailRegex = new Regex(
                            @"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}",
                            RegexOptions.Compiled);

                        c.Item().PaddingTop(2)
                            .Text(t =>
                            {
                                t.DefaultTextStyle(s => s
                                    .FontFamily(DanfeTheme.FontePadrao)
                                    .FontSize(DanfeTheme.TamanhoFonteSubtitulo));

                                for (int li = 0; li < infComplLines.Count; li++)
                                {
                                    if (li > 0) t.Span("\n");
                                    var linha = infComplLines[li];
                                    var lastIdx = 0;
                                    foreach (Match m in emailRegex.Matches(linha))
                                    {
                                        if (m.Index > lastIdx)
                                            t.Span(linha.Substring(lastIdx, m.Index - lastIdx));
                                        t.Span(m.Value).Bold();
                                        lastIdx = m.Index + m.Length;
                                    }
                                    if (lastIdx < linha.Length)
                                        t.Span(linha.Substring(lastIdx));
                                }
                            });
                    });

                row.RelativeItem(4)
                    .BorderLeft(DanfeTheme.EspessuraBorda)
                    .BorderRight(DanfeTheme.EspessuraBorda)
                    .BorderBottom(DanfeTheme.EspessuraBorda)
                    .BorderColor(DanfeTheme.CorBorda)
                    .Padding(4)
                    .Column(c =>
                    {
                        c.Item().Text("RESERVADO AO FISCO")
                            .FontFamily(DanfeTheme.FontePadrao)
                            .FontSize(DanfeTheme.TamanhoFonteLabel)
                            .Bold();

                        var infFiscoRaw = _dados.InformacoesFisco ?? string.Empty;
                        var infFiscoLines = infFiscoRaw.Split(';', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .Where(x => !string.IsNullOrEmpty(x));
                        var infFiscoFormatted = string.Join(Environment.NewLine, infFiscoLines);

                        c.Item().PaddingTop(2)
                            .Text(infFiscoFormatted)
                            .FontFamily(DanfeTheme.FontePadrao)
                            .FontSize(DanfeTheme.TamanhoFonteSubtitulo);
                    });
            });
        });
    }
}

public static class DadosAdicionaisBoxExtensions
{
    public static void DadosAdicionaisBox(this IContainer container, DadosAdicionaisModel dados)
    {
        container.Component(new DadosAdicionaisBox(dados));
    }
}
