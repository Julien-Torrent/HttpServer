using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace HttpServer.WebServer.Http;

public class Response
{
    public required HttpStatusCode StatusCode { get; set; }
    public required Dictionary<string, string> Headers { get; init; }
    public string Body { get; init; } = string.Empty;

    public static Response Json<T>(HttpStatusCode statusCode, T body)
    {
        var stringBody = JsonSerializer.Serialize(body, JsonSerializerOptions.Web);

        return new Response()
        {
            StatusCode = statusCode,
            Headers = new Dictionary<string, string>()
            {
                { "Content-Type", "application/json" },
                { "Content-Length", stringBody.Length.ToString() }
            },
            Body = stringBody
        };
    }

    public static Response Html(HttpStatusCode statusCode, string body)
    {
        return new Response()
        {
            StatusCode = statusCode,
            Headers = new Dictionary<string, string>()
            {
                { "Content-Type", "text/html" },
                { "Content-Length", body.Length.ToString() }
            },
            Body = body
        };
    }

    public static Response NoContent()
    {
        return new Response()
        {
            StatusCode = HttpStatusCode.NoContent,
            Headers = [],
        };
    }

    public static Response NotFound<T>(T body)
    {
        var stringBody = JsonSerializer.Serialize(body, JsonSerializerOptions.Web);

        return new Response()
        {
            StatusCode = HttpStatusCode.NotFound,
            Headers = new Dictionary<string, string>()
            {
                { "Content-Type", "application/json" },
                { "Content-Length", stringBody.Length.ToString() }
            },
            Body = stringBody
        };
    }

    public static Response InternalServerError<T>(T body)
    {
        var stringBody = JsonSerializer.Serialize(body, JsonSerializerOptions.Web);

        return new Response()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Headers = new Dictionary<string, string>()
            {
                { "Content-Type", "application/json" },
                { "Content-Length", stringBody.Length.ToString() }
            },
            Body = stringBody
        };
    }
}
