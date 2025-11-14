using Grpc.Net.Client.Balancer;
using System.Text.Json;

namespace GrpcWebClient;

public class FileResolver : PollingResolver
{
    private readonly Uri _address;
    private readonly int _port;

    public FileResolver(Uri address, int defaultPort, ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        _address = address;
        _port = defaultPort;
    }

    protected override async Task ResolveAsync(CancellationToken cancellationToken)
    {
        // Load JSON from a file on disk and deserialize into endpoints.
        var jsonString = await File.ReadAllTextAsync(_address.LocalPath);
        var results = JsonSerializer.Deserialize<string[]>(jsonString);
        var addresses = results.Select(r => new BalancerAddress(r, _port)).ToArray();

        // Pass the results back to the channel.
        Listener(ResolverResult.ForResult(addresses));
    }
}

public class FileResolverFactory : ResolverFactory
{
    // Create a FileResolver when the URI has a 'file' scheme.
    public override string Name => "file";

    public override Resolver Create(ResolverOptions options)
    {
        return new FileResolver(options.Address, options.DefaultPort, options.LoggerFactory);
    }
}
