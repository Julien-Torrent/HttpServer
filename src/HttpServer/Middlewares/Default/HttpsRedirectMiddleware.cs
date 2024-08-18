using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using HttpServer.WebServer.Http;

namespace HttpServer.Middlewares.Default;

public class HttpsRedirectMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, Func<Task> next)
    {
        if (!context.IsSecureConnection)
        {
            if(context.LocalAddress == null)
            {
                return;
            }

            context.Response = new Response()
            {
                StatusCode = HttpStatusCode.MovedPermanently,
                Headers = new Dictionary<string, string>
                {
                    { "Location", $"https://{context.LocalAddress.Address}:443{context.Request.Path}" }
                }
            };

            return;
        }

        await next();
    }
}
