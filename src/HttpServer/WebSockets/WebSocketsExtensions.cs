using System;
using System.Threading.Tasks;
using HttpServer.Http;
using HttpServer.WebServer.Http;

namespace HttpServer.WebSockets;

public static class WebsocketsExtensions
{
    public static bool CheckWebSockets(this HttpContext ctx)
    {
        var isGet = ctx.Request.Method == HttpMethod.GET;

        var version = ctx.Version;
        var isValidVersion = (version.Major > 1) || (version.Major == 1 && version.Minor == 1);

        var hasWsUpgrade = ctx.Request.Headers.TryGetValue("upgrade", out var up) && up == "websocket";
        var hasConnUpgrade = ctx.Request.Headers.TryGetValue("connection", out var conn) && conn == "Upgrade";
        var hasKey = ctx.Request.Headers.ContainsKey(Constants.KeyHeader);

        return isGet && isValidVersion && hasWsUpgrade && hasConnUpgrade && hasKey;
    }


    public static async Task<WebSocket> AcceptWebSocket(this HttpContext ctx)
    {
        if(!ctx.Request.Headers.TryGetValue(Constants.KeyHeader, out var key) || !ctx.IsWebSocket)
        {
            throw new InvalidOperationException("Trying to accept invalid websocket");
        }

        ctx.Response = Response.SwtichingProtocol(key);

        await ctx.Connection.WriteHttpResponse(ctx);

        return new WebSocket(ctx.Connection);
    }
}
