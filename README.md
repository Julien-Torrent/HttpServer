# Http Server From Scratch in C#

> [!WARNING]  
> This project shall not be used in production, it lacks reliability and safety features,
> This project is intended as an experiment on how to build a web server.

This repo contains the resulting code of a self learing session. 
The goal was to learn how to build a web server form scratch.

The server has the features of a modern web server (ASP.NET Core Minimal API):

- HTTP and HTTPS
- Middlewares
- User defined endpoints
- Convenience functions to return HTTP responses

## How it works?

The `HttpServer.Http` handles the incoming socket connections (HTTP/HTTPS) and parses the HTTP request.
The request is the passed to the given delegate.

The `HttpServer.Middlewares` contains the infrastructure to add middlewares and run the middleware pipelien for incoming request.

The `HttpServer.Routes` contains the infrastructure to send the request to the right endpoint. 
The matching between the request path and the template is done through the `Microsoft.AspNetCore.Routing.Template` namespace.
The execution of the endpoint handler is done via a middleware.

The `HttpServer.WebServer` is the main part of the repo. 
This combines the different `HttpServer` that have been regisered.
This adds the `ErrorHandlerMiddleware`, `RouterMiddleware` and user registered ones.
This then writes the response returned by the endpoint or middleware to the connection.

## Build & Run

```bash
cd src/
dotnet build

cd HttpServer/
dotnet run
```

# Using The Web Server

1. Acquire a `WebServerBuilder`
2. Configure the builder with endpoints and middlewares
3. Build the `WebServer`
4. Run the `WebServer`

```csharp
var builder = new WebServerBuilder();

// Configure the web server
... 

// Run the server
using var server = builder.Build();
await server.RunAsync();
```

## Configure Ports

The webserver ports need to be explicitly configured. The server supports HTTP and HTTPS running at the same time.

The ports and certificate are configured in the following way:

```csharp
// Listen on port 80 for HTTP requests
builder.UseHttp(80);

// Listen on port 443 for HTTPS requests
builder.UseHttps(443, "certificate.pfx", "password");
```

### Generate a certificate for HTTPS

To use HTTPS, you need to create a certificate. Make sure that the `Common Name (CN)` is set to `localhost`.

Use the following commands, and fill the required information:

```bash
openssl req -new -newkey rsa:4096 -x509 -sha256 -days 365 -out certificate.crt -keyout certificate.key

openssl pkcs12 -export -out certificate.pfx -inkey certificate.key -in certificate.crt
```

## Endpoints

You can register endpoints using the predefined methods (`MapGet`, `MapPost`, ...). 

It is also possible to register other HTTP methods with the `Map` function e.g. `Map(HttpMethod.OPTIONS, ...)`.

> [!NOTE]
> Uses the ASP.NET Core url syntax and matching

```csharp
// Add a endpoint for GET /my-endpoint/my-id
builder.Router.MapGet("/my-endpoint/{id:string}", async (context) => {
    ...
    // Needs to return a response
    // if the handler returns null, the status code will be 204 (No Content)
    return Response.Json(HttpStatusCode.OK, context.Request.Headers);
});
```

## Middlewares

The web server support middlewares, the middlewares can return a response and interrupt the request pipeline.

The web server supports middlewares, in the form of lambdas and classes.

> [!NOTE]
> Middlewares are run in the order they are registered

```csharp
// Use a functionnal middlware
builder.Middlewares.AddMiddleware(async (ctx, next) =>
{
    ...
});

// Or use a class that implements IMiddlware
builder.Middlewares.AddMiddleware(new MyMiddleware());
```
