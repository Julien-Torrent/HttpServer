using System;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace HttpServer.Http.Tls;

public static class TlsHelpers
{
    public static async Task<SslStream> CreateSslStream(this Stream stream, X509Certificate2 certificate)
    {
        var ssl = new SslStream(stream, false, (object _, X509Certificate? _, X509Chain? _, SslPolicyErrors _) => true);

        await ssl.AuthenticateAsServerAsync(
                certificate,
                false,
                SslProtocols.Tls12,
                false).ConfigureAwait(false);

        if (!ssl.IsAuthenticated)
        {
            throw new InvalidOperationException("TLS connection is incorrect");
        }

        return ssl;
    }
}
