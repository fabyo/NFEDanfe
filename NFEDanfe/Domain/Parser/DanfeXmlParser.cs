using System.Xml;
using System.Xml.Linq;
using NFEDanfe.Domain.Models;
using NFEDanfe.Domain.Formatting;

namespace NFEDanfe.Domain.Parser;

public static class DanfeXmlParser
{
    private static readonly XNamespace NfeNamespace = "http://www.portalfiscal.inf.br/nfe";

    public static DanfeModel Parse(string xmlPath, DanfeOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(xmlPath))
        {
            throw new ArgumentException("O caminho do XML deve ser informado.", nameof(xmlPath));
        }

        if (!File.Exists(xmlPath))
        {
            throw new FileNotFoundException($"Arquivo XML não encontrado: {xmlPath}", xmlPath);
        }

        using FileStream stream = File.OpenRead(xmlPath);
        return Parse(stream, options);
    }

    public static DanfeModel Parse(Stream xmlStream, DanfeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(xmlStream);
        return ParseDocument(LoadSecure(xmlStream, options));
    }

    public static DanfeModel ParseXmlContent(string xmlContent)
    {
        if (string.IsNullOrWhiteSpace(xmlContent))
        {
            throw new ArgumentException("O conteúdo XML não pode ser vazio.", nameof(xmlContent));
        }

        using System.IO.StringReader reader = new(xmlContent);
        XmlReaderSettings settings = new()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersFromEntities = 0,
            MaxCharactersInDocument = 10_000_000
        };

        using XmlReader xmlReader = XmlReader.Create(reader, settings);
        XDocument doc = XDocument.Load(xmlReader, LoadOptions.None);
        return ParseDocument(doc);
    }

    public static DanfeModel ParseDocument(XDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);

        XElement infNFe = GetInfNFe(doc);
        XElement ide = Required(infNFe, "ide");
        XElement emit = Required(infNFe, "emit");
        XElement dest = Required(infNFe, "dest");
        XElement total = Required(Required(infNFe, "total"), "ICMSTot");

        DadosDanfe dadosDanfe = ParseDadosDanfe(doc, infNFe, ide);
        Emitente emitente = ParseEmitente(emit);
        Destinatario destinatario = ParseDestinatario(dest);
        ImpostosModel impostos = ParseImpostos(total);
        CobrancaModel? cobranca = ParseCobranca(infNFe.Element(NfeNamespace + "cobr"));
        TransportadorModel? transportador = ParseTransportador(infNFe.Element(NfeNamespace + "transp"));
        IReadOnlyList<ProdutoModel> produtos = ParseProdutos(infNFe);
        DadosAdicionaisModel dadosAdicionais = ParseDadosAdicionais(infNFe.Element(NfeNamespace + "infAdic"));
        LocalEntrega? localEntrega = ParseLocalEntrega(infNFe.Element(NfeNamespace + "entrega"), destinatario);

        return new DanfeModel(
            emitente,
            dadosDanfe,
            destinatario,
            impostos.ValorNota,
            cobranca,
            impostos,
            transportador,
            produtos,
            dadosAdicionais,
            localEntrega);
    }

    private static XDocument LoadSecure(Stream xmlStream, DanfeOptions? options)
    {
        XmlReaderSettings settings = new()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersFromEntities = 0,
            MaxCharactersInDocument = 10_000_000
        };

        if (options?.CustomXmlEncoding != null)
        {
            var streamReader = new StreamReader(xmlStream, options.CustomXmlEncoding, detectEncodingFromByteOrderMarks: true);
            using XmlReader reader = XmlReader.Create(streamReader, settings);
            return XDocument.Load(reader, LoadOptions.None);
        }
        else
        {
            using XmlReader reader = XmlReader.Create(xmlStream, settings);
            return XDocument.Load(reader, LoadOptions.None);
        }
    }

    private static XElement GetInfNFe(XDocument doc)
    {
        XElement? nfe = doc.Element(NfeNamespace + "nfeProc")?.Element(NfeNamespace + "NFe")
            ?? doc.Element(NfeNamespace + "NFe")
            ?? doc.Descendants(NfeNamespace + "NFe").FirstOrDefault();

        if (nfe == null)
        {
            throw new InvalidOperationException("Tag <NFe> não encontrada no XML.");
        }

        return nfe.Element(NfeNamespace + "infNFe")
            ?? throw new InvalidOperationException("Tag <infNFe> não encontrada no XML.");
    }

    private static XElement Required(XElement parent, string name)
    {
        return parent.Element(NfeNamespace + name)
            ?? throw new InvalidOperationException($"Tag <{name}> não encontrada.");
    }

    private static DadosDanfe ParseDadosDanfe(XDocument doc, XElement infNFe, XElement ide)
    {
        string chave = infNFe.Attribute("Id")?.Value?.Trim() ?? string.Empty;
        if (chave.StartsWith("NFe", StringComparison.OrdinalIgnoreCase))
        {
            chave = chave[3..];
        }

        XElement? infProt = doc.Element(NfeNamespace + "nfeProc")
            ?.Element(NfeNamespace + "protNFe")
            ?.Element(NfeNamespace + "infProt");

        string cStat = infProt?.Element(NfeNamespace + "cStat")?.Value ?? string.Empty;
        bool isCancelada = cStat == "101" || cStat == "151";

        return new DadosDanfe(
            TipoOperacao: ide.Int(NfeNamespace, "tpNF", 1),
            NaturezaOperacao: ide.Text(NfeNamespace, "natOp"),
            ChaveAcesso: chave,
            ProtocoloAutorizacao: infProt.Text(NfeNamespace, "nProt"),
            DataProtocolo: infProt.NullableDateTime(NfeNamespace, "dhRecbto") ?? DateTime.MinValue,
            Numero: ide.Int(NfeNamespace, "nNF"),
            Serie: ide.Int(NfeNamespace, "serie", 1),
            PaginaAtual: 1,
            TotalPaginas: 1,
            DataEmissao: ide.DateTime(NfeNamespace, "dhEmi"),
            DataEntradaSaida: ide.NullableDateTime(NfeNamespace, "dhSaiEnt"),
            VersaoLayout: infNFe.Attribute("versao")?.Value ?? "4.00",
            TipoImpressao: ide.Int(NfeNamespace, "tpImp", 1),
            IsCancelada: isCancelada);
    }

    private static Emitente ParseEmitente(XElement emit)
    {
        XElement endereco = Required(emit, "enderEmit");

        return new Emitente(
            RazaoSocial: emit.Text(NfeNamespace, "xNome"),
            NomeFantasia: emit.Text(NfeNamespace, "xFant"),
            Cnpj: emit.Text(NfeNamespace, "CNPJ"),
            InscricaoEstadual: emit.Text(NfeNamespace, "IE"),
            InscricaoEstadualSt: emit.Element(NfeNamespace + "IEST")?.Value,
            Endereco: ParseEndereco(endereco),
            Telefone: endereco.Element(NfeNamespace + "fone")?.Value,
            LogoBytes: null);
    }

    private static Destinatario ParseDestinatario(XElement dest)
    {
        XElement endereco = Required(dest, "enderDest");

        return new Destinatario(
            RazaoSocial: dest.Text(NfeNamespace, "xNome"),
            Documento: dest.Text(NfeNamespace, "CNPJ", dest.Text(NfeNamespace, "CPF")),
            Endereco: ParseEndereco(endereco),
            InscricaoEstadual: dest.Text(NfeNamespace, "IE"),
            Telefone: endereco.Element(NfeNamespace + "fone")?.Value);
    }

    private static ImpostosModel ParseImpostos(XElement total)
    {
        decimal vICMS = total.Decimal(NfeNamespace, "vICMS");
        decimal vST = total.Decimal(NfeNamespace, "vST");
        decimal vIPI = total.Decimal(NfeNamespace, "vIPI");
        decimal vPIS = total.Decimal(NfeNamespace, "vPIS");
        decimal vCOFINS = total.Decimal(NfeNamespace, "vCOFINS");
        decimal vII = total.Decimal(NfeNamespace, "vII");
        decimal? vFCP = total.NullableDecimal(NfeNamespace, "vFCP");

        decimal valorTotTrib = vICMS + vST + vIPI + vPIS + vCOFINS + vII + (vFCP ?? 0m);

        return new ImpostosModel(
            BaseCalculoIcms: total.Decimal(NfeNamespace, "vBC"),
            ValorIcms: vICMS,
            BaseCalculoIcmsSt: total.Decimal(NfeNamespace, "vBCST"),
            ValorIcmsSt: vST,
            ValorFcp: vFCP,
            ValorProdutos: total.Decimal(NfeNamespace, "vProd"),
            ValorFrete: total.Decimal(NfeNamespace, "vFrete"),
            ValorSeguro: total.Decimal(NfeNamespace, "vSeg"),
            ValorDesconto: total.Decimal(NfeNamespace, "vDesc"),
            OutrasDespesas: total.Decimal(NfeNamespace, "vOutro"),
            ValorIpi: vIPI,
            ValorIcmsUfDest: total.Decimal(NfeNamespace, "vICMSUFDest"),
            ValorTotTrib: valorTotTrib,
            ValorIi: vII,
            ValorIcmsUfRemet: total.Decimal(NfeNamespace, "vICMSUFRemet"),
            ValorPis: vPIS,
            ValorCofins: vCOFINS,
            ValorNota: total.Decimal(NfeNamespace, "vNF"));
    }

    private static CobrancaModel? ParseCobranca(XElement? cobr)
    {
        if (cobr == null)
        {
            return null;
        }

        XElement? fat = cobr.Element(NfeNamespace + "fat");
        List<DuplicataModel> duplicatas = cobr.Elements(NfeNamespace + "dup")
            .Select(dup => new DuplicataModel(
                Numero: dup.Text(NfeNamespace, "nDup"),
                Vencimento: dup.DateTime(NfeNamespace, "dVenc"),
                Valor: dup.Decimal(NfeNamespace, "vDup")))
            .ToList();

        return new CobrancaModel(
            NumeroFatura: fat?.Element(NfeNamespace + "nFat")?.Value,
            ValorOriginal: fat.NullableDecimal(NfeNamespace, "vOrig"),
            ValorDesconto: fat.NullableDecimal(NfeNamespace, "vDesc"),
            ValorLiquido: fat.NullableDecimal(NfeNamespace, "vLiq"),
            Duplicatas: duplicatas);
    }

    private static TransportadorModel? ParseTransportador(XElement? transp)
    {
        if (transp == null)
        {
            return null;
        }

        XElement? transporta = transp.Element(NfeNamespace + "transporta");
        XElement? veicTransp = transp.Element(NfeNamespace + "veicTransp");
        XElement? vol = transp.Element(NfeNamespace + "vol");

        return new TransportadorModel(
            RazaoSocial: transporta.Text(NfeNamespace, "xNome"),
            FretePorConta: ObterDescricaoFrete(transp.Text(NfeNamespace, "modFrete", "9")),
            CodigoAntt: veicTransp.Text(NfeNamespace, "RNTC"),
            PlacaVeiculo: veicTransp.Text(NfeNamespace, "placa"),
            UfPlaca: veicTransp.Text(NfeNamespace, "UF"),
            Documento: transporta.Text(NfeNamespace, "CNPJ", transporta.Text(NfeNamespace, "CPF")),
            EnderecoCompleto: transporta.Text(NfeNamespace, "xEnder"),
            Municipio: transporta.Text(NfeNamespace, "xMun"),
            Uf: transporta.Text(NfeNamespace, "UF"),
            InscricaoEstadual: transporta.Text(NfeNamespace, "IE"),
            QuantidadeVolumes: vol.NullableDecimal(NfeNamespace, "qVol"),
            Especie: vol.Text(NfeNamespace, "esp"),
            Marca: vol.Text(NfeNamespace, "marca"),
            Numeracao: vol.Text(NfeNamespace, "nVol"),
            PesoBruto: vol.NullableDecimal(NfeNamespace, "pesoB"),
            PesoLiquido: vol.NullableDecimal(NfeNamespace, "pesoL"));
    }

    private static IReadOnlyList<ProdutoModel> ParseProdutos(XElement infNFe)
    {
        List<ProdutoModel> produtos = [];

        foreach (XElement det in infNFe.Elements(NfeNamespace + "det"))
        {
            XElement? prod = det.Element(NfeNamespace + "prod");
            if (prod == null)
            {
                continue;
            }

            (string cstCsosn, decimal baseIcms, decimal valorIcms, decimal aliqIcms) = ParseIcms(det.Element(NfeNamespace + "imposto"));
            (decimal valorIpi, decimal aliqIpi) = ParseIpi(det.Element(NfeNamespace + "imposto"));

            produtos.Add(new ProdutoModel(
                Codigo: prod.Text(NfeNamespace, "cProd"),
                Descricao: prod.Text(NfeNamespace, "xProd"),
                Ncm: prod.Text(NfeNamespace, "NCM"),
                CstCsosn: cstCsosn,
                Cfop: prod.Text(NfeNamespace, "CFOP"),
                Unidade: prod.Text(NfeNamespace, "uCom"),
                Quantidade: prod.Decimal(NfeNamespace, "qCom"),
                ValorUnitario: prod.Decimal(NfeNamespace, "vUnCom"),
                ValorTotal: prod.Decimal(NfeNamespace, "vProd"),
                ValorDesconto: prod.Decimal(NfeNamespace, "vDesc"),
                BaseCalculoIcms: baseIcms,
                ValorIcms: valorIcms,
                ValorIpi: valorIpi,
                AliquotaIcms: aliqIcms,
                AliquotaIpi: aliqIpi));
        }

        return produtos;
    }

    private static (string CstCsosn, decimal BaseIcms, decimal ValorIcms, decimal AliquotaIcms) ParseIcms(XElement? imposto)
    {
        XElement? icmsNode = imposto?.Element(NfeNamespace + "ICMS")?.Elements().FirstOrDefault();
        if (icmsNode == null)
        {
            return (string.Empty, 0, 0, 0);
        }

        string orig = icmsNode.Text(NfeNamespace, "orig");
        string cst = icmsNode.Text(NfeNamespace, "CST", icmsNode.Text(NfeNamespace, "CSOSN"));

        return (
            CstCsosn: orig + cst,
            BaseIcms: icmsNode.Decimal(NfeNamespace, "vBC"),
            ValorIcms: icmsNode.Decimal(NfeNamespace, "vICMS"),
            AliquotaIcms: icmsNode.Decimal(NfeNamespace, "pICMS"));
    }

    private static (decimal ValorIpi, decimal AliquotaIpi) ParseIpi(XElement? imposto)
    {
        XElement? ipiTrib = imposto?.Element(NfeNamespace + "IPI")?.Element(NfeNamespace + "IPITrib");
        return ipiTrib == null
            ? (0, 0)
            : (ipiTrib.Decimal(NfeNamespace, "vIPI"), ipiTrib.Decimal(NfeNamespace, "pIPI"));
    }

    private static DadosAdicionaisModel ParseDadosAdicionais(XElement? infAdic)
    {
        return new DadosAdicionaisModel(
            InformacoesComplementares: infAdic?.Element(NfeNamespace + "infCpl")?.Value,
            InformacoesFisco: infAdic?.Element(NfeNamespace + "infAdFisco")?.Value);
    }

    private static LocalEntrega? ParseLocalEntrega(XElement? entrega, Destinatario dest)
    {
        if (entrega == null)
        {
            return null;
        }

        string cnpjCpf = entrega.Element(NfeNamespace + "CNPJ")?.Value ?? entrega.Element(NfeNamespace + "CPF")?.Value ?? string.Empty;

        return new LocalEntrega(
            RazaoSocial: dest.RazaoSocial,
            Documento: cnpjCpf,
            InscricaoEstadual: dest.InscricaoEstadual,
            Endereco: ParseEndereco(entrega),
            Telefone: dest.Telefone);
    }

    private static Endereco ParseEndereco(XElement el)
    {
        return new Endereco(
            Logradouro: el.Text(NfeNamespace, "xLgr"),
            Numero: el.Text(NfeNamespace, "nro"),
            Complemento: el.Element(NfeNamespace + "xCpl")?.Value,
            Bairro: el.Text(NfeNamespace, "xBairro"),
            Municipio: el.Text(NfeNamespace, "xMun"),
            Uf: el.Text(NfeNamespace, "UF"),
            Cep: el.Text(NfeNamespace, "CEP"));
    }

    private static string ObterDescricaoFrete(string modFrete)
    {
        return modFrete switch
        {
            "0" => "0 - REMETENTE (CIF)",
            "1" => "1 - DESTINATÁRIO (FOB)",
            "2" => "2 - TERCEIROS",
            "3" => "3 - PRÓPRIO REMETENTE",
            "4" => "4 - PRÓPRIO DESTINATÁRIO",
            "9" => "9 - SEM FRETE",
            _ => $"{modFrete} - OUTROS"
        };
    }
}
