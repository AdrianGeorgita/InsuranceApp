using System;
using System.Collections.Generic;

namespace InsuranceApp.Domain.Entities;

public partial class BuildingRiskIndicator
{
    public Guid BuildingId { get; set; }

    public int RiskIndicatorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Building Building { get; set; } = null!;

    public virtual RiskIndicator RiskIndicator { get; set; } = null!;
}
