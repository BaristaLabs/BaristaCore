namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using BaristaLabs.BaristaCore;
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

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
                        JavaScriptValue result = ctx.EvaluateModule(script);

                        Assert.IsType<JavaScriptTypedArray>(result);

                        var typedArray = result as JavaScriptTypedArray;
                        Assert.Equal(JavaScriptTypedArrayType.Int16, typedArray.ArrayType);
                    }
                }
            }

        }
    }
}
