using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HttpServer.Http;
using Microsoft.AspNetCore.Routing;

namespace HttpServer.Routes;

public class Router
{
    private readonly FrozenDictionary<HttpMethod, List<EndpointHandler>> _routes;

    public Router(IEnumerable<KeyValuePair<HttpMethod, List<EndpointHandler>>> routes)
    {
        _routes = routes.ToFrozenDictionary();
    }

    public bool TryGetHandler(HttpMethod method, string path, [NotNullWhen(true)] out MatchedEndpoint? handler)
    {
        if (!_routes.TryGetValue(method, out var endpoints))
        {
            handler = null;
            return false;
        }

        foreach (var endpoint in endpoints)
        {
            var dict = new RouteValueDictionary();
            if (endpoint.Matcher.TryMatch(path, dict))
            {
                handler = new MatchedEndpoint()
                {
                    Handler = endpoint.Handler,
                    Parameters = dict
                };
                return true;
            }
        }

        handler = null;
        return false;
    }
}
