using Duende.IdentityServer.Models;

var builder = WebApplication.CreateBuilder(args);

// Add IdentityServer
builder.Services.AddIdentityServer()
    .AddInMemoryClients(new[] {
        new Client
        {
            ClientId = "demo-client",
            ClientSecrets = { new Secret("demo-secret".Sha256()) },
            AllowedGrantTypes = GrantTypes.Code,
            RedirectUris = { "https://localhost:7002/signin-oidc" },
            PostLogoutRedirectUris = { "https://localhost:7002/signout-callback-oidc" },
            AllowedScopes = { "openid", "profile", "api1" }
        }
    })
    .AddInMemoryIdentityResources(new IdentityResource[] {
        new IdentityResources.Profile(),
        new IdentityResources.OpenId(),
    })
    .AddInMemoryApiScopes(new[] {
        new ApiScope("api1", "Demo API")
    })
    .AddTestUsers(new List<Duende.IdentityServer.Test.TestUser>
    {
        new Duende.IdentityServer.Test.TestUser
        {
            SubjectId = "1",
            Username = "alice",
            Password = "password",
            Claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim("name", "Alice Smith"),
                new System.Security.Claims.Claim("email", "alice@example.com"),
                new System.Security.Claims.Claim("roles", "admin")
            }
        }
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseRouting();
app.UseIdentityServer();  // add IdentityServer middleware

app.MapDefaultControllerRoute();

app.Run();