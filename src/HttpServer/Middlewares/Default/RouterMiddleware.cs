using System;
using System.Net;
using System.Threading.Tasks;
using HttpServer.Routes;
using HttpServer.WebServer.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttpServer.Middlewares.Default;

public class RouterMiddleware : IMiddleware
{
    private readonly Router _router;

    public RouterMiddleware(Router router)
    {
        _router = router;
    }

    public async Task InvokeAsync(HttpContext context, Func<Task> next)
    {
        if (_router.TryGetHandler(context.Request.Method, context.Request.Path, out var endpoint))
        {
            context.Parameters = endpoint.Parameters;
            
            context.IsMatchedRoute = true;
            context.HttpEndpoint = endpoint.Handler;

            await next();
        }
        else
        {
            context.Response = Response.NotFound(new ProblemDetails()
            {
                Title = "Not Found",
                Status = (int)HttpStatusCode.NotFound,
                Detail = "Not Found"
            });
        }
    }
}
