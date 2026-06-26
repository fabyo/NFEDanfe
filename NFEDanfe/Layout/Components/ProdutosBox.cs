using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class ProdutosBox : IComponent
{
    private static readonly float[] DashedGridPattern = [2f, 2f];
    private const float InternalGridThickness = 0.35f;

    private readonly IReadOnlyList<ProdutoModel> _produtos;
    private readonly bool _isLandscape;
    private readonly int _targetRows;

    public ProdutosBox(IReadOnlyList<ProdutoModel>? produtos, bool isLandscape, int targetRows)
    {
        _produtos = produtos ?? [];
        _isLandscape = isLandscape;
        _targetRows = Math.Max(0, targetRows);
    }

    public ProdutosBox(IReadOnlyList<ProdutoModel>? produtos, bool isLandscape = false)
        : this(produtos, isLandscape, isLandscape ? 5 : 20)
    {
    }

    public void Compose(IContainer container)
    {
        container.Border(DanfeTheme.EspessuraBorda)
            .BorderColor(DanfeTheme.CorBorda)
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    if (_isLandscape)
                    {
                        columns.ConstantColumn(70); // Código
                        columns.RelativeColumn();   // Descrição
                        columns.ConstantColumn(45); // NCM
                        columns.ConstantColumn(24); // CST
                        columns.ConstantColumn(26); // CFOP
                        columns.ConstantColumn(28); // UN
                        columns.ConstantColumn(45); // Quant
                        columns.ConstantColumn(52); // Valor unit
                        columns.ConstantColumn(52); // Valor total
                        columns.ConstantColumn(44); // Valor desc
                        columns.ConstantColumn(44); // BC ICMS
                        columns.ConstantColumn(48); // Valor ICMS
                        columns.ConstantColumn(42); // Valor IPI
                        columns.ConstantColumn(30); // Alíq. ICMS
                        columns.ConstantColumn(30); // Alíq. IPI
                    }
                    else
                    {
                        columns.ConstantColumn(58); // Código
                        columns.RelativeColumn();   // Descrição
                        columns.ConstantColumn(34); // NCM
                        columns.ConstantColumn(18); // CST
                        columns.ConstantColumn(20); // CFOP
                        columns.ConstantColumn(19); // UN
                        columns.ConstantColumn(34); // Quant
                        columns.ConstantColumn(38); // Valor unit
                        columns.ConstantColumn(40); // Valor total
                        columns.ConstantColumn(33); // Valor desc
                        columns.ConstantColumn(32); // BC ICMS
                        columns.ConstantColumn(36); // Valor ICMS
                        columns.ConstantColumn(30); // Valor IPI
                        columns.ConstantColumn(21); // Alíq. ICMS
                        columns.ConstantColumn(21); // Alíq. IPI
                    }
                });


                table.Header(header =>
                {
                    Header(header.Cell(), "CÓDIGO PRODUTO", isFirst: true);
                    Header(header.Cell(), "DESCRIÇÃO DO PRODUTO/SERVIÇO");
                    Header(header.Cell(), "NCM/SH", alignCenter: true);
                    Header(header.Cell(), "CST", alignCenter: true);
                    Header(header.Cell(), "CFOP", alignCenter: true);
                    Header(header.Cell(), "UN", alignCenter: true);
                    Header(header.Cell(), "QUANT", alignCenter: true);
                    Header(header.Cell(), "VALOR UNIT", alignRight: true);
                    Header(header.Cell(), "VALOR TOTAL", alignRight: true);
                    Header(header.Cell(), "VALOR DESC", alignRight: true);
                    Header(header.Cell(), "BC ICMS", alignRight: true);
                    Header(header.Cell(), "VALOR ICMS", alignRight: true);
                    Header(header.Cell(), "VALOR IPI", alignRight: true);
                    Header(header.Cell(), "ALÍQ. ICMS", alignRight: true);
                    Header(header.Cell(), "ALÍQ. IPI", alignRight: true);
                });

                foreach (ProdutoModel prod in _produtos)
                {
                    ProductRow(table, prod);
                }

                for (int i = _produtos.Count; i < _targetRows; i++)
                {
                    EmptyRow(table);
                }
            });
    }

    private static void ProductRow(TableDescriptor table, ProdutoModel prod)
    {
        Data(table.Cell(), prod.Codigo, isFirst: true);
        Data(table.Cell(), prod.Descricao);
        Data(table.Cell(), prod.Ncm, alignCenter: true);
        Data(table.Cell(), prod.CstCsosn, alignCenter: true);
        Data(table.Cell(), prod.Cfop, alignCenter: true);
        Data(table.Cell(), prod.Unidade, alignCenter: true);
        Data(table.Cell(), FormatQuantity(prod.Quantidade), alignRight: true);
        Data(table.Cell(), FormatPrice(prod.ValorUnitario), alignRight: true);
        Data(table.Cell(), DocumentFormatter.Money(prod.ValorTotal), alignRight: true);
        Data(table.Cell(), DocumentFormatter.Money(prod.ValorDesconto), alignRight: true);
        Data(table.Cell(), DocumentFormatter.Money(prod.BaseCalculoIcms), alignRight: true);
        Data(table.Cell(), DocumentFormatter.Money(prod.ValorIcms), alignRight: true);
        Data(table.Cell(), DocumentFormatter.Money(prod.ValorIpi), alignRight: true);
        Data(table.Cell(), FormatPercentage(prod.AliquotaIcms), alignRight: true);
        Data(table.Cell(), FormatPercentage(prod.AliquotaIpi), alignRight: true);
    }

    private static void EmptyRow(TableDescriptor table)
    {
        Data(table.Cell(), string.Empty, isFirst: true);
        for (int i = 1; i < 15; i++)
        {
            Data(table.Cell(), string.Empty);
        }
    }

    private static void Header(IContainer container, string text, bool isFirst = false, bool alignRight = false, bool alignCenter = false)
    {
        Cell(container, text, isHeader: true, isFirstColumn: isFirst, alignRight: alignRight, alignCenter: alignCenter, drawHorizontalBorder: true);
    }

    private static void Data(IContainer container, string text, bool isFirst = false, bool alignRight = false, bool alignCenter = false, bool drawHorizontalBorder = true, bool drawVerticalBorder = true)
    {
        Cell(container, text, isHeader: false, isFirstColumn: isFirst, alignRight: alignRight, alignCenter: alignCenter, drawHorizontalBorder: drawHorizontalBorder, drawVerticalBorder: drawVerticalBorder);
    }

    private static void Cell(IContainer container, string text, bool isHeader, bool isFirstColumn, bool alignRight = false, bool alignCenter = false, bool drawHorizontalBorder = true, bool drawVerticalBorder = true)
    {
        container.Layers(layers =>
        {
            layers.PrimaryLayer().Element(content =>
            {
                IContainer cell = content;
                if (isHeader)
                {
                    cell = cell.Background(DanfeTheme.CorFundoTabela);
                }

                cell = cell
                    .PaddingHorizontal(2)
                    .PaddingVertical(1);

                IContainer alignedCell = cell.AlignMiddle();

                if (alignRight)
                {
                    alignedCell = alignedCell.AlignRight();
                }
                else if (alignCenter)
                {
                    alignedCell = alignedCell.AlignCenter();
                }

                alignedCell.Text(text)
                    .FontFamily(DanfeTheme.FontePadrao)
                    .FontSize(isHeader ? 4.2f : 5f)
                    .Style(isHeader ? TextStyle.Default.Bold() : TextStyle.Default);
            });

            if (isHeader)
            {
                layers.Layer().AlignTop().ExtendHorizontal().LineHorizontal(InternalGridThickness).LineColor(Colors.Grey.Lighten1);
                layers.Layer().AlignBottom().ExtendHorizontal().LineHorizontal(InternalGridThickness).LineColor(Colors.Grey.Lighten1);
                layers.Layer().AlignRight().ExtendVertical().LineVertical(InternalGridThickness).LineColor(Colors.Grey.Lighten1);
            }
            else
            {
                if (drawHorizontalBorder)
                {
                    layers.Layer().AlignBottom().ExtendHorizontal().LineHorizontal(InternalGridThickness).LineColor(Colors.Grey.Lighten1).LineDashPattern(DashedGridPattern);
                }
                if (drawVerticalBorder)
                {
                    layers.Layer().AlignRight().ExtendVertical().LineVertical(InternalGridThickness).LineColor(Colors.Grey.Lighten1).LineDashPattern(DashedGridPattern);
                }
            }
        });
    }

    private static string FormatQuantity(decimal value)
    {
        return value.ToString("N4", DocumentFormatter.Brazil);
    }

    private static string FormatPrice(decimal value)
    {
        return value % 0.01m != 0
            ? value.ToString("N4", DocumentFormatter.Brazil)
            : value.ToString("N2", DocumentFormatter.Brazil);
    }

    private static string FormatPercentage(decimal value)
    {
        return value.ToString("N2", DocumentFormatter.Brazil);
    }
}

public static class ProdutosBoxExtensions
{
    public static void ProdutosBox(this IContainer container, IReadOnlyList<ProdutoModel>? produtos, bool isLandscape, int targetRows)
    {
        container.Component(new ProdutosBox(produtos, isLandscape, targetRows));
    }

    public static void ProdutosBox(this IContainer container, IReadOnlyList<ProdutoModel>? produtos, bool isLandscape = false)
    {
        container.Component(new ProdutosBox(produtos, isLandscape));
    }
}
