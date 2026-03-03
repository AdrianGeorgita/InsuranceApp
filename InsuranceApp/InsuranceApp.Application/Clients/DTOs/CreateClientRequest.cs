using System.ComponentModel.DataAnnotations;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Clients.DTOs;

public class CreateClientRequest
{
    public ClientType Type { get; set; }

    public string Name { get; set; } = null!;

    public string IdentificationNumber { get; set; } = null!;

    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Address { get; set; }
}