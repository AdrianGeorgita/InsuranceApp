using InsuranceApp.Domain.Common.Interfaces;

namespace InsuranceApp.Domain.Entities;

public partial class County : IAuditable
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid CountryId { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Country Country { get; set; } = null!;

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
