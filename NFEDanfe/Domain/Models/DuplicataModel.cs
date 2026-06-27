using System;

namespace NFEDanfe.Domain.Models;

/// <summary>Representa uma duplicata de cobrança no DANFE.</summary>
public record DuplicataModel(
    string Numero,
    DateTime Vencimento,
    decimal Valor
);
