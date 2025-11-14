using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcWebClient;

public class ClientLoggingInterceptor : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        Console.WriteLine("Starting gRPC call.");
        return continuation(request, context);
    }
}