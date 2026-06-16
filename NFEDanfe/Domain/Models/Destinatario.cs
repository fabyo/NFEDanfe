namespace NFEDanfe.Domain.Models;

public record Destinatario(
    string RazaoSocial,
    string Documento,
    Endereco Endereco,
    string InscricaoEstadual,
    string? Telefone
);
