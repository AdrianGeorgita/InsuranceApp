using System;
using System.Collections.Generic;

namespace InsuranceApp.Domain.Entities;

public partial class County
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid CountryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Country Country { get; set; } = null!;
}
