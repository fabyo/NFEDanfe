using System;
using System.Collections.Generic;
using NFEDanfe.Grid;

namespace NFEDanfe.Pagination;

/// <summary>Motor de cálculo de paginação da grade de itens com alturas dinâmicas.</summary>
public static class DanfePaginator
{
    /// <summary>Calcula o plano de paginação para os itens do DANFE com base em alturas individuais.</summary>
    public static DanfePagePlan Calculate(
        IReadOnlyList<double> rowHeights,
        double availableHeightPage1Pt,
        double availableHeightContinuationPt)
    {
        ArgumentNullException.ThrowIfNull(rowHeights);

        if (rowHeights.Count == 0)
        {
            return new DanfePagePlan
            {
                TotalPages = 1,
                Slices = new[]
                {
                    new DanfeItemPageSlice
                    {
                        PageIndex = 0,
                        FirstItemIndex = 0,
                        ItemCount = 0,
                        AvailableHeight = availableHeightPage1Pt
                    }
                }
            };
        }

        var slices = new List<DanfeItemPageSlice>();
        int currentIndex = 0;
        int pageIndex = 0;

        while (currentIndex < rowHeights.Count)
        {
            double availableHeight = (pageIndex == 0) ? availableHeightPage1Pt : availableHeightContinuationPt;
            double currentSum = 0;
            int count = 0;

            for (int i = currentIndex; i < rowHeights.Count; i++)
            {
                double nextHeight = rowHeights[i];
                if (currentSum + nextHeight > availableHeight)
                {
                    break;
                }
                currentSum += nextHeight;
                count++;
            }

            // Garante progresso caso uma única linha seja mais alta que o espaço disponível da página
            if (count == 0)
            {
                count = 1;
                currentSum = rowHeights[currentIndex];
            }

            slices.Add(new DanfeItemPageSlice
            {
                PageIndex = pageIndex,
                FirstItemIndex = currentIndex,
                ItemCount = count,
                AvailableHeight = availableHeight
            });

            currentIndex += count;
            pageIndex++;
        }

        return new DanfePagePlan
        {
            TotalPages = slices.Count,
            Slices = slices
        };
    }
}
