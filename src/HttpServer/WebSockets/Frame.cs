using System;
using System.Linq;
using HttpServer.WebSockets.Helpers;

namespace HttpServer.WebSockets;

public class Frame
{
    public Header Header { get; }

    public byte[] Payload { get; }

    public Frame(ReadOnlySpan<byte> bytes)
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
}
