using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using RazorAppIdentity.Areas.Identity.Data;
using RazorAppIdentity.Data;
using RazorAppIdentity.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("RazorAppIdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'RazorAppIdentityContextConnection' not found.");;

builder.Services.AddDbContext<RazorAppIdentityContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<RazorAppIdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<RazorAppIdentityContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
});

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddTransient<IEmailSender, AwsEmailSender>();
builder.Services.Configure<AwsSesOptions>(builder.Configuration.GetSection("AwsSes"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
