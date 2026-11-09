using Core.Common.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Core.Common.Api.ExceptionHandling;

/// <summary>
/// Traduz as exceções do domínio em respostas <c>application/problem+json</c> (RFC 9457).
/// </summary>
/// <remarks>
/// Só trata <see cref="DomainException"/> e derivadas. As demais seguem para o tratamento padrão
/// do ASP.NET Core, que responde 500 sem expor a mensagem da exceção.
/// </remarks>
internal sealed partial class DomainExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<DomainExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        var (status, title) = domainException switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Dados inválidos"),
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflito com o estado atual"),
            _ => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada"),
        };

        var problem = domainException is ValidationException validation
            ? new HttpValidationProblemDetails(validation.Errors.ToDictionary())
            : new ProblemDetails();

        problem.Status = status;
        problem.Title = title;
        problem.Detail = domainException.Message;

        LogRejected(logger, status, domainException.GetType().Name, domainException.Message);

        httpContext.Response.StatusCode = status;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = domainException,
        });
    }

    // Recusa por regra de negócio é resposta esperada, não falha do servidor: fica em Information.
    [LoggerMessage(Level = LogLevel.Information, Message = "Requisição recusada com {StatusCode} ({ExceptionType}): {Detail}")]
    private static partial void LogRejected(ILogger logger, int statusCode, string exceptionType, string detail);
}
