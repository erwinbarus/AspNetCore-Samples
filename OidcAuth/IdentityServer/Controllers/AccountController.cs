using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityServerHost.Controllers
{
    public class AccountController : Controller
    {
        private readonly TestUserStore _users;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;

        public AccountController(
            TestUserStore users,
            IIdentityServerInteractionService interaction,
            IEventService events)
        {
            _users = users;
            _interaction = interaction;
            _events = events;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (_users.ValidateCredentials(model.Username, model.Password))
            {
                var user = _users.FindByUsername(model.Username);

                await _events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username));

                // Build claims, including 'sub'
                var claims = new List<Claim>
                {
                    new Claim("sub", user.SubjectId),
                    new Claim("name", user.Username)
                };
                claims.AddRange(user.Claims); // optional extra claims

                var identity = new ClaimsIdentity(claims, IdentityServerConstants.DefaultCookieAuthenticationScheme, "name", "roles");
                var principal = new ClaimsPrincipal(identity);

                var props = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                // Sign in with the IdentityServer cookie scheme
                await HttpContext.SignInAsync(
                    IdentityServerConstants.DefaultCookieAuthenticationScheme,
                    principal,
                    props
                );

                // Redirect back to returnUrl if valid
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return Redirect("~/");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await HttpContext.SignOutAsync();
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            if (logout?.PostLogoutRedirectUri != null)
                return Redirect(logout.PostLogoutRedirectUri);

            return Redirect("~/");
        }
    }
}
