using Endpoints.GrpcEndpoints;
using Grpc.Net.Client.Balancer;
using GrpcWebClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// region Static Resolver Registration
var factory = new StaticResolverFactory(addr => new[]
{
    new BalancerAddress("localhost", 5048),
    new BalancerAddress("localhost", 5212)
});

builder.Services.AddSingleton<ResolverFactory>(factory);

// region File Resolver Registration
builder.Services.AddSingleton<ResolverFactory, FileResolverFactory>();

// region Load Balancer Registration
builder.Services.AddSingleton<LoadBalancerFactory, RandomBalancerFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    return "Ok";
});

GrpcEndpoints.Map(app);

app.Run();
