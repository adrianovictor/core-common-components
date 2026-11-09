# Changelog

O formato segue o [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e as versões seguem o [Versionamento Semântico](https://semver.org/lang/pt-BR/).

## [0.1.0] - 2026-11-09

Primeira versão, com o que o GeoTrack usa.

### Adicionado

- `Core.Common.Domain`, sem dependências:
  - `Entity<TId>` com igualdade por identidade;
  - `AuditableEntity<TId>` e `IAuditable`, com `CreatedAt` e `UpdatedAt` preenchidos pela persistência;
  - `ValueObject` com igualdade por componentes;
  - `PagedRequest` (página mínima 1, tamanho entre 1 e 100) e `PagedResult<T>`;
  - `DomainException`, `ValidationException`, `NotFoundException` e `ConflictException`.
- `Core.Common.Api`: `AddDomainProblemDetails()`, que responde às exceções de domínio com `ProblemDetails` (400, 404, 409 e 422).

[0.1.0]: https://github.com/adrianovictor/core-common-components/releases/tag/v0.1.0
