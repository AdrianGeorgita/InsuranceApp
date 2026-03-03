using FluentValidation.Results;
using InsuranceApp.Application.Common.Errors;

namespace InsuranceApp.Application.Common.Validation;

public static class ValidationResultExtensions
{
    public static ValidationError ToApiError(this ValidationResult validationResult)
    {
        var fieldErrors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return new ValidationError
        {
            FieldErrors = fieldErrors
        };
    }
}