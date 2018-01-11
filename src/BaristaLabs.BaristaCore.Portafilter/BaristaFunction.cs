namespace BaristaLabs.BaristaCore.Portafilter
{
    using BaristaLabs.BaristaCore.AspNetCore;
    using BaristaLabs.BaristaCore.Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class BaristaFunction
    {
        private static readonly IBaristaPipeline s_pipeline;
        private static readonly IBaristaRuntimeFactory s_baristaRuntimeFactory;

        static BaristaFunction()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore();

            var provider = serviceCollection.BuildServiceProvider();

            s_baristaRuntimeFactory = provider.GetRequiredService<IBaristaRuntimeFactory>();
            s_pipeline = new BaristaPipeline();
        }

        [FunctionName("BaristaFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "patch", "put", "delete", "options", "head", "brew", Route = "{*path}")]HttpRequest req, string path, TraceWriter log)
        {
            log.Info("Barista Function processed a request.");

            var brewOrder = await s_pipeline.TakeOrder(path, req);

            if (brewOrder.IsCodeSet == false)
            {
                //Return an exception.
            }

            var moduleLoader = s_pipeline.Tamp(brewOrder, req);

            try
            {
                return s_pipeline.Brew(brewOrder, s_baristaRuntimeFactory, moduleLoader);
            }
            finally
            {
                if (moduleLoader is IDisposable disposableLoader)
                {
                    disposableLoader.Dispose();
                }
            }
        }
    }
}
