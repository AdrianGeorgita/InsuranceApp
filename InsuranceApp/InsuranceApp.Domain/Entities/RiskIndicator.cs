using System;
using System.Collections.Generic;

namespace InsuranceApp.Domain.Entities;

public partial class RiskIndicator
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Building> Buildings { get; set; } = new List<Building>();
}
