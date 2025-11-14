using Google.Protobuf.WellKnownTypes;
using Google.Rpc;
using Grpc.Core;
using System.Runtime.CompilerServices;

namespace GrpcGreeter.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        ArgumentNotNullOrEmpty(request.Name);

        _logger.LogInformation("Address 1 - Incoming request from {Name}", request.Name);
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }

    public static void ArgumentNotNullOrEmpty(string value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value.Length < 2)
        {
            var status = new Google.Rpc.Status
            {
                Code = (int)Code.InvalidArgument,
                Message = "Bad request",
                Details =
                {
                    Any.Pack(new BadRequest
                    {
                        FieldViolations =
                        {
                            new BadRequest.Types.FieldViolation { Field = paramName, Description = "Value is null or empty" }
                        }
                    })
                }
            };
            throw status.ToRpcException();
        }
    }
}
