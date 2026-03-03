using FluentValidation;
using FluentValidation.TestHelper;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Application.Policies.Validators;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.UnitTests.Validators;

public class CancelPolicyValidatorTests
{

    private readonly IValidator<UpdatePolicyRequest> _validator = new UpdatePolicyValidator();

    [Fact]
    public async Task ValidateAsync_ValidRequest_ShouldNotHaveError()
    {
        var req = ValidRequest();

        var result = await _validator.TestValidateAsync(req);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("ThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReason" +
                "ThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReason" +
                "ThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReason" +
                "ThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReason" +
                "ThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReason" +
                "ThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReason" +
                "ThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReason" +
                "ThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReasonThisIsAVeryLongReason")]
    public async Task ValidateAsync_InvalidReasonWhenCancelled_ShouldHaveError(string reason)
    {
        var req = ValidRequest();
        req.Reason = reason;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Reason)
            .WithErrorMessage("Reason is required when cancelling a Policy and must be between 1 and 512 characters.");
    }
    private UpdatePolicyRequest ValidRequest() => new UpdatePolicyRequest
    {
        NewStatus = PolicyStatus.Cancelled,
        Reason = "Building demolished"
    };
}
