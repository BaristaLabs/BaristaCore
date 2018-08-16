namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.Modules;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    [Collection("BaristaCore Tests")]
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
                        var specifier = ctx.CreateString("");
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
                        var specifier = ctx.CreateString("");
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
                        var specifier = ctx.CreateString("");
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
                        var specifier = ctx.CreateString("");
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
                        var specifier = ctx.CreateString("");
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
        public void JsModulesExportPromisesThatResolve()
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
                        result = ctx.Promise.Wait(result as JsObject);
                        Assert.True(result.ToString() == "hello, world!");
                    }
                }
            }
        }

        [Fact]
        public void JsModulesExportPromisesThatCanReject()
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
                                result = ctx.Promise.Wait(result as JsObject);
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
            var bananaModule = new BaristaScriptModule("banana")
            {
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
            var bananaModule = new BaristaResourceScriptModule("banana", Properties.Resources.ResourceManager)
            {
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
        public void JsScriptModulesWithEmptyScriptsThrow()
        {
            var script = @"
import banana from 'banana';
export default 'hello, world! ' + banana;
";
            var bananaModule = new BaristaScriptModule("banana")
            {
                Script = ""
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
        public void JsModulesImportingThemselvesContinue()
        {
            var script = @"
import banana from 'banana';
export default 'hello, world! ' + banana;
";
            var bananaModule = new BaristaScriptModule("banana")
            {
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
            var bananaModule = new BaristaScriptModule("badbanana")
            {
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
            var bananaModule = new BaristaScriptModule("level2")
            {
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

        [Fact]
        public void JsModulesCanExposeNativeObjects()
        {
            var script = @"
import carlyRae from 'native-object';
carlyRae.name = 'New Name';
for(var i = 0; i < 10; i++)
{
    carlyRae.callMeMaybe();
}
export default carlyRae;
";
            var nomModule = new NativeObjectModule();
            ModuleLoader.RegisterModule(nomModule);

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var result = ctx.EvaluateModule(script) as JsObject;
                        Assert.NotNull(result);

                        Assert.Equal(10, result["calls"].ToInt32());
                        Assert.Equal("New Name", result["name"].ToString());
                    }
                }
            }
        }

        #region Module Types
        [BaristaModule("hello_world", "Only the best module ever.")]
        private sealed class HelloWorldModule : IBaristaModule
        {
            public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return context.CreateString("Hello, World!");
            }
        }

        [BaristaModule("reverse", "reverses the string passed in.")]
        private sealed class ReverseModule : IBaristaModule
        {
            public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                var fnResult = context.CreateFunction(new Func<JsObject, JsValue, JsValue>((thisObj, toReverse) =>
                {
                    if (toReverse == null || String.IsNullOrWhiteSpace(toReverse.ToString()))
                    {
                        return context.Undefined;
                    }

                    var str = toReverse.ToString();
                    var charArray = str.ToCharArray();
                    Array.Reverse(charArray);
                    var reversed = new string(charArray);

                    return context.CreateString(reversed);
                }));

                return fnResult;
            }
        }

        [BaristaModule("FourtyTwo", "The answer is...")]
        private sealed class FourtyTwoModule : IBaristaModule
        {
            public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return context.CreateNumber(42);
            }
        }

        [BaristaModule("Fawlty", "Derp!")]
        private sealed class FawltyModule : IBaristaModule
        {
            public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                throw new Exception("Derp!");
            }
        }

        [BaristaModule("TooSmart", "So complicated!")]
        private sealed class TooSmartModule : IBaristaModule
        {
            public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return context.CreateString(" ");
            }
        }

        [BaristaModule("depedendent", "Returns the name of the requesting module.")]
        private sealed class ReturnDependentNameModule : IBaristaModule
        {
            public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return context.CreateString(referencingModule.Name);
            }
        }

        private sealed class NoBaristaModuleAttributeModule : IBaristaModule
        {
            public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return context.CreateString("You'll not see me.");
            }
        }

        [BaristaModule("native-object", "Returns a native .net object that can be used within scripts.")]
        private sealed class NativeObjectModule : IBaristaModule
        {
            public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                var foo = new CarlyRae() { Name = "Kilroy" };
                context.Converter.TryFromObject(context, foo, out JsValue resultObj);
                return resultObj;
            }

            private class CarlyRae
            {
                private int m_calls = -1;

                public CarlyRae()
                {
                    m_calls = 0;
                }

                public string Name
                {
                    get;
                    set;
                }

                public int Calls
                {
                    get { return m_calls; }
                }

                public void CallMeMaybe()
                {
                    m_calls++;
                }
            }
        }
        #endregion
    }
}
