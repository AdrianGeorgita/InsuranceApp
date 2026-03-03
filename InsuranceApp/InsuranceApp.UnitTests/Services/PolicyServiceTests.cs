using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using InsuranceApp.Application.Common.Audit;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Messaging;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Application.Policies;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Application.Policies.Pricing;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Pricing;
using Microsoft.Extensions.Logging;
using Moq;
using System.Globalization;
using System.Linq.Expressions;

namespace InsuranceApp.UnitTests.Services;

public class PolicyServiceTests
{
    private readonly Mock<IPolicyRepository> _policyRepository = new();
    private readonly Mock<IPriceFetchService> _priceFetchService = new();
    private readonly Mock<IRequestValidator> _requestValidator = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IAuditEventPublisher> _auditEventPublisher = new();
    private readonly Mock<ILogger<PolicyService>> _logger = new();
    private readonly Mock<IPolicyEventPublisher> _eventPublisher = new();
    private readonly Mock<IPricingCalculator> _pricingCalculator = new();
    private readonly IPolicyService _policyService;

    public PolicyServiceTests()
    {
        _policyService = new PolicyService(_policyRepository.Object, _priceFetchService.Object,
            _requestValidator.Object, _mapper.Object,
            _auditEventPublisher.Object, _logger.Object, _eventPublisher.Object,
            _pricingCalculator.Object);
    }

