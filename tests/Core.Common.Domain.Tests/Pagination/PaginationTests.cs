using Core.Common.Domain.Pagination;

namespace Core.Common.Domain.Tests.Pagination;

public class PaginationTests
{
    [Fact]
    public void DefaultRequestIsFirstPageWithDefaultSize()
    {
        var request = new PagedRequest();

        Assert.Equal(1, request.Page);
        Assert.Equal(PagedRequest.DefaultPageSize, request.PageSize);
        Assert.Equal(0, request.Skip);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(3, 3)]
    public void PageIsAtLeastOne(int page, int expected)
    {
        Assert.Equal(expected, PagedRequest.Of(page, 10).Page);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(50, 50)]
    [InlineData(10_000, PagedRequest.MaxPageSize)]
    public void PageSizeIsKeptWithinLimits(int pageSize, int expected)
    {
        // sem o teto, um cliente pede 10 mil registros numa chamada só
        Assert.Equal(expected, PagedRequest.Of(1, pageSize).PageSize);
    }

    [Fact]
    public void SkipFollowsPageAndSize()
    {
        Assert.Equal(40, PagedRequest.Of(3, 20).Skip);
    }

    [Theory]
    [InlineData(0, 0, false)]
    [InlineData(20, 1, false)]
    [InlineData(21, 2, true)]
    [InlineData(45, 3, true)]
    public void TotalPagesRoundsUp(int totalCount, int expectedPages, bool hasNext)
    {
        var result = PagedResult<int>.From([1], totalCount, PagedRequest.Of(1, 20));

        Assert.Equal(expectedPages, result.TotalPages);
        Assert.Equal(hasNext, result.HasNext);
        Assert.False(result.HasPrevious);
    }

    [Fact]
    public void LastPageHasPreviousAndNoNext()
    {
        var result = PagedResult<int>.From([41, 42], 42, PagedRequest.Of(3, 20));

        Assert.True(result.HasPrevious);
        Assert.False(result.HasNext);
    }

    [Fact]
    public void EmptyKeepsTheRequestedPage()
    {
        var result = PagedResult<string>.Empty(PagedRequest.Of(2, 10));

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public void MapConvertsItemsAndKeepsPagination()
    {
        var result = PagedResult<int>.From([1, 2], 12, PagedRequest.Of(2, 2)).Map(n => $"#{n}");

        Assert.Equal(["#1", "#2"], result.Items);
        Assert.Equal(12, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public void NegativeTotalIsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PagedResult<int>.From([], -1, new PagedRequest()));
    }
}
