using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/chainlog", async (HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    var httpClient = httpClientFactory.CreateClient();

    var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                        ?? Guid.NewGuid().ToString();

    using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5164/weatherforecast/");
        request.Headers.Add("X-Correlation-ID", correlationId);

        using var response = await httpClient.SendAsync(request);

        var content = await response.Content.ReadAsStringAsync();
        return Results.Content(content, "application/json");
    }
});


app.Run();
