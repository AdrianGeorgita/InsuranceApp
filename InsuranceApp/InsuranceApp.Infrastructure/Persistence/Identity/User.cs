using InsuranceApp.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace InsuranceApp.Infrastructure.Persistence.Identity;

public partial class User : IdentityUser<Guid>, IAuditable, ISoftDeletable
{
    public override Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
