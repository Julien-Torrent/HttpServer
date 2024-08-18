namespace HttpServer.Http;

public readonly struct ParsedRequest
{
    public required HttpVersion Version { get; init; }

    public required HttpMethod Method { get; init; }

    public required string Path { get; init; }
}
