using GrpcService.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GrpcService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("customPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            });
        });
        builder.Services.AddGrpc().AddJsonTranscoding();
        builder.Services.AddGrpcHealthChecks()
                .AddCheck("Sample", () => HealthCheckResult.Healthy());

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseCors("customPolicy");
        app.UseGrpcWeb();

        app.MapGrpcService<GreeterService>().EnableGrpcWeb();
        app.MapGrpcService<CalculatorService>();
        app.MapGrpcHealthChecksService().AllowAnonymous(); ;

        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }
}