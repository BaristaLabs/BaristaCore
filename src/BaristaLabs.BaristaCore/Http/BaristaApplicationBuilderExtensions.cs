namespace BaristaLabs.BaristaCore.Http
{
    using JavaScript;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;

    public static class BaristaApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBarista(this IApplicationBuilder app)
        {
            var evalRouteHandler = new RouteHandler(context =>
            {
                var jsr = context.RequestServices.GetRequiredService<JavaScriptRuntime>();
                using (var engine = jsr.CreateEngine())
                {
                    using (var jsContext = engine.AcquireContext())
                    {
                        if (context.Request.Body != null && context.Request.Body.CanRead)
                        {
                            //Attempt to locate the code from the body.
                        }
                        var code = context.GetRouteValue("c");

                        if (code == null)
                            code = "1+41";

                        var codeToExecute = code as string;

                        JavaScriptValue jsValue;
                        try
                        {
                            var fn = engine.Evaluate(new ScriptSource("[eval code]", codeToExecute));

                            jsValue = fn.Invoke(Enumerable.Empty<JavaScriptValue>());
                        }
                        catch(Exception)
                        {
                            //Catch the exception but don't do any additional processing.
                            //ExecuteResultAsync should pick the exception up.
                            jsValue = engine.UndefinedValue;
                        }

                        var result = new JavaScriptValueResult(jsValue);
                        return result.ExecuteResultAsync(context);
                    }
                }
            });

            var routeBuilder = new RouteBuilder(app, evalRouteHandler);

            routeBuilder.MapRoute(
                "BaristaCore Eval Route",
                "api/eval/{c?}");

            var routes = routeBuilder.Build();
            app.UseRouter(routes);

            return app;
        }
    }
}
