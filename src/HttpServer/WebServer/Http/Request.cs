using System.Collections.Generic;
using HttpServer.Http;

namespace HttpServer.WebServer.Http;

public class Request
{
    public required HttpMethod Method { get; init; }
    public required string Path { get; init; }
    public required Dictionary<string, string> Headers { get; init; }

    public byte[] Body { get; init; } = [];
}
