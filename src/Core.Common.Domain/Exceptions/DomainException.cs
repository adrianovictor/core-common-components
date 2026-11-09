namespace Core.Common.Domain.Exceptions;

/// <summary>
/// Regra de negócio violada. Base de todas as exceções do domínio.
/// </summary>
/// <remarks>
/// A camada de API traduz cada tipo num status HTTP (veja <c>Core.Common.Api</c>):
/// <see cref="ValidationException"/> vira 400, <see cref="NotFoundException"/> vira 404,
/// <see cref="ConflictException"/> vira 409 e as demais viram 422.
/// A mensagem é devolvida ao cliente, então escreva para quem usa a API.
/// </remarks>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}
