using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using HttpServer.Http;
using HttpServer.WebServer.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace HttpServer.Routes;

public class RouterBuilder
{
    private readonly Dictionary<HttpMethod, List<EndpointHandler>> _routes = new()
    {
        { HttpMethod.GET, [] },
        { HttpMethod.POST, [] },
        { HttpMethod.PATCH, [] },
        { HttpMethod.DELETE, [] },
        { HttpMethod.OPTIONS, [] },
        { HttpMethod.HEAD, [] },
        { HttpMethod.CONNECT, [] },
        { HttpMethod.TRACE, [] }
    };

    public RouterBuilder MapGet([StringSyntax("Route")] string endpoint, Func<HttpContext, Task<Response>> func)
    {
        return Map(HttpMethod.GET, endpoint, func);
    }

    public RouterBuilder MapPatch([StringSyntax("Route")] string endpoint, Func<HttpContext, Task<Response>> func)
    {
        return Map(HttpMethod.PATCH, endpoint, func);
    }

    public RouterBuilder MapPost([StringSyntax("Route")] string endpoint, Func<HttpContext, Task<Response>> func)
    {
        return Map(HttpMethod.POST, endpoint, func);
    }

    public RouterBuilder MapPut([StringSyntax("Route")] string endpoint, Func<HttpContext, Task<Response>> func)
    {
        return Map(HttpMethod.PUT, endpoint, func);
    }

    public RouterBuilder MapDelete([StringSyntax("Route")] string endpoint, Func<HttpContext, Task<Response>> func)
    {
        return Map(HttpMethod.DELETE, endpoint, func);
    }

    public RouterBuilder Map(HttpMethod method, [StringSyntax("Route")] string endpoint, Func<HttpContext, Task<Response>> func)
    {
        ArgumentNullException.ThrowIfNull(endpoint);

        var template = TemplateParser.Parse(endpoint);
        var defaults = template.Parameters.Where(x => x.DefaultValue != null).Select(x => KeyValuePair.Create(x.Name ?? string.Empty, x.DefaultValue));

        if (_routes.TryGetValue(method, out var list))
        {
            list.Add(new EndpointHandler()
            {
                Method = method,
                Matcher = new TemplateMatcher(template, new RouteValueDictionary(defaults)),
                Handler = func
            });
        }

        return this;
    }

    public Router Build()
    {
        return new Router(_routes);
    }
}
