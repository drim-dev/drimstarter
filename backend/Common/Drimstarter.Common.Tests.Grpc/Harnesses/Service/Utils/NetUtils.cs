using System.Net.NetworkInformation;

namespace Drimstarter.Common.Tests.Grpc.Harnesses.Service.Utils;

public static class NetUtils
{
    public static int GetAvailablePort()
    {
        var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        var usedPorts = Enumerable.Empty<int>()
            .Concat(ipProperties.GetActiveTcpConnections().Select(c => c.LocalEndPoint.Port))
            .Concat(ipProperties.GetActiveTcpListeners().Select(l => l.Port))
            .Concat(ipProperties.GetActiveUdpListeners().Select(l => l.Port))
            .ToHashSet();

        for (var i = 0; i < 100; i++)
        {
            var port = Random.Shared.Next(10_000, 50_000);
            if (!usedPorts.Contains(port)) return port;
        }

        throw new Exception("Failed to find an available port");
    }
}
