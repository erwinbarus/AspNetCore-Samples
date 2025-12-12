using Duende.IdentityServer.Models;

var builder = WebApplication.CreateBuilder(args);

// Add IdentityServer
builder.Services.AddIdentityServer()
    .AddInMemoryIdentityResources(new IdentityResource[] {
        new IdentityResources.Profile(),
        new IdentityResources.OpenId(),
    })
    .AddInMemoryApiScopes(new[] {
        new ApiScope("api", "Demo API"),
    })
    .AddInMemoryApiResources(new[] {
        new ApiResource("api", "Demo API Resource")
        {
            Scopes = { "api" }
        }
    })
    .AddInMemoryClients(new[] {
        new Client
        {
            ClientId = "demo-client",
            ClientSecrets = { new Secret("demo-secret".Sha256()) },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AllowedScopes = { "api" }
        }
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseRouting();
app.UseIdentityServer();  // add IdentityServer middleware

app.MapDefaultControllerRoute();

app.Run();