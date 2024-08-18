using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.WebServer.Http;

public static class ResponseWriterExtensions
{
    public static async Task WriteHttpResponse(this Stream stream, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(context.Response);

        var responseLine = $"{context.Version} {(int)context.Response.StatusCode}\r\n";
        await stream.WriteAsync(Encoding.ASCII.GetBytes(responseLine));

        foreach (var (k, v) in context.Response.Headers)
        {
            var header = $"{k}: {v}\r\n";
            await stream.WriteAsync(Encoding.ASCII.GetBytes(header));
        }

        await stream.WriteAsync(Encoding.ASCII.GetBytes("\r\n"));
        await stream.WriteAsync(Encoding.UTF8.GetBytes(context.Response.Body));
    }
}
