using NFEDanfe.Grid;
using NFEDanfe.Options;

namespace NFEDanfe;

/// <summary>Modelo de dados de entrada para geração do DANFE.</summary>
public sealed class DanfeData
{
    // Emitente
    /// <summary>Razão social do emitente.</summary>
    public string RazaoSocialEmitente { get; set; } = "";
    /// <summary>CNPJ do emitente.</summary>
    public string CnpjEmitente { get; set; } = "";
    /// <summary>Endereço do emitente.</summary>
    public string EnderecoEmitente { get; set; } = "";
    /// <summary>Município do emitente.</summary>
    public string MunicipioEmitente { get; set; } = "";
    /// <summary>UF do emitente.</summary>
    public string UfEmitente { get; set; } = "";
    /// <summary>CEP do emitente.</summary>
    public string CepEmitente { get; set; } = "";
    /// <summary>Telefone do emitente.</summary>
    public string FoneEmitente { get; set; } = "";
    /// <summary>Inscrição estadual do emitente.</summary>
    public string IeEmitente { get; set; } = "";

    // Destinatário
    /// <summary>Razão social do destinatário.</summary>
    public string RazaoSocialDest { get; set; } = "";
    /// <summary>CNPJ ou CPF do destinatário.</summary>
    public string CnpjCpfDest { get; set; } = "";
    /// <summary>Endereço do destinatário.</summary>
    public string EnderecoDest { get; set; } = "";
    /// <summary>Bairro do destinatário.</summary>
    public string BairroDest { get; set; } = "";
    /// <summary>Município do destinatário.</summary>
    public string MunicipioDest { get; set; } = "";
    /// <summary>UF do destinatário.</summary>
    public string UfDest { get; set; } = "";
    /// <summary>CEP do destinatário.</summary>
    public string CepDest { get; set; } = "";
    /// <summary>Telefone do destinatário.</summary>
    public string FoneDest { get; set; } = "";
    /// <summary>Inscrição estadual do destinatário.</summary>
    public string IeDest { get; set; } = "";

    // Nota
    /// <summary>Número da NF-e.</summary>
    public string Numero { get; set; } = "";
    /// <summary>Série da NF-e.</summary>
    public string Serie { get; set; } = "";
    /// <summary>Data de emissão formatada.</summary>
    public string DataEmissao { get; set; } = "";
    /// <summary>Data de entrada/saída formatada.</summary>
    public string DataEntradaSaida { get; set; } = "";
    /// <summary>Natureza da operação.</summary>
    public string NaturezaOperacao { get; set; } = "";
    /// <summary>Tipo de operação (0=entrada, 1=saída).</summary>
    public string TipoOperacao { get; set; } = "1";
    /// <summary>Chave de acesso de 44 dígitos.</summary>
    public string ChaveAcesso { get; set; } = "";
    /// <summary>Protocolo de autorização.</summary>
    public string Protocolo { get; set; } = "";
    /// <summary>Data/hora do protocolo.</summary>
    public string DataProtocolo { get; set; } = "";
    /// <summary>URL do QR Code (opcional).</summary>
    public string? UrlQrCode { get; set; }
    /// <summary>Ambiente detectado do XML.</summary>
    public DanfeAmbiente Ambiente { get; set; } = DanfeAmbiente.Producao;
    /// <summary>Versão do layout da NF-e (ex: "4.00").</summary>
    public string VersaoLayout { get; set; } = "4.00";

    // Transporte
    /// <summary>Modalidade do frete.</summary>
    public string ModalidadeFrete { get; set; } = "";
    /// <summary>Nome da transportadora.</summary>
    public string TransportadoraNome { get; set; } = "";
    /// <summary>CNPJ da transportadora.</summary>
    public string TransportadoraCnpj { get; set; } = "";
    /// <summary>Placa do veículo.</summary>
    public string PlacaVeiculo { get; set; } = "";
    /// <summary>UF do veículo.</summary>
    public string UfVeiculo { get; set; } = "";

    // Totais
    /// <summary>Base de cálculo do ICMS.</summary>
    public string BaseIcms { get; set; } = "";
    /// <summary>Valor do ICMS.</summary>
    public string ValorIcms { get; set; } = "";
    /// <summary>Base de cálculo ICMS ST.</summary>
    public string BaseIcmsSt { get; set; } = "";
    /// <summary>Valor do ICMS ST.</summary>
    public string ValorIcmsSt { get; set; } = "";
    /// <summary>Valor total dos produtos.</summary>
    public string ValorProdutos { get; set; } = "";
    /// <summary>Valor do frete.</summary>
    public string ValorFrete { get; set; } = "";
    /// <summary>Valor do seguro.</summary>
    public string ValorSeguro { get; set; } = "";
    /// <summary>Valor do desconto.</summary>
    public string Desconto { get; set; } = "";
    /// <summary>Outras despesas acessórias.</summary>
    public string OutrasDespesas { get; set; } = "";
    /// <summary>Valor do IPI.</summary>
    public string ValorIpi { get; set; } = "";
    /// <summary>Valor do PIS.</summary>
    public string ValorPis { get; set; } = "";
    /// <summary>Valor da COFINS.</summary>
    public string ValorCofins { get; set; } = "";
    /// <summary>Valor total da nota.</summary>
    public string ValorTotal { get; set; } = "";
    /// <summary>Valor aproximado dos tributos.</summary>
    public string ValorTributos { get; set; } = "";

    // Itens
    /// <summary>Itens da NF-e.</summary>
    public IReadOnlyList<DanfeItemRow> Itens { get; set; } = [];

    // Adicionais
    /// <summary>Informações complementares.</summary>
    public string InformacoesComplementares { get; set; } = "";
    /// <summary>Duplicatas de cobrança.</summary>
    public IReadOnlyList<DanfeDuplicata> Duplicatas { get; set; } = [];
}

/// <summary>Dados de uma duplicata de cobrança.</summary>
public sealed record DanfeDuplicata(string Numero, string Vencimento, string Valor);
