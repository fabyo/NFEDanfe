namespace NFEDanfe.Grid;

/// <summary>Dados pré-formatados de uma linha de item do DANFE.</summary>
public sealed record DanfeItemRow(
    string CodigoProduto,
    string Descricao,
    string Ncm,
    string Cst,
    string Cfop,
    string Unidade,
    string Quantidade,
    string ValorUnitario,
    string ValorTotal,
    string ValorDesconto,
    string BaseIcms,
    string ValorIcms,
    string ValorIpi,
    string AliquotaIcms,
    string AliquotaIpi
);
