using System;
using System.Threading.Tasks;
using HttpServer.WebServer.Http;

namespace HttpServer.Middlewares;

public interface IMiddleware
{
    public Task InvokeAsync(HttpContext context, Func<Task> next);
}
