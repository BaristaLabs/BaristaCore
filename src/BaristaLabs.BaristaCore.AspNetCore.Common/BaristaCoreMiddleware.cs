namespace BaristaLabs.BaristaCore.AspNetCore
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Threading.Tasks;

    public class BaristaCoreMiddleware
    {
        private readonly RequestDelegate m_next;
        private readonly IBaristaRuntimeFactory m_baristaRuntimeFactory;
        private readonly IBaristaPipeline m_pipeline;

        public BaristaCoreMiddleware(RequestDelegate next, IBaristaRuntimeFactory baristaRuntimeFactory, IBaristaPipeline pipeline)
        {
            m_next = next;
            m_baristaRuntimeFactory = baristaRuntimeFactory;
            m_pipeline = pipeline;
        }

        public async Task Invoke(HttpContext context)
        {
            var brewOrder = await m_pipeline.TakeOrder("", context.Request);

            if (brewOrder.IsCodeSet == false)
            {
                //Return an exception.
            }

            var moduleLoader = m_pipeline.Tamp(brewOrder, context.Request);

            try
            {
                var responseMessage = m_pipeline.Brew(brewOrder, m_baristaRuntimeFactory, moduleLoader);
            }
            finally
            {
                if (moduleLoader is IDisposable disposableLoader)
                {
                    disposableLoader.Dispose();
                }
            }

            await m_next(context);
        }
    }
}
