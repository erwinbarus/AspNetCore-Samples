using Grpc.Core;

namespace GrpcService.Services;

public class CalculatorService : Calculator.CalculatorBase
{
    private readonly ILogger<CalculatorService> _logger;
    public CalculatorService(ILogger<CalculatorService> logger)
    {
        _logger = logger;
    }

    public override Task<CalculatorResponse> MakeDouble(CalculatorRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Make {request.Value} value double.");
        return Task.FromResult(new CalculatorResponse
        {
            Value = request.Value * 2
        });
    }
}
