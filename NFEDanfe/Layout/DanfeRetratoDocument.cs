using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Components;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout;

public class DanfeRetratoDocument : IDocument
{
    private readonly DanfeModel _model;
    private readonly DanfeOptions _options;

    public DanfeRetratoDocument(DanfeModel model)
        : this(model, DanfeOptions.Default)
    {
    }

    public DanfeRetratoDocument(DanfeModel model, DanfeOptions options)
    {
        _model = model;
        _options = options;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        DanfeTheme.PaddingInternoVertical = 1.8f;
        try
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(12);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteValor).FontColor(DanfeTheme.CorTexto));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);

                if (_options.EmitFooter || _model.DadosAdicionais != null)
                {
                    page.Footer().Element(ComposeFooter);
                }
            });
        }
        finally
        {
            DanfeTheme.PaddingInternoVertical = 2.0f;
        }
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ShowOnce().PaddingBottom(4).CanhotoBox(_model.Emitente, _model.DadosDanfe, _model.Destinatario, _model.ValorTotal);

            column.Item().Row(row =>
            {
                row.RelativeItem(5).Element(x => x.EmitenteBox(_model.Emitente));
                row.RelativeItem(2).Element(x => x.DanfeBox(_model.DadosDanfe));
                row.RelativeItem(5).Element(x => x.ChaveAcessoBox(_model.DadosDanfe));
            });

            column.Item().Row(row =>
            {
                row.RelativeItem(8).LabelValueCell("NATUREZA DA OPERAÇÃO", _model.DadosDanfe.NaturezaOperacao, true, top: false);

                string protocoloTexto = $"{_model.DadosDanfe.ProtocoloAutorizacao} - {_model.DadosDanfe.DataProtocolo:dd/MM/yyyy HH:mm:ss}";
                row.RelativeItem(4).LabelValueCell("PROTOCOLO DE AUTORIZAÇÃO DE USO", protocoloTexto, true, top: false, left: false);
            });

            column.Item().Row(row =>
            {
                row.RelativeItem(4).LabelValueCell("INSCRIÇÃO ESTADUAL", _model.Emitente.InscricaoEstadual, true, top: false);
                row.RelativeItem(4).LabelValueCell("INSCRIÇÃO ESTADUAL DO SUBST. TRIBUT.", _model.Emitente.InscricaoEstadualSt ?? string.Empty, true, top: false, left: false);
                row.RelativeItem(4).LabelValueCell("CNPJ", DocumentFormatter.CnpjCpf(_model.Emitente.Cnpj), true, top: false, left: false);
            });

            column.Item().ShowOnce().PaddingTop(6).Element(x => x.DestinatarioBox(_model.Destinatario, _model.DadosDanfe));

            if (_model.LocalEntrega != null)
            {
                column.Item().ShowOnce().Element(x => x.LocalEntregaBox(_model.LocalEntrega));
            }

            if (_model.Cobranca != null)
            {
                column.Item().ShowOnce().PaddingTop(6).Element(x => x.CobrancaBox(_model.Cobranca));
            }

            if (_model.Impostos != null)
            {
                column.Item().ShowOnce().PaddingTop(6).Element(x => x.ImpostosBox(_model.Impostos));
            }

            if (_model.Transportador != null)
            {
                column.Item().ShowOnce().PaddingTop(6).Element(x => x.TransportadorBox(_model.Transportador));
            }

            if (_model.Produtos is { Count: > 0 })
            {
                column.Item().PaddingTop(6).Background(Colors.Grey.Lighten3)
                    .Border(DanfeTheme.EspessuraBorda)
                    .BorderColor(DanfeTheme.CorBorda)
                    .PaddingLeft(4)
                    .PaddingVertical(1)
                    .Text("DADOS DO PRODUTO / SERVIÇOS")
                    .FontFamily(DanfeTheme.FontePadrao)
                    .FontSize(DanfeTheme.TamanhoFonteLabel + 1f)
                    .Bold();
            }
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.ExtendVertical().Column(column =>
        {
            if (_model.Produtos is { Count: > 0 })
            {
                int target = 32;
                if (_model.LocalEntrega != null)
                {
                    target -= 8;
                }
                if (_model.Transportador != null && !string.IsNullOrWhiteSpace(_model.Transportador.RazaoSocial))
                {
                    target -= 6;
                }
                column.Item().ExtendVertical().PaddingTop(6).Element(x => x.ProdutosBox(_model.Produtos, false, target));
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            if (_model.DadosAdicionais != null)
            {
                column.Item().PaddingBottom(4).Element(x => x.DadosAdicionaisBox(_model.DadosAdicionais));
            }

            if (_options.EmitFooter)
            {
                column.Item().AlignRight().Text(text =>
                {
                    text.Span($"NFEDanfe - impresso em {DateTime.Now:dd/MM/yyyy HH:mm:ss} - layout NF-e {_model.DadosDanfe.VersaoLayout}")
                        .FontSize(6)
                        .Italic();
                });
            }
        });
    }
}
