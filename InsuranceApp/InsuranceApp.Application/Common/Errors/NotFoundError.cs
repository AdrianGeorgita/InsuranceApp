namespace InsuranceApp.Application.Common.Errors;
public sealed class NotFoundError(string message = "Resource not found.") : ApiError(message)
{
    public override int HttpStatus => 404;
    public override string Title => "Not Found";
    public override string? Code => "NOT_FOUND";
}