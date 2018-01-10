namespace BaristaLabs.BaristaCore.AspNetCore.Middleware
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Pulls the shot of espresso. E.g. Invokes the JavaScript runtime to execute the BrewOrder that returns a JsValue.
    /// </summary>
    /// <param name="brewOrder"></param>
    /// <param name="baristaRuntimeFactory"></param>
    /// <param name="moduleLoader"></param>
    /// <returns></returns>
    public class BrewMiddleware
    {
        private readonly RequestDelegate m_next;
        private readonly IBaristaRuntimeFactory m_baristaRuntimeFactory;

        public BrewMiddleware(RequestDelegate next, IBaristaRuntimeFactory baristaRuntimeFactory)
        {
            m_next = next;
            m_baristaRuntimeFactory = baristaRuntimeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var brewOrder = context.Items[BrewKeys.BrewOrderKey] as BrewOrder;
            if (brewOrder == null)
                throw new InvalidOperationException("BrewOrder was not defined within the http context.");

            var moduleLoader = context.Items[BrewKeys.BrewModuleLoader] as IBaristaModuleLoader;
            if (moduleLoader == null)
                throw new InvalidOperationException("BrewModuleLoader was not defined within the http context.");

            await Invoke(brewOrder, m_baristaRuntimeFactory, moduleLoader, async (ctx, brewResult) =>
            {
                context.Items[BrewKeys.BrewContext] = ctx;
                context.Items[BrewKeys.BrewResult] = brewResult;
                
                await m_next(context);

                context.Items.Remove(BrewKeys.BrewResult);
            });
        }

        public static async Task Invoke(BrewOrder brewOrder, IBaristaRuntimeFactory baristaRuntimeFactory, IBaristaModuleLoader moduleLoader, Func<BaristaContext, JsValue, Task> processBrewResult)
        {
            //Brew:
            using (var rt = baristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            using (var scope = ctx.Scope())
            {
                JsValue result = null;
                switch (brewOrder.Language)
                {
                    case ResourceKind.Tsx:
                    case ResourceKind.Jsx:
                        result = ctx.EvaluateTypeScriptModule(brewOrder.Code, moduleLoader, true);
                        break;
                    case ResourceKind.TypeScript:
                        result = ctx.EvaluateTypeScriptModule(brewOrder.Code, moduleLoader);
                        break;
                    case ResourceKind.Json:
                        result = ctx.JSON.Parse(ctx.CreateString(brewOrder.Code));
                        break;
                    case ResourceKind.JavaScript:
                        result = ctx.EvaluateModule(brewOrder.Code, moduleLoader);
                        break;
                }

                await processBrewResult?.Invoke(ctx, result);
            }
        }
    }
}
