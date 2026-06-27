using System.Collections.Generic;

namespace NFEDanfe.Domain.Models;

/// <summary>Representa as informações adicionais do DANFE.</summary>
public record DadosAdicionaisModel(
    string? InformacoesComplementares,
    string? InformacoesFisco,
    IReadOnlyList<string>? PedidosCompra = null
);
