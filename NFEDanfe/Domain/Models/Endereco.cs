namespace NFEDanfe.Domain.Models;

public record Endereco(
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Municipio,
    string Uf,
    string Cep
);
