using Microsoft.AspNetCore.Identity;

namespace InsuranceApp.Infrastructure.Persistence.Identity;

public partial class Role : IdentityRole<Guid>
{
    public override Guid Id { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
