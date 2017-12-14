namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaLabs.BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Xunit;

    [ExcludeFromCodeCoverage]
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
        public void ValueFactoryCanCreateString()
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

        [Fact]
        public void ValueFactoryCanCreateAPromiseFromATask()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var iRan = false;
                        var myTask = new Task<string>(() =>
                        {
                            Task.Delay(1000);
                            iRan = true;
                            return "foo";
                        });
                       

                        var jsPromise = ctx.ValueFactory.CreatePromise(myTask);
                        Assert.NotNull(jsPromise);
                        Assert.True(iRan);
                    }
                }
            }
        }
    }
}
