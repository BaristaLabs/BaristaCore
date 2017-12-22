namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.JavaScript.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
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

        public IBaristaRuntimeFactory BaristaRuntimeFactory
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeFactory>(); }
        }

        public InMemoryModuleLoader ModuleLoader
        {
            get { return m_provider.GetRequiredService<IBaristaModuleLoader>() as InMemoryModuleLoader; }
        }

        [Fact]
        public void BaristaModuleThrowsIfNameNotSpecified()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var converter = m_provider.GetRequiredService<IBaristaConversionStrategy>();
                    var moduleRecordFactory = m_provider.GetRequiredService<IBaristaModuleRecordFactory>();
                    var moduleLoader = m_provider.GetRequiredService<IBaristaModuleLoader>();

                    using (ctx.Scope())
                    {
                        var specifier = ctx.ValueFactory.CreateString("");
                        var moduleHandle = rt.Engine.JsInitializeModuleRecord(JavaScriptModuleRecord.Invalid, specifier.Handle);

                        try
                        {
                            Assert.Throws<ArgumentNullException>(() =>
                            {
                                var mod = new BaristaModuleRecord(null, specifier.Handle, null, rt.Engine, ctx, moduleRecordFactory, moduleLoader, moduleHandle);
                            });
                        }
                        finally
                        {
                            //Without disposing of the moduleHandle, the runtime *will* crash the process.
                            moduleHandle.Dispose();
                        }
                    }
                }
            }

            Assert.True(true);
        }

        [Fact]
        public void BaristaModuleThrowsIfContextNotSpecified()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var converter = m_provider.GetRequiredService<IBaristaConversionStrategy>();
                    var moduleRecordFactory = m_provider.GetRequiredService<IBaristaModuleRecordFactory>();
                    var moduleLoader = m_provider.GetRequiredService<IBaristaModuleLoader>();

                    using (ctx.Scope())
                    {
                        var specifier = ctx.ValueFactory.CreateString("");
                        var moduleHandle = rt.Engine.JsInitializeModuleRecord(JavaScriptModuleRecord.Invalid, specifier.Handle);

                        try
                        {
                            Assert.Throws<ArgumentNullException>(() =>
                            {
                                var mod = new BaristaModuleRecord("", null, null, rt.Engine, null, moduleRecordFactory, moduleLoader, moduleHandle);
                            });
                        }
                        finally
                        {
                            //Without disposing of the moduleHandle, the runtime *will* crash the process.
                            moduleHandle.Dispose();
                        }
                    }
                }
            }

            Assert.True(true);
        }

        [Fact]
        public void BaristaModuleThrowsIfModuleRecordFactoryNotSpecified()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var converter = m_provider.GetRequiredService<IBaristaConversionStrategy>();
                    var moduleRecordFactory = m_provider.GetRequiredService<IBaristaModuleRecordFactory>();
                    var moduleLoader = m_provider.GetRequiredService<IBaristaModuleLoader>();

                    using (ctx.Scope())
                    {
                        var specifier = ctx.ValueFactory.CreateString("");
                        var moduleHandle = rt.Engine.JsInitializeModuleRecord(JavaScriptModuleRecord.Invalid, specifier.Handle);

                        try
                        {
                            Assert.Throws<ArgumentNullException>(() =>
                            {
                                var mod = new BaristaModuleRecord("", specifier.Handle, null, rt.Engine, ctx, null, moduleLoader, moduleHandle);
                            });
                        }
                        finally
                        {
                            //Without disposing of the moduleHandle, the runtime *will* crash the process.
                            moduleHandle.Dispose();
                        }
                    }
                }
            }

            Assert.True(true);
        }

        [Fact]
        public void BaristaModuleCanParseAndIndicateReady()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var converter = m_provider.GetRequiredService<IBaristaConversionStrategy>();
                    var moduleRecordFactory = m_provider.GetRequiredService<IBaristaModuleRecordFactory>();
                    var moduleLoader = m_provider.GetRequiredService<IBaristaModuleLoader>();

                    using (ctx.Scope())
                    {
                        var specifier = ctx.ValueFactory.CreateString("");
                        var moduleHandle = rt.Engine.JsInitializeModuleRecord(JavaScriptModuleRecord.Invalid, specifier.Handle);

                        try
                        {
                            var mod = new BaristaModuleRecord("", specifier.Handle, null, rt.Engine, ctx, moduleRecordFactory, moduleLoader, moduleHandle);
                            mod.ParseModuleSource("export default 'hello, world!'");
                            Assert.True(mod.IsReady);
                        }
                        finally
                        {
                            //Without disposing of the moduleHandle, the runtime *will* crash the process.
                            moduleHandle.Dispose();
                        }
                    }
                }
            }

            Assert.True(true);
        }

        [Fact]
        public void BaristaModuleCanReferenceItself()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var converter = m_provider.GetRequiredService<IBaristaConversionStrategy>();
                    var moduleRecordFactory = m_provider.GetRequiredService<IBaristaModuleRecordFactory>();
                    var moduleLoader = m_provider.GetRequiredService<IBaristaModuleLoader>();

                    using (ctx.Scope())
                    {
                        var specifier = ctx.ValueFactory.CreateString("");
                        var moduleHandle = rt.Engine.JsInitializeModuleRecord(JavaScriptModuleRecord.Invalid, specifier.Handle);

                        try
                        {
                            var mod = new BaristaModuleRecord("foo", specifier.Handle, null, rt.Engine, ctx, moduleRecordFactory, moduleLoader, moduleHandle);
                            mod.ParseModuleSource("import foo from 'foo'; export default 'hello, world!'");
                            Assert.True(mod.IsReady);
                        }
                        finally
                        {
                            //Without disposing of the moduleHandle, the runtime *will* crash the process.
                            moduleHandle.Dispose();
                        }
                    }
                }
            }

            Assert.True(true);
        }

        [Fact]
        public void JsModulesCanBeEvaluated()
        {
            var script = @"
export default 'hello, world!';
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
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
        public void JsModulesCannotBeEvaluatedWhenContextIsDisposed()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    ctx.Dispose();
                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        ctx.EvaluateModule("export default 'asdf';");
                    });
                }
            }
        }

        [Fact]
        public void JsModulesWithErrorsWillThrow()
        {
            var script = @"
export default asdf@11;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule(script);
                        });
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
            using (var rt = BaristaRuntimeFactory.CreateRuntime(JavaScriptRuntimeAttributes.None))
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
        public void JsModulesThatRejectPromisesWillThrow()
        {
            var script = @"
var result = new Promise((resolve, reject) => {
        reject('No. Bad.');
    });
export default result;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime(JavaScriptRuntimeAttributes.None))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.Throws<JsScriptException>(() =>
                        {
                            try
                            {
                                var result = ctx.EvaluateModule(script);
                            }
                            catch (JsScriptException ex)
                            {
                                Assert.Equal("No. Bad.", ex.Message);
                                throw;
                            }
                        });

                    }
                }
            }
        }

        [Fact]
        public void JsModulesThatImportUnknownModulesWillThrow()
        {
            var script = @"
import soAmazing from 'myModule';
export default soAmazing;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime(JavaScriptRuntimeAttributes.None))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule(script);
                        });

                    }
                }
            }
        }

        [Fact]
        public void JsModulesWillExecuteEvenWithoutATaskQueue()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                var converter = m_provider.GetRequiredService<IBaristaConversionStrategy>();
                var valueFactoryBuilder = m_provider.GetRequiredService<IBaristaValueFactoryBuilder>();
                var moduleRecordFactory = m_provider.GetRequiredService<IBaristaModuleRecordFactory>();
                IPromiseTaskQueue taskQueue = null;

                var contextHandle = rt.Engine.JsCreateContext(rt.Handle);
                using (var ctx = new BaristaContext(rt.Engine, valueFactoryBuilder, converter, moduleRecordFactory, taskQueue, contextHandle))
                {
                    var result = ctx.EvaluateModule("export default 'foo';");
                    Assert.Equal("foo", result.ToString());
                }
                Assert.True(contextHandle.IsClosed);
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

            ModuleLoader.RegisterModule(bananaModule);

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
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
        public void JsModulesCanUseResourceStrings()
        {
            var script = @"
import banana from 'banana';
export default 'hello, world! ' + banana;
";
            var bananaModule = new BaristaResourceScriptModule(Properties.Resources.ResourceManager)
            {
                Name = "banana",
                ResourceName = "String1"
            };

            ModuleLoader.RegisterModule(bananaModule);

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var result = ctx.EvaluateModule(script);

                        Assert.Equal("hello, world! hello, world!", result.ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsScriptModulesWithNullScriptsContinue()
        {
            var script = @"
import banana from 'banana';
export default 'hello, world! ' + banana;
";
            var bananaModule = new BaristaScriptModule
            {
                Name = "banana",
                Script = null
            };

            ModuleLoader.RegisterModule(bananaModule);

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var result = ctx.EvaluateModule(script);

                        Assert.True(result.ToString() == "hello, world! null");
                    }
                }
            }
        }

        [Fact]
        public void JsModulesImportingThemselvesContinue()
        {
            var script = @"
import banana from 'banana';
export default 'hello, world! ' + banana;
";
            var bananaModule = new BaristaScriptModule
            {
                Name = "banana",
                Script = @"
import banana from 'banana'
export default 'banana' + ' ' + banana;
"
            };

            ModuleLoader.RegisterModule(bananaModule);

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var result = ctx.EvaluateModule(script);

                        //FIXME: Not sure the full discussion around self-referencing modules, are they allowed for n?
                        Assert.Equal("hello, world! banana undefined", result.ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsModulesImportingBadScriptsThrow()
        {
            var script = @"
import banana from 'badbanana';
export default 'hello, world! ' + banana;
";
            var bananaModule = new BaristaScriptModule
            {
                Name = "badbanana",
                Script = @"
export default asdf@11;
"
            };

            ModuleLoader.RegisterModule(bananaModule);

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule(script);
                        });
                    }
                }
            }
        }

        [Fact]
        public void BaristaModulesCanReturnJsValues()
        {
            var script = @"
        import helloworld from 'hello_world';
        export default helloworld;
        ";
            var myHelloWorldModule = new HelloWorldModule();
            ModuleLoader.RegisterModule(myHelloWorldModule);

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var result = ctx.EvaluateModule(script);

                    Assert.Equal("Hello, World!", result.ToString());
                }
            }
        }

        [Fact]
        public void BaristaModulesCanReturnSafeHandles()
        {
            var script = @"
        import reverse from 'reverse';
        export default reverse('hello, world!');
        ";
            var myReverseModule = new ReverseModule();
            ModuleLoader.RegisterModule(myReverseModule);

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var result = ctx.EvaluateModule(script);

                    Assert.Equal("!dlrow ,olleh", result.ToString());
                }
            }

            myReverseModule.Dispose();
        }


        [Fact]
        public void BaristaModulesCanReturnNativeObjects()
        {
            var script = @"
        import fourtytwo from 'FourtyTwo';
        export default fourtytwo;
        ";
            var fourtyTwoModule = new FourtyTwoModule();
            ModuleLoader.RegisterModule(fourtyTwoModule);

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var result = ctx.EvaluateModule<JsNumber>(script);

                    Assert.NotNull(result);
                    Assert.Equal(42, result.ToInt32());
                }
            }
        }

        [Fact]
        public void BaristaFawltyModulesWillThrow()
        {
            var script = @"
        import derp from 'Fawlty';
        export default derp;
        ";
            var fawltyModule = new FawltyModule();
            ModuleLoader.RegisterModule(fawltyModule);

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    Assert.Throws<JsScriptException>(() =>
                    {
                        var result = ctx.EvaluateModule<JsNumber>(script);
                    });
                }
            }
        }

        [Fact]
        public void BaristaModulesThatDoNotProvideAnAttributeWillThrow()
        {
            var noAttributeModule = new NoBaristaModuleAttributeModule();
            Assert.Throws<BaristaModuleMissingAttributeException>(() =>
            {
                ModuleLoader.RegisterModule(noAttributeModule);
            });
        }

        [Fact]
        public void BaristaModulesWithUnconvertableDefaultExportsWillThrow()
        {
            var script = @"
        import derp from 'TooSmart';
        export default derp;
        ";
            var tooSmartModule = new TooSmartModule();
            ModuleLoader.RegisterModule(tooSmartModule);

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    Assert.Throws<JsScriptException>(() =>
                    {
                        var result = ctx.EvaluateModule<JsNumber>(script);
                    });
                }
            }
        }

        [Fact]
        public void MultipleModulesCanBeRegisteredAndUsed()
        {
            var script = @"
        import helloWorld from 'hello_world';
        import fourtytwo from 'FourtyTwo';
        export default helloWorld + ' ' + fourtytwo;
        ";
            ModuleLoader.RegisterModule(new FourtyTwoModule());
            ModuleLoader.RegisterModule(new HelloWorldModule());

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var result = ctx.EvaluateModule<JsString>(script);

                    Assert.NotNull(result);
                    Assert.Equal("Hello, World! 42", result.ToString());
                }
            }
        }

        [Fact]
        public void JsModulesCanDetermineTheModuleRequestingIt()
        {
            var script = @"
import banana from 'level2';
export default 'hello, world! ' + banana;
";
            var bananaModule = new BaristaScriptModule
            {
                Name = "level2",
                Script = @"
import requestorName from 'depedendent';
export default 'Requested By: ' + requestorName;
"
            };

            ModuleLoader.RegisterModule(bananaModule);
            ModuleLoader.RegisterModule(new ReturnDependentNameModule());

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var result = ctx.EvaluateModule(script);

                        Assert.Equal("hello, world! Requested By: level2", result.ToString());
                    }
                }
            }
        }

        [BaristaModule("hello_world", "Only the best module ever.")]
        private sealed class HelloWorldModule : IBaristaModule
        {
            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>(context.ValueFactory.CreateString("Hello, World!"));
            }
        }

        [BaristaModule("reverse", "reverses the string passed in.")]
        private sealed class ReverseModule : IBaristaModule, IDisposable
        {
            private GCHandle m_reverseDelegateHandle;

            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                //This module goes through the trouble of creating a JavaScriptValueSafeHandle to ensure that it can be done.
                JavaScriptNativeFunction fnReverse = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
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
                };
                m_reverseDelegateHandle = GCHandle.Alloc(fnReverse);
                return Task.FromResult<object>(context.Engine.JsCreateFunction(fnReverse, IntPtr.Zero));
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

                    m_reverseDelegateHandle.Free();
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

        [BaristaModule("FourtyTwo", "The answer is...")]
        private sealed class FourtyTwoModule : IBaristaModule
        {
            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>(42);
            }
        }

        [BaristaModule("Fawlty", "Derp!")]
        private sealed class FawltyModule : IBaristaModule
        {
            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                throw new Exception("Derp!");
            }
        }

        [BaristaModule("TooSmart", "So complicated!")]
        private sealed class TooSmartModule : IBaristaModule
        {
            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>(new StringBuilder());
            }
        }

        [BaristaModule("depedendent", "Returns the name of the requesting module.")]
        private sealed class ReturnDependentNameModule : IBaristaModule
        {
            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>(referencingModule.Name);
            }
        }

        private sealed class NoBaristaModuleAttributeModule : IBaristaModule
        {
            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>("You'll not see me.");
            }
        }
    }
}
