using System.Text;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Domain.Models;

namespace NFEDanfe;

public static class DanfeSnapshot
{
    public static string CreateText(DanfeModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        StringBuilder builder = new();

        builder.AppendLine($"Chave={model.DadosDanfe.ChaveAcesso}");
        builder.AppendLine($"Numero={model.DadosDanfe.Numero}");
        builder.AppendLine($"Serie={model.DadosDanfe.Serie}");
        builder.AppendLine($"TipoImpressao={model.DadosDanfe.TipoImpressao}");
        builder.AppendLine($"Emitente={model.Emitente.RazaoSocial}|{DocumentFormatter.CnpjCpf(model.Emitente.Cnpj)}");
        builder.AppendLine($"Destinatario={model.Destinatario.RazaoSocial}|{DocumentFormatter.CnpjCpf(model.Destinatario.Documento)}");
        builder.AppendLine($"ValorTotal={DocumentFormatter.Money(model.ValorTotal)}");

        if (model.Impostos != null)
        {
            builder.AppendLine($"Impostos.ValorProdutos={DocumentFormatter.Money(model.Impostos.ValorProdutos)}");
            builder.AppendLine($"Impostos.ValorDesconto={DocumentFormatter.Money(model.Impostos.ValorDesconto)}");
            builder.AppendLine($"Impostos.ValorNota={DocumentFormatter.Money(model.Impostos.ValorNota)}");
        }

        builder.AppendLine($"Produtos.Count={model.Produtos?.Count ?? 0}");

        if (model.Produtos != null)
        {
            foreach (ProdutoModel produto in model.Produtos)
            {
                builder.AppendLine(
                    $"Produto={produto.Codigo}|{produto.Descricao}|Qtd={produto.Quantidade:N4}|Unit={DocumentFormatter.Money(produto.ValorUnitario)}|Total={DocumentFormatter.Money(produto.ValorTotal)}|Desc={DocumentFormatter.Money(produto.ValorDesconto)}");
            }
        }

        return builder.ToString();
    }
}
