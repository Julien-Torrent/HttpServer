using System;
using System.Threading.Tasks;
using HttpServer.WebServer.Http;

namespace HttpServer.Middlewares;

public interface IMiddlewareBuilder
{
    public void AddMiddleware(IMiddleware middleware);
    public void AddMiddleware(Func<HttpContext, Func<Task>, Task> middleware);
}
