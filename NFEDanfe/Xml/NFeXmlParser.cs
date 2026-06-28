using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NFEDanfe.Grid;
using NFEDanfe.Options;

namespace NFEDanfe.Xml;

/// <summary>Parser de XML de NF-e autorizado para DanfeData (modelo com string).</summary>
public static class NFeXmlParser
{
    private static readonly XNamespace Ns = "http://www.portalfiscal.inf.br/nfe";

    /// <summary>Parseia XML a partir de um Stream UTF-8.</summary>
    public static DanfeData Parse(Stream xmlStream)
    {
        ArgumentNullException.ThrowIfNull(xmlStream);
        var settings = new System.Xml.XmlReaderSettings { DtdProcessing = System.Xml.DtdProcessing.Prohibit, XmlResolver = null };
        using var reader = System.Xml.XmlReader.Create(xmlStream, settings);
        var doc = XDocument.Load(reader);
        return ParseDocument(doc);
    }

    /// <summary>Parseia XML a partir de uma string com o conteúdo XML.</summary>
    public static DanfeData Parse(string xmlContent)
    {
        ArgumentNullException.ThrowIfNull(xmlContent);
        var settings = new System.Xml.XmlReaderSettings { DtdProcessing = System.Xml.DtdProcessing.Prohibit, XmlResolver = null };
        using var stringReader = new StringReader(xmlContent);
        using var reader = System.Xml.XmlReader.Create(stringReader, settings);
        var doc = XDocument.Load(reader);
        return ParseDocument(doc);
    }

