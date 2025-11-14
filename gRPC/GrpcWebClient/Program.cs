using Google.Rpc;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Grpc.Net.Client.Web;
using GrpcService;
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

app.MapGet("/{name}", async (string name) =>
{
    try
    {
        using var channel = GrpcChannel.ForAddress("http://localhost:5048");
        var invoker = channel.Intercept(new ClientLoggingInterceptor());

        var client = new Greeter.GreeterClient(invoker);
        var reply = await client.SayHelloAsync(new HelloRequest { Name = name });
        Console.WriteLine("Greeting: " + reply.Message);
        return "Successful";
    }
    catch (RpcException ex)
    {
        Console.WriteLine($"Server error: {ex.Status.Detail}");
        var badRequest = ex.GetRpcStatus()?.GetDetail<BadRequest>();
        if (badRequest != null)
        {
            foreach (var fieldViolation in badRequest.FieldViolations)
            {
                Console.WriteLine($"Field: {fieldViolation.Field}");
                Console.WriteLine($"Description: {fieldViolation.Description}");
            }
        }
        return "Error";
    }
});

app.MapGet("/static/{name}", async (string name) =>
{
    using var channel = GrpcChannel.ForAddress(
        "static:my-example-dns",
        new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.Insecure,
            ServiceProvider = app.Services.GetRequiredService<IServiceProvider>(),
            ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new PickFirstConfig() } }
        });
    var client = new Greeter.GreeterClient(channel);
    var reply = await client.SayHelloAsync(new HelloRequest { Name = name });

    return "Successful";
});

app.MapGet("/file/{name}", async (string name) =>
{
    using var channel = GrpcChannel.ForAddress(
        "file:///Projects/Training/ASP.NET%20Core%20documentation/GrpcWebClient/servers.json",
        new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.Insecure,
            ServiceProvider = app.Services.GetRequiredService<IServiceProvider>(),
            ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new PickFirstConfig() } }
        });
    var client = new Greeter.GreeterClient(channel);
    var reply = await client.SayHelloAsync(new HelloRequest { Name = name });

    return "Successful";
});

app.MapGet("/random/balancer/{name}", async (string name) =>
{
    using var channel = GrpcChannel.ForAddress(
        "static:my-example-dns",
        new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.Insecure,
            ServiceProvider = app.Services.GetRequiredService<IServiceProvider>(),
            ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new LoadBalancingConfig("random") } }
        });
    var client = new Greeter.GreeterClient(channel);
    var reply = await client.SayHelloAsync(new HelloRequest { Name = name });

    return "Successful";
});

app.MapGet("/grpc/web/{name}", async (string name) =>
{
    using var channel = GrpcChannel.ForAddress(
        "http://localhost:5212",
        new GrpcChannelOptions
        {
            HttpHandler = new GrpcWebHandler(new HttpClientHandler())
        });
    var client = new Greeter.GreeterClient(channel);
    var reply = await client.SayHelloAsync(new HelloRequest { Name = name });

    return "Successful";
});

app.Run();
