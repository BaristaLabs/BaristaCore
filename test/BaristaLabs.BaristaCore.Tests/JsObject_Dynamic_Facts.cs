namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class JsObject_Dynamic_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public JsObject_Dynamic_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            m_provider = ServiceCollection.BuildServiceProvider();
        }

        public IBaristaRuntimeService BaristaRuntimeService
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeService>(); }
        }

        [Fact]
        public void JsValueConvertsToAnIntDirectly()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
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
        public void JsValueConvertsToAnIntWithCoersion()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
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
        public void JsValueConvertsToADoubleDirectly()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
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
        public void JsValueConvertsToADoubleWithCoersion()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
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

        [Fact]
        public void JsObjectCanGetDynamicProperty()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar' }";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.Equal("bar", (string)result.foo);
                    }
                }
            }
        }

        [Fact]
        public void JsObjectCanSetDynamicProperty()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar' }";
                        dynamic result = ctx.EvaluateModule(script);

                        result.baz = "qix";

                        Assert.Equal("qix", (string)result.baz);
                    }
                }
            }
        }

        [Fact]
        public void JsObjectCanInvokeDynamicProperty()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar', baz: () => 'qix' }";
                        dynamic result = ctx.EvaluateModule(script);

                        var qix = (string)result.baz();

                        Assert.Equal("qix", qix);
                    }
                }
            }
        }

        [Fact]
        public void JsArrayCanGetDynamicProperty()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default [ 1, 3, 5 ]";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.Equal(3, (int)result[1]);
                    }
                }
            }
        }
    }
}
