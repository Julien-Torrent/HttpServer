using System;
using System.Threading.Tasks;
using HttpServer.WebServer.Http;

namespace HttpServer.Middlewares.Default;

public class EndpointMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, Func<Task> next)
    {
        if(context.IsMatchedRoute)
        {
            context.Response = await context.HttpEndpoint(context);
        }

        await next();
    }
}
