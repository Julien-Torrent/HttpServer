using System;

namespace HttpServer.WebSockets.Helpers;

[Flags]
public enum Fields
{
    Fin = 128,
    RSV1 = 64,
    RSV2 = 32,
    RSV3 = 16,
    OPCODE = 15,
}
