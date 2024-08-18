using System;
using System.Threading.Tasks;
using HttpServer.WebServer.Http;
using Microsoft.AspNetCore.Routing;

namespace HttpServer.Routes;

public class MatchedEndpoint
{
    public required Func<HttpContext, Task<Response>> Handler { get; init; }
    public required RouteValueDictionary Parameters { get; init; }
}
