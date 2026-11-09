using Core.Common.Domain.Exceptions;

namespace Core.Common.Domain.Tests.Exceptions;

public class ValidationExceptionTests
{
    [Fact]
    public void SingleFieldErrorUsesTheErrorAsMessage()
    {
        var exception = new ValidationException("plate", "Placa inválida.");

        Assert.Equal("Placa inválida.", exception.Message);
        Assert.Equal(["Placa inválida."], exception.Errors["plate"]);
    }

    [Fact]
    public void SeveralErrorsUseTheDefaultMessage()
    {
        var exception = new ValidationException(new Dictionary<string, string[]>
        {
            ["plate"] = ["Placa inválida."],
            ["year"] = ["Ano no futuro."],
        });

        Assert.Equal(ValidationException.DefaultMessage, exception.Message);
        Assert.Equal(2, exception.Errors.Count);
    }

    [Fact]
    public void ErrorsAreCopied()
    {
        // alterar o dicionário original depois não pode mudar a exceção já lançada
        var errors = new Dictionary<string, string[]> { ["plate"] = ["Placa inválida."] };
        var exception = new ValidationException(errors);

        errors["year"] = ["Ano no futuro."];

        Assert.Single(exception.Errors);
    }

    [Fact]
    public void RequiresAtLeastOneError()
    {
        Assert.Throws<ArgumentException>(() => new ValidationException(new Dictionary<string, string[]>()));
    }

    [Fact]
    public void AllDomainExceptionsShareTheBaseType()
    {
        Assert.IsAssignableFrom<DomainException>(new NotFoundException("x"));
        Assert.IsAssignableFrom<DomainException>(new ConflictException("x"));
        Assert.IsAssignableFrom<DomainException>(new ValidationException("f", "x"));
    }
}
