using System;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer.WebServer;

public interface IServer : IDisposable
{
    Task StartAsync(CancellationToken ct = default);

    Task RunAsync(CancellationToken ct = default);

    Task StopAsync(CancellationToken ct = default);
}
