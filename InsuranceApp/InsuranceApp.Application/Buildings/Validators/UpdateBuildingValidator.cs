using FluentValidation;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Common.Repository;

namespace InsuranceApp.Application.Buildings.Validators;

public class UpdateBuildingValidator : AbstractValidator<UpdateBuildingRequest>
{
    public UpdateBuildingValidator(IRiskIndicatorRepository riskIndicatorRepository, ICityRepository cityRepository)
    {
        RuleFor(b => b.Address)
            .Length(1, 255)
            .WithMessage("Address Length must be between 1 and 255 characters.");
        RuleFor(b => b.CityId)
            .MustAsync(async (cityId, ct) =>
            {
               return cityId is null || await cityRepository.ExistsAsync(c => c.Id == cityId, ct);
            })
            .WithMessage($"City does not exist.");
        RuleFor(b => b.BuildingType)
            .IsInEnum();
        RuleFor(b => b.NumberOfFloors)
            .GreaterThan(0)
            .WithMessage("NumberOfFloors must be greater than 0.");
        RuleFor(b => b.SurfaceArea)
            .GreaterThan(0)
            .WithMessage("SurfaceArea must be greater than 0.");
        RuleFor(b => b.SurfaceArea)
            .PrecisionScale(6, 2, true);
        RuleFor(b => b.InsuredValue)
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