namespace NFEDanfe.Domain.Models;

public record DadosDanfe(
    int TipoOperacao, // 0 = Entrada, 1 = Saída
    string NaturezaOperacao,
    string ChaveAcesso,
    string ProtocoloAutorizacao,
    DateTime DataProtocolo,
    int Numero,
    int Serie,
    int PaginaAtual,
    int TotalPaginas,
    DateTime DataEmissao,
    DateTime? DataEntradaSaida = null,
    string VersaoLayout = "4.00",
    int TipoImpressao = 1,
    bool IsCancelada = false
);
