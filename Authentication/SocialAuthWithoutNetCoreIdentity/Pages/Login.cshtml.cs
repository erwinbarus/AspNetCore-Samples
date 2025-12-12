using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SocialAuthWithoutNetCoreIdentity.Pages
{
    [Authorize]
    public class LoginModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
