using FluentAssertions;
using InsuranceApp.Application.Common.Persistence;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Policies.Expiry;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Moq;

namespace InsuranceApp.UnitTests.Services;

public class PolicyExpiryServiceTests
{
    private readonly Mock<IPolicyRepository> _policyRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly IPolicyExpiryService _policyExpiryService;

    public PolicyExpiryServiceTests()
    {
        _policyExpiryService = new PolicyExpiryService(_policyRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task MarkExpiredPoliciesAsync_GivenNoUnmarkedExpiredPolicies_ShouldReturnZeroUpdatedPolicies()
    {
        var expiredPolicies = new List<Policy>();
        _policyRepository.Setup(r => r.GetExpiredPoliciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiredPolicies);

        var result = await _policyExpiryService.MarkExpiredPoliciesAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Value.Should().Be(0);
        _policyRepository.Verify(r => r.GetExpiredPoliciesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkExpiredPoliciesAsync_GivenExistingUnmarkedExpiredPolicies_ShouldReturnUpdatedPoliciesCount()
    {
        var expiredPolicies = new List<Policy>()
        {
            new() {Status = PolicyStatus.Active, StartDate = DateTime.UtcNow.AddYears(-1), EndDate = DateTime.UtcNow.AddDays(-5)},
            new() {Status = PolicyStatus.Draft, StartDate = DateTime.UtcNow.AddYears(-1), EndDate = DateTime.UtcNow.AddMonths(-2)},
        };
        _policyRepository.Setup(r => r.GetExpiredPoliciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiredPolicies);

        var result = await _policyExpiryService.MarkExpiredPoliciesAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Value.Should().Be(2);
        _policyRepository.Verify(r => r.GetExpiredPoliciesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}