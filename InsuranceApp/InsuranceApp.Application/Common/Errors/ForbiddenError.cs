namespace InsuranceApp.Application.Common.Errors;
public sealed class ForbiddenError(string message = "Forbidden.") : ApiError(message)
{
    public override int HttpStatus => 403;
    public override string Title => "Forbidden";
    public override string? Code => "FORBIDDEN";
}
