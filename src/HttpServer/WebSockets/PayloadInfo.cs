using System;
using System.Buffers.Binary;

namespace HttpServer.WebSockets;

public readonly struct PayloadInfo
{
    public bool IsMasked { get; }
    public int MaskOffset { get; }
    public int PayloadStart => MaskOffset + 4;
    public long Length { get; }

    public PayloadInfo(ReadOnlySpan<byte> value)
    {
        IsMasked = (value[1] & 0b10000000) != 0;

        byte lengthByte = (byte)(value[1] & 0b01111111);
        long length = lengthByte;
        var maskOffset = 2;

        if(length == 126)
        {
            length = BinaryPrimitives.ReadInt32BigEndian([0, 0, ..value[2..4]]);
            maskOffset = 4;
        }
        else if(length == 127)
        {
            length = BinaryPrimitives.ReadInt64BigEndian([0, 0, ..value[2..9]]);
            maskOffset = 10;
        }

        MaskOffset = maskOffset;
        Length = length;
    }
}
