using System.ComponentModel.DataAnnotations;

namespace InsuranceApp.Application.Common.Pagination;
public class PageRequest
{
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
}

