using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDataProtection();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", (IDataProtectionProvider dataProtector) =>
{
    var _dataProtector = dataProtector.CreateProtector("MyPurpose");

    // The original data to protect
    string originalData = "original data";

            // Protect the data (encrypt)
    string protectedData = _dataProtector.Protect(originalData);
    Console.WriteLine($"Protected Data: {protectedData}");

    // Unprotect the data (decrypt)
    string unprotectedData = _dataProtector.Unprotect(protectedData);
    Console.WriteLine($"Unprotected Data: {unprotectedData}");

    return TypedResults.Ok(new
    {
        OriginalData = originalData,
        ProtectedData = protectedData,
        UnprotectedData = unprotectedData
    });
});

app.Run();
