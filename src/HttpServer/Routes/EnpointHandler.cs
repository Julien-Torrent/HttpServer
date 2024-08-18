using System;
using System.Threading.Tasks;
using HttpServer.Http;
using HttpServer.WebServer.Http;
using Microsoft.AspNetCore.Routing.Template;

namespace HttpServer.Routes;

public class EndpointHandler
{
    public required HttpMethod Method { get; init; }
    public required TemplateMatcher Matcher { get; init; }
    public required Func<HttpContext, Task<Response>> Handler { get; init; }
}
