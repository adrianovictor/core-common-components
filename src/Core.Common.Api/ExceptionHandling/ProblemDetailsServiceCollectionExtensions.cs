using Core.Common.Api.ExceptionHandling;

// Mesmo namespace do AddProblemDetails, para aparecer junto no IntelliSense.
namespace Microsoft.Extensions.DependencyInjection;

public static class ProblemDetailsServiceCollectionExtensions
{
    /// <summary>
    /// Registra o <c>ProblemDetails</c> e o tratamento das exceções do domínio.
    /// Na aplicação, chame também <c>app.UseExceptionHandler()</c>.
    /// </summary>
    public static IServiceCollection AddDomainProblemDetails(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddProblemDetails();
        services.AddExceptionHandler<DomainExceptionHandler>();
        return services;
    }
}
