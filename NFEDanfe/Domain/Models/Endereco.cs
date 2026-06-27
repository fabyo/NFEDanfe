namespace NFEDanfe.Domain.Models;

/// <summary>Representa um endereço no DANFE.</summary>
public record Endereco(
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Municipio,
    string Uf,
    string Cep
);
