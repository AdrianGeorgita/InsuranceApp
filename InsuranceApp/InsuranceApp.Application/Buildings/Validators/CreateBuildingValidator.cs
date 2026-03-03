using FluentValidation;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Common.Repository;

namespace InsuranceApp.Application.Buildings.Validators;

public class CreateBuildingValidator : AbstractValidator<CreateBuildingRequest>
{
    public CreateBuildingValidator(IRiskIndicatorRepository riskIndicatorRepository, ICityRepository cityRepository)
    {
        RuleFor(b => b.Address)
            .NotEmpty()
            .Length(1, 255)
            .WithMessage("Address Length must be between 1 and 255 characters.");
        RuleFor(b => b.CityId)
            .NotEmpty()
            .MustAsync(async (cityId, ct) =>
            {
               return await cityRepository.ExistsAsync(c => c.Id == cityId, ct);
            })
            .WithMessage($"City does not exist.");
        RuleFor(b => b.ConstructionYear)
            .NotEmpty()
            .WithMessage("ConstructionYear is required.");
        RuleFor(b => b.BuildingType)
            .NotNull()
            .IsInEnum();
        RuleFor(b => b.NumberOfFloors)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("NumberOfFloors must be greater than 0.");
        RuleFor(b => b.SurfaceArea)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("SurfaceArea must be greater than 0.");
        RuleFor(b => b.SurfaceArea)
            .PrecisionScale(6, 2, true);
        RuleFor(b => b.InsuredValue)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("InsuredValue must be greater than 0.");

        RuleFor(b => b.RiskIndicatorIds)
            .CustomAsync(async (value, context, ct) =>
            {
                var distinctIds = value.Distinct().ToList();
                var existingIds = await riskIndicatorRepository.ListAllKeysAsync(ct);
                var invalid = distinctIds.Where(id => !existingIds.Contains(id)).ToList();

                if (invalid.Count > 0)
                {
                    context.AddFailure(
                        "riskIndicatorIds",
                        $"The following riskIndicatorIds do not exist: {string.Join(", ", invalid)}");
                }
            });

    }
}