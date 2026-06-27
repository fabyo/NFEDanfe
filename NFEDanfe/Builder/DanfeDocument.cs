using PdfSharp.Drawing;
using NFEDanfe.Grid;
using NFEDanfe.Pagination;

namespace NFEDanfe.Builder;

/// <summary>Opções de configuração do documento DANFE.</summary>
public sealed class DanfeDocumentOptions
{
    /// <summary>Configuração de fontes.</summary>
    public DanfeFontConfig Font { get; } = new();

    /// <summary>Margens do documento.</summary>
    public DanfeMargins Margins { get; set; } = new(5);

    /// <summary>Modo de impressão.</summary>
    public DanfePageMode Mode { get; set; } = DanfePageMode.Portrait;

    /// <summary>Número da NF-e para cabeçalhos de continuação.</summary>
    public string NfeNumero { get; set; } = string.Empty;

    /// <summary>Série da NF-e para cabeçalhos de continuação.</summary>
    public string NfeSerie { get; set; } = string.Empty;
}

/// <summary>Builder fluente para construção do documento DANFE.</summary>
public sealed class DanfeDocument : IDanfeDocument
{
    private readonly DanfeDocumentOptions _options = new();
    private readonly List<DanfePageBuilder> _pages = new();

    private DanfeDocument() { }

    /// <summary>Cria uma nova instância do builder DANFE.</summary>
    public static IDanfeDocument Create() => new DanfeDocument();

    /// <inheritdoc />
    public IDanfeDocument Configure(Action<DanfeDocumentOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(_options);
        return this;
    }

    /// <inheritdoc />
    public IDanfeDocument AddPage(Action<IDanfePage> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var page = new DanfePageBuilder();
        configure(page);
        _pages.Add(page);
        return this;
    }

    /// <inheritdoc />
    public byte[] BuildAsBytes()
    {
        using var engine = new DanfeEngine(_options.Font, _options.Margins, _options.Mode);
        foreach (var page in _pages)
        {
            page.Render(engine, _options);
        }
        return engine.Build();
    }

    /// <inheritdoc />
    public void BuildAsStream(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using var engine = new DanfeEngine(_options.Font, _options.Margins, _options.Mode);
        foreach (var page in _pages)
        {
            page.Render(engine, _options);
        }
        engine.Build(stream);
    }
}

internal sealed class DanfePageBuilder : IDanfePage
{
    private readonly List<DanfeSectionData> _sections = new();
    private DanfeItemGridData? _itemGridData;
    private int _itemGridInsertIndex = -1;

    public IDanfePage Section(string title, Action<IDanfeSection> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var section = new DanfeSectionBuilder(title);
        configure(section);
        _sections.Add(section.Build());
        return this;
    }

    public IDanfePage ItemGrid(
        IReadOnlyList<DanfeItemRow> items,
        double fixedUpperZoneHeightPt,
        double fixedLowerZoneHeightPt,
        double fixedLowerZoneContinuationPt,
        Action<IReadOnlyList<DanfeItemColumn>>? configureColumns = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        var columns = DanfeItemColumn.Default;
        configureColumns?.Invoke(columns);

        _itemGridData = new DanfeItemGridData
        {
            Items = items,
            Columns = columns,
            FixedUpperZoneHeightPt = fixedUpperZoneHeightPt,
            FixedLowerZoneHeightPt = fixedLowerZoneHeightPt,
            FixedLowerZoneContinuationPt = fixedLowerZoneContinuationPt
        };
        _itemGridInsertIndex = _sections.Count;
        return this;
    }

    internal void Render(DanfeEngine engine, DanfeDocumentOptions options)
    {
        const double itemRowHeight = 14.0;

        if (_itemGridData is not null)
        {
            RenderWithItemGrid(engine, options, itemRowHeight);
        }
        else
        {
            RenderSinglePage(engine);
        }
    }

    private void RenderSinglePage(DanfeEngine engine)
    {
        using var gfx = engine.BeginPage();
        double currentY = engine.MarginTopPt;

        foreach (var section in _sections)
        {
            currentY = RenderSection(gfx, engine, section, currentY);
        }
    }

