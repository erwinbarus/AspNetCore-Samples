using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var validAudiences = builder.Configuration.GetSection("Authentication:Schemes:Bearer:ValidAudiences").Get<string[]>();
var validIssuer = builder.Configuration["Authentication:Schemes:Bearer:ValidIssuer"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = validIssuer,
            ValidAudiences = validAudiences,
            IssuerSigningKey = new SymmetricSecurityKey(
                Convert.FromBase64String("bEumFw+3wAzqDx6wxwY/0cx/31oTUuUPk25j40raMrM="))
        };
    });

builder.Services.AddAuthorizationBuilder()
  .AddPolicy("admin_policy", policy =>
        policy
            .RequireRole("admin")
            .RequireClaim("scope", "api"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/hello", () =>
{
    return TypedResults.Ok("You've authorized.");
})
.RequireAuthorization("admin_policy");

await app.RunAsync();
