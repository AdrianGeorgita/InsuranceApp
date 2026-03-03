using FluentResults;
using InsuranceApp.Application.Brokers.DTOs;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Brokers;

public interface IBrokerService
{
    Task<Result<PagedResult<BrokerDto>>> ListAllBrokersAsync(PageRequest pageRequest, CancellationToken ct);
    Task<Result<BrokerDto>> GetBrokerByIdAsync(Guid guid, CancellationToken ct);

    Task<Result<Guid>> CreateBrokerAsync(CreateBrokerRequest createBrokerDto, CancellationToken ct);
    Task<Result<Guid>> UpdateBrokerAsync(Guid brokerId, UpdateBrokerRequest updateBrokerDto, CancellationToken ct);
    Task<Result<Guid>> UpdateBrokerStatusAsync(Guid brokerId, BrokerStatus newStatus, CancellationToken ct);
}