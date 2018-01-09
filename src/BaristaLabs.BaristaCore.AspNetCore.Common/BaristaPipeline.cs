namespace BaristaLabs.BaristaCore.AspNetCore
{
    using BaristaLabs.BaristaCore.AspNetCore.Middleware;
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.TypeScript;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class BaristaPipeline : IBaristaPipeline
    {
        public async Task<BrewOrder> TakeOrder(string path, HttpRequest req)
        {
            return await TakeOrderMiddleware.Invoke(path, req);
        }

        public IBaristaModuleLoader Tamp(BrewOrder brewOrder, HttpRequest req)
        {
            return TampMiddleware.Invoke(brewOrder, req);
        }

        public HttpResponseMessage Brew(BrewOrder brewOrder, IBaristaRuntimeFactory baristaRuntimeFactory, IBaristaModuleLoader moduleLoader)
        {
            if (brewOrder == null)
                throw new ArgumentNullException(nameof(brewOrder));

            if (baristaRuntimeFactory == null)
                throw new ArgumentNullException(nameof(baristaRuntimeFactory));

            //Brew:
            HttpResponseMessage responseMessage = null;
            BrewMiddleware.Invoke(brewOrder, baristaRuntimeFactory, moduleLoader, (ctx, brewResult) =>
            {
                responseMessage = Serve(ctx, brewResult);
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

            return responseMessage;
        }

        public HttpResponseMessage Serve(BaristaContext ctx, JsValue result)
        {
            return ServeMiddleware.Invoke(ctx, result);
        }
    }
}