    [Fact]
    public async Task ListAllPoliciesAsync_ShouldReturnPagedListOfPolicies()
    {
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };
        var filter = new PolicyFilter(
            Guid.NewGuid(),
            Guid.NewGuid(),
            PolicyStatus.Active,
            DateTime.UtcNow,
            DateTime.UtcNow);
        var pagedPolicies = new PagedResult<PolicyDto>
        {
            Items = new List<PolicyDto>
            {
                new() { BasePremium = 50000, FinalPremium = 72500, PolicyNumber = "POLICY-231123-123123-23123-123123"},
                new() { BasePremium = 40000, FinalPremium = 52500, PolicyNumber = "POLICY-231123-123123-23123-123124"},
                new() { BasePremium = 30000, FinalPremium = 32500, PolicyNumber = "POLICY-231123-123123-23123-123125"},
            },
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };
        _policyRepository.Setup(r => r.GetAllPoliciesAsync(pageRequest, filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedPolicies);

        var result = await _policyService.ListAllPoliciesAsync(pageRequest, filter, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A paged list of policies should be returned in any case");
        result.Errors.Should().BeEmpty("There should be no errors when fetching a paged list of policies");
        result.Value.Should().NotBeNull("A PagedResult should be returned");
        result.Value.Should().BeEquivalentTo(pagedPolicies);
        _policyRepository.Verify(r => r.GetAllPoliciesAsync(pageRequest, filter, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPolicyByIdAsync_GivenExistingPolicyNumber_ShouldReturnPolicy()
    {
        var policyNumber = "POLICY-125125-4161-3414-341613461";
        var policy = new Policy()
        {
            PolicyNumber = policyNumber,
            BasePremium = 12500,
            FinalPremium = 20000
        };
        var policyDto = new DetailedPolicyDto()
        {
            PolicyNumber = policyNumber,
            BasePremium = policy.BasePremium,
            FinalPremium = policy.FinalPremium
        };
        _policyRepository.Setup(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(policy);
        _mapper.Setup(m => m.Map<DetailedPolicyDto>(policy)).Returns(policyDto);

        var result = await _policyService.GetPolicyByIdAsync(policyNumber, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid policy Id should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing an existing policy id");
        result.Value.Should().NotBeNull("A PolicyDto should be returned");
        result.Value.Should().BeEquivalentTo(policyDto);
        _mapper.Verify(m => m.Map<DetailedPolicyDto>(policy), Times.Once);
        _policyRepository.Verify(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task GetPolicyByIdAsync_GivenNonExistingPolicyNumber_ShouldReturnNotFound()
    {
        var policyNumber = "POLICY-125125-4161-3414-341613461";
        Policy? policy = null;
        _policyRepository.Setup(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(policy);

        var result = await _policyService.GetPolicyByIdAsync(policyNumber, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid policy Id should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Policy '{policyNumber}' not found.");
        _mapper.Verify(m => m.Map<DetailedPolicyDto>(policy), Times.Never);
        _policyRepository.Verify(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task CreatePolicyAsync_GivenValidRequest_ShouldReturnPolicyNumber()
    {
        var createRequest = GetValidCreatePolicyRequest();
        var policy = new Policy()
        {
            PolicyNumber = "POLICY-" + Guid.NewGuid().ToString("N"),
            BasePremium = createRequest.BasePremium,
            BrokerId = createRequest.BrokerId,
            BuildingId = createRequest.BuildingId,
            ClientId = createRequest.ClientId,
            CurrencyCode = createRequest.CurrencyCode,
            StartDate = createRequest.StartDate,
            EndDate = createRequest.EndDate,
            FinalPremium = 15000,
            Status = PolicyStatus.Active
        };
        var pricingContext = GetValidPricingContextDto(policy);
        var adjustments = new List<decimal>() { 0.1000M, 0.4000M };
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _policyRepository.Setup(r =>
                r.ExistsAsync(It.IsAny<Expression<Func<Policy, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mapper.Setup(m => m.Map<Policy>(createRequest)).Returns(policy);
        _priceFetchService.Setup(s => s.FetchPricingContextAsync(policy.BuildingId, policy.StartDate,
            policy.BrokerId, It.IsAny<CancellationToken>())).ReturnsAsync(pricingContext);
        _pricingCalculator
            .Setup(s => s.GetAllApplicableAdjustments(pricingContext.Building, pricingContext.Broker,
                pricingContext.RiskFactorConfigurations, pricingContext.FeeConfigurations)).Returns(adjustments);

        var result = await _policyService.CreatePolicyAsync(createRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(policy.PolicyNumber, "The policy number should be returned");
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _policyRepository.Verify(r => r.ExistsAsync(It.IsAny<Expression<Func<Policy, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<Policy>(createRequest), Times.Once);
        _priceFetchService.Verify(s => s.FetchPricingContextAsync(policy.BuildingId, policy.StartDate, policy.BrokerId, It.IsAny<CancellationToken>()), Times.Once);
        _pricingCalculator.Verify(s => s.GetAllApplicableAdjustments(pricingContext.Building, pricingContext.Broker,
            pricingContext.RiskFactorConfigurations, pricingContext.FeeConfigurations), Times.Once);
        _policyRepository.Verify(r => r.AddAsync(policy, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreatePolicyAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        var createRequest = GetValidCreatePolicyRequest();
        createRequest.BasePremium = -5;
        var failures = new[]
        {
            new ValidationFailure("BasePremium", "BasePremium is required and must be greater than 0.")
            {
                ErrorCode = "BasePremiumValidator"
            }
        };
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _policyService.CreatePolicyAsync(createRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _policyRepository.Verify(r => r.ExistsAsync(It.IsAny<Expression<Func<Policy, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _mapper.Verify(m => m.Map<Policy>(createRequest), Times.Never);
        _priceFetchService.Verify(s => s.FetchPricingContextAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(),
            It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _priceFetchService.Verify(s => s.FetchPricingContextAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(),
            It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _pricingCalculator.Verify(s => s.GetAllApplicableAdjustments(It.IsAny<Building>(), It.IsAny<Broker>(),
            It.IsAny<List<RiskFactorConfiguration>>(), It.IsAny<List<FeeConfiguration>>()), Times.Never);
        _policyRepository.Verify(r => r.AddAsync(It.IsAny<Policy>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreatePolicyAsync_GivenExistingPolicyCombination_ShouldReturnConflict()
    {
        var createRequest = GetValidCreatePolicyRequest();
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _policyRepository.Setup(r =>
                r.ExistsAsync(It.IsAny<Expression<Func<Policy, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _policyService.CreatePolicyAsync(createRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("A request with an already existing policy Code should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ConflictError);
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _policyRepository.Verify(r => r.ExistsAsync(It.IsAny<Expression<Func<Policy, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<Policy>(createRequest), Times.Never);
        _priceFetchService.Verify(s => s.FetchPricingContextAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(),
            It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _pricingCalculator.Verify(s => s.GetAllApplicableAdjustments(It.IsAny<Building>(), It.IsAny<Broker>(),
            It.IsAny<List<RiskFactorConfiguration>>(), It.IsAny<List<FeeConfiguration>>()), Times.Never);
        _policyRepository.Verify(r => r.AddAsync(It.IsAny<Policy>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePolicyStatusAsync_GivenValidTransitionFromDraftToActive_ShouldReturnPolicyNumber()
    {
        var policyNumber = "POLICY-125125-4161-3414-341613461";
        var updateRequest = new UpdatePolicyRequest()
        {
            NewStatus = PolicyStatus.Active
        };
        var policy = GetValidPolicy();
        _policyRepository.Setup(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>())).ReturnsAsync(policy);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var result = await _policyService.UpdatePolicyStatusAsync(policyNumber, updateRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid transition should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid transition");
        result.Value.Should().Be(policy.PolicyNumber, "The policy number should be returned");
        _policyRepository.Verify(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePolicyStatusAsync_GivenValidTransitionFromActiveToCancelled_ShouldReturnPolicyNumber()
    {
        var policyNumber = "POLICY-125125-4161-3414-341613461";
        var updateRequest = new UpdatePolicyRequest()
        {
            NewStatus = PolicyStatus.Cancelled,
            Reason = "Building demolished"
        };
        var policy = GetValidPolicy();
        policy.Status = PolicyStatus.Active;
        _policyRepository.Setup(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>())).ReturnsAsync(policy);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var result = await _policyService.UpdatePolicyStatusAsync(policyNumber, updateRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid transition should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid transition");
        result.Value.Should().Be(policy.PolicyNumber, "The policy number should be returned");
        _policyRepository.Verify(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePolicyStatusAsync_GivenNonExistingPolicyNumber_ShouldReturnNotFound()
    {
        var policyNumber = "POLICY-125125-4161-3414-341613461";
        var updateRequest = new UpdatePolicyRequest()
        {
            NewStatus = PolicyStatus.Cancelled,
            Reason = "Building demolished"
        };
        Policy? policy = null;
        _policyRepository.Setup(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>())).ReturnsAsync(policy);

        var result = await _policyService.UpdatePolicyStatusAsync(policyNumber, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid policy number should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Policy '{policyNumber}' not found.");
        _policyRepository.Verify(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePolicyStatusAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        var policyNumber = "POLICY-125125-4161-3414-341613461";
        var updateRequest = new UpdatePolicyRequest()
        {
            NewStatus = PolicyStatus.Cancelled
        };
        var policy = GetValidPolicy();
        var failures = new[]
        {
            new ValidationFailure("Reason", "Reason is required when cancelling a Policy and must be between 1 and 512 characters.")
            {
                ErrorCode = "ReasonValidator"
            }
        };
        _policyRepository.Setup(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>())).ReturnsAsync(policy);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _policyService.UpdatePolicyStatusAsync(policyNumber, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);
        _policyRepository.Verify(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePolicyStatusAsync_GivenTransitionToActiveAndStartDateInThePast_ShouldReturnValidationError()
    {
        var policyNumber = "POLICY-125125-4161-3414-341613461";
        var updateRequest = new UpdatePolicyRequest()
        {
            NewStatus = PolicyStatus.Active,
        };
        var policy = GetValidPolicy();
        policy.StartDate = DateTime.UtcNow.AddYears(-2);
        _policyRepository.Setup(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>())).ReturnsAsync(policy);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var result = await _policyService.UpdatePolicyStatusAsync(policyNumber, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);
        _policyRepository.Verify(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePolicyStatusAsync_GivenInvalidTransition_ShouldReturnValidationError()
    {
        var policyNumber = "POLICY-125125-4161-3414-341613461";
        var updateRequest = new UpdatePolicyRequest()
        {
            NewStatus = PolicyStatus.Active,
        };
        var policy = GetValidPolicy();
        policy.Status = PolicyStatus.Cancelled;
        _policyRepository.Setup(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>())).ReturnsAsync(policy);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var result = await _policyService.UpdatePolicyStatusAsync(policyNumber, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);
        _policyRepository.Verify(r => r.GetDetailedPolicyByIdAsync(policyNumber, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    private CreatePolicyRequest GetValidCreatePolicyRequest() => new CreatePolicyRequest()
    {
        BasePremium = 10000,
        BrokerId = Guid.NewGuid(),
        BuildingId = Guid.NewGuid(),
        ClientId = Guid.NewGuid(),
        CurrencyCode = "RON",
        StartDate = DateTime.Parse("2026-04-04", CultureInfo.InvariantCulture),
        EndDate = DateTime.Parse("2027-04-04", CultureInfo.InvariantCulture)
    };

    private Policy GetValidPolicy() => new Policy()
    {
        PolicyNumber = "POLICY-" + Guid.NewGuid().ToString("N"),
        BasePremium = 10000,
        BrokerId = Guid.NewGuid(),
        BuildingId = Guid.NewGuid(),
        ClientId = Guid.NewGuid(),
        CurrencyCode = "RON",
        StartDate = DateTime.UtcNow.AddDays(1),
        EndDate = DateTime.UtcNow.AddYears(1),
        FinalPremium = 15000,
        Status = PolicyStatus.Draft
    };

    private PricingContextDto GetValidPricingContextDto(Policy policy) => new PricingContextDto
    {
        Broker = new Broker(),
        Building = new Building(),
        FeeConfigurations = new List<FeeConfiguration>(),
        RiskFactorConfigurations = new List<RiskFactorConfiguration>()
    };
}
