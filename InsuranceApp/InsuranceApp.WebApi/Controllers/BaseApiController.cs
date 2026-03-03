using FluentResults;
using InsuranceApp.Application.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected ActionResult FromResult(Result result)
        => result.IsSuccess ? NoContent() : FromErrors(result);

    protected ActionResult<T> FromResult<T>(Result<T> result)
        => result.IsSuccess ? Ok(result.Value) : FromErrors(result);

    protected ActionResult<T> FromCreated<T>(Result<T> result, string actionName, Func<T, object> routeValues)
        => result.IsSuccess
            ? CreatedAtRoute(actionName, routeValues(result.Value), result.Value)
            : FromErrors(result);

    private ActionResult FromErrors(IResultBase result)
    {
        var apiError = result.Errors.OfType<ApiError>().FirstOrDefault();

        if (apiError is null)
        {
            return StatusCode(500, new ProblemDetails
            {
                Status = 500,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred.",
                Instance = HttpContext.Request.Path
            });
        }

        if (apiError is ValidationError { FieldErrors: not null } ve)
        {
            var validationProblemDetails = new ValidationProblemDetails(ve.FieldErrors)
            {
                Status = ve.HttpStatus,
                Title = ve.Title,
                Detail = ve.Message,
                Instance = HttpContext.Request.Path
            };

            if (ve.Code is not null) validationProblemDetails.Extensions["code"] = ve.Code;

            return StatusCode(ve.HttpStatus, validationProblemDetails);
        }

        var problemDetails = new ProblemDetails
        {
            Status = apiError.HttpStatus,
            Title = apiError.Title,
            Detail = apiError.Message,
            Instance = HttpContext.Request.Path
        };

        if (apiError.Code is not null) problemDetails.Extensions["code"] = apiError.Code;
        if (apiError.Details is not null) problemDetails.Extensions["details"] = apiError.Details;

        return StatusCode(apiError.HttpStatus, problemDetails);
    }
}