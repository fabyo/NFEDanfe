namespace NFEDanfe.Pagination;

/// <summary>Resultado do cálculo antecipado de paginação.</summary>
public sealed class DanfePagePlan
{
    /// <summary>Total de páginas necessárias.</summary>
    public int TotalPages { get; init; }

    /// <summary>Fatias de itens por página.</summary>
    public IReadOnlyList<DanfeItemPageSlice> Slices { get; init; } = Array.Empty<DanfeItemPageSlice>();

    /// <summary>Retorna true se o documento cabe em uma única página.</summary>
    public bool IsSinglePage => TotalPages == 1;
}
