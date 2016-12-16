namespace BaristaLabs.BaristaCore.JavaScript
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class JavaScriptValue_Facts
    {
        private IServiceProvider Provider;

        public JavaScriptValue_Facts()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore();

            Provider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void CanRepresentTypedArray()
        {
            var script = @"
var int16 = new Int16Array(2);
int16[0] = 42;

int16;
";
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fn = ctx.ParseScriptText(script);

                        //Invoke it.
                        JavaScriptValue result = fn.Invoke();
                        Assert.IsType<JavaScriptTypedArray>(result);

                        var typedArray = result as JavaScriptTypedArray;
                        Assert.Equal(JavaScriptTypedArrayType.Int16, typedArray.ArrayType);
                    }
                }
            }

        }
    }
}
