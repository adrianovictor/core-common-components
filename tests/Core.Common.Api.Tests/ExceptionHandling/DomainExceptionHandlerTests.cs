using System.Net;
using System.Text.Json;
using Core.Common.Domain.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Common.Api.Tests.ExceptionHandling;

/// <summary>Sobe uma API de verdade em memória e confere a resposta HTTP de cada exceção.</summary>
public class DomainExceptionHandlerTests
{
    private static async Task<(HttpStatusCode Status, string? ContentType, JsonElement Body)> GetAsync(Exception toThrow)
    {
        var ct = TestContext.Current.CancellationToken;

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddDomainProblemDetails();

        await using var app = builder.Build();
        app.UseExceptionHandler();
        app.MapGet("/", () => { throw toThrow; });
        await app.StartAsync(ct);

        using var response = await app.GetTestClient().GetAsync("/", ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        return (response.StatusCode, response.Content.Headers.ContentType?.MediaType, JsonDocument.Parse(body).RootElement.Clone());
    }

    [Fact]
    public async Task NotFoundBecomes404()
    {
        var (status, contentType, body) = await GetAsync(new NotFoundException("Veículo 9999 não encontrado."));

        Assert.Equal(HttpStatusCode.NotFound, status);
        Assert.Equal("application/problem+json", contentType);
        Assert.Equal(404, body.GetProperty("status").GetInt32());
        Assert.Equal("Recurso não encontrado", body.GetProperty("title").GetString());
        Assert.Equal("Veículo 9999 não encontrado.", body.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task ConflictBecomes409()
    {
        var (status, _, _) = await GetAsync(new ConflictException("Já existe um veículo com essa placa."));

        Assert.Equal(HttpStatusCode.Conflict, status);
    }

    [Fact]
    public async Task OtherDomainRulesBecome422()
    {
        var (status, _, body) = await GetAsync(new DomainException("Veículo excluído não muda de status."));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, status);
        Assert.Equal("Veículo excluído não muda de status.", body.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task ValidationBecomes400WithErrorsByField()
    {
        var (status, _, body) = await GetAsync(new ValidationException("plate", "Placa inválida."));

        Assert.Equal(HttpStatusCode.BadRequest, status);
        var errors = body.GetProperty("errors").GetProperty("plate");
        Assert.Equal("Placa inválida.", errors[0].GetString());
    }

    [Fact]
    public async Task ResponseCarriesTraceIdForSupport()
    {
        var (_, _, body) = await GetAsync(new NotFoundException("x"));

        Assert.False(string.IsNullOrEmpty(body.GetProperty("traceId").GetString()));
    }

    [Fact]
    public async Task UnexpectedErrorsBecome500WithoutLeakingTheMessage()
    {
        var (status, contentType, body) = await GetAsync(new InvalidOperationException("Connection string: Server=prod;Password=123"));

        Assert.Equal(HttpStatusCode.InternalServerError, status);
        Assert.Equal("application/problem+json", contentType);
        Assert.DoesNotContain("Password", body.GetRawText());
    }
}