    private static DanfeData ParseDocument(XDocument doc)
    {
        var root = doc.Root ?? throw new DanfeXmlException("/", "Documento XML vazio.");
        XElement nfe;
        XElement? protNFe = null;

        if (root.Name.LocalName == "nfeProc")
        {
            nfe = root.Element(Ns + "NFe")
                ?? throw new DanfeXmlException("/nfeProc/NFe", "Elemento NFe não encontrado no envelope.");
            protNFe = root.Element(Ns + "protNFe");
        }
        else if (root.Name.LocalName == "NFe")
        {
            nfe = root;
        }
        else
        {
            throw new DanfeXmlException("/", $"Elemento raiz inesperado: '{root.Name.LocalName}'. Esperado: 'nfeProc' ou 'NFe'.");
        }

        var infNFe = nfe.RequiredChild("infNFe", "/NFe/infNFe");

        var ide = infNFe.RequiredChild("ide", "/NFe/infNFe/ide");
        var emit = infNFe.RequiredChild("emit", "/NFe/infNFe/emit");
        var dest = infNFe.RequiredChild("dest", "/NFe/infNFe/dest");
        var total = infNFe.RequiredChild("total", "/NFe/infNFe/total");
        var transp = infNFe.Child("transp");
        var cobr = infNFe.Child("cobr");
        var infAdic = infNFe.Child("infAdic");
        var infNFeSupl = infNFe.Child("infNFeSupl");

        var data = new DanfeData();

        // IDE
        data.Numero = ide.RequiredElementValue("nNF", "/NFe/infNFe/ide/nNF");
        data.Serie = ide.RequiredElementValue("serie", "/NFe/infNFe/ide/serie");
        data.NaturezaOperacao = ide.RequiredElementValue("natOp", "/NFe/infNFe/ide/natOp");
        data.TipoOperacao = ide.RequiredElementValue("tpNF", "/NFe/infNFe/ide/tpNF");

        string dhEmi = ide.RequiredElementValue("dhEmi", "/NFe/infNFe/ide/dhEmi");
        data.DataEmissao = DateTimeOffset.Parse(dhEmi).ToString("dd/MM/yyyy");

        string dhSaiEnt = ide.ElementValue("dhSaiEnt");
        data.DataEntradaSaida = string.IsNullOrEmpty(dhSaiEnt) ? "" : DateTimeOffset.Parse(dhSaiEnt).ToString("dd/MM/yyyy HH:mm");

        string tpAmb = ide.RequiredElementValue("tpAmb", "/NFe/infNFe/ide/tpAmb");
        data.Ambiente = tpAmb == "2" ? DanfeAmbiente.Homologacao : DanfeAmbiente.Producao;
        data.VersaoLayout = infNFe.Attribute("versao")?.Value ?? "4.00";

        // Chave de acesso
        string idAttr = infNFe.Attribute("Id")?.Value ?? "";
        data.ChaveAcesso = idAttr.StartsWith("NFe", StringComparison.OrdinalIgnoreCase)
            ? idAttr[3..] : idAttr;

        // EMIT
        data.RazaoSocialEmitente = emit.RequiredElementValue("xNome", "/NFe/infNFe/emit/xNome");
        data.CnpjEmitente = emit.ElementValue("CNPJ").FormatCnpj();
        data.IeEmitente = emit.ElementValue("IE");

        var enderEmit = emit.RequiredChild("enderEmit", "/NFe/infNFe/emit/enderEmit");
        string lgr = enderEmit.ElementValue("xLgr");
        string nro = enderEmit.ElementValue("nro");
        data.EnderecoEmitente = string.IsNullOrEmpty(nro) ? lgr : $"{lgr}, {nro}";
        data.MunicipioEmitente = enderEmit.RequiredElementValue("xMun", "/NFe/infNFe/emit/enderEmit/xMun");
        data.UfEmitente = enderEmit.RequiredElementValue("UF", "/NFe/infNFe/emit/enderEmit/UF");
        data.CepEmitente = enderEmit.ElementValue("CEP").FormatCep();
        data.FoneEmitente = enderEmit.ElementValue("fone");

        // DEST
        data.RazaoSocialDest = dest.ElementValue("xNome");
        string destCnpj = dest.ElementValue("CNPJ");
        string destCpf = dest.ElementValue("CPF");
        data.CnpjCpfDest = (!string.IsNullOrEmpty(destCnpj) ? destCnpj : destCpf).FormatCnpjCpf();
        data.IeDest = dest.ElementValue("IE");

        var enderDest = dest.Child("enderDest");
        if (enderDest is not null)
        {
            string destLgr = enderDest.ElementValue("xLgr");
            string destNro = enderDest.ElementValue("nro");
            data.EnderecoDest = string.IsNullOrEmpty(destNro) ? destLgr : $"{destLgr}, {destNro}";
            data.BairroDest = enderDest.ElementValue("xBairro");
            data.MunicipioDest = enderDest.ElementValue("xMun");
            data.UfDest = enderDest.ElementValue("UF");
            data.CepDest = enderDest.ElementValue("CEP").FormatCep();
            data.FoneDest = enderDest.ElementValue("fone");
        }

        // TOTAL
        var icmsTot = total.RequiredChild("ICMSTot", "/NFe/infNFe/total/ICMSTot");
        data.BaseIcms = icmsTot.ElementValue("vBC");
        data.ValorIcms = icmsTot.ElementValue("vICMS");
        data.BaseIcmsSt = icmsTot.ElementValue("vBCST");
        data.ValorIcmsSt = icmsTot.ElementValue("vST");
        data.ValorProdutos = icmsTot.ElementValue("vProd");
        data.ValorFrete = icmsTot.ElementValue("vFrete");
        data.ValorSeguro = icmsTot.ElementValue("vSeg");
        data.Desconto = icmsTot.ElementValue("vDesc");
        data.OutrasDespesas = icmsTot.ElementValue("vOutro");
        data.ValorIpi = icmsTot.ElementValue("vIPI");
        data.ValorPis = icmsTot.ElementValue("vPIS");
        data.ValorCofins = icmsTot.ElementValue("vCOFINS");
        data.ValorTotal = icmsTot.ElementValue("vNF");
        data.ValorTributos = icmsTot.ElementValue("vTotTrib");

        // TRANSP
        if (transp is not null)
        {
            string modFrete = transp.ElementValue("modFrete");
            data.ModalidadeFrete = modFrete switch
            {
                "0" => "0 - CIF",
                "1" => "1 - FOB",
                "2" => "2 - Terceiros",
                "3" => "3 - Próprio Rem.",
                "4" => "4 - Próprio Des.",
                "9" => "9 - S/Frete",
                _ => modFrete
            };

            var transporta = transp.Child("transporta");
            if (transporta is not null)
            {
                data.TransportadoraNome = transporta.ElementValue("xNome");
                data.TransportadoraCnpj = transporta.ElementValue("CNPJ").FormatCnpj();
            }

            var veicTransp = transp.Child("veicTransp");
            if (veicTransp is not null)
            {
                data.PlacaVeiculo = veicTransp.ElementValue("placa");
                data.UfVeiculo = veicTransp.ElementValue("UF");
            }
        }

        // ITENS
        data.Itens = infNFe.Elements(Ns + "det").Select(det =>
        {
            var prod = det.RequiredChild("prod", $"/NFe/infNFe/det[{det.Attribute("nItem")?.Value}]/prod");
            var imposto = det.Child("imposto");
            var icmsGroup = imposto?.Element(Ns + "ICMS");
            var icms = icmsGroup?.Elements().FirstOrDefault();

            return new DanfeItemRow(
                CodigoProduto: prod.ElementValue("cProd"),
                Descricao: prod.ElementValue("xProd"),
                Ncm: prod.ElementValue("NCM"),
                Cst: icms?.ElementValue("CST") ?? icms?.ElementValue("CSOSN") ?? "",
                Cfop: prod.ElementValue("CFOP"),
                Unidade: prod.ElementValue("uCom"),
                Quantidade: prod.ElementValue("qCom"),
                ValorUnitario: prod.ElementValue("vUnCom"),
                ValorTotal: prod.ElementValue("vProd"),
                ValorDesconto: prod.ElementValue("vDesc") ?? "0,00",
                BaseIcms: icms?.ElementValue("vBC") ?? "",
                ValorIcms: icms?.ElementValue("vICMS") ?? "",
                ValorIpi: imposto?.Child("IPI")?.Child("IPITrib")?.ElementValue("vIPI") ?? "",
                AliquotaIcms: icms?.ElementValue("pICMS") ?? "",
                AliquotaIpi: imposto?.Child("IPI")?.Child("IPITrib")?.ElementValue("pIPI") ?? ""
            );
        }).ToList();

        // COBR / Duplicatas
        if (cobr is not null)
        {
            data.Duplicatas = cobr.Elements(Ns + "dup").Select(dup =>
            {
                string venc = dup.ElementValue("dVenc");
                string vencFormatado = string.IsNullOrEmpty(venc) ? "" : DateTimeOffset.Parse(venc).ToString("dd/MM/yyyy");
                return new DanfeDuplicata(
                    Numero: dup.ElementValue("nDup"),
                    Vencimento: vencFormatado,
                    Valor: dup.ElementValue("vDup")
                );
            }).ToList();
        }

        // INFADIC
        data.InformacoesComplementares = infAdic?.ElementValue("infCpl") ?? "";

        // PROTOCOLO
        if (protNFe is not null)
        {
            var infProt = protNFe.Child("infProt");
            if (infProt is not null)
            {
                data.Protocolo = infProt.ElementValue("nProt");
                string dhRecbto = infProt.ElementValue("dhRecbto");
                data.DataProtocolo = string.IsNullOrEmpty(dhRecbto) ? "" : DateTimeOffset.Parse(dhRecbto).ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        // QR Code URL
        data.UrlQrCode = infNFeSupl?.ElementValue("qrCode") ?? "";

        return data;
    }
}
