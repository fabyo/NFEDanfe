using System;
using System.IO;
using NFEDanfe.Domain.Models;
using NFEDanfe.Domain.Parser;
using NFEDanfe.Layout;
using NFEDanfe.Options;

namespace NFEDanfe;

/// <summary>Gerador principal de DANFE para compatibilidade total com NFEDanfe.</summary>
public static class DanfeGenerator
{
    /// <summary>Carrega o modelo a partir do caminho do arquivo XML.</summary>
    public static DanfeModel LoadFromXml(string xmlPath, DanfeOptions? options = null)
    {
        DanfeModel model = DanfeXmlParser.Parse(xmlPath, options);
        return PrepareModel(model, options ?? DanfeOptions.Default);
    }

    /// <summary>Carrega o modelo a partir de um Stream XML.</summary>
    public static DanfeModel LoadFromXml(Stream xmlStream, DanfeOptions? options = null)
    {
        DanfeModel model = DanfeXmlParser.Parse(xmlStream, options);
        return PrepareModel(model, options ?? DanfeOptions.Default);
    }

    /// <summary>Carrega o modelo a partir de uma string contendo o XML.</summary>
    public static DanfeModel LoadFromXmlContent(string xmlContent, DanfeOptions? options = null)
    {
        DanfeModel model = DanfeXmlParser.ParseXmlContent(xmlContent);
        return PrepareModel(model, options ?? DanfeOptions.Default);
    }

    /// <summary>Gera o DANFE em PDF a partir do caminho do arquivo XML.</summary>
    public static void GenerateFromXml(string xmlPath, Stream output, DanfeOptions? options = null)
    {
        DanfeOptions effectiveOptions = options ?? DanfeOptions.Default;
        DanfeModel model = LoadFromXml(xmlPath, effectiveOptions);
        Generate(model, output, effectiveOptions);
    }

    /// <summary>Gera o DANFE em PDF a partir de um Stream XML.</summary>
    public static void GenerateFromXml(Stream xmlStream, Stream output, DanfeOptions? options = null)
    {
        DanfeOptions effectiveOptions = options ?? DanfeOptions.Default;
        DanfeModel model = LoadFromXml(xmlStream, effectiveOptions);
        Generate(model, output, effectiveOptions);
    }

    /// <summary>Gera o DANFE em PDF a partir de uma string contendo o XML.</summary>
    public static void GenerateFromXmlContent(string xmlContent, Stream output, DanfeOptions? options = null)
    {
        DanfeOptions effectiveOptions = options ?? DanfeOptions.Default;
        DanfeModel model = LoadFromXmlContent(xmlContent, effectiveOptions);
        Generate(model, output, effectiveOptions);
    }

    /// <summary>Gera o DANFE em PDF a partir de um DanfeModel para o Stream de saída.</summary>
    public static void Generate(DanfeModel model, Stream output, DanfeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(output);

        DanfeOptions effectiveOptions = options ?? DanfeOptions.Default;
        DanfeModel preparedModel = PrepareModel(model, effectiveOptions);
        
        var builder = new DanfeLayoutBuilder();
        builder.Build(preparedModel, effectiveOptions, output);
    }

    /// <summary>Gera o DANFE em PDF a partir de um DanfeModel salvando no caminho especificado.</summary>
    public static void Generate(DanfeModel model, string outputPath, DanfeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(model);
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("O caminho de saída não pode ser vazio.", nameof(outputPath));
        }

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

        if (options.CanceledOverride.HasValue)
        {
            prepared = prepared with
            {
                DadosDanfe = prepared.DadosDanfe with { IsCancelada = options.CanceledOverride.Value }
            };
        }

        return prepared;
    }
}