    private void RenderWithItemGrid(DanfeEngine engine, DanfeDocumentOptions options, double itemRowHeight)
    {
        var data = _itemGridData!;

        // Calcular alturas dinâmicas das linhas usando o método GetLinesCount
        var valueFont = new XFont(DanfeFontResolver.FamilyName, 5.0, XFontStyleEx.Regular);
        double descColumnWidth = options.Mode == DanfePageMode.Landscape ? 140.0 : 151.0;
        
        var tempDoc = new PdfSharp.Pdf.PdfDocument();
        var tempPage = tempDoc.AddPage();
        var rowHeights = new List<double>();
        using (var measureGfx = XGraphics.FromPdfPage(tempPage))
        {
            foreach (var item in data.Items)
            {
                int lines = GetLinesCount(measureGfx, item.Descricao, valueFont, descColumnWidth);
                double h = Math.Max(11.0, lines * 6.5 + 4.5);
                rowHeights.Add(h);
            }
        }

        double availableHeightPage1 = engine.UsableHeight - data.FixedUpperZoneHeightPt - data.FixedLowerZoneHeightPt;
        double continuationHeaderHeight = 22.0; // 11.0 * 2
        double availableHeightContinuation = engine.UsableHeight - data.FixedLowerZoneContinuationPt - continuationHeaderHeight;

        var plan = DanfePaginator.Calculate(rowHeights, availableHeightPage1, availableHeightContinuation);

        var grid = new DanfeItemGrid();

        // Page 1
        {
            using var gfx = engine.BeginPage();
            double currentY = engine.MarginTopPt;

            for (int i = 0; i < _itemGridInsertIndex && i < _sections.Count; i++)
            {
                currentY = RenderSection(gfx, engine, _sections[i], currentY);
            }

            if (plan.Slices.Count > 0)
            {
                currentY = grid.Draw(gfx, engine.Styles, plan.Slices[0], data.Items, rowHeights, data.Columns,
                    engine.MarginLeftPt, currentY, engine.UsableWidth, drawColumnHeaders: true);
            }

            for (int i = _itemGridInsertIndex; i < _sections.Count; i++)
            {
                currentY = RenderSection(gfx, engine, _sections[i], currentY);
            }
        }

        // Continuation pages
        for (int p = 1; p < plan.TotalPages; p++)
        {
            using var gfx = engine.BeginPage();
            double currentY = engine.MarginTopPt;

            var header = new DanfeContinuationHeader(
                options.NfeNumero, options.NfeSerie, p + 1, plan.TotalPages);
            header.Draw(gfx, engine.Styles, engine.MarginLeftPt, currentY, engine.UsableWidth, continuationHeaderHeight);
            currentY += continuationHeaderHeight;

            if (p < plan.Slices.Count)
            {
                currentY = grid.Draw(gfx, engine.Styles, plan.Slices[p], data.Items, rowHeights, data.Columns,
                    engine.MarginLeftPt, currentY, engine.UsableWidth, drawColumnHeaders: true);
            }

            if (p == plan.TotalPages - 1)
            {
                for (int i = _itemGridInsertIndex; i < _sections.Count; i++)
                {
                    currentY = RenderSection(gfx, engine, _sections[i], currentY);
                }
            }
        }
    }

    private static double RenderSection(XGraphics gfx, DanfeEngine engine, DanfeSectionData section, double currentY)
    {
        double sectionX = engine.MarginLeftPt;
        double sectionWidth = engine.UsableWidth;

        if (!string.IsNullOrEmpty(section.Title))
        {
            var titleFormat = new XStringFormat
            {
                Alignment = XStringAlignment.Near,
                LineAlignment = XLineAlignment.Center
            };
            double titleHeight = engine.Styles.LabelFont.Size + 4.0;
            var titleRect = new XRect(sectionX + 1.5, currentY, sectionWidth, titleHeight);
            gfx.DrawString(section.Title, engine.Styles.LabelFont, engine.Styles.TextBrush, titleRect, titleFormat);
            currentY += titleHeight;
        }

        foreach (var row in section.Rows)
        {
            double rowHeight = DanfeField.DefaultHeight(engine.Styles);
            double fieldX = sectionX;

            foreach (var field in row.Fields)
            {
                double fieldWidth = sectionWidth * field.WidthPct / 100.0;
                field.Draw(gfx, engine.Styles, fieldX, currentY, fieldWidth, rowHeight);
                fieldX += fieldWidth;
            }

            currentY += rowHeight;
        }

        return currentY;
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

internal sealed class DanfeSectionBuilder : IDanfeSection
{
    private readonly string _title;
    private readonly List<DanfeRowData> _rows = new();

    internal DanfeSectionBuilder(string title)
    {
        _title = title;
    }

    public IDanfeSection Row(Action<IDanfeRow> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var row = new DanfeRowBuilder();
        configure(row);
        _rows.Add(row.Build());
        return this;
    }

    internal DanfeSectionData Build() => new(_title, _rows);
}

internal sealed class DanfeRowBuilder : IDanfeRow
{
    private readonly List<DanfeField> _fields = new();

    public IDanfeRow Field(string label, string value, double widthPct)
    {
        _fields.Add(new DanfeField(label, value, widthPct));
        return this;
    }

    internal DanfeRowData Build() => new(_fields);
}

internal sealed record DanfeSectionData(string Title, IReadOnlyList<DanfeRowData> Rows);

internal sealed record DanfeRowData(IReadOnlyList<DanfeField> Fields);

internal sealed class DanfeItemGridData
{
    public IReadOnlyList<DanfeItemRow> Items { get; init; } = Array.Empty<DanfeItemRow>();
    public IReadOnlyList<DanfeItemColumn> Columns { get; init; } = DanfeItemColumn.Default;
    public double FixedUpperZoneHeightPt { get; init; }
    public double FixedLowerZoneHeightPt { get; init; }
    public double FixedLowerZoneContinuationPt { get; init; }
}
