namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class JsObject_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public JsObject_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            m_provider = ServiceCollection.BuildServiceProvider();
        }

        public IBaristaRuntimeFactory BaristaRuntimeFactory
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeFactory>(); }
        }

        public IBaristaValueFactory ValueFactory
        {
            get { return m_provider.GetRequiredService<IBaristaValueFactory>(); }
        }

        [Fact]
        public void JsObjectCanBeCreated()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var obj = ctx.ValueFactory.CreateObject();
                        Assert.True(obj != null);
                    }
                }
            }
        }

        [Fact]
        public void JsObjectKeysCanBeRetrieved()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var keys = result.Keys;
                        Assert.True(keys != null);
                        Assert.Equal(1, keys.Length);
                        Assert.Equal("foo", keys[0].ToString());
                    }
                }
            }
        }
    }
}
