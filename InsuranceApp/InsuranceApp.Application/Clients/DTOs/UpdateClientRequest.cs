using System.ComponentModel.DataAnnotations;

namespace InsuranceApp.Application.Clients.DTOs;

public class UpdateClientRequest
{
    public string? Name { get; set; }

    public string? IdentificationNumber { get; set; }

    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }
}