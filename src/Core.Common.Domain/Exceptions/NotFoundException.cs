namespace Core.Common.Domain.Exceptions;

/// <summary>O recurso pedido não existe.</summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message) { }
}
