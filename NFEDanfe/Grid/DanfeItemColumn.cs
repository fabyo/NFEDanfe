using PdfSharp.Drawing;
using System.Collections.Generic;

namespace NFEDanfe.Grid;

/// <summary>Definição de uma coluna da grade de itens do DANFE em pontos.</summary>
public sealed class DanfeItemColumn
{
    /// <summary>Texto do cabeçalho da coluna.</summary>
    public string Header { get; init; } = string.Empty;

    /// <summary>Largura da coluna em pontos. 0 significa largura relativa (ocupa o espaço restante).</summary>
    public double WidthPt { get; init; }

    /// <summary>Alinhamento horizontal do texto na coluna.</summary>
    public XStringAlignment Alignment { get; init; } = XStringAlignment.Near;

    /// <summary>Colunas padrão para modo retrato (retrocompatibilidade).</summary>
    public static IReadOnlyList<DanfeItemColumn> Default => GetDefaultColumns(false);

    /// <summary>Retorna as colunas padrão para o modo retrato ou paisagem de acordo com correto.png.</summary>
    public static IReadOnlyList<DanfeItemColumn> GetDefaultColumns(bool isLandscape)
    {
        if (isLandscape)
        {
            return new[]
            {
                new DanfeItemColumn { Header = "CÓDIGO PRODUTO",                  WidthPt = 70, Alignment = XStringAlignment.Near },
                new DanfeItemColumn { Header = "DESCRIÇÃO DO PRODUTO/SERVIÇO",    WidthPt = 0,  Alignment = XStringAlignment.Near },
                new DanfeItemColumn { Header = "NCM/SH",            WidthPt = 45, Alignment = XStringAlignment.Center },
                new DanfeItemColumn { Header = "CST",               WidthPt = 24, Alignment = XStringAlignment.Center },
                new DanfeItemColumn { Header = "CFOP",              WidthPt = 26, Alignment = XStringAlignment.Center },
                new DanfeItemColumn { Header = "UN",                WidthPt = 28, Alignment = XStringAlignment.Center },
                new DanfeItemColumn { Header = "QUANT",             WidthPt = 45, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "VALOR UNIT",         WidthPt = 52, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "VALOR TOTAL",         WidthPt = 52, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "VALOR DESC",         WidthPt = 44, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "BC ICMS",           WidthPt = 44, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "VALOR ICMS",          WidthPt = 48, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "VALOR IPI",           WidthPt = 42, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "ALÍQ. ICMS",        WidthPt = 30, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "ALÍQ. IPI",         WidthPt = 30, Alignment = XStringAlignment.Far }
            };
        }
        else
        {
            return new[]
            {
                new DanfeItemColumn { Header = "CÓDIGO PRODUTO",                  WidthPt = 58, Alignment = XStringAlignment.Near },
                new DanfeItemColumn { Header = "DESCRIÇÃO DO PRODUTO/SERVIÇO",    WidthPt = 0,  Alignment = XStringAlignment.Near },
                new DanfeItemColumn { Header = "NCM/SH",            WidthPt = 34, Alignment = XStringAlignment.Center },
                new DanfeItemColumn { Header = "CST",               WidthPt = 18, Alignment = XStringAlignment.Center },
                new DanfeItemColumn { Header = "CFOP",              WidthPt = 20, Alignment = XStringAlignment.Center },
                new DanfeItemColumn { Header = "UN",                WidthPt = 19, Alignment = XStringAlignment.Center },
                new DanfeItemColumn { Header = "QUANT",             WidthPt = 34, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "VALOR UNIT",         WidthPt = 38, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "VALOR TOTAL",         WidthPt = 40, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "VALOR DESC",         WidthPt = 33, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "BC ICMS",           WidthPt = 32, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "VALOR ICMS",          WidthPt = 36, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "VALOR IPI",           WidthPt = 30, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "ALÍQ. ICMS",        WidthPt = 21, Alignment = XStringAlignment.Far },
                new DanfeItemColumn { Header = "ALÍQ. IPI",         WidthPt = 21, Alignment = XStringAlignment.Far }
            };
        }
    }
}
