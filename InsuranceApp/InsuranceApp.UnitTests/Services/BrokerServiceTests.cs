using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using InsuranceApp.Application.Brokers;
using InsuranceApp.Application.Brokers.DTOs;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Persistence;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace InsuranceApp.UnitTests.Services;

public class BrokerServiceTests
{
    private readonly Mock<IBrokerRepository> _brokerRepository = new();
    private readonly Mock<IRequestValidator> _requestValidator = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ILogger<BrokerService>> _logger = new();
    private readonly IBrokerService _brokerService;

    public BrokerServiceTests()
    {
        _brokerService = new BrokerService(_brokerRepository.Object,
            _requestValidator.Object, _mapper.Object, _logger.Object);
    }

    [Fact]
    public async Task ListAllBrokersAsync_ShouldReturnPagedListOfBrokers()
    {
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };
        var pagedBrokers = new PagedResult<BrokerDto>
        {
            Items = new List<BrokerDto>
            {
                new() { Code = "BRK-RO-001", Name = "John Doe", CommissionPercentage = 0.0025M, Status = BrokerStatus.Active},
                new() { Code = "BRK-RO-002", Name = "John Does", CommissionPercentage = 0.0025M, Status = BrokerStatus.Active},
                new() { Code = "BRK-RO-003", Name = "John Did", CommissionPercentage = 0.0025M, Status = BrokerStatus.Active},
            },
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };
        _brokerRepository.Setup(r => r.GetAllAsync<BrokerDto>(pageRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedBrokers);

        var result = await _brokerService.ListAllBrokersAsync(pageRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A paged list of brokers should be returned in any case");
        result.Errors.Should().BeEmpty("There should be no errors when fetching a paged list of brokers");
        result.Value.Should().NotBeNull("A PagedResult should be returned");
        result.Value.Should().BeEquivalentTo(pagedBrokers);
        _brokerRepository.Verify(r => r.GetAllAsync<BrokerDto>(pageRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBrokerByIdAsync_GivenExistingId_ShouldReturnBroker()
    {
        var brokerId = Guid.NewGuid();
        var broker = new Broker
        {
            Code = "BRK-RO-001",
            Name = "John Doe",
            CommissionPercentage = 0.0025M,
            Status = BrokerStatus.Active
        };
        var brokerDto = new BrokerDto
        {
            Id = brokerId,
            Code = broker.Code,
            CommissionPercentage = broker.CommissionPercentage,
            Status = broker.Status
        };
        _brokerRepository.Setup(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(broker);
        _mapper.Setup(m => m.Map<BrokerDto>(broker)).Returns(brokerDto);

        var result = await _brokerService.GetBrokerByIdAsync(brokerId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid broker Id should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing an existing broker id");
        result.Value.Should().NotBeNull("A BrokerDto should be returned");
        result.Value.Should().BeEquivalentTo(brokerDto);
        _mapper.Verify(m => m.Map<BrokerDto>(broker), Times.Once);
        _brokerRepository.Verify(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task GetBrokerByIdAsync_GivenNonExistingId_ShouldReturnNotFound()
    {
        var brokerId = Guid.NewGuid();
        Broker? broker = null;
        _brokerRepository.Setup(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(broker);

        var result = await _brokerService.GetBrokerByIdAsync(brokerId, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid broker Id should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Broker '{brokerId}' not found.");
        _mapper.Verify(m => m.Map<BrokerDto>(broker), Times.Never);
        _brokerRepository.Verify(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task CreateBrokerAsync_GivenValidRequest_ShouldReturnBrokerId()
    {
        var createRequest = GetValidCreateBrokerRequest();
        var broker = new Broker()
        {
            Id = Guid.NewGuid(),
            Code = createRequest.Code,
            Name = createRequest.Name,
            Email = createRequest.Email,
            Phone = createRequest.Phone,
            Status = createRequest.Status
        };
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _brokerRepository.Setup(r =>
                r.ExistsByCodeAsync(createRequest.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mapper.Setup(m => m.Map<Broker>(createRequest)).Returns(broker);

        var result = await _brokerService.CreateBrokerAsync(createRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(broker.Id, "The broker id should be returned");
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _brokerRepository.Verify(r => r.ExistsByCodeAsync(createRequest.Code, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<Broker>(createRequest), Times.Once);
        _brokerRepository.Verify(r => r.AddAsync(broker, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBrokerAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        var createRequest = GetValidCreateBrokerRequest();
        createRequest.Phone = "07122321321345678";
        var failures = new[]
        {
            new ValidationFailure("Phone", "Phone number is required and must be between 4 and 15 characters.")
            {
                ErrorCode = "PhoneValidator"
            }
        };
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _brokerService.CreateBrokerAsync(createRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _brokerRepository.Verify(r => r.ExistsByCodeAsync(createRequest.Code, It.IsAny<CancellationToken>()), Times.Never);
        _mapper.Verify(m => m.Map<Broker>(createRequest), Times.Never);
        _brokerRepository.Verify(r => r.AddAsync(It.IsAny<Broker>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateBrokerAsync_GivenExistingBrokerCode_ShouldReturnConflict()
    {
        var createRequest = GetValidCreateBrokerRequest();
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _brokerRepository.Setup(r =>
                r.ExistsByCodeAsync(createRequest.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _brokerService.CreateBrokerAsync(createRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("A request with an already existing broker Code should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ConflictError);
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _brokerRepository.Verify(r => r.ExistsByCodeAsync(createRequest.Code, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<Broker>(createRequest), Times.Never);
        _brokerRepository.Verify(r => r.AddAsync(It.IsAny<Broker>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBrokerAsync_GivenExistingId_ShouldReturnBrokerId()
    {
        var brokerId = Guid.NewGuid();
        var updateRequest = new UpdateBrokerRequest()
        {
            CommissionPercentage = 0.0125M,
        };
        var broker = GetValidBroker();
        broker.Id = brokerId;
        _brokerRepository.Setup(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>())).ReturnsAsync(broker);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _brokerRepository.Setup(r =>
                r.ExistsByCodeAsync(updateRequest.Code ?? broker.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _brokerService.UpdateBrokerAsync(brokerId, updateRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(broker.Id, "The broker id should be returned");
        _brokerRepository.Verify(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _brokerRepository.Verify(r => r.ExistsByCodeAsync(updateRequest.Code ?? broker.Code, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBrokerAsync_GivenNonExistingId_ShouldReturnNotFound()
    {
        var brokerId = Guid.NewGuid();
        var updateRequest = new UpdateBrokerRequest()
        {
            CommissionPercentage = 0.0125M,
        };
        Broker? broker = null;
        _brokerRepository.Setup(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>())).ReturnsAsync(broker);

        var result = await _brokerService.UpdateBrokerAsync(brokerId, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid broker Id should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Broker '{brokerId}' not found.");
        _brokerRepository.Verify(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Never);
        _brokerRepository.Verify(r => r.ExistsByCodeAsync(updateRequest.Code!, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBrokerAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        var brokerId = Guid.NewGuid();
        var updateRequest = new UpdateBrokerRequest()
        {
            CommissionPercentage = 0.0125M,
            Phone = "012512516613311"
        };
        var broker = GetValidBroker();
        broker.Id = brokerId;
        var failures = new[]
        {
            new ValidationFailure("Phone", "Phone number is required and must be between 4 and 15 characters.")
            {
                ErrorCode = "PhoneValidator"
            }
        };
        _brokerRepository.Setup(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>())).ReturnsAsync(broker);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _brokerService.UpdateBrokerAsync(brokerId, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);
        _brokerRepository.Verify(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _brokerRepository.Verify(r => r.ExistsByCodeAsync(null!, It.IsAny<CancellationToken>()), Times.Never);
    }


    [Fact]
    public async Task UpdateBrokerAsync_GivenExistingNewBrokerCode_ShouldReturnConflict()
    {
        var brokerId = Guid.NewGuid();
        var updateRequest = new UpdateBrokerRequest()
        {
            CommissionPercentage = 0.0125M,
            Code = "BRK-RO-000"
        };
        var broker = GetValidBroker();
        broker.Id = brokerId;
        _brokerRepository.Setup(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>())).ReturnsAsync(broker);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _brokerRepository.Setup(r =>
                r.ExistsByCodeAsync(updateRequest.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _brokerService.UpdateBrokerAsync(brokerId, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("A request with an already existing broker Code should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ConflictError);
        _brokerRepository.Verify(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _brokerRepository.Verify(r => r.ExistsByCodeAsync(updateRequest.Code, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBrokerStatusAsync_GivenExistingId_ShouldReturnBrokerId()
    {
        var brokerId = Guid.NewGuid();
        const BrokerStatus newStatus = BrokerStatus.Inactive;
        var broker = GetValidBroker();
        broker.Id = brokerId;
        _brokerRepository.Setup(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>())).ReturnsAsync(broker);

        var result = await _brokerService.UpdateBrokerStatusAsync(brokerId, newStatus, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid status should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid new status");
        result.Value.Should().Be(broker.Id, "The broker id should be returned");
        _brokerRepository.Verify(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBrokerStatusAsync_GivenNonExistingId_ShouldNotFound()
    {
        var brokerId = Guid.NewGuid();
        const BrokerStatus newStatus = BrokerStatus.Inactive;
        Broker? broker = null;
        _brokerRepository.Setup(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>())).ReturnsAsync(broker);

        var result = await _brokerService.UpdateBrokerStatusAsync(brokerId, newStatus, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid broker Id should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Broker '{brokerId}' not found.");
        _brokerRepository.Verify(r => r.GetAsync(brokerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    private CreateBrokerRequest GetValidCreateBrokerRequest() => new CreateBrokerRequest()
    {
        Code = "BRK-RO-001",
        Name = "John Doe",
        Email = "john.doe@mail.com",
        Phone = "0712345678",
        Status = BrokerStatus.Active
    };

    private Broker GetValidBroker() => new Broker()
    {
        Id = Guid.NewGuid(),
        Code = "BRK-RO-001",
        CommissionPercentage = 0M,
        Status = BrokerStatus.Active
    };
}
