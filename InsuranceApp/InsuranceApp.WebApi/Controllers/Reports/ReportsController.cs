using Asp.Versioning;
using InsuranceApp.Application.Common.Constants;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Reports;
using InsuranceApp.Application.Reports.DTOs;
using InsuranceApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.WebApi.Controllers.Reports;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/v{version:apiVersion}/admin/[controller]")]
[ApiVersion("1.0")]
public class ReportsController(IReportService reportService) : BaseApiController
{
    [HttpGet("", Name = "ListAllReportsAsync")]
    [ProducesResponseType(typeof(PagedResult<ReportDto>), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [EndpointSummary("Get a paged list of reports")]
    [EndpointDescription("Retrieves a filtered paged list of reports according to the passed pageNumber and pageSize and filters applied.")]
    public async Task<ActionResult<PagedResult<ReportDto>>> ListAllClientsAsync([FromQuery] PageRequest pageRequest,
        [FromQuery] ReportGroup groupedBy, [FromQuery] ReportFilter filter, CancellationToken ct)
    {
        var result = await reportService.ListAllReportsAsync(pageRequest, groupedBy, filter, ct);
        return FromResult(result);
    }
}