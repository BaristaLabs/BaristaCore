namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class JavaScriptObject_Dynamic_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider Provider;

        public JavaScriptObject_Dynamic_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            Provider = ServiceCollection.BuildServiceProvider();
        }

        [Fact]
        public void JavaScriptValueConvertsToAnIntDirectly()
        {
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
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
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
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
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
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
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
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
