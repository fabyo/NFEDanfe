using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class EmitenteBox : IComponent
{
    private readonly Emitente _emitente;
    private readonly bool _isLandscape;

    public EmitenteBox(Emitente emitente, bool isLandscape = false)
    {
        _emitente = emitente;
        _isLandscape = isLandscape;
    }

    public void Compose(IContainer container)
    {
        container
            .BorderTop(DanfeTheme.EspessuraBorda)
            .BorderBottom(DanfeTheme.EspessuraBorda)
            .BorderRight(DanfeTheme.EspessuraBorda)
            .BorderLeft(_isLandscape ? 0 : DanfeTheme.EspessuraBorda)
            .BorderColor(DanfeTheme.CorBorda)
            .Padding(5)
            .Row(row =>
            {
                if (_emitente.LogoBytes != null && _emitente.LogoBytes.Length > 0)
                {
                    row.ConstantItem(65) // Largura reservada para o logotipo
                        .Height(55)
                        .Image(_emitente.LogoBytes)
                        .FitArea();
                    
                    row.RelativeItem()
                        .PaddingLeft(5)
                        .Element(ComposeText);
                }
                else
                {
                    row.RelativeItem()
                        .Element(ComposeText);
                }
            });
    }

    private void ComposeText(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Text(_emitente.RazaoSocial.ToUpper())
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(DanfeTheme.TamanhoFonteValorDestaque)
                .Bold();

            if (!string.IsNullOrWhiteSpace(_emitente.NomeFantasia) &&
                !_emitente.NomeFantasia.Equals(_emitente.RazaoSocial, System.StringComparison.OrdinalIgnoreCase))
            {
                column.Item().Text(_emitente.NomeFantasia.ToUpper())
                    .FontFamily(DanfeTheme.FontePadrao)
                    .FontSize(DanfeTheme.TamanhoFonteValor)
                    .Italic();
            }

            string enderecoText = $"{_emitente.Endereco.Logradouro}, {_emitente.Endereco.Numero}";
            if (!string.IsNullOrWhiteSpace(_emitente.Endereco.Complemento))
            {
                enderecoText += $" - {_emitente.Endereco.Complemento}";
            }
            enderecoText += $"\n{_emitente.Endereco.Bairro} - CEP: {_emitente.Endereco.Cep}";
            enderecoText += $"\n{_emitente.Endereco.Municipio} - {_emitente.Endereco.Uf}";
            
            if (!string.IsNullOrWhiteSpace(_emitente.Telefone))
            {
                enderecoText += $" - Fone: {_emitente.Telefone}";
            }

            column.Item().Text(enderecoText)
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(DanfeTheme.TamanhoFonteSubtitulo)
                .LineHeight(1.2f);
        });
    }
}

public static class EmitenteBoxExtensions
{
    public static void EmitenteBox(this IContainer container, Emitente emitente, bool isLandscape = false)
    {
        container.Component(new EmitenteBox(emitente, isLandscape));
    }
}
