using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using HttpServer.WebSockets;
using Microsoft.AspNetCore.Routing;

namespace HttpServer.WebServer.Http;

public class HttpContext
{
    public required IPEndPoint? LocalAddress { get; init; }
    public required IPEndPoint? RemoteAddress { get; init; }
    public required Stream Connection { get; init;}

    public required HttpServer.Http.HttpVersion Version { get; init; }
    public required bool IsSecureConnection { get; init; }
    public bool IsWebSocket => this.CheckWebSockets();

    public required Request Request { get; init; }
    public Response? Response { get; set; }

    [MemberNotNullWhen(true, nameof(HttpEndpoint))]
    public bool IsMatchedRoute { get; set; }
    public Func<HttpContext, Task<Response>>? HttpEndpoint { get; set;}
    public RouteValueDictionary Parameters { get; set; } = [];
} 
