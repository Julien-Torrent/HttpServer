using System;
using System.Diagnostics.CodeAnalysis;

namespace HttpServer.Http;

public readonly struct HttpVersion : ISpanParsable<HttpVersion>
{
    private const string HttpPrefix = "HTTP/";
    public static readonly HttpVersion Empty = new() { Major = 0, Minor = 0 };

    public required int Major { get; init; }
    public required int Minor { get; init; }
    public bool IsValid => Major == 0 && Minor == 9 || Major == 1 && Minor == 0 || Major == 1 && Minor == 1;

    public static HttpVersion Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(s.Length, 8);

        return s switch
        {
            "HTTP/0.9" => new HttpVersion() { Major = 0, Minor = 9 },
            "HTTP/1.0" => new HttpVersion() { Major = 1, Minor = 0 },
            "HTTP/1.1" => new HttpVersion() { Major = 1, Minor = 1 },
            _ => throw new FormatException("Invalid HTTP Version")
        };
    }

    public static HttpVersion Parse(string s, IFormatProvider? provider)
    {
        ArgumentException.ThrowIfNullOrEmpty(s);
        return Parse(s, provider);
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out HttpVersion result)
    {
        result =  s switch
        {
            "HTTP/0.9" => new HttpVersion() { Major = 0, Minor = 9 },
            "HTTP/1.0" => new HttpVersion() { Major = 1, Minor = 0 },
            "HTTP/1.1" => new HttpVersion() { Major = 1, Minor = 1 },
            _ => Empty
        };

        return result.IsValid;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out HttpVersion result)
    {
        return TryParse(s, provider, out result);
    }

    public override string ToString()
    {
        return $"{HttpPrefix}{Major}.{Minor}";
    }
}
