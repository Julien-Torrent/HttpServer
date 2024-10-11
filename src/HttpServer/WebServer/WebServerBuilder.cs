using System.Collections.Generic;
using System.Threading.Tasks;
using HttpServer.Middlewares;
using HttpServer.Middlewares.Default;
using HttpServer.Routes;

namespace HttpServer.WebServer;

public class WebServerBuilder
{
    public RouterBuilder Router { get; } = new();

    private readonly MiddlewarePipeline _pipeline = new();
    public IMiddlewareBuilder Middlewares => _pipeline;

    private readonly List<HttpServer.Http.HttpServer> _httpListeners = [];
    private readonly WebServer _webServer;

    public WebServerBuilder()
    {
        _webServer = new WebServer(_pipeline, _httpListeners);

        _pipeline.AddMiddleware(new ErrorHandlerMiddleware());
        _pipeline.AddMiddleware(new RouterMiddleware(Router.Build()));
    }

    public WebServerBuilder UseHttps(int port, string certificatePath, string password)
    {
        _httpListeners.Add(new HttpServer.Http.HttpServer(port, _webServer.HandleRequestAsync).WithHttps(certificatePath, password));
        return this;
    }

    public WebServerBuilder UseHttp(int port)
    {
        _httpListeners.Add(new HttpServer.Http.HttpServer(port, _webServer.HandleRequestAsync));
        return this;
    }

    public WebServer Build()
    {
        _pipeline.AddMiddleware(new EndpointMiddleware());
        // Terminal middleware
        _pipeline.AddMiddleware((_, _) => Task.CompletedTask);
        return _webServer;
    }
}
