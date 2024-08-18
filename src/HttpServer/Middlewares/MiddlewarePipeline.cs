using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HttpServer.WebServer.Http;

namespace HttpServer.Middlewares;

// All middlewares need to be stateless
public class MiddlewarePipeline : IMiddlewareBuilder
{
    private readonly List<IMiddleware> _middlewares = [];

    public void AddMiddleware(IMiddleware middleware)
    {
        _middlewares.Add(middleware);
    }

    public void AddMiddleware(Func<HttpContext, Func<Task>, Task> middleware)
    {
        AddMiddleware(new FunctionMiddleware(middleware));
    }

    public async Task StartPipeline(HttpContext context)
    {
        if(_middlewares.Count > 0)
        {
            await ExecuteMiddleware(context, 0);
        }
    }

    private async Task ExecuteMiddleware(HttpContext ctx, int idx)
    {
        if(idx < _middlewares.Count)
        {
            await _middlewares[idx].InvokeAsync(ctx, () => ExecuteMiddleware(ctx, idx + 1));
        }
    }
}
