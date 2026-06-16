using NFEDanfe.Domain.Models;
using NFEDanfe.Domain.Validation;
using Xunit;

namespace NFEDanfe.Tests;

public sealed class ValidationTests
{
    [Fact]
    public void Invalid_access_key_is_rejected()
    {
        DanfeModel model = CreateBaseModel() with
        {
            DadosDanfe = CreateBaseModel().DadosDanfe with
            {
                ChaveAcesso = "123",
                ProtocoloAutorizacao = "135260000000001"
            }
        };

        var ex = Assert.Throws<DanfeDomainException>(() => DanfeValidator.Validate(model));

        Assert.Contains("Chave de acesso NF-e", ex.Message);
    }

    [Fact]
    public void Mismatched_totals_are_rejected()
    {
        DanfeModel model = CreateBaseModel() with
        {
            Impostos = CreateBaseModel().Impostos! with
            {
                ValorProdutos = 999.99m,
                ValorNota = 999.99m
            }
        };

        var ex = Assert.Throws<DanfeDomainException>(() => DanfeValidator.Validate(model));

        Assert.Contains("A soma dos produtos", ex.Message);
    }

    [Fact]
    public void Invalid_transport_document_is_rejected()
    {
        DanfeModel model = CreateBaseModel() with
        {
            Transportador = CreateBaseModel().Transportador! with
            {
                Documento = "abc"
            }
        };

        var ex = Assert.Throws<DanfeDomainException>(() => DanfeValidator.Validate(model));

        Assert.Contains("Documento do transportador", ex.Message);
    }

    [Fact]
    public void Valid_model_passes_without_errors()
    {
        DanfeModel model = CreateBaseModel();

        // Should not throw
        DanfeValidator.Validate(model);
    }

    [Fact]
    public void Invalid_emitente_cnpj_is_rejected()
    {
        DanfeModel model = CreateBaseModel() with
        {
            Emitente = CreateBaseModel().Emitente with
            {
                Cnpj = "11.111.111/1111-11"
            }
        };

        var ex = Assert.Throws<DanfeDomainException>(() => DanfeValidator.Validate(model));

        Assert.Contains("CNPJ do emitente inválido", ex.Message);
    }

    [Fact]
    public void Negative_values_are_rejected()
    {
        DanfeModel model = CreateBaseModel() with
        {
            Impostos = CreateBaseModel().Impostos! with
            {
                ValorNota = -10.0m
            }
        };

        var ex = Assert.Throws<DanfeDomainException>(() => DanfeValidator.Validate(model));

        Assert.Contains("O valor total da nota não pode ser negativo.", ex.Message);
    }

    [Fact]
    public void Missing_required_fields_are_rejected()
    {
        DanfeModel model = CreateBaseModel() with
        {
            Destinatario = CreateBaseModel().Destinatario with
            {
                RazaoSocial = ""
            }
        };

        var ex = Assert.Throws<DanfeDomainException>(() => DanfeValidator.Validate(model));

        Assert.Contains("A razão social do destinatário é obrigatória.", ex.Message);
    }

    private static DanfeModel CreateBaseModel()
    {
        Emitente emitente = new(
            RazaoSocial: "EMPRESA EXEMPLO LTDA",
            NomeFantasia: "EXEMPLO INDUSTRIAL",
            Cnpj: "06.990.590/0001-23",
            InscricaoEstadual: "123.456.789.111",
            InscricaoEstadualSt: null,
            Endereco: new Endereco("Rua Exemplo", "100", null, "Centro", "Maua", "SP", "09300-000"),
            Telefone: "(11) 4555-9000",
            LogoBytes: null);

        DadosDanfe dados = new(
            TipoOperacao: 1,
            NaturezaOperacao: "VENDA",
            ChaveAcesso: "35260806990590000123550010000105001234567896",
            ProtocoloAutorizacao: "135260000000001",
            DataProtocolo: new DateTime(2026, 06, 15),
            Numero: 1050,
            Serie: 1,
            PaginaAtual: 1,
            TotalPaginas: 1,
            DataEmissao: new DateTime(2026, 06, 15),
            TipoImpressao: 1);

        Destinatario destinatario = new(
            RazaoSocial: "CLIENTE EXEMPLO LTDA",
            Documento: "06.990.590/0001-23",
            Endereco: new Endereco("Rua Cliente", "200", null, "Bairro", "Maua", "SP", "09370-840"),
            InscricaoEstadual: "442.123.456.789",
            Telefone: "(11) 4555-1234");

        ProdutoModel produto = new(
            Codigo: "PROD-001",
            Descricao: "PRODUTO EXEMPLO PARA DANFE",
            Ncm: "87089990",
            CstCsosn: "0400",
            Cfop: "5102",
            Unidade: "UN",
            Quantidade: 2,
            ValorUnitario: 100.00m,
            ValorTotal: 200.00m,
            ValorDesconto: 0,
            BaseCalculoIcms: 200.00m,
            ValorIcms: 36.00m,
            ValorIpi: 0,
            AliquotaIcms: 18.00m,
            AliquotaIpi: 0);

        ImpostosModel impostos = new(
            BaseCalculoIcms: 200.00m,
            ValorIcms: 36.00m,
            BaseCalculoIcmsSt: 0,
            ValorIcmsSt: 0,
            ValorFcp: 0,
            ValorProdutos: 200.00m,
            ValorFrete: 0,
            ValorSeguro: 0,
            ValorDesconto: 0,
            OutrasDespesas: 0,
            ValorIpi: 0,
            ValorNota: 200.00m);

        TransportadorModel transportador = new(
            RazaoSocial: "TRANSPORTADORA EXEMPLO LTDA",
            FretePorConta: "9 - SEM FRETE",
            CodigoAntt: "12345678",
            PlacaVeiculo: "ABC-1234",
            UfPlaca: "SP",
            Documento: "06.990.590/0001-23",
            EnderecoCompleto: "Rodovia Exemplo, KM 45",
            Municipio: "Maua",
            Uf: "SP",
            InscricaoEstadual: "987.654.321.111",
            QuantidadeVolumes: 1,
            Especie: "CAIXAS",
            Marca: "DANFE",
            Numeracao: "1",
            PesoBruto: 10,
            PesoLiquido: 9);

        return new DanfeModel(emitente, dados, destinatario, 200.00m, null, impostos, transportador, [produto], null);
    }
}
