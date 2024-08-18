using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer.Http;

public class HttpRequestParser
{
    public const int maxHeaderSize = 8 * 1024; // 8KB

    public static bool TryParseRequestLine(ReadOnlySpan<char> line, [NotNullWhen(true)] out ParsedRequest result)
    {
        Span<Range> indexes = stackalloc Range[3];

        if (line.Split(indexes, ' ') < 3)
        {
            result = new ParsedRequest()
            {
                Version = HttpVersion.Empty,
                Method = HttpMethod.GET,
                Path = string.Empty,
            };
            return false;
        }

        var methodParsed = Enum.TryParse<HttpMethod>(line[indexes[0]], out var method);
        var versionParsed = HttpVersion.TryParse(line[indexes[2]], null, out var version);

        result = new ParsedRequest()
        {
            Version = version,
            Method = method,
            Path = line[indexes[1]].ToString(),
        };
        return methodParsed && versionParsed;
    }

    public static bool TryGetBodySize(Dictionary<string, string> headers, out int bodySize)
    {
        if (headers.TryGetValue("content-length", out var value) && int.TryParse(value, out var length))
        {
            bodySize = length;
            return true;
        }

        bodySize = 0;
        return false;
    }

    public static async Task<byte[]> ReadBodyAsync(StreamReader reader, int length, CancellationToken ct = default)
    {
        var body = new byte[length];
        var buffer = ArrayPool<char>.Shared.Rent(length);

        var read = await reader.ReadAsync(buffer, ct);
        int size = Encoding.UTF8.GetBytes(buffer.AsSpan(0, length), body);

        ArrayPool<char>.Shared.Return(buffer);

        if (read != length || size != length)
        {
            throw new InvalidDataException("invalid content-length");
        }

        return body;
    }

    public static async Task<Dictionary<string, string>> ReadHeaders(StreamReader reader)
    {
        var headers = new Dictionary<string, string>();

        var totalBytesRead = 0;

        var line = await reader.ReadLineAsync();
        while (!string.IsNullOrEmpty(line))
        {
            totalBytesRead += line.Length;
            if(totalBytesRead > maxHeaderSize)
            {
                throw new InvalidOperationException("header too large");
            }

            var keyVal = line.Split(": ");
            if (keyVal.Length == 2)
            {
                headers.Add(keyVal[0].ToLower(), keyVal[1]);
            }

            line = await reader.ReadLineAsync();
        }

        return headers;
    }
}
