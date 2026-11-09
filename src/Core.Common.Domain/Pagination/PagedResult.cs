namespace Core.Common.Domain.Pagination;

/// <summary>Uma página de resultados, com o total de registros da consulta.</summary>
public sealed record PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    private PagedResult(IReadOnlyList<T> items, int totalCount, PagedRequest request)
    {
        Items = items;
        TotalCount = totalCount;
        Page = request.Page;
        PageSize = request.PageSize;
    }

    public static PagedResult<T> From(IReadOnlyList<T> items, int totalCount, PagedRequest request)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentOutOfRangeException.ThrowIfNegative(totalCount);
        return new(items, totalCount, request);
    }

    public static PagedResult<T> Empty(PagedRequest request) => From([], 0, request);

    /// <summary>Converte os itens (por exemplo, entidade para DTO) mantendo os dados da paginação.</summary>
    public PagedResult<TOut> Map<TOut>(Func<T, TOut> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);
        return PagedResult<TOut>.From(Items.Select(mapper).ToList(), TotalCount, PagedRequest.Of(Page, PageSize));
    }
}
