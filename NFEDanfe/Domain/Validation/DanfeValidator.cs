using NFEDanfe.Domain.Models;

namespace NFEDanfe.Domain.Validation;

public static class DanfeValidator
{
    private const decimal MoneyTolerance = 0.01m;

    public static void Validate(DanfeModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ValidateEmitente(model.Emitente);
        ValidateDadosDanfe(model.DadosDanfe);
        ValidateDestinatario(model.Destinatario);

        if (model.Impostos != null)
        {
            ValidateImpostos(model.Impostos);
        }

        if (model.Transportador != null)
        {
            ValidateTransportador(model.Transportador);
        }

        if (model.Produtos != null)
        {
            ValidateProdutos(model.Produtos);
            ValidateTotals(model);
        }

        ValidateNonNegative(model.ValorTotal, "O valor total da nota fiscal não pode ser negativo.");
    }

    private static void ValidateEmitente(Emitente emitente)
    {
        ArgumentNullException.ThrowIfNull(emitente);

        Required(emitente.RazaoSocial, "A razão social do emitente é obrigatória.");
        Required(emitente.Cnpj, "O CNPJ do emitente é obrigatório.");
        Required(emitente.InscricaoEstadual, "A inscrição estadual do emitente é obrigatória.");

        if (!BrazilianDocumentValidator.IsValidCnpj(emitente.Cnpj))
        {
            throw new DanfeDomainException($"CNPJ do emitente inválido: '{emitente.Cnpj}'.");
        }
    }

    private static void ValidateDestinatario(Destinatario destinatario)
    {
        ArgumentNullException.ThrowIfNull(destinatario);

        Required(destinatario.RazaoSocial, "A razão social do destinatário é obrigatória.");
        Required(destinatario.Documento, "O documento do destinatário é obrigatório.");

        if (!BrazilianDocumentValidator.IsValidCnpjOrCpf(destinatario.Documento))
        {
            throw new DanfeDomainException($"Documento do destinatário inválido: '{destinatario.Documento}'.");
        }
    }

    private static void ValidateDadosDanfe(DadosDanfe dados)
    {
        ArgumentNullException.ThrowIfNull(dados);

        if (dados.TipoOperacao is not 0 and not 1)
        {
            throw new DanfeDomainException($"Tipo de operação inválido: '{dados.TipoOperacao}'. Deve ser 0 (Entrada) ou 1 (Saída).");
        }

        Required(dados.NaturezaOperacao, "A natureza da operação é obrigatória.");
        Required(dados.ChaveAcesso, "A chave de acesso é obrigatória.");

        if (!NFeAccessKeyValidator.IsValid(dados.ChaveAcesso))
        {
            throw new DanfeDomainException($"Chave de acesso NF-e inválida: '{dados.ChaveAcesso}'.");
        }

        if (dados.Numero <= 0)
        {
            throw new DanfeDomainException($"O número da NF-e deve ser maior que zero. Informado: {dados.Numero}.");
        }

        if (dados.Serie <= 0)
        {
            throw new DanfeDomainException($"A série da NF-e deve ser maior que zero. Informada: {dados.Serie}.");
        }

        if (dados.PaginaAtual <= 0 || dados.TotalPaginas <= 0 || dados.PaginaAtual > dados.TotalPaginas)
        {
            throw new DanfeDomainException($"Paginação inválida: página {dados.PaginaAtual} de {dados.TotalPaginas}.");
        }

        Required(dados.ProtocoloAutorizacao, "O protocolo de autorização é obrigatório.");
    }

    private static void ValidateImpostos(ImpostosModel impostos)
    {
        ArgumentNullException.ThrowIfNull(impostos);

        ValidateNonNegative(impostos.BaseCalculoIcms, "A base de cálculo do ICMS não pode ser negativa.");
        ValidateNonNegative(impostos.ValorIcms, "O valor do ICMS não pode ser negativo.");
        ValidateNonNegative(impostos.BaseCalculoIcmsSt, "A base de cálculo do ICMS ST não pode ser negativa.");
        ValidateNonNegative(impostos.ValorIcmsSt, "O valor do ICMS ST não pode ser negativo.");
        ValidateNonNegative(impostos.ValorFcp, "O valor do FCP não pode ser negativo.");
        ValidateNonNegative(impostos.ValorProdutos, "O valor total dos produtos não pode ser negativo.");
        ValidateNonNegative(impostos.ValorFrete, "O valor do frete não pode ser negativo.");
        ValidateNonNegative(impostos.ValorSeguro, "O valor do seguro não pode ser negativo.");
        ValidateNonNegative(impostos.ValorDesconto, "O valor do desconto não pode ser negativo.");
        ValidateNonNegative(impostos.OutrasDespesas, "O valor de outras despesas não pode ser negativo.");
        ValidateNonNegative(impostos.ValorIpi, "O valor do IPI não pode ser negativo.");
        ValidateNonNegative(impostos.ValorNota, "O valor total da nota não pode ser negativo.");
    }

