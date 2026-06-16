using System;

namespace NFEDanfe.Domain.Models;

public record DuplicataModel(
    string Numero,
    DateTime Vencimento,
    decimal Valor
);
