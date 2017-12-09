namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class JavaScriptObject_Dynamic_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public JavaScriptObject_Dynamic_Facts()
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
        public void JavaScriptValueConvertsToAnIntDirectly()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default 41+1";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.True((int)result == 42);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptValueConvertsToAnIntWithCoersion()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default '42'";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.True((int)result == 42);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptValueConvertsToADoubleDirectly()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default 41.0+1.1";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.True((double)result == 42.1);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptValueConvertsToADoubleWithCoersion()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default '42.1'";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.True((double)result == 42.1);
                    }
                }
            }
        }
    }
}
