namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.JavaScript.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class BaristaModule_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public BaristaModule_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            m_provider = ServiceCollection.BuildServiceProvider();
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
        public void JsDynamicModulesCanExportNativeObjectsThatReturnJsValues()
        {
            var script = @"
        import helloworld from 'hello_world';
        export default helloworld;
        ";
            var myHelloWorldModule = new HelloWorldModule();
            ModuleService.RegisterModule(myHelloWorldModule);

            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var result = ctx.EvaluateModule(script);

                    Assert.Equal("Hello, World!", result.ToString());
                }
            }
        }

        [Fact]
        public void JsDynamicModulesCanExportNativeObjectsThatReturnSafeHandles()
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

        private sealed class HelloWorldModule : IBaristaModule
        {
            public string Name => "hello_world";

            public string Description => "Only the best module ever.";

            public object InstallModule(BaristaContext context, JavaScriptModuleRecord referencingModule)
            {
                return context.ValueService.CreateString("Hello, World!");
            }
        }

        private sealed class ReverseModule : IBaristaModule, IDisposable
        {
            public string Name => "reverse";

            public string Description => "reverses the string passed in.";

            public object InstallModule(BaristaContext context, JavaScriptModuleRecord referencingModule)
            {
                //This module goes through the trouble of creating a JavaScriptValueSafeHandle to ensure that it can be done.
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
