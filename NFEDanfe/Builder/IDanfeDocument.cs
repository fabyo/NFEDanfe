namespace NFEDanfe.Builder;

/// <summary>Interface fluente para construção do documento DANFE.</summary>
public interface IDanfeDocument
{
    /// <summary>Configura opções do documento.</summary>
    IDanfeDocument Configure(Action<DanfeDocumentOptions> configure);

    /// <summary>Adiciona uma página ao documento.</summary>
    IDanfeDocument AddPage(Action<IDanfePage> configure);

    /// <summary>Gera o PDF e retorna os bytes.</summary>
    byte[] BuildAsBytes();

    /// <summary>Gera o PDF e salva no stream.</summary>
    void BuildAsStream(Stream stream);
}
