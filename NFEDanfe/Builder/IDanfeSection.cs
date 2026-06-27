namespace NFEDanfe.Builder;

/// <summary>Interface para adição de linhas em uma seção do DANFE.</summary>
public interface IDanfeSection
{
    /// <summary>Adiciona uma linha de campos à seção.</summary>
    IDanfeSection Row(Action<IDanfeRow> configure);
}
