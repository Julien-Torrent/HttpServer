using System;
using System.Buffers;
using System.IO;
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

        return new Frame(buffer.AsSpan(0..read));
    }
}
