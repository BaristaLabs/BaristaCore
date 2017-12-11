namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaLabs.BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class BaristaValueFactory_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public BaristaValueFactory_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            m_provider = ServiceCollection.BuildServiceProvider();
        }

        public IBaristaRuntimeFactory BaristaRuntimeFactory
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeFactory>(); }
        }

        [Fact]
        public void JavaScriptContextCanCreateString()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var jsString = ctx.ValueFactory.CreateString("Hello, world!");
                        Assert.NotNull(jsString);
                        jsString.Dispose();
                    }
                }
            }
        }
    }
}
