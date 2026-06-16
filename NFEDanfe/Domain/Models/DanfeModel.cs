using System.Collections.Generic;

namespace NFEDanfe.Domain.Models;

public record DanfeModel(
    Emitente Emitente,
    DadosDanfe DadosDanfe,
    Destinatario Destinatario,
    decimal ValorTotal,
    CobrancaModel? Cobranca = null,
    ImpostosModel? Impostos = null,
    TransportadorModel? Transportador = null,
    IReadOnlyList<ProdutoModel>? Produtos = null,
    DadosAdicionaisModel? DadosAdicionais = null
);
