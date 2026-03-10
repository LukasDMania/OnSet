namespace OnSet.Application.Exceptions;

public class DomainRuleException : Exception
{
    public DomainRuleException(string message) : base(message) { }
}

