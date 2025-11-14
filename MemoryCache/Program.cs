using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/counter", (IMemoryCache cache) =>
{
    if (cache.TryGetValue<int>("counter", out var counter))
    {
        counter++;
    }
    else
    {
        counter = 1;
    }

    cache.Set("counter", counter);
    
    return $"Total visits: {counter}";
})
.WithName("GetCounter");

app.Run();
