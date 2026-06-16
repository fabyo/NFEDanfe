namespace NFEDanfe.Domain.Models;

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
