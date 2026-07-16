using System.Xml.Linq;
using NFEDanfe.Options;
using UglyToad.PdfPig;
using Xunit;

namespace NFEDanfe.Tests;

public sealed class PdfContentVerificationTests
{
    [Fact]
    public void Generated_pdf_text_is_audited_without_external_pdftotext()
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();
        XDocument document = XDocument.Load(xmlPath);
        XNamespace ns = "http://www.portalfiscal.inf.br/nfe";
        XElement infAdic = document.Descendants(ns + "infAdic").Single();

        infAdic.Element(ns + "infCpl")!.Value =
            "Linha complementar 1<br />Linha complementar 2;<strong>R$ 511,10</strong>";
        infAdic.Element(ns + "infAdFisco")!.Value =
            "<p>Reservado ao fisco</p><script>conteúdo proibido</script>";

        using var output = new MemoryStream();
        DanfeGenerator.GenerateFromXmlContent(
            document.ToString(SaveOptions.DisableFormatting),
            output,
            new DanfeOptions { UseDefaultLogo = false });

        using PdfDocument pdf = PdfDocument.Open(output.ToArray());
        string extractedText = string.Join('\n', pdf.GetPages().Select(page => page.Text));
        string compactText = RemoveWhitespace(extractedText);

        Assert.Contains("Linhacomplementar1", compactText);
        Assert.Contains("Linhacomplementar2", compactText);
        Assert.Contains("R$511,10", compactText);
        Assert.Contains("Reservadoaofisco", compactText);
        Assert.DoesNotContain("<br", compactText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<strong", compactText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("conteúdoproibido", compactText, StringComparison.OrdinalIgnoreCase);
    }

    private static string RemoveWhitespace(string value) => new(value.Where(character => !char.IsWhiteSpace(character)).ToArray());
}
