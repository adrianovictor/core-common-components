# core-common-components

[![CI](https://github.com/adrianovictor/core-common-components/actions/workflows/ci.yml/badge.svg)](https://github.com/adrianovictor/core-common-components/actions/workflows/ci.yml)

Biblioteca com as peças que se repetem entre minhas APIs .NET. Começa pequena, só com o que o [GeoTrack](https://github.com/adrianovictor/geo-track-api) usa, e só ganha um componente novo quando um segundo projeto precisar dele.

## O que tem

| Projeto | Conteúdo | Dependências |
|---|---|---|
| `Core.Common.Domain` | `Entity<TId>`, `AuditableEntity<TId>`, `ValueObject`, `PagedRequest`, `PagedResult<T>` e as exceções de domínio | nenhuma |
| `Core.Common.Api` | Tratamento das exceções de domínio com `ProblemDetails` (RFC 9457) | só o framework do ASP.NET Core |

Requer .NET 10.

## Como usar

### Entidades

```csharp
public class Vehicle : AuditableEntity<int>
{
    public Vehicle(BrazilianPlate plate) => Plate = plate;

    public BrazilianPlate Plate { get; private set; }
}
```

- **Igualdade por identidade:** duas instâncias com o mesmo tipo e o mesmo `Id` são iguais. Entidades ainda não salvas (`Id` padrão) só são iguais a si mesmas.
- **Datas de auditoria:** `CreatedAt` e `UpdatedAt` têm `protected set`. Quem preenche é a persistência, por exemplo um `SaveChangesInterceptor`:

```csharp
foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
{
    if (entry.State == EntityState.Added)
        entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = DateTime.UtcNow;
    else if (entry.State == EntityState.Modified)
        entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = DateTime.UtcNow;
}
```

### Value objects

```csharp
public sealed class BrazilianPlate : ValueObject
{
    public BrazilianPlate(string value)
    {
        // valide aqui e lance ValidationException("plate", "...") se for inválida
        Value = value.Replace("-", "").ToUpperInvariant();
    }

    public string Value { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

Para casos simples, um `record` já resolve. A base serve quando a igualdade precisa normalizar ou ignorar campos.

### Paginação

```csharp
var request = PagedRequest.Of(page: 2, pageSize: 20);   // página mínima 1, tamanho entre 1 e 100

var items = await query.OrderBy(v => v.Id).Skip(request.Skip).Take(request.PageSize).ToListAsync();
var total = await query.CountAsync();

PagedResult<VehicleDto> result = PagedResult<Vehicle>.From(items, total, request).Map(VehicleDto.From);
```

Valores fora do intervalo são corrigidos, não geram erro. O teto de 100 impede que um cliente peça a tabela inteira numa chamada.

### Erros da API

```csharp
builder.Services.AddDomainProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();
```

O domínio lança exceções sem saber nada de HTTP, e a API traduz:

| Exceção | Status | Quando usar |
|---|---|---|
| `ValidationException` | 400 | Valor de entrada inválido. Leva os erros por campo. |
| `NotFoundException` | 404 | O recurso pedido não existe. |
| `ConflictException` | 409 | Conflita com o estado atual, como um cadastro duplicado. |
| `DomainException` (e outras derivadas) | 422 | Outra regra de negócio violada. |
| qualquer outra | 500 | Erro inesperado. A mensagem não vai para o cliente. |

Exemplo de resposta:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Dados inválidos",
  "status": 400,
  "detail": "Placa inválida.",
  "errors": { "plate": ["Placa inválida."] },
  "traceId": "00-6f1c…-01"
}
```

A mensagem da exceção vai no `detail`. Escreva pensando em quem usa a API.

## Como adicionar a um projeto

Como submodule, fixado numa versão:

```bash
git submodule add https://github.com/adrianovictor/core-common-components.git libs/core-common-components
cd libs/core-common-components && git checkout v0.1.0 && cd -
git add .gitmodules libs/core-common-components
```

No `.csproj` que for usar:

```xml
<ProjectReference Include="..\..\libs\core-common-components\src\Core.Common.Domain\Core.Common.Domain.csproj" />
```

Quem clonar o projeto precisa trazer o submodule junto: `git clone --recurse-submodules`, ou `git submodule update --init` depois do clone. No GitHub Actions, use `submodules: true` no `actions/checkout`.

## Regras da lib

1. **O domínio não depende de nenhum pacote.** O CI falha se alguém adicionar um.
2. **Só entra o que um projeto usa.** Nada de "pode ser útil um dia".
3. **Aviso é erro.** `TreatWarningsAsErrors` está ligado, inclusive para a auditoria do NuGet: pacote com vulnerabilidade conhecida, mesmo indireto, quebra o build.
4. **Toda mudança tem teste** e entra no [CHANGELOG](CHANGELOG.md).

## Desenvolvimento

```bash
dotnet build
dotnet test --solution Core.Common.Components.slnx
```

Os testes usam xUnit v3 com a Microsoft.Testing.Platform (habilitada no `global.json`). Os testes da API sobem uma aplicação em memória com `TestServer` e conferem a resposta HTTP de verdade.

## Licença

[MIT](LICENSE)
