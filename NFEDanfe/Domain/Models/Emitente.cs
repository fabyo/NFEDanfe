namespace NFEDanfe.Domain.Models;

/// <summary>Representa o emitente da NF-e.</summary>
public record Emitente(
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    string InscricaoEstadual,
    string? InscricaoEstadualSt,
    Endereco Endereco,
    string? Telefone,
    byte[]? LogoBytes
);
