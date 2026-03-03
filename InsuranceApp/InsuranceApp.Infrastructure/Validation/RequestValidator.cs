using FluentValidation;
using FluentValidation.Results;
using InsuranceApp.Application.Common.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace InsuranceApp.Infrastructure.Validation;

public class RequestValidator(IServiceProvider sp) : IRequestValidator
{
    public async Task<ValidationResult> ValidateAsync<TRequest>(TRequest request, CancellationToken ct)
    {
        var validator = sp.GetService<IValidator<TRequest>>();
        if (validator is null)
            return new ValidationResult();

        return await validator.ValidateAsync(request, ct);
    }
}
