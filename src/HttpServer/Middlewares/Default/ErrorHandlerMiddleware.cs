using System;
using System.Net;
using System.Threading.Tasks;
using HttpServer.WebServer.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttpServer.Middlewares.Default;

public class ErrorHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, Func<Task> next)
    {
        try
        {
            await next();
        }
        catch
        {
            context.Response = Response.InternalServerError(new ProblemDetails()
            {
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "Internal Server Error",
            });
        }
    }
}
