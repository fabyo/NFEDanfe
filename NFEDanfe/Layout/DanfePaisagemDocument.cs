using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Components;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout;

public class DanfePaisagemDocument : IDocument
{
    private readonly DanfeModel _model;
    private readonly DanfeOptions _options;

    public DanfePaisagemDocument(DanfeModel model)
        : this(model, DanfeOptions.Default)
    {
    }

    public DanfePaisagemDocument(DanfeModel model, DanfeOptions options)
    {
        _model = model;
        _options = options;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(10);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteValor).FontColor(DanfeTheme.CorTexto));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);

            if (_options.EmitFooter)
            {
                page.Footer().Element(ComposeFooter);
            }
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingBottom(4).CanhotoBox(_model.Emitente, _model.DadosDanfe, _model.Destinatario, _model.ValorTotal);

            column.Item().Row(row =>
            {
                row.RelativeItem(4.5f).Element(x => x.EmitenteBox(_model.Emitente));
                row.RelativeItem(1.5f).Element(x => x.DanfeBox(_model.DadosDanfe));
                row.RelativeItem(4).Element(x => x.ChaveAcessoBox(_model.DadosDanfe));
            });

            column.Item().PaddingTop(2).Row(row =>
            {
                row.RelativeItem(7).LabelValueCell("NATUREZA DA OPERAÇÃO", _model.DadosDanfe.NaturezaOperacao, true, top: false, left: false);

                string protocoloTexto = $"{_model.DadosDanfe.ProtocoloAutorizacao} - {_model.DadosDanfe.DataProtocolo:dd/MM/yyyy HH:mm:ss}";
                row.RelativeItem(5).LabelValueCell("PROTOCOLO DE AUTORIZAÇÃO DE USO", protocoloTexto, true, top: false, left: false);
            });

            column.Item().Row(row =>
            {
                row.RelativeItem(4).LabelValueCell("INSCRIÇÃO ESTADUAL", _model.Emitente.InscricaoEstadual, true, top: false, left: false);
                row.RelativeItem(4).LabelValueCell("INSCRIÇÃO ESTADUAL DO SUBST. TRIBUT.", _model.Emitente.InscricaoEstadualSt ?? string.Empty, true, top: false, left: false);
                row.RelativeItem(4).LabelValueCell("CNPJ", DocumentFormatter.CnpjCpf(_model.Emitente.Cnpj), true, top: false, left: false);
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Element(x => x.DestinatarioBox(_model.Destinatario, _model.DadosDanfe, true));

            if (_model.Cobranca != null)
            {
                column.Item().PaddingTop(5).Element(x => x.CobrancaBox(_model.Cobranca));
            }

            if (_model.Impostos != null)
            {
                column.Item().PaddingTop(5).Element(x => x.ImpostosBox(_model.Impostos));
            }

            if (_model.Transportador != null)
            {
                column.Item().PaddingTop(5).Element(x => x.TransportadorBox(_model.Transportador));
            }

            if (_model.Produtos is { Count: > 0 })
            {
                column.Item().ExtendVertical().PaddingTop(5).Element(x => x.ProdutosBox(_model.Produtos, true));
            }

            if (_model.DadosAdicionais != null)
            {
                column.Item()
                    .PaddingTop(5)
                    .Element(x => x.DadosAdicionaisBox(_model.DadosAdicionais));
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignRight().Text(text =>
        {
            text.Span($"NFEDanfe - impresso em {DateTime.Now:dd/MM/yyyy HH:mm:ss} - layout NF-e {_model.DadosDanfe.VersaoLayout}")
                .FontSize(6)
                .Italic();
        });
    }
}
