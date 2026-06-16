namespace NFEDanfe.Domain.Models;

public record ProdutoModel(
    string Codigo,
    string Descricao,
    string Ncm,
    string CstCsosn,
    string Cfop,
    string Unidade,
    decimal Quantidade,
    decimal ValorUnitario,
    decimal ValorTotal,
    decimal ValorDesconto,
    decimal BaseCalculoIcms,
    decimal ValorIcms,
    decimal ValorIpi,
    decimal AliquotaIcms,
    decimal AliquotaIpi
);
