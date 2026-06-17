namespace NFEDanfe.Domain.Models;

public record LocalEntrega(
    string? RazaoSocial,
    string? Documento,
    string? InscricaoEstadual,
    Endereco Endereco,
    string? Telefone
);
