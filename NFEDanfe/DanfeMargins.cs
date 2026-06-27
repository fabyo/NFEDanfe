namespace NFEDanfe;

/// <summary>Margens do DANFE em milímetros.</summary>
public sealed class DanfeMargins
{
    /// <summary>Margem superior em mm.</summary>
    public double Top { get; }

    /// <summary>Margem direita em mm.</summary>
    public double Right { get; }

    /// <summary>Margem inferior em mm.</summary>
    public double Bottom { get; }

    /// <summary>Margem esquerda em mm.</summary>
    public double Left { get; }

    /// <summary>Cria margens iguais em todos os lados.</summary>
    public DanfeMargins(double all = 5.0)
    {
        Top = all;
        Right = all;
        Bottom = all;
        Left = all;
    }

    /// <summary>Cria margens independentes para cada lado.</summary>
    public DanfeMargins(double top, double right, double bottom, double left)
    {
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
    }
}
