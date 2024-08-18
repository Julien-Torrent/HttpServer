using System;
using System.Threading.Tasks;
using HttpServer.WebServer.Http;

namespace HttpServer.Middlewares;

public class FunctionMiddleware : IMiddleware
{
    private readonly Func<HttpContext, Func<Task>, Task> _action; 

    public FunctionMiddleware(Func<HttpContext, Func<Task>, Task> action)
    {
        _action = action;
    }

    public async Task InvokeAsync(HttpContext context, Func<Task> next)
    {
        await _action(context, next);
    }
}
