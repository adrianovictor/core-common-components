namespace Core.Common.Domain.Exceptions;

/// <summary>
/// Um ou mais valores de entrada são inválidos. Guarda os erros por campo,
/// para o cliente mostrar cada mensagem no lugar certo.
/// </summary>
public class ValidationException : DomainException
{
    public const string DefaultMessage = "Um ou mais campos são inválidos.";

    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(string field, string error)
        : this(new Dictionary<string, string[]> { [field] = [error] }, error) { }

    public ValidationException(IDictionary<string, string[]> errors, string message = DefaultMessage)
        : base(message)
    {
        ArgumentNullException.ThrowIfNull(errors);
        if (errors.Count == 0)
        {
            throw new ArgumentException("Informe ao menos um erro.", nameof(errors));
        }

        Errors = new Dictionary<string, string[]>(errors);
    }
}
