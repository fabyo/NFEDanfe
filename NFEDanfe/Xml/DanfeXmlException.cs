using System;

namespace NFEDanfe.Xml;

/// <summary>Exceção lançada quando o XML de NF-e não contém um campo obrigatório ou está estruturalmente inválido.</summary>
public sealed class DanfeXmlException : Exception
{
    /// <summary>XPath do elemento ausente.</summary>
    public string XPath { get; }

    /// <summary>Cria uma nova exceção de parsing de XML.</summary>
    public DanfeXmlException(string xpath, string message)
        : base($"Elemento obrigatório ausente no XML da NF-e: {xpath}. {message}")
    {
        XPath = xpath;
    }
}
