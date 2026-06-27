namespace NFEDanfe.Domain.Models;

/// <summary>Representa o local de entrega alternativo da NF-e.</summary>
public record LocalEntrega(
    string? RazaoSocial,
    string? Documento,
    string? InscricaoEstadual,
    Endereco Endereco,
    string? Telefone
);
