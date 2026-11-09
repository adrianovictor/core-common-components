namespace Core.Common.Domain.Pagination;

/// <summary>
/// Página pedida numa consulta. Valores fora do intervalo são corrigidos em vez de gerar erro:
/// página mínima 1, tamanho entre 1 e <see cref="MaxPageSize"/>.
/// </summary>
public sealed record PagedRequest
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    private readonly int _page = 1;
    private readonly int _pageSize = DefaultPageSize;

    public int Page
    {
        get => _page;
        init => _page = Math.Max(1, value);
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }

    /// <summary>Quantos registros pular, para usar com <c>Skip</c>.</summary>
    public int Skip => (Page - 1) * PageSize;

    public static PagedRequest Of(int page, int pageSize) => new() { Page = page, PageSize = pageSize };
}
