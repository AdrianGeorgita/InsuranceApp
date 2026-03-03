using System;
using System.Collections.Generic;

namespace InsuranceApp.Domain.Entities;

public partial class Country
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Iso2 { get; set; } = null!;

    public string Iso3 { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<County> Counties { get; set; } = new List<County>();
}
