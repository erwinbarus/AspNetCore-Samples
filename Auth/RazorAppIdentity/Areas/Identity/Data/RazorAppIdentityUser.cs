using Microsoft.AspNetCore.Identity;

namespace RazorAppIdentity.Areas.Identity.Data;

// Add profile data for application users by adding properties to the RazorAppIdentityUser class
public class RazorAppIdentityUser : IdentityUser<Guid>
{
    [PersonalData]
    public string? Name { get; set; }
    [PersonalData]
    public DateTime DOB { get; set; }

    public virtual ICollection<IdentityUserClaim<Guid>>? Claims { get; set; }
    public virtual ICollection<IdentityUserLogin<Guid>>? Logins { get; set; }
    public virtual ICollection<IdentityUserToken<Guid>>? Tokens { get; set; }
    public virtual ICollection<IdentityUserRole<Guid>>? UserRoles { get; set; }
}

