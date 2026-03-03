using FluentValidation;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Policies.Validators;

public class CreatePolicyValidator : AbstractValidator<CreatePolicyRequest>
{
    public CreatePolicyValidator(IClientRepository clientRepository, IBuildingRepository buildingRepository
        , ICurrencyRepository currencyRepository, IBrokerRepository brokerRepository)
    {
        RuleFor(b => b.ClientId)
            .NotEmpty()
            .WithMessage("ClientId is required.");
        RuleFor(b => b.ClientId)
            .MustAsync(async (clientId, ct) =>
            {
                return await clientRepository.ExistsAsync(c => c.Id == clientId, ct);
            })
            .WithMessage($"Client does not exist.");
        RuleFor(b => b.BuildingId)
            .NotEmpty()
            .WithMessage("BuildingId is required.");
        RuleFor(b => b.BuildingId)
            .MustAsync(async (buildingId, ct) =>
            {
                return await buildingRepository.ExistsAsync(c => c.Id == buildingId, ct);
            })
            .WithMessage($"Building does not exist.");
        RuleFor(b => b.StartDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("StartDate is required and cannot be in the past.");
        RuleFor(b => b.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("EndDate is required and cannot be in the past.");
        RuleFor(b => b.EndDate)
            .GreaterThan(b => b.StartDate)
            .WithMessage("EndDate must be after the StartDate.");
        RuleFor(b => b.BasePremium)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("BasePremium is required and must be greater than 0.");
        RuleFor(b => b.BasePremium)
            .PrecisionScale(20, 4, true);
        RuleFor(b => b.CurrencyCode)
            .NotEmpty()
            .Length(3)
            .WithMessage("CurrencyCode is required and must have a length of 3 characters");
        RuleFor(b => b.CurrencyCode)
            .Matches("^(?i)[a-z]{3}");
        RuleFor(b => b.CurrencyCode)
            .MustAsync(async (currencyCode, ct) =>
            {
                return await currencyRepository.ExistsAsync(c => c.Code == currencyCode
                    && c.IsActive && !c.Deprecated, ct);
            })
            .WithMessage($"Currency does not exist or is deprecated.");
        RuleFor(b => b.BrokerId)
            .NotEmpty()
            .WithMessage("BrokerId is required.");
        RuleFor(b => b.BrokerId)
            .MustAsync(async (brokerId, ct) =>
            {
                return await brokerRepository.ExistsAsync(broker => broker.Id == brokerId &&
                                                                    broker.Status == BrokerStatus.Active);
            })
            .WithMessage($"Broker does not exist or is inactive.");
    }
}