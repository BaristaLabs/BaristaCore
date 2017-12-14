namespace BaristaLabs.BaristaCore
{
    using System;
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;

    public class BaristaValueFactoryBuilder : IBaristaValueFactoryBuilder
    {
        private readonly IServiceProvider m_serviceProvider;

        public BaristaValueFactoryBuilder(IServiceProvider provider)
        {
            m_serviceProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IBaristaValueFactory CreateValueFactory(BaristaContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var jsEngine = m_serviceProvider.GetRequiredService<IJavaScriptEngine>();
            return new BaristaValueFactory(jsEngine, context);
        }
    }
}
