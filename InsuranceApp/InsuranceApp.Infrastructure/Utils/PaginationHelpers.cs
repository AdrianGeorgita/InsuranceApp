using InsuranceApp.Application.Common.Pagination;

namespace InsuranceApp.Infrastructure.Utils;

public static class PaginationHelpers
{
    public static PagedResult<TResult> GetPagedResult<TResult>(IEnumerable<TResult> items, PageRequest pageRequest,
        int totalSize)
        => new()
        {
            Items = items,
            PageNumber = pageRequest.PageNumber,
            PageSize = pageRequest.PageSize,
            TotalCount = totalSize
        };
}