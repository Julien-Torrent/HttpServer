namespace HttpServer.WebSockets.Helpers;

public readonly struct Header
{
    private readonly Fields _value;

    public Header(Fields value)
    {
        _value = value;
    }

    public bool Fin => _value.HasFlag(Fields.Fin);
    public bool Rsv1 => _value.HasFlag(Fields.RSV1);
    public bool Rsv2 => _value.HasFlag(Fields.RSV2);
    public bool Rsv3 => _value.HasFlag(Fields.RSV3);
    public int OpCode => (int)(_value & Fields.OPCODE);
}
