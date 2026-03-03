using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using InsuranceApp.Application.Clients;
using InsuranceApp.Application.Clients.DTOs;
using InsuranceApp.Application.Common.Audit;
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

public class ClientServiceTests
{
    private readonly Mock<IClientRepository> _repository = new();
    private readonly Mock<IRequestValidator> _requestValidator = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IAuditEventPublisher> _auditEventPublisher = new();
    private readonly Mock<ILogger<ClientService>> _logger = new();
    private readonly IClientService _clientService;

    public ClientServiceTests()
    {
        _clientService = new ClientService(_repository.Object, _requestValidator.Object,
            _mapper.Object, _auditEventPublisher.Object, _logger.Object);
    }

    [Fact]
    public async Task ListAllClientsAsync_ShouldReturnPagedListOfClients()
    {
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };
        ClientFilter? filter = null;
        var pagedClients = new PagedResult<ClientDto>
        {
            Items = new List<ClientDto>
            {
                new() { Id = Guid.NewGuid(), Name = "John Doe", Type = "Individual" },
                new() { Id = Guid.NewGuid(), Name = "John Doe Company", Type = "Company" },
                new() { Id = Guid.NewGuid(), Name = "Jonathan Doe", Type = "Individual" },
            },
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };
        _repository.Setup(r => r.GetAllClients(pageRequest, filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedClients);

        var result = await _clientService.ListAllClientsAsync(pageRequest, filter, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A paged list of clients should be returned in any case");
        result.Errors.Should().BeEmpty("There should be no errors when fetching a paged list of clients");
        result.Value.Should().NotBeNull("A PagedResult should be returned");
        result.Value.Should().BeEquivalentTo(pagedClients);

        _repository.Verify(r => r.GetAllClients(pageRequest, filter, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetClientByIdAsync_GivenExistingClientId_ShouldReturnClient()
    {
        var clientId = Guid.NewGuid();
        var client = GetValidClient();
        client.Id = clientId;
        var clientDto = GetClientDtoFromClient(client);
        _repository.Setup(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        _mapper.Setup(m => m.Map<ClientDto>(client)).Returns(clientDto);

        var result = await _clientService.GetClientByIdAsync(clientId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid ClientId should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing an existing client id");
        result.Value.Should().NotBeNull("A ClientDto should be returned");
        result.Value.Should().BeEquivalentTo(clientDto);

        _mapper.Verify(m => m.Map<ClientDto>(client), Times.Once);
        _repository.Verify(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task GetClientByIdAsync_GivenNonExistingClientId_ShouldReturnNotFound()
    {
        var clientId = Guid.NewGuid();
        Client? client = null;
        _repository.Setup(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var result = await _clientService.GetClientByIdAsync(clientId, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid ClientId should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Client '{clientId}' not found.");

        _mapper.Verify(m => m.Map<ClientDto>(client), Times.Never);
        _repository.Verify(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateClientAsync_GivenValidRequest_ShouldReturnClientId()
    {
        var createRequest = GetValidCreateClientRequest();
        var client = new Client
        {
            Id = Guid.NewGuid(),
            Type = createRequest.Type.ToString(),
            Name = createRequest.Name,
            Address = createRequest.Address,
            Email = createRequest.Email,
            Phone = createRequest.Phone,
            IdentificationNumber = createRequest.IdentificationNumber
        };
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _repository.Setup(r =>
                r.ExistsByIdentifierAsync(createRequest.IdentificationNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mapper.Setup(m => m.Map<Client>(createRequest)).Returns(client);

        var result = await _clientService.CreateClientAsync(createRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(client.Id, "The client's Guid should be returned");

        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.ExistsByIdentifierAsync(createRequest.IdentificationNumber, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<Client>(createRequest), Times.Once);
        _repository.Verify(r => r.AddAsync(client, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateClientAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        var createRequest = GetValidCreateClientRequest();
        createRequest.Email = "john.doe.mail.com";
        var failures = new[]
        {
            new ValidationFailure("Email", "Email must be a valid email address.")
            {
                ErrorCode = "EmailValidator"
            }
        };
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _clientService.CreateClientAsync(createRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);

        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.ExistsByIdentifierAsync(createRequest.IdentificationNumber, It.IsAny<CancellationToken>()), Times.Never);
        _mapper.Verify(m => m.Map<Client>(createRequest), Times.Never);
        _repository.Verify(r => r.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateClientAsync_GivenExistingClientIdentifier_ShouldReturnConflict()
    {
        var createRequest = GetValidCreateClientRequest();
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _repository.Setup(r =>
                r.ExistsByIdentifierAsync(createRequest.IdentificationNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _clientService.CreateClientAsync(createRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("A request with an already existing client IdentificationNumber should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ConflictError);

        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.ExistsByIdentifierAsync(createRequest.IdentificationNumber, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<Client>(createRequest), Times.Never);
        _repository.Verify(r => r.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateClientAsync_GivenValidIdUpdate_ShouldReturnClientId()
    {
        var clientId = Guid.NewGuid();
        var updateRequest = GetValidUpdateClientRequest();
        var client = GetValidClient();
        client.Id = clientId;
        client.IdentificationNumber = "3020202123122";
        _repository.Setup(r => r.GetAsync(clientId, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _repository.Setup(r =>
                r.ExistsByIdentifierAsync(updateRequest.IdentificationNumber!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _clientService.UpdateClientAsync(clientId, updateRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(client.Id, "The client's Guid should be returned");

        _repository.Verify(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.ExistsByIdentifierAsync(updateRequest.IdentificationNumber!, It.IsAny<CancellationToken>()), Times.Once);
        _logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateClientAsync_GivenNoIdUpdate_ShouldReturnClientId()
    {
        var clientId = Guid.NewGuid();
        var updateRequest = GetValidUpdateClientRequest();
        var client = GetValidClient();
        client.Id = clientId;
        client.IdentificationNumber = updateRequest.IdentificationNumber!;
        _repository.Setup(r => r.GetAsync(clientId, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _repository.Setup(r =>
                r.ExistsByIdentifierAsync(updateRequest.IdentificationNumber!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _clientService.UpdateClientAsync(clientId, updateRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(client.Id, "The client's Guid should be returned");

        _repository.Verify(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.ExistsByIdentifierAsync(updateRequest.IdentificationNumber!, It.IsAny<CancellationToken>()), Times.Once);
        _logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateClientAsync_GivenNonExistingClientId_ShouldReturnNotFound()
    {
        var clientId = Guid.NewGuid();
        var updateRequest = GetValidUpdateClientRequest();
        Client? client = null;
        _repository.Setup(r => r.GetAsync(clientId, It.IsAny<CancellationToken>())).ReturnsAsync(client);

        var result = await _clientService.UpdateClientAsync(clientId, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid ClientId should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Client '{clientId}' not found.");

        _repository.Verify(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Never);
        _repository.Verify(r => r.ExistsByIdentifierAsync(updateRequest.IdentificationNumber!, It.IsAny<CancellationToken>()), Times.Never);
        _logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateClientAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        var clientId = Guid.NewGuid();
        var updateRequest = GetValidUpdateClientRequest();
        updateRequest.Email = "john.doe.mail.com";
        var client = GetValidClient();
        client.Id = clientId;
        client.IdentificationNumber = updateRequest.IdentificationNumber!;
        var failures = new[]
        {
            new ValidationFailure("Email", "Email must be a valid email address.")
            {
                ErrorCode = "EmailValidator"
            }
        };
        _repository.Setup(r => r.GetAsync(clientId, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _clientService.UpdateClientAsync(clientId, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);

        _repository.Verify(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.ExistsByIdentifierAsync(updateRequest.IdentificationNumber!, It.IsAny<CancellationToken>()), Times.Never);
        _logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateClientAsync_GivenExistingIdentifier_ShouldReturnConflict()
    {
        var clientId = Guid.NewGuid();
        var updateRequest = GetValidUpdateClientRequest();
        var client = GetValidClient();
        _repository.Setup(r => r.GetAsync(clientId, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _repository.Setup(r =>
                r.ExistsByIdentifierAsync(updateRequest.IdentificationNumber!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _clientService.UpdateClientAsync(clientId, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An already existing IdentificationNumber should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ConflictError
                                                  && e.Message == $"Client with IdentificationNumber: '{updateRequest.IdentificationNumber}' already exists.");

        _repository.Verify(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.ExistsByIdentifierAsync(updateRequest.IdentificationNumber!, It.IsAny<CancellationToken>()), Times.Once);
        _logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Never);
    }

    private Client GetValidClient() => new Client()
    {
        Id = Guid.NewGuid(),
        Name = "John Doe",
        Type = "Individual",
        Address = "Some address",
        Email = "john.doe@mail.com",
        IdentificationNumber = "5020202123123",
        Phone = "0712345678"
    };

    private ClientDto GetClientDtoFromClient(Client client) => new ClientDto()
    {
        Id = client.Id,
        Address = client.Address ?? "",
        Email = client.Email,
        IdentificationNumber = client.IdentificationNumber,
        Name = client.Name,
        Phone = client.Phone,
        Type = client.Type
    };

    private CreateClientRequest GetValidCreateClientRequest() => new CreateClientRequest()
    {
        Type = ClientType.Individual,
        Name = "John Doe",
        Address = "Some street, nr. 9",
        Email = "john.doe@mail.com",
        Phone = "0712345678",
        IdentificationNumber = "3020202123123"
    };

    private UpdateClientRequest GetValidUpdateClientRequest() => new UpdateClientRequest()
    {
        Name = "John Doe",
        Address = "Some street, nr. 9",
        Email = "john.doe@mail.com",
        Phone = "0712345678",
        IdentificationNumber = "3020202123123"
    };
}
