using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttpServer.Middlewares;
using HttpServer.WebServer.Http;

namespace HttpServer.WebServer;

public class WebServer : IServer
{
    private readonly MiddlewarePipeline _middlewarePipeline;

    private readonly IEnumerable<HttpServer.Http.HttpServer> _httpListeners;

    public WebServer(MiddlewarePipeline middlewarePipeline,  IEnumerable<HttpServer.Http.HttpServer> httpListeners)
    {
        _httpListeners = httpListeners;
        _middlewarePipeline = middlewarePipeline;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        foreach(var listener in _httpListeners)
        {
           await listener.StartAsync(ct);
        }
    }

    public async Task StopAsync(CancellationToken ct = default)
    {
        foreach(var listener in _httpListeners)
        {
           await listener.StopAsync(ct);
        }
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        await Task.WhenAll(_httpListeners.Select(x => x.RunAsync()));
    }

    public async Task HandleRequestAsync(HttpContext context, CancellationToken ct)
    {
        Console.WriteLine($"{context.LocalAddress} {context.IsSecureConnection}");

        try
        {
            await _middlewarePipeline.StartPipeline(context);

            context.Response ??= Response.NoContent();

            await context.Connection.WriteHttpResponse(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        foreach(var listener in _httpListeners)
        {
            listener.Dispose();
        }
    }
}
