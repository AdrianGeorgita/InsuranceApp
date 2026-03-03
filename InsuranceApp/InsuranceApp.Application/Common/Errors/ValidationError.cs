namespace InsuranceApp.Application.Common.Errors;
public sealed class ValidationError(string message = "Validation failed.") : ApiError(message)
{
    public override int HttpStatus => 400;
    public override string Title => "Bad Request";
    public override string? Code => "VALIDATION_FAILED";
    public Dictionary<string, string[]>? FieldErrors { get; init; }
    public override object? Details => FieldErrors;
}