    private static void ValidateTransportador(TransportadorModel transportador)
    {
        ArgumentNullException.ThrowIfNull(transportador);

        Required(transportador.FretePorConta, "A modalidade do frete é obrigatória.");

        if (!string.IsNullOrWhiteSpace(transportador.Documento) &&
            !BrazilianDocumentValidator.IsValidCnpjOrCpf(transportador.Documento))
        {
            throw new DanfeDomainException($"Documento do transportador inválido: '{transportador.Documento}'.");
        }

        ValidateNonNegative(transportador.QuantidadeVolumes, "A quantidade de volumes não pode ser negativa.");
        ValidateNonNegative(transportador.PesoBruto, "O peso bruto não pode ser negativo.");
        ValidateNonNegative(transportador.PesoLiquido, "O peso líquido não pode ser negativo.");
    }

    private static void ValidateProdutos(IReadOnlyList<ProdutoModel> produtos)
    {
        foreach (ProdutoModel prod in produtos)
        {
            ArgumentNullException.ThrowIfNull(prod);

            Required(prod.Codigo, "O código do produto é obrigatório.");
            Required(prod.Descricao, $"A descrição do produto '{prod.Codigo}' é obrigatória.");
            Required(prod.Ncm, $"O NCM do produto '{prod.Codigo}' é obrigatório.");
            Required(prod.Cfop, $"O CFOP do produto '{prod.Codigo}' é obrigatório.");
            Required(prod.Unidade, $"A unidade do produto '{prod.Codigo}' é obrigatória.");

            ValidateNonNegative(prod.Quantidade, $"A quantidade do produto '{prod.Codigo}' não pode ser negativa.");
            ValidateNonNegative(prod.ValorUnitario, $"O valor unitário do produto '{prod.Codigo}' não pode ser negativo.");
            ValidateNonNegative(prod.ValorTotal, $"O valor total do produto '{prod.Codigo}' não pode ser negativo.");
            ValidateNonNegative(prod.ValorDesconto, $"O valor de desconto do produto '{prod.Codigo}' não pode ser negativo.");
            ValidateNonNegative(prod.BaseCalculoIcms, $"A base de cálculo de ICMS do produto '{prod.Codigo}' não pode ser negativa.");
            ValidateNonNegative(prod.ValorIcms, $"O valor de ICMS do produto '{prod.Codigo}' não pode ser negativo.");
            ValidateNonNegative(prod.ValorIpi, $"O valor de IPI do produto '{prod.Codigo}' não pode ser negativo.");
            ValidateNonNegative(prod.AliquotaIcms, $"A alíquota de ICMS do produto '{prod.Codigo}' não pode ser negativa.");
            ValidateNonNegative(prod.AliquotaIpi, $"A alíquota de IPI do produto '{prod.Codigo}' não pode ser negativa.");
        }
    }

    private static void ValidateTotals(DanfeModel model)
    {
        if (model.Impostos == null || model.Produtos == null || model.Produtos.Count == 0)
        {
            return;
        }

        decimal produtosTotal = model.Produtos.Sum(x => x.ValorTotal);
        decimal descontoTotal = model.Produtos.Sum(x => x.ValorDesconto);

        ValidateClose(produtosTotal, model.Impostos.ValorProdutos, "A soma dos produtos não confere com o total de produtos da nota.");

        if (descontoTotal > 0 || model.Impostos.ValorDesconto > 0)
        {
            ValidateClose(descontoTotal, model.Impostos.ValorDesconto, "A soma dos descontos dos produtos não confere com o desconto total da nota.");
        }

        ValidateClose(model.ValorTotal, model.Impostos.ValorNota, "O valor total do modelo não confere com o valor total da nota.");
    }

    private static void ValidateClose(decimal actual, decimal expected, string message)
    {
        if (Math.Abs(actual - expected) > MoneyTolerance)
        {
            throw new DanfeDomainException($"{message} Esperado: {expected:N2}; encontrado: {actual:N2}.");
        }
    }

    private static void Required(string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DanfeDomainException(message);
        }
    }

    private static void ValidateNonNegative(decimal value, string message)
    {
        if (value < 0)
        {
            throw new DanfeDomainException(message);
        }
    }

    private static void ValidateNonNegative(decimal? value, string message)
    {
        if (value < 0)
        {
            throw new DanfeDomainException(message);
        }
    }
}
