namespace NFEDanfe.Pagination;

/// <summary>Fatia de itens pertencente a uma página específica do DANFE.</summary>
public sealed class DanfeItemPageSlice
{
    /// <summary>Índice da página (0-based).</summary>
    public int PageIndex { get; init; }

    /// <summary>Índice do primeiro item no array original.</summary>
    public int FirstItemIndex { get; init; }

    /// <summary>Quantidade de itens nesta página.</summary>
    public int ItemCount { get; init; }

    /// <summary>Altura disponível para itens em pontos.</summary>
    public double AvailableHeight { get; init; }
}
