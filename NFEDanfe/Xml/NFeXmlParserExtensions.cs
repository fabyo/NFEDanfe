using System;
using System.IO;
using NFEDanfe.Layout;
using NFEDanfe.Options;

namespace NFEDanfe.Xml;

/// <summary>Extensões para converter DanfeData em documento PDF.</summary>
public static class NFeXmlParserExtensions
{
    /// <summary>Cria um builder de documento a partir dos dados da NF-e.</summary>
    public static DanfeDocumentBuilder ToDocument(
        this DanfeData data,
        Action<DanfeOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(data);
        var options = new DanfeOptions();
        options.Ambiente = data.Ambiente;
        configure?.Invoke(options);
        return new DanfeDocumentBuilder(data, options);
    }
}

/// <summary>Builder de documento DANFE a partir de DanfeData.</summary>
public sealed class DanfeDocumentBuilder
{
    private readonly DanfeData _data;
    private readonly DanfeOptions _options;

    /// <summary>Cria o builder com dados e opções.</summary>
    internal DanfeDocumentBuilder(DanfeData data, DanfeOptions options)
    {
        _data = data;
        _options = options;
    }

    /// <summary>Gera o PDF e retorna os bytes.</summary>
    public byte[] BuildAsBytes()
        => new DanfeLayoutBuilder().Build(_data, _options);

    /// <summary>Gera o PDF e salva no stream.</summary>
    public void BuildAsStream(Stream output)
        => new DanfeLayoutBuilder().Build(_data, _options, output);
}
