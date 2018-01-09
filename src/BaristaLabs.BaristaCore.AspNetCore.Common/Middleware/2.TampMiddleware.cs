namespace BaristaLabs.BaristaCore.AspNetCore.Middleware
{
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.TypeScript;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class TampMiddleware
    {
        private readonly RequestDelegate m_next;

        public TampMiddleware(RequestDelegate next)
        {
            m_next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var brewOrder = context.Items[BrewKeys.BrewOrderKey] as BrewOrder;
            if (brewOrder == null)
                throw new InvalidOperationException("BrewOrder was not defined within the http context.");

            var baristaModuleLoader = Invoke(brewOrder, context.Request);
            context.Items[BrewKeys.BrewModuleLoader] = baristaModuleLoader;

            await m_next(context);

            if (baristaModuleLoader is IDisposable disposableModuleLoader)
            {
                disposableModuleLoader.Dispose();
            }

            context.Items.Remove(BrewKeys.BrewModuleLoader);
        }

        public static IBaristaModuleLoader Invoke(BrewOrder brewOrder, HttpRequest req)
        {
            //Set up the module loader for the request.
            var moduleLoader = new PrioritizedAggregateModuleLoader();

            //Register all the modules within the BaristaLabs.BaristaCore.Extensions assembly.
            moduleLoader.RegisterModuleLoader(new AssemblyModuleLoader(typeof(TypeScriptTranspiler).Assembly), 1);

            //Register modules needing context.
            var currentContextModule = new BaristaContextModule(req);

            var contextModuleLoader = new InMemoryModuleLoader();
            contextModuleLoader.RegisterModule(currentContextModule);

            moduleLoader.RegisterModuleLoader(contextModuleLoader, 2);

            //Register the web resource module loader rooted at the target path.
            var path = Path.Combine(brewOrder.BaseUrl, brewOrder.Path);
            var fileName = Path.GetFileName(path);
            if (Uri.TryCreate(path, UriKind.Absolute, out Uri targetUri))
            {
                var targetPath = targetUri.GetLeftPart(UriPartial.Authority) + String.Join("", targetUri.Segments.Take(targetUri.Segments.Length - 1));
                var webResourceModuleLoader = new WebResourceModuleLoader(targetPath);
                moduleLoader.RegisterModuleLoader(webResourceModuleLoader, 100);
            }

            return moduleLoader;
        }
    }
}
