namespace NFEDanfe.Domain.Models;

/// <summary>Representa o destinatário/remetente da NF-e.</summary>
public record Destinatario(
    string RazaoSocial,
    string Documento,
    Endereco Endereco,
    string InscricaoEstadual,
    string? Telefone
);
