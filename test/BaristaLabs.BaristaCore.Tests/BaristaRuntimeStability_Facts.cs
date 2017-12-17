namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class BaristaRuntimeStability_Facts
    {
        private IServiceProvider m_provider;

        public BaristaRuntimeStability_Facts()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore();

            m_provider = serviceCollection.BuildServiceProvider();
        }

        public IBaristaRuntimeFactory BaristaRuntimeFactory
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeFactory>(); }
        }

        [Fact]
        public void JavaScriptRuntimeCanBeConstructed()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
            }
            Assert.True(true);
        }

        [Fact]
        public void JsValuesDisposedOutsideOfContextsDoNotCrashTheRuntimeOnObjectCallback()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                JsValue myValue;
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        myValue = ctx.ValueFactory.CreateString("Hello, World");
                    }
                }

                rt.CollectGarbage();
            }

            Assert.True(true);
        }

        [Fact]
        public void JsContextsDisposedOutsideOfRuntimeDoNotCrashTheRuntimeOnObjectCallback()
        {
            BaristaContext ctx; 
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                ctx = rt.CreateContext();
            }

            ctx.Dispose();
            Assert.True(true);
        }

        [Fact]
        public void JsRuntimesAreRemovedFromRuntimeFactory()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                
            }

            Assert.Equal(0, BaristaRuntimeFactory.Count);
        }

        [Fact]
        public void LargeNumbersOfRuntimesPropertyEvaluate()
        {
            for (int i = 0; i < 100; i++)
            {
                using (var rt = BaristaRuntimeFactory.CreateRuntime())
                {
                    using (var ctx = rt.CreateContext())
                    {
                        using (ctx.Scope())
                        {
                            var result = ctx.EvaluateModule<JsNumber>("export default 1+1");
                            Assert.Equal(2, result.ToInt32());
                        }
                    }
                }
            }

            Assert.Equal(0, BaristaRuntimeFactory.Count);
        }
    }
}
