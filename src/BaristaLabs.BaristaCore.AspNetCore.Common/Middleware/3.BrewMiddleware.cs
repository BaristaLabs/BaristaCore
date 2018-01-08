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

        public async Task Invoke(HttpContext context, BrewOrder brewOrder, IBaristaModuleLoader moduleLoader)
        {
            if (brewOrder == null)
                throw new ArgumentNullException(nameof(brewOrder));

            //Brew:
            using (var rt = m_baristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            using (var scope = ctx.Scope())
            {
                JsValue result;
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
                    default:
                        //Just pass-through the response.
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.Body = new MemoryStream(Encoding.UTF8.GetBytes(brewOrder.Code.ToString()));
                        return;
                }

                await m_next(context);
            }
        }
    }
}
