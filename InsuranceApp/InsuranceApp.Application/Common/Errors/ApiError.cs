using FluentResults;

namespace InsuranceApp.Application.Common.Errors;
public abstract class ApiError(string message) : Error(message)
{
    public abstract int HttpStatus { get; }
    public abstract string Title { get; }
    public virtual string? Code => null;
    public virtual object? Details => null;
}
