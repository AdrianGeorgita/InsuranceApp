using InsuranceApp.Domain.Common.Interfaces;

namespace InsuranceApp.Domain.Entities;

public partial class Administrator : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
