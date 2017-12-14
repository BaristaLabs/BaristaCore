namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
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

        public IBaristaRuntimeService BaristaRuntimeService
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeService>(); }
        }

        [Fact]
        public void JsObjectCanBeCreated()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var obj = ctx.ValueService.CreateObject();
                        Assert.True(obj != null);
                    }
                }
            }
        }

        [Fact]
        public void JsObjectKeysCanBeRetrieved()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
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
