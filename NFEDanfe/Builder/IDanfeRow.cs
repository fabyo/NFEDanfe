namespace NFEDanfe.Builder;

/// <summary>Interface para adição de campos em uma linha do DANFE.</summary>
public interface IDanfeRow
{
    /// <summary>Adiciona um campo à linha.</summary>
    IDanfeRow Field(string label, string value, double widthPct);
}
