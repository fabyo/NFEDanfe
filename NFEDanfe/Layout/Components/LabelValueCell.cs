using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class LabelValueCell : IComponent
{
    private readonly string _label;
    private readonly string _value;
    private readonly bool _boldValue;
    private readonly float? _valueSize;
    private readonly bool _topBorder;
    private readonly bool _leftBorder;
    private readonly bool _bottomBorder;
    private readonly bool _rightBorder;
    private readonly bool _alignRightValue;

    public LabelValueCell(
        string label,
        string value,
        bool boldValue = false,
        float? valueSize = null,
        bool topBorder = true,
        bool leftBorder = true,
        bool bottomBorder = true,
        bool rightBorder = true,
        bool alignRightValue = false)
    {
        _label = label;
        _value = value;
        _boldValue = boldValue;
        _valueSize = valueSize;
        _topBorder = topBorder;
        _leftBorder = leftBorder;
        _bottomBorder = bottomBorder;
        _rightBorder = rightBorder;
        _alignRightValue = alignRightValue;
    }

    public void Compose(IContainer container)
    {
        container
            .BorderTop(_topBorder ? DanfeTheme.EspessuraBorda : 0)
            .BorderLeft(_leftBorder ? DanfeTheme.EspessuraBorda : 0)
            .BorderBottom(_bottomBorder ? DanfeTheme.EspessuraBorda : 0)
            .BorderRight(_rightBorder ? DanfeTheme.EspessuraBorda : 0)
            .BorderColor(DanfeTheme.CorBorda)
            .PaddingHorizontal(DanfeTheme.PaddingInternoHorizontal)
            .PaddingVertical(DanfeTheme.PaddingInternoVertical)
            .Column(column =>
            {
                column.Item().Text(_label.ToUpper())
                    .FontFamily(DanfeTheme.FontePadrao)
                    .FontSize(DanfeTheme.TamanhoFonteLabel);

                IContainer itemContainer = column.Item().PaddingTop(1);
                if (_alignRightValue)
                {
                    itemContainer = itemContainer.AlignRight();
                }

                if (_boldValue)
                {
                    itemContainer.Text(_value)
                        .FontFamily(DanfeTheme.FontePadrao)
                        .FontSize(_valueSize ?? DanfeTheme.TamanhoFonteValor)
                        .Bold();
                }
                else
                {
                    itemContainer.Text(_value)
                        .FontFamily(DanfeTheme.FontePadrao)
                        .FontSize(_valueSize ?? DanfeTheme.TamanhoFonteValor);
                }
            });
    }
}

public static class LabelValueCellExtensions
{
    public static void LabelValueCell(
        this IContainer container,
        string label,
        string value,
        bool boldValue = false,
        float? valueSize = null,
        bool top = true,
        bool left = true,
        bool bottom = true,
        bool right = true,
        bool alignRightValue = false)
    {
        container.Component(new LabelValueCell(label, value, boldValue, valueSize, top, left, bottom, right, alignRightValue));
    }
}
