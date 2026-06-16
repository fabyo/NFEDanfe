using NFEDanfe;
using NFEDanfe.Domain.Parser;
using QuestPDF.Infrastructure;
using Xunit;

namespace NFEDanfe.Tests;

public sealed class ParserAndSnapshotTests
{
    static ParserAndSnapshotTests()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [Fact]
    public void SampleXml_is_parsed_with_real_data()
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();

        var model = DanfeXmlParser.Parse(xmlPath);

        Assert.Equal("EMPRESA EXEMPLO LTDA", model.Emitente.RazaoSocial);
        Assert.Equal("EXEMPLO INDUSTRIAL", model.Emitente.NomeFantasia);
        Assert.Equal("CLIENTE EXEMPLO LTDA", model.Destinatario.RazaoSocial);
        Assert.NotNull(model.Produtos);
        Assert.Single(model.Produtos);
        Assert.Equal("PRODUTO EXEMPLO PARA DANFE", model.Produtos[0].Descricao);
        Assert.Equal(200.00m, model.ValorTotal);
        Assert.Equal(1, model.DadosDanfe.TipoImpressao);
    }

    [Fact]
    public void Snapshot_includes_critical_fields()
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();
        var model = DanfeXmlParser.Parse(xmlPath);

        string snapshot = DanfeSnapshot.CreateText(model);

        Assert.Contains("Chave=", snapshot);
        Assert.Contains("Emitente=EMPRESA EXEMPLO LTDA", snapshot);
        Assert.Contains("Destinatario=CLIENTE EXEMPLO LTDA", snapshot);
        Assert.Contains("Produtos.Count=1", snapshot);
        Assert.Contains("Produto=PROD-001", snapshot);
    }

    [Fact]
    public void Generate_from_xml_to_stream_produces_pdf_bytes()
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();
        string outputPath = Path.Combine(Path.GetTempPath(), $"danfe-{Guid.NewGuid():N}.pdf");

        try
        {
            using FileStream output = File.Create(outputPath);
            DanfeGenerator.GenerateFromXml(xmlPath, output, new DanfeOptions { ValidateBeforeGenerate = true });

            Assert.True(new FileInfo(outputPath).Length > 0);
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }

    [Fact]
    public void Landscape_override_changes_the_loaded_model_orientation()
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();

        var model = DanfeGenerator.LoadFromXml(xmlPath, new DanfeOptions
        {
            TipoImpressaoOverride = 2
        });

        Assert.Equal(2, model.DadosDanfe.TipoImpressao);
    }

    [Fact]
    public void Malformed_xml_is_rejected()
    {
        string outputPath = Path.Combine(Path.GetTempPath(), $"danfe-invalid-{Guid.NewGuid():N}.xml");
        File.WriteAllText(outputPath, "<nfeProc><broken></nfeProc>");

        try
        {
            Assert.ThrowsAny<Exception>(() => DanfeGenerator.LoadFromXml(outputPath));
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }

    [Fact]
    public void Missing_nfe_root_node_is_rejected_by_parser()
    {
        string outputPath = Path.Combine(Path.GetTempPath(), $"danfe-missing-root-{Guid.NewGuid():N}.xml");
        File.WriteAllText(outputPath, "<?xml version=\"1.0\"?><outroNode></outroNode>");

        try
        {
            Assert.Throws<InvalidOperationException>(() => DanfeXmlParser.Parse(outputPath));
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }
}
