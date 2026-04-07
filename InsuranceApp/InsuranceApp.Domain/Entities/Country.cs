using InsuranceApp.Domain.Common.Interfaces;

namespace InsuranceApp.Domain.Entities;

public partial class Country : IAuditable
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Iso2 { get; set; } = null!;

    public string Iso3 { get; set; } = null!;

    public virtual ICollection<County> Counties { get; set; } = new List<County>();

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
