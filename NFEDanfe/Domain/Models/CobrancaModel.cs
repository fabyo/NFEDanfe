using System.Collections.Generic;

namespace NFEDanfe.Domain.Models;

/// <summary>Representa a cobrança/fatura no DANFE.</summary>
public record CobrancaModel(
    string? NumeroFatura,
    decimal? ValorOriginal,
    decimal? ValorDesconto,
    decimal? ValorLiquido,
    IEnumerable<DuplicataModel> Duplicatas
);
