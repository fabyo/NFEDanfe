using System.Collections.Generic;

namespace NFEDanfe.Domain.Models;

public record CobrancaModel(
    string? NumeroFatura,
    decimal? ValorOriginal,
    decimal? ValorDesconto,
    decimal? ValorLiquido,
    IEnumerable<DuplicataModel> Duplicatas
);
