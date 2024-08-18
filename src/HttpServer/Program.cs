using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HttpServer.WebServer;
using HttpServer.WebServer.Http;
using System.Diagnostics;
using HttpServer.Middlewares.Default;

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
