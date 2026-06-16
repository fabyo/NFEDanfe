using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Models;

namespace NFEDanfe.Layout;

public static class DanfeDocumentFactory
{
    public static IDocument Create(DanfeModel model)
    {
        return Create(model, DanfeOptions.Default);
    }

    public static IDocument Create(DanfeModel model, DanfeOptions options)
    {
        return model.DadosDanfe.TipoImpressao switch
        {
            2 => new DanfePaisagemDocument(model, options),
            1 => new DanfeRetratoDocument(model, options),
            _ => new DanfeRetratoDocument(model, options)
        };
    }
}
