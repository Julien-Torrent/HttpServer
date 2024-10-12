using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer.WebSockets;

public class WebSocket
{
    private const int _MaxFrameSize = 1024 * 1024; // 1MB

    private readonly Stream _stream;

    public WebSocket(Stream stream)
    {
        _stream = stream;
    }

    public async Task<Frame> GetFrameAsync(CancellationToken ct = default)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(_MaxFrameSize);

        var read = await _stream.ReadAsync(buffer, ct);

        var frame = Frame.Decode(buffer.AsSpan(0..read));

        ArrayPool<byte>.Shared.Return(buffer);
        return frame;
    }

    public async Task WriteAsync(byte[] data)
    {
        await _stream.WriteAsync(Frame.Encode(data));
    }

    public async Task WriteJsonAsync<T>(T body)
    {
        var send = JsonSerializer.Serialize(body);

        await WriteAsync(Encoding.UTF8.GetBytes(send));
    }
}
