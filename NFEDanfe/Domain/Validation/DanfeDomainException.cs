using System;

namespace NFEDanfe.Domain.Validation;

public class DanfeDomainException : Exception
{
    public DanfeDomainException(string message) : base(message) { }
}
