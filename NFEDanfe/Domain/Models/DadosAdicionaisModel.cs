namespace NFEDanfe.Domain.Models;

public record DadosAdicionaisModel(
    string? InformacoesComplementares,
    string? InformacoesFisco,
    IReadOnlyList<string>? PedidosCompra = null
);
