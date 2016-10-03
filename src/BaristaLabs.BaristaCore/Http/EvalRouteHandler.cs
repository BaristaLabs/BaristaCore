namespace BaristaLabs.BaristaCore.Http
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class EvalRouteHandler : IRouteHandler
    {
        public RequestDelegate GetRequestHandler(HttpContext httpContext, RouteData routeData)
        {
            throw new NotImplementedException();
        }
    }
}
