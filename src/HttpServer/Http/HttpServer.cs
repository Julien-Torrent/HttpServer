using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using HttpServer.Http.Tls;
using HttpServer.WebServer;
using HttpServer.WebServer.Http;

namespace HttpServer.Http;

public class HttpServer : IServer
{
    private readonly Func<HttpContext, CancellationToken, Task> _handler;

    private readonly TcpListener _listener;

    [MemberNotNullWhen(true, nameof(TlsCertificate))]
    private bool EnableHttps { get; set; }
    
    public X509Certificate2? TlsCertificate { get; set; }

    public HttpServer(int port, Func<HttpContext, CancellationToken, Task> handler)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _handler = handler;
    }

    public HttpServer WithHttps(string path, ReadOnlySpan<char> password)
    {
        EnableHttps = true;
        TlsCertificate = X509CertificateLoader.LoadPkcs12FromFile(path, password);

        return this;
    }

    public Task StartAsync(CancellationToken ct = default)
    {
        _listener.Start();
        return Task.CompletedTask;
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        await StartAsync(ct);

        while (!ct.IsCancellationRequested)
        {
            var client = await _listener.AcceptTcpClientAsync(ct);

            _ = Task.Run(() => HandleClientAsync(client, ct), ct);
        }

        await StopAsync(ct);
    }

    public Task StopAsync(CancellationToken ct = default)
    {
        _listener.Stop();
        return Task.CompletedTask;
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken ct)
    {
        try
        {
            Stream stream = client.GetStream();

            if(EnableHttps)
            {
                stream = await stream.CreateSslStream(TlsCertificate);
            }

            using var reader = new StreamReader(stream);

            var requestLine = await reader.ReadLineAsync(ct);
            if (!HttpRequestParser.TryParseRequestLine(requestLine, out var request))
            {
                return;
            }

            var headers = await HttpRequestParser.ReadHeaders(reader);

            var body = Array.Empty<byte>();
            if (HttpRequestParser.TryGetBodySize(headers, out var length))
            {
                body = await HttpRequestParser.ReadBodyAsync(reader, length, ct);
            }

            var context = new HttpContext()
            {
                LocalAddress = (IPEndPoint?)client.Client.LocalEndPoint,
                RemoteAddress = (IPEndPoint?)client.Client.RemoteEndPoint,

                Version = request.Version,
                Connection = stream,
                IsSecureConnection = EnableHttps,

                Request = new Request()
                {
                    Method = request.Method,
                    Path = request.Path,
                    Headers = headers,
                    Body = body,
                },
            };

            await _handler(context, ct);
        }
        catch(Exception ex)
        {
            // Unrecoverable or invalid protocol
            Console.WriteLine(ex);
        }
        finally
        {
            client.Close();
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _listener.Dispose();
    }
}
