namespace BaristaLabs.BaristaCore
{
    using System;
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;

    public class BaristaValueServiceFactory : IBaristaValueServiceFactory
    {
        private readonly IServiceProvider m_serviceProvider;

        public BaristaValueServiceFactory(IServiceProvider provider)
        {
            m_serviceProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IBaristaValueService CreateValueService(BaristaContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var jsEngine = m_serviceProvider.GetRequiredService<IJavaScriptEngine>();
            return new BaristaValueService(jsEngine, context);
        }
    }
}
