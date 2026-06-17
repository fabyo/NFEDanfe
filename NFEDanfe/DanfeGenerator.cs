using QuestPDF.Fluent;
using NFEDanfe.Domain.Models;
using NFEDanfe.Domain.Parser;
using NFEDanfe.Domain.Validation;
using NFEDanfe.Layout;

namespace NFEDanfe;

public static class DanfeGenerator
{
    public static DanfeModel LoadFromXml(string xmlPath, DanfeOptions? options = null)
    {
        DanfeModel model = DanfeXmlParser.Parse(xmlPath);
        return PrepareModel(model, options ?? DanfeOptions.Default);
    }

    public static DanfeModel LoadFromXml(Stream xmlStream, DanfeOptions? options = null)
    {
        DanfeModel model = DanfeXmlParser.Parse(xmlStream);
        return PrepareModel(model, options ?? DanfeOptions.Default);
    }

    public static DanfeModel LoadFromXmlContent(string xmlContent, DanfeOptions? options = null)
    {
        DanfeModel model = DanfeXmlParser.ParseXmlContent(xmlContent);
        return PrepareModel(model, options ?? DanfeOptions.Default);
    }

    public static void GenerateFromXml(string xmlPath, Stream output, DanfeOptions? options = null)
    {
        DanfeOptions effectiveOptions = options ?? DanfeOptions.Default;
        DanfeModel model = LoadFromXml(xmlPath, effectiveOptions);
        Generate(model, output, effectiveOptions);
    }

    public static void GenerateFromXml(Stream xmlStream, Stream output, DanfeOptions? options = null)
    {
        DanfeOptions effectiveOptions = options ?? DanfeOptions.Default;
        DanfeModel model = LoadFromXml(xmlStream, effectiveOptions);
        Generate(model, output, effectiveOptions);
    }

    public static void GenerateFromXmlContent(string xmlContent, Stream output, DanfeOptions? options = null)
    {
        DanfeOptions effectiveOptions = options ?? DanfeOptions.Default;
        DanfeModel model = LoadFromXmlContent(xmlContent, effectiveOptions);
        Generate(model, output, effectiveOptions);
    }

    public static void Generate(DanfeModel model, Stream output, DanfeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(output);

        DanfeOptions effectiveOptions = options ?? DanfeOptions.Default;
        DanfeModel preparedModel = PrepareModel(model, effectiveOptions);
        DanfeDocumentFactory.Create(preparedModel, effectiveOptions).GeneratePdf(output);
    }

    public static void Generate(DanfeModel model, string outputPath, DanfeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(model);

        using FileStream output = File.Create(outputPath);
        Generate(model, output, options);
    }

    private static DanfeModel PrepareModel(DanfeModel model, DanfeOptions options)
    {
        DanfeModel prepared = model;

        if (options.LogoBytes is { Length: > 0 })
        {
            prepared = prepared with
            {
                Emitente = prepared.Emitente with { LogoBytes = options.LogoBytes }
            };
        }

        if (options.TipoImpressaoOverride.HasValue)
        {
            prepared = prepared with
            {
                DadosDanfe = prepared.DadosDanfe with { TipoImpressao = options.TipoImpressaoOverride.Value }
            };
        }

        if (options.ValidateBeforeGenerate)
        {
            DanfeValidator.Validate(prepared);
        }

        return prepared;
    }
}
