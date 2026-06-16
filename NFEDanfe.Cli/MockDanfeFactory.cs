using NFEDanfe.Domain.Models;

namespace NFEDanfe.Cli;

internal static class MockDanfeFactory
{
    public static DanfeModel Create(byte[]? logoBytes)
    {
        Emitente emitente = new(
            RazaoSocial: "EMPRESA DEMONSTRACAO LTDA",
            NomeFantasia: "EMPRESA DEMONSTRACAO",
            Cnpj: "06.990.590/0001-23",
            InscricaoEstadual: "123.456.789.111",
            InscricaoEstadualSt: null,
            Endereco: new Endereco("Rua do Exemplo", "500", null, "Distrito Industrial", "Maua", "SP", "09300-000"),
            Telefone: "(11) 4555-9000",
            LogoBytes: logoBytes);

        DadosDanfe dados = new(
            TipoOperacao: 1,
            NaturezaOperacao: "VENDA DE MERCADORIA ADQUIRIDA DE TERCEIROS",
            ChaveAcesso: "35260806990590000123550010000105001234567896",
            ProtocoloAutorizacao: "135160000325412",
            DataProtocolo: new DateTime(2026, 06, 15, 20, 39, 45),
            Numero: 4766,
            Serie: 1,
            PaginaAtual: 1,
            TotalPaginas: 1,
            DataEmissao: new DateTime(2026, 06, 15),
            TipoImpressao: 1);

        Destinatario destinatario = new(
            RazaoSocial: "CLIENTE DEMONSTRACAO LTDA",
            Documento: "06.990.590/0001-23",
            Endereco: new Endereco("Rua do Cliente", "85", null, "Sertaozinho", "Maua", "SP", "09370-840"),
            InscricaoEstadual: "442.123.456.789",
            Telefone: "(11) 4555-1234");

        CobrancaModel cobranca = new(
            NumeroFatura: "FAT-4766-1",
            ValorOriginal: 9408.00m,
            ValorDesconto: 0,
            ValorLiquido: 9408.00m,
            Duplicatas:
            [
                new DuplicataModel("001", DateTime.Today.AddDays(30), 4704.00m),
                new DuplicataModel("002", DateTime.Today.AddDays(60), 4704.00m)
            ]);

        ImpostosModel impostos = new(
            BaseCalculoIcms: 15000.00m,
            ValorIcms: 2700.00m,
            BaseCalculoIcmsSt: 0,
            ValorIcmsSt: 0,
            ValorFcp: 0,
            ValorProdutos: 9408.00m,
            ValorFrete: 0,
            ValorSeguro: 0,
            ValorDesconto: 0,
            OutrasDespesas: 0,
            ValorIpi: 0,
            ValorNota: 9408.00m);

        TransportadorModel transportador = new(
            RazaoSocial: "EXPRESSO LOGISTICA BRASIL LTDA",
            FretePorConta: "0 - REMETENTE (CIF)",
            CodigoAntt: "12345678",
            PlacaVeiculo: "ABC-1234",
            UfPlaca: "SP",
            Documento: "06.990.590/0001-23",
            EnderecoCompleto: "RODOVIA DOS BANDEIRANTES, KM 45",
            Municipio: "JUNDIAI",
            Uf: "SP",
            InscricaoEstadual: "987.654.321.111",
            QuantidadeVolumes: 15,
            Especie: "CAIXAS",
            Marca: "DANFE",
            Numeracao: "1 A 15",
            PesoBruto: 1450.500m,
            PesoLiquido: 1420.000m);

        IReadOnlyList<ProdutoModel> produtos =
        [
            new ProdutoModel("PRD-001", "PECA DEMONSTRACAO A", "87089990", "0400", "5124", "PECA", 2000, 1.8900m, 3780.00m, 0, 0, 0, 0, 0, 0),
            new ProdutoModel("PRD-002", "PECA DEMONSTRACAO B", "87089990", "0400", "5124", "PECA", 2000, 0.6100m, 1220.00m, 0, 0, 0, 0, 0, 0),
            new ProdutoModel("PRD-003", "SERVICO DEMONSTRACAO C", "87089990", "0400", "5124", "PECA", 800, 5.5100m, 4408.00m, 0, 0, 0, 0, 0, 0)
        ];

        DadosAdicionaisModel dadosAdicionais = new(
            InformacoesComplementares: "Email do destinatario: nfe@exemplo.com.br; Suspensao do ICMS prevista na Portaria CAT 22/2007.",
            InformacoesFisco: "Documento gerado em ambiente de demonstracao.");

        return new DanfeModel(emitente, dados, destinatario, impostos.ValorNota, cobranca, impostos, transportador, produtos, dadosAdicionais);
    }
}
