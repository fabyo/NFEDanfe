using NFEDanfe.Grid;

namespace NFEDanfe.Builder;

/// <summary>Interface para composição de uma página do DANFE.</summary>
public interface IDanfePage
{
    /// <summary>Adiciona uma seção à página.</summary>
    IDanfePage Section(string title, Action<IDanfeSection> configure);

    /// <summary>Adiciona a grade de itens com paginação automática.</summary>
    IDanfePage ItemGrid(
        IReadOnlyList<DanfeItemRow> items,
        double fixedUpperZoneHeightPt,
        double fixedLowerZoneHeightPt,
        double fixedLowerZoneContinuationPt,
        Action<IReadOnlyList<DanfeItemColumn>>? configureColumns = null);
}
