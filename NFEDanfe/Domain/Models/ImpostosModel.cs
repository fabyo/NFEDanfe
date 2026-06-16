namespace NFEDanfe.Domain.Models;

public record ImpostosModel(
    decimal BaseCalculoIcms,
    decimal ValorIcms,
    decimal BaseCalculoIcmsSt,
    decimal ValorIcmsSt,
    decimal ValorFcp,
    decimal ValorProdutos,
    decimal ValorFrete,
    decimal ValorSeguro,
    decimal ValorDesconto,
    decimal OutrasDespesas,
    decimal ValorIpi,
    decimal ValorNota
);
