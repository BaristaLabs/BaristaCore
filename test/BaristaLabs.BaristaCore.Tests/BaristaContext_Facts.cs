namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.JavaScript.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class BaristaContext_Facts
    {
        private IServiceProvider m_provider;

        public BaristaContext_Facts()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore();

            m_provider = serviceCollection.BuildServiceProvider();
        }

        public IBaristaRuntimeService BaristaRuntimeService
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeService>(); }
        }

        public InMemoryModuleService ModuleService
        {
            get { return m_provider.GetRequiredService<IBaristaModuleService>() as InMemoryModuleService; }
        }

        [Fact]
        public void JavaScriptContextCanBeCreated()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
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
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                var ctx1 = rt.CreateContext();
                var ctx2 = rt.CreateContext();

                Assert.NotEqual(ctx1, ctx2);
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetFalse()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
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
            using (var rt = BaristaRuntimeService.CreateRuntime())
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
            using (var rt = BaristaRuntimeService.CreateRuntime())
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
            using (var rt = BaristaRuntimeService.CreateRuntime())
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
        public void JavaScriptContextShouldGetGlobalObject()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.GlobalObject);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetJSONBuiltIn()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.JSON);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptContextsShouldHaveUniqueUndefineds()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                var ctx1 = rt.CreateContext();
                var ctx2 = rt.CreateContext();

                try
                {
                    JsValue undefined1;
                    JsValue undefined2;

                    using (ctx1.Scope())
                    {
                        undefined1 = ctx1.Undefined;
                    }

                    using (ctx2.Scope())
                    {
                        undefined2 = ctx2.Undefined;
                    }

                    Assert.NotStrictEqual(undefined1, undefined2);
                }
                finally
                {
                    ctx1.Dispose();
                    ctx2.Dispose();
                }

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
        public void JsModuleCanBeEvaluated()
        {
            var script = @"
export default 'hello, world!';
";
            using (var rt = BaristaRuntimeService.CreateRuntime())
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
        public void JsModulesContainingPromisesCanBeEvaluated()
        {
            var script = @"
var result = new Promise((resolve, reject) => {
        resolve('hello, world!');
    });
export default result;
";
            using (var rt = BaristaRuntimeService.CreateRuntime(JavaScriptRuntimeAttributes.None))
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

            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fooObj = ctx.EvaluateModule(script) as JsObject;
                        Assert.NotNull(fooObj);

                        dynamic fooValue = fooObj.GetPropertyByName<JsString>("foo");
                        Assert.Equal("bar", (string)fooValue);
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

            using (var rt = BaristaRuntimeService.CreateRuntime())
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
            var myReverseModule = new ReverseModule();
            ModuleService.RegisterModule(myReverseModule);

            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var result = ctx.EvaluateModule(script);

                    Assert.Equal("!dlrow ,olleh", result.ToString());
                }
            }

            myReverseModule.Dispose();
        }

        private sealed class ReverseModule : IBaristaModule, IDisposable
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

            #region IDisposable Support
            private bool m_isDisposed = false;

            private void Dispose(bool disposing)
            {
                if (!m_isDisposed)
                {
                    if (disposing)
                    {
                    }

                    m_isDisposed = true;
                }
            }

             ~ReverseModule()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            #endregion
        }
    }
}
