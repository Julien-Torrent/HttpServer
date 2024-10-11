using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HttpServer.WebServer;
using HttpServer.WebServer.Http;
using System.Diagnostics;
using HttpServer.Middlewares.Default;
using HttpServer.WebSockets;

var builder = new WebServerBuilder();

builder.Router.MapGet("/ok", (context) =>
{
    return Task.FromResult(Response.Html(HttpStatusCode.OK, "<html>OK</html>"));
})
.MapGet("/headers", (context) =>
{
    return Task.FromResult(Response.Json(HttpStatusCode.OK, context.Request.Headers));
})
.MapGet("/headers/{id:string}", (context) =>
{
    return Task.FromResult(Response.Json(HttpStatusCode.OK, context.Parameters));
})
.MapGet("/throw", (context) =>
{
    throw new Exception("Hello");
})
.MapGet("/body", (context) =>
{
    return Task.FromResult(Response.Json(HttpStatusCode.OK, Encoding.UTF8.GetString(context.Request.Body)));
})
.MapGet("/null", (context) =>
{
    return Task.FromResult<Response>(null!);
})
.MapGet("/ws", async (context) => 
{
    if(context.IsWebSocket)
    {
        var socket = await context.AcceptWebSocket();
        var tcs = new TaskCompletionSource<Response>();

        _ = Task.Run(async () => 
        {
            try
            {
                while(true)
                {
                    var res = await socket.GetFrameAsync();
                    Console.WriteLine(Encoding.UTF8.GetString(res.Payload));
                }
            }
            catch(Exception)
            {
                tcs.SetResult(Response.NoContent());
            }
        });

        return await tcs.Task;
    }   
    else
    {
        return new Response()
        {
            StatusCode = HttpStatusCode.BadRequest,
            Headers = [],
            Body = "Bad request! Only Ws"
        };
    }
});


builder.Middlewares.AddMiddleware(async (ctx, next) =>
{
    Console.WriteLine($"{ctx.Request.Method} {ctx.Request.Path}");
    var start = Stopwatch.GetTimestamp();
    await next();

    Console.WriteLine(Stopwatch.GetElapsedTime(start));
});
builder.Middlewares.AddMiddleware(new HttpsRedirectMiddleware());

builder.UseHttp(80);
builder.UseHttps(443, "certificate.pfx", "password");

using var server = builder.Build();

await server.RunAsync();
