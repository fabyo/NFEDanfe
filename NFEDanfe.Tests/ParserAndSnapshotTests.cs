using NFEDanfe;
using NFEDanfe.Domain.Parser;
using NFEDanfe.Options;
using PdfSharp.Pdf.IO;
using Xunit;

namespace NFEDanfe.Tests;

public sealed class ParserAndSnapshotTests
{
    [Fact]
    public void LoadFromXmlContent_parses_raw_xml_string()
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();
        string xmlContent = File.ReadAllText(xmlPath);

        var model = DanfeGenerator.LoadFromXmlContent(xmlContent);

        Assert.Equal("EMPRESA EXEMPLO LTDA", model.Emitente.RazaoSocial);
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
        Assert.Equal(5, model.Produtos.Count);
        Assert.Equal("TUBO ACO INOX ASTM A312 TP316L Ø42,20MM X 2,77MM ESP X 6000MM", model.Produtos[0].Descricao);
        Assert.Equal(13857.00m, model.ValorTotal);
        Assert.Equal(1, model.DadosDanfe.TipoImpressao);
    }

    [Fact]
    public void Purchase_order_is_parsed_into_additional_data()
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();
        System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(xmlPath);
        System.Xml.Linq.XNamespace ns = "http://www.portalfiscal.inf.br/nfe";
        var infNFe = doc.Descendants(ns + "infNFe").First();

        infNFe.Add(new System.Xml.Linq.XElement(
            ns + "compra",
            new System.Xml.Linq.XElement(ns + "xPed", "181712")));

        var model = DanfeXmlParser.ParseDocument(doc);

        Assert.NotNull(model.DadosAdicionais);
        Assert.Contains("181712", model.DadosAdicionais.PedidosCompra!);
    }

    [Fact]
    public void UserXml_decodes_special_characters_in_descriptions()
    {
        string xmlPath = IntegrationTestHelpers.FindTestDataXml("special-chars-1-product-procNFe.xml");

        var model = DanfeXmlParser.Parse(xmlPath);

        Assert.NotNull(model.Produtos);
        Assert.NotEmpty(model.Produtos);
        Assert.NotNull(model.Produtos[0].Descricao);
        Assert.Contains("Ø", model.Produtos[0].Descricao);
        Assert.Equal("TUBO ACO DIN EN 10305-2 E195+A-Ø12+0,08x1,50+-0,15X44+0,5", model.Produtos[0].Descricao);
    }

    [Fact]
    public void UserXml2_decodes_special_characters_in_descriptions()
    {
        string xmlPath = IntegrationTestHelpers.FindTestDataXml("special-chars-2-products-procNFe.xml");

        var model = DanfeXmlParser.Parse(xmlPath);

        Assert.NotNull(model.Produtos);
        Assert.Equal(2, model.Produtos.Count);
        Assert.NotNull(model.Produtos[1].Descricao);
        Assert.Contains("Ø", model.Produtos[1].Descricao);
        Assert.Equal("TUBO ACO TREFILADO S/C DIN 2391 SAE 1045 Ø26,60X16,00X 3 A 7M", model.Produtos[1].Descricao);
    }

    [Fact]
    public void Homologation_environment_is_parsed_from_tpAmb()
    {
        string xmlPath = IntegrationTestHelpers.FindTestDataXml("special-chars-1-product-procNFe.xml");

        var model = DanfeXmlParser.Parse(xmlPath);

        Assert.Equal(2, model.DadosDanfe.TipoAmbiente);
    }

    [Fact]
    public void FcpParsing_HandlesNullEmptyAndValue()
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();

        // 1. Tag present with 0.00
        var modelOriginal = DanfeXmlParser.Parse(xmlPath);
        Assert.Equal(0.00m, modelOriginal.Impostos!.ValorFcp);

        // 2. Empty tag
        System.Xml.Linq.XDocument docEmpty = System.Xml.Linq.XDocument.Load(xmlPath);
        System.Xml.Linq.XNamespace ns = "http://www.portalfiscal.inf.br/nfe";
        var vFcpEmpty = docEmpty.Descendants(ns + "vFCP").FirstOrDefault();
        Assert.NotNull(vFcpEmpty);
        vFcpEmpty.Value = "";

        var modelEmpty = DanfeXmlParser.ParseDocument(docEmpty);
        Assert.Null(modelEmpty.Impostos!.ValorFcp);

        // 3. Absent tag
        System.Xml.Linq.XDocument docAbsent = System.Xml.Linq.XDocument.Load(xmlPath);
        var vFcpAbsent = docAbsent.Descendants(ns + "vFCP").FirstOrDefault();
        Assert.NotNull(vFcpAbsent);
        vFcpAbsent.Remove();

        var modelAbsent = DanfeXmlParser.ParseDocument(docAbsent);
        Assert.Null(modelAbsent.Impostos!.ValorFcp);
    }

    [Fact]
    public void FontName_FromOptions_IsRespectedDuringComposition()
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();
        var model = DanfeXmlParser.Parse(xmlPath);

        using var stream = new MemoryStream();
        var options = new DanfeOptions { CustomFontName = "MyCustomFontFamilyName" };

        DanfeGenerator.Generate(model, stream, options);

        Assert.True(stream.Length > 0);
    }

    [Fact]
    public void FontEnum_FromOptions_IsRespectedDuringComposition()
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();
        var model = DanfeXmlParser.Parse(xmlPath);

        using var stream = new MemoryStream();
        var options = new DanfeOptions { Font = DanfeFont.Inter };

        DanfeGenerator.Generate(model, stream, options);

        Assert.True(stream.Length > 0);
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
    public void Generate_from_xml_landscape_produces_pdf_bytes()
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();
        string outputPath = Path.Combine(Path.GetTempPath(), $"danfe-landscape-{Guid.NewGuid():N}.pdf");

        try
        {
            using FileStream output = File.Create(outputPath);
            DanfeGenerator.GenerateFromXml(xmlPath, output, new DanfeOptions
            {
                ValidateBeforeGenerate = true,
                TipoImpressaoOverride = 2 // Paisagem
            });

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

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void Generated_pdf_has_expected_page_orientation(int orientation, bool landscape)
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();
        using var output = new MemoryStream();

        DanfeGenerator.GenerateFromXml(xmlPath, output, new DanfeOptions
        {
            ValidateBeforeGenerate = true,
            TipoImpressaoOverride = orientation
        });

        output.Position = 0;
        using var document = PdfReader.Open(output, PdfDocumentOpenMode.Import);
        Assert.NotEmpty(document.Pages);
        foreach (var page in document.Pages)
        {
            bool pageIsLandscape = page.Width.Point > page.Height.Point;
            Assert.Equal(landscape, pageIsLandscape);
        }
    }

    [Fact]
    public void TpImp_2_in_xml_generates_landscape_without_override()
    {
        string xmlPath = IntegrationTestHelpers.FindTestDataXml("special-chars-1-product-procNFe.xml");
        string xmlContent = File.ReadAllText(xmlPath).Replace("<tpImp>1</tpImp>", "<tpImp>2</tpImp>");
        using var output = new MemoryStream();

        DanfeGenerator.GenerateFromXmlContent(xmlContent, output);

        output.Position = 0;
        using var document = PdfReader.Open(output, PdfDocumentOpenMode.Import);
        Assert.NotEmpty(document.Pages);
        Assert.All(document.Pages.Cast<PdfSharp.Pdf.PdfPage>(), page =>
            Assert.True(page.Width.Point > page.Height.Point));
    }

    [Fact]
    public void Authorized_cancellation_event_marks_invoice_as_canceled()
    {
        string xmlPath = IntegrationTestHelpers.FindTestDataXml("special-chars-1-product-procNFe.xml");
        System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(xmlPath);
        System.Xml.Linq.XNamespace ns = "http://www.portalfiscal.inf.br/nfe";
        doc.Root!.Add(new System.Xml.Linq.XElement(ns + "procEventoNFe",
            new System.Xml.Linq.XElement(ns + "evento",
                new System.Xml.Linq.XElement(ns + "infEvento",
                    new System.Xml.Linq.XElement(ns + "tpEvento", "110111"))),
            new System.Xml.Linq.XElement(ns + "retEvento",
                new System.Xml.Linq.XElement(ns + "infEvento",
                    new System.Xml.Linq.XElement(ns + "cStat", "101")))));

        var model = DanfeXmlParser.ParseDocument(doc);

        Assert.True(model.DadosDanfe.IsCancelada);
    }

    [Fact]
    public void Generate_with_many_products_paginates_without_layout_errors()
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();
        var model = DanfeGenerator.LoadFromXml(xmlPath);
        var produtos = Enumerable.Range(1, 80)
            .Select(i => model.Produtos![0] with
            {
                Codigo = $"PROD-{i:000}",
                Descricao = $"PRODUTO EXEMPLO PARA TESTE DE PAGINACAO {i:000}"
            })
            .ToList();

        var largeModel = model with { Produtos = produtos };
        string outputPath = Path.Combine(Path.GetTempPath(), $"danfe-many-products-{Guid.NewGuid():N}.pdf");

        try
        {
            using (FileStream output = File.Create(outputPath))
            {
                DanfeGenerator.Generate(largeModel, output, new DanfeOptions { ValidateBeforeGenerate = false });
            }

            Assert.True(new FileInfo(outputPath).Length > 0);
            Assert.True(GetPdfPageCount(outputPath) > 1);
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
    public void Generate_landscape_with_many_products_and_long_descriptions_paginates_without_layout_errors()
    {
        string xmlPath = IntegrationTestHelpers.FindSampleXml();
        var model = DanfeGenerator.LoadFromXml(xmlPath, new DanfeOptions
        {
            TipoImpressaoOverride = 2
        });

        var produtos = Enumerable.Range(1, 60)
            .Select(i => model.Produtos![0] with
            {
                Codigo = $"LAND-{i:000}",
                Descricao = $"PRODUTO EXEMPLO PARA TESTE DE PAGINACAO EM PAISAGEM COM DESCRICAO LONGA {i:000} - ITEM AUXILIAR DE VALIDACAO"
            })
            .ToList();

        var largeModel = model with { Produtos = produtos };
        string outputPath = Path.Combine(Path.GetTempPath(), $"danfe-landscape-many-products-{Guid.NewGuid():N}.pdf");

        try
        {
            using (FileStream output = File.Create(outputPath))
            {
                DanfeGenerator.Generate(largeModel, output, new DanfeOptions
                {
                    ValidateBeforeGenerate = false,
                    TipoImpressaoOverride = 2
                });
            }

            Assert.True(new FileInfo(outputPath).Length > 0);
            Assert.True(GetPdfPageCount(outputPath) > 1);
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

    [Fact]
    public void AllGeneratedPdfs_are_exactly_one_page()
    {
        var xmlFiles = FindAllSampleXmls();
        Assert.NotEmpty(xmlFiles);

        byte[]? logoBytes = GetRealLogoBytes();

        List<string> errors = new();

        List<string> diagnosticInfo = new();
        foreach (var xmlPath in xmlFiles)
        {
            var optionsList = new[]
            {
                new { Name = "Portrait_NoLogo", Orientation = 1, HasLogo = false },
                new { Name = "Portrait_WithLogo", Orientation = 1, HasLogo = true },
                new { Name = "Landscape_NoLogo", Orientation = 2, HasLogo = false },
                new { Name = "Landscape_WithLogo", Orientation = 2, HasLogo = true }
            };

            foreach (var opt in optionsList)
            {
                string pdfPath = Path.Combine(Path.GetTempPath(), $"danfe-{Path.GetFileNameWithoutExtension(xmlPath)}-{opt.Name}.pdf");
                try
                {
                    using (FileStream output = File.Create(pdfPath))
                    {
                        DanfeGenerator.GenerateFromXml(xmlPath, output, new DanfeOptions
                        {
                            ValidateBeforeGenerate = true,
                            TipoImpressaoOverride = opt.Orientation,
                            LogoBytes = opt.HasLogo ? logoBytes : null,
                            EmitFooter = true
                        });
                    }

                    int pageCount = GetPdfPageCount(pdfPath);
                    diagnosticInfo.Add($"{Path.GetFileName(xmlPath)} ({opt.Name}): {pageCount} pages");
                    if (pageCount != 1)
                    {
                        errors.Add($"{Path.GetFileName(xmlPath)} ({opt.Name}): expected 1 page, but got {pageCount} pages");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{Path.GetFileName(xmlPath)} ({opt.Name}) failed to generate: {ex.Message}");
                }
                finally
                {
                    if (File.Exists(pdfPath))
                    {
                        File.Delete(pdfPath);
                    }
                }
            }
        }

        if (errors.Count > 0)
        {
            throw new Exception("PDF page count validation errors:\n" + string.Join("\n", errors) + "\n\nAll results:\n" + string.Join("\n", diagnosticInfo));
        }
    }

    private static byte[]? GetRealLogoBytes()
    {
        string? current = AppContext.BaseDirectory;
        for (int i = 0; i < 8 && !string.IsNullOrWhiteSpace(current); i++)
        {
            string candidate = Path.Combine(current, "logo.png");
            if (File.Exists(candidate))
            {
                return File.ReadAllBytes(candidate);
            }
            current = Directory.GetParent(current)?.FullName;
        }
        return null;
    }


    private static List<string> FindAllSampleXmls()
    {
        string? current = AppContext.BaseDirectory;
        for (int i = 0; i < 8 && !string.IsNullOrWhiteSpace(current); i++)
        {
            string candidateDir = Path.Combine(current, "samples");
            if (Directory.Exists(candidateDir))
            {
                return Directory.GetFiles(candidateDir, "*.xml").ToList();
            }
            current = Directory.GetParent(current)?.FullName;
        }
        return new List<string>();
    }

    private static int GetPdfPageCount(string path)
    {
        using var document = PdfReader.Open(path, PdfDocumentOpenMode.Import);
        return document.PageCount;
    }
}
