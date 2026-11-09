namespace Core.Common.Domain.Exceptions;

/// <summary>A operação conflita com o estado atual, por exemplo um cadastro duplicado.</summary>
public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message) { }
}
