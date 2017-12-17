namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaLabs.BaristaCore;
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class JavaScriptValue_Facts
    {
        private IServiceProvider m_provider;

        public JavaScriptValue_Facts()
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
        public void CanGetBoolean()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsValue zip;
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: true, bar: false, baz: 'true', qix: 'false', zip: 1 };";
                        var result = ctx.EvaluateModule<JsObject>(script);
                        var value = result["foo"];

                        Assert.True(result["foo"].ToBoolean());
                        Assert.False(result["bar"].ToBoolean());
                        Assert.True(result["baz"].ToBoolean());
                        Assert.True(result["qix"].ToBoolean());

                        zip = result["zip"];
                    }

                    Assert.True(zip.ToBoolean());
                }
            }
        }

        [Fact]
        public void CanRepresentTypedArray()
        {
            var script = @"
var int16 = new Int16Array(2);
int16[0] = 42;

export default int16;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        JsValue result = ctx.EvaluateModule(script);

                        Assert.IsType<JsTypedArray>(result);

                        var typedArray = result as JsTypedArray;
                        Assert.Equal(JavaScriptTypedArrayType.Int16, typedArray.ArrayType);
                    }
                }
            }

        }
    }
}
