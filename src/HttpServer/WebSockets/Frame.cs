using System;
using System.Buffers.Binary;
using System.Linq;
using HttpServer.WebSockets.Helpers;

namespace HttpServer.WebSockets;

public class Frame
{
    public Header Header { get; }

    public byte[] Payload { get; }

    private Frame(ReadOnlySpan<byte> bytes)
    {
        Header = new Header((Fields)bytes[0]);

        var payloadInfo = new PayloadInfo(bytes);

        Payload = bytes[payloadInfo.PayloadStart..].ToArray();

        if(payloadInfo.IsMasked)
        {
           foreach(var (i, b) in Payload.Index())
           {
                Payload[i] = (byte)(b ^ (bytes[payloadInfo.MaskOffset + (i % 4)]));
           }
        }
    }

    public static Frame Decode(ReadOnlySpan<byte> bytes)
    {
        return new Frame(bytes);
    }

    public static byte[] Encode(byte[] data)
    {
        // flags + length + mask + data
        Span<byte> lengthBytes = stackalloc byte[9];
        var lengthSize = 1;

        if(data.Length < 126)
        {
            lengthBytes[0] = (byte)data.Length;
            lengthSize = 1;
        }
        if(data.Length >= 126)
        { 
            lengthBytes[0] = 126;
            BinaryPrimitives.WriteUInt16BigEndian(lengthBytes[1..], (ushort)data.Length);
            lengthSize = 3;
        }
        if(data.Length > 65535)
        {
            lengthBytes[0] = 127;
            BinaryPrimitives.WriteUInt64BigEndian(lengthBytes[1..], (uint)data.Length);
            lengthSize = 9;
        }

        var payloadStart = 1 + lengthSize;
    
        var result = new byte[1 + lengthSize + data.Length];
        result[0] = 0x81; // We only send data in a single frame
        lengthBytes[0..lengthSize].CopyTo(result.AsSpan(1..));
        data.CopyTo(result.AsSpan(payloadStart..));

        return result;
    }
}
