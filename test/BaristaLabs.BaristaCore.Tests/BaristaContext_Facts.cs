namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.JavaScript.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public class BaristaContext_Facts
    {
        private IServiceProvider Provider;
        private InMemoryModuleService ModuleService;

        public BaristaContext_Facts()
        {
            ModuleService = new InMemoryModuleService();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore();
            serviceCollection.AddSingleton<IBaristaModuleService>(ModuleService);

            Provider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void JavaScriptContextCanBeCreated()
        {
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {

                }
            }
            Assert.True(true);
        }

        [Fact]
        public void MultipleJavaScriptContextsCanBeCreated()
        {
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
                var ctx1 = rt.CreateContext();
                var ctx2 = rt.CreateContext();

                Assert.NotEqual(ctx1, ctx2);
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetFalse()
        {
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.False);
                        Assert.False(ctx.False);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetNull()
        {
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.Null);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetTrue()
        {
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.True);
                        Assert.True(ctx.True);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetUndefined()
        {
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.Undefined);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptContextCanCreateString()
        {
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var jsString = ctx.CreateString("Hello, world!");
                        Assert.NotNull(jsString);
                        jsString.Dispose();
                    }
                }
            }
        }

        [Fact]
        public void JsModuleCanBeEvaluated()
        {
            var script = @"
export default 'hello, world!';
";
            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var result = ctx.EvaluateModule(script);
                        Assert.True(result.ToString() == "hello, world!");
                    }
                }
            }
        }

        [Fact]
        public void JsPropertyCanBeRetrievedByName()
        {
            var script = @"var fooObj = {
    foo: 'bar',
    baz: 'qix'
};

export default fooObj;";

            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fooObj = ctx.EvaluateModule(script) as JavaScriptObject;
                        Assert.NotNull(fooObj);

                        var fooValue = fooObj.GetPropertyByName<string>("foo");
                        Assert.Equal("bar", fooValue);
                    }
                }
            }
        }

        [Fact]
        public void JsModulesCanBeDynamicallyProvided()
        {
            var script = @"
import banana from 'banana';
export default 'hello, world! ' + banana;
";
            var bananaModule = new BaristaScriptModule
            {
                Name = "banana",
                Script = @"
export default 'banana';
"
            };

            ModuleService.RegisterModule(bananaModule);

            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var result = ctx.EvaluateModule(script);

                        Assert.True(result.ToString() == "hello, world! banana");
                    }
                }
            }
        }

        [Fact]
        public void JsDynamicModulesCanExportNativeObjects()
        {
            var script = @"
        import reverse from 'reverse';
        export default reverse('hello, world!');
        ";
            ModuleService.RegisterModule(new ReverseModule());

            using (var rt = BaristaRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var result = ctx.EvaluateModule(script);

                        Assert.Equal("!dlrow ,olleh", result.ToString());
                    }
                }
            }
        }

        private class ReverseModule : IBaristaModule
        {
            public string Name => "reverse";

            public string Description => "reverses the string passed in.";

            public object InstallModule(BaristaContext context, JavaScriptModuleRecord referencingModule)
            {
                IntPtr fnReverse(IntPtr callee, bool isConstructCall, IntPtr[] arguments, ushort argumentCount, IntPtr callbackData)
                {
                    if (argumentCount < 2)
                    {
                        return context.Undefined.Handle.DangerousGetHandle();
                    }

                    var str = context.Engine.GetStringUtf8(new JavaScriptValueSafeHandle(arguments[1]), true);

                    var charArray = str.ToCharArray();
                    Array.Reverse(charArray);
                    var reversed = new string(charArray);

                    var reversedHandle = context.Engine.JsCreateString(reversed, (ulong)reversed.Length);
                    return reversedHandle.DangerousGetHandle();
                }

                return context.Engine.JsCreateFunction(fnReverse, IntPtr.Zero);
            }
        }
    }
}
