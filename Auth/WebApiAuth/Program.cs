using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApiAuth.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApiDocument();

builder.Services.AddDbContext<ApplicationDbContext>(options=>
    options.UseInMemoryDatabase("AppDb"));

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();
app.MapIdentityApi<IdentityUser>();

app.MapGet("/", () =>
{
    return "Hello World";
})
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
})
.WithOpenApi()
.RequireAuthorization();

app.Run();
