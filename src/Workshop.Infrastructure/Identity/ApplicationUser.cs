using Microsoft.AspNetCore.Identity;

namespace Workshop.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public Guid? WorkerId { get; set; }
}
