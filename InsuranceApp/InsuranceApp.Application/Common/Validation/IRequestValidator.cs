using FluentValidation.Results;

namespace InsuranceApp.Application.Common.Validation;

public interface IRequestValidator
{
    Task<ValidationResult> ValidateAsync<TRequest>(TRequest request, CancellationToken ct);
}