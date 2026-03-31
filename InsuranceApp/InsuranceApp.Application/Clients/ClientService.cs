using AutoMapper;
using FluentResults;
using InsuranceApp.Application.Clients.DTOs;
using InsuranceApp.Application.Common.Audit;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace InsuranceApp.Application.Clients;
public class ClientService(IClientRepository clientRepository, IRequestValidator requestValidator,
    IMapper mapper, IAuditEventPublisher auditEventPublisher, ILogger<ClientService> logger) : IClientService
{
    public async Task<Result<PagedResult<ClientDto>>> ListAllClientsAsync(PageRequest pageRequest, ClientFilter? filter, CancellationToken ct)
    {
        var pagedResult = await clientRepository.GetAllClients(pageRequest, filter, ct);
        return Result.Ok(pagedResult);
    }

    public async Task<Result<ClientDto>> GetClientByIdAsync(Guid guid, CancellationToken ct)
    {
        var client = await clientRepository.GetAsync(guid, ct);

        if (client is null)
            return Result.Fail<ClientDto>(new NotFoundError($"Client '{guid}' not found."));

        return Result.Ok(mapper.Map<ClientDto>(client));
    }

    public async Task<Result<Guid>> CreateClientAsync(CreateClientRequest createClientDto, CancellationToken ct)
    {
        var requestValidationResult = await EnsureValidRequestAndNoIdentifierConflictAsync(createClientDto,
                createClientDto?.IdentificationNumber, ct);
        if (requestValidationResult.IsFailed) return requestValidationResult;

        var client = mapper.Map<Client>(createClientDto);

        client.Id = Guid.NewGuid();
        client.CreatedAt = DateTime.UtcNow;
        client.UpdatedAt = DateTime.UtcNow;

        logger.LogInformation("Client with id '{ClientId}' has been created.", client.Id);

        await clientRepository.AddAsync(client, ct);

        return Result.Ok(client.Id);
    }

    public async Task<Result<Guid>> UpdateClientAsync(Guid clientId, UpdateClientRequest updateClientDto, CancellationToken ct)
    {
        var existingClient = await clientRepository.GetAsync(clientId, ct);
        if (existingClient is null)
            return Result.Fail<Guid>(new NotFoundError($"Client '{clientId}' not found."));

        var requestValidationResult = await EnsureValidRequestAndNoIdentifierConflictAsync(updateClientDto,
            updateClientDto?.IdentificationNumber, ct);
        if (requestValidationResult.IsFailed) return requestValidationResult;

        existingClient.Name = updateClientDto?.Name ?? existingClient.Name;
        existingClient.Phone = updateClientDto?.Phone ?? existingClient.Phone;
        existingClient.Email = updateClientDto?.Email ?? existingClient.Email;
        existingClient.Address = updateClientDto?.Address ?? existingClient.Address;

        existingClient.UpdatedAt = DateTime.UtcNow;

        if (updateClientDto?.IdentificationNumber is not null
            && existingClient.IdentificationNumber != updateClientDto.IdentificationNumber)
            await LogIdentificationNumberChange(existingClient.IdentificationNumber,
                updateClientDto.IdentificationNumber, existingClient.Id, ct);

        existingClient.IdentificationNumber = updateClientDto?.IdentificationNumber ?? existingClient.IdentificationNumber;

        logger.LogInformation("Client with id '{ClientId}' has been updated.", existingClient.Id);

        return Result.Ok(existingClient.Id);
    }

    public async Task<Result<Guid>> DeleteClientByIdAsync(Guid clientId, CancellationToken ct)
    {
        var existingClient = await clientRepository.GetAsync(clientId, ct);
        if (existingClient is null)
            return Result.Fail<Guid>(new NotFoundError($"Client '{clientId}' not found."));

        clientRepository.Remove(existingClient);
        logger.LogInformation("Client with id '{ClientId}' has been deleted.", existingClient.Id);

        return Result.Ok(existingClient.Id);
    }

    private async Task LogIdentificationNumberChange(string oldIdentificationNumber, string newIdentificationNumber,
        Guid clientId, CancellationToken ct)
    {
        var auditEventId = Guid.NewGuid();
        var auditChangeEvent = new AuditTableChangeEvent
        {
            EventId = auditEventId,
            UserId = Guid.NewGuid(),
            TableName = "Clients",
            ColumnName = "IdentificationNumber",
            OldValue = oldIdentificationNumber,
            NewValue = newIdentificationNumber,
            RowId = clientId.ToString()
        };

        await auditEventPublisher.PublishAuditEventAsync(auditChangeEvent, ct);
    }

    private async Task<Result> EnsureValidRequestAndNoIdentifierConflictAsync<TRequest>(TRequest request, string? identificationNumber, CancellationToken ct)
    {
        var validation = await requestValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Fail(validation.ToApiError());

        if (!string.IsNullOrWhiteSpace(identificationNumber)
            && await clientRepository.ExistsByIdentifierAsync(identificationNumber, ct))
            return Result.Fail(new ConflictError($"Client with IdentificationNumber: '{identificationNumber}' already exists."));

        return Result.Ok();
    }
}

