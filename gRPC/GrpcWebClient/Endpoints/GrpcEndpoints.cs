using Google.Rpc;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Health.V1;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Grpc.Net.Client.Web;
using GrpcService;
using GrpcWebClient;

namespace Endpoints.GrpcEndpoints;

public static class GrpcEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/grpc/{name}", async (string name, ILogger<ClientLoggingInterceptor> logger) =>
        {
            try
            {
                var handler = new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(5),
                    EnableMultipleHttp2Connections = true
                };

                using var channel = GrpcChannel.ForAddress("http://localhost:5048", 
                    new GrpcChannelOptions
                    {
                        HttpHandler = handler,
                    });
                var invoker = channel.Intercept(new ClientLoggingInterceptor(logger));

                var client = new Greeter.GreeterClient(invoker);
                var reply = await client.SayHelloAsync(new HelloRequest { Name = name });
                logger.LogInformation("Greeting: " + reply.Message);
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

        app.MapGet("/grpc/static/{name}", async (string name) =>
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

        app.MapGet("/grpc/file/{name}", async (string name) =>
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

        app.MapGet("/grpc/balancer/{name}", async (string name) =>
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

        app.MapGet("/grpc/web-client/{name}", async (string name) =>
        {
            using var channel = GrpcChannel.ForAddress(
                "http://localhost:5212",
                new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler(new HttpClientHandler())
                }
            );
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest { Name = name });

            return "Successful";
        });

        app.MapGet("/grpc/health/check", async () =>
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5212");
            var client = new Health.HealthClient(channel);

            var response = await client.CheckAsync(new HealthCheckRequest());
            var status = response.Status;
            
            return status.ToString();
        });
    }
}