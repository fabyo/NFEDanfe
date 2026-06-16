namespace NFEDanfe.Domain.Models;

public record TransportadorModel(
    string RazaoSocial,
    string FretePorConta,
    string CodigoAntt,
    string PlacaVeiculo,
    string UfPlaca,
    string Documento,
    string EnderecoCompleto,
    string Municipio,
    string Uf,
    string InscricaoEstadual,
    decimal? QuantidadeVolumes,
    string Especie,
    string Marca,
    string Numeracao,
    decimal? PesoBruto,
    decimal? PesoLiquido
);
