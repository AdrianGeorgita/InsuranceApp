using InsuranceApp.Domain.Common.Interfaces;

namespace InsuranceApp.Domain.Entities;

public partial class City : IAuditable
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid CountyId { get; set; }

    public virtual ICollection<Building> Buildings { get; set; } = new List<Building>();

    public virtual County County { get; set; } = null!;

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
