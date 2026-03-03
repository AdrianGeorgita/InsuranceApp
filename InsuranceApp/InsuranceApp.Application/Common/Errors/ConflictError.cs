namespace InsuranceApp.Application.Common.Errors;
public sealed class ConflictError(string message = "Conflict.") : ApiError(message)
{
    public override int HttpStatus => 409;
    public override string Title => "Conflict";
    public override string? Code => "CONFLICT";
}