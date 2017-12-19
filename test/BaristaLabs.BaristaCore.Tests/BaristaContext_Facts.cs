namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class BaristaContext_Facts
    {
        private IServiceProvider m_provider;

        public BaristaContext_Facts()
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
        public void BaristaContextCanBeCreated()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {

                }
            }
            Assert.True(true);
        }

        [Fact]
        public void BaristaContextThrowsIfValueFactoryNotSpecified()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                var converter = m_provider.GetRequiredService<IBaristaConversionStrategy>();
                var moduleRecordFactory = m_provider.GetRequiredService<IBaristaModuleRecordFactory>();
                var taskQueue = m_provider.GetRequiredService<IPromiseTaskQueue>();

                var contextHandle = rt.Engine.JsCreateContext(rt.Handle);

                try
                {
                    Assert.Throws<ArgumentNullException>(() =>
                    {
                        var ctx = new BaristaContext(rt.Engine, null, converter, moduleRecordFactory, taskQueue, contextHandle);
                    });
                }
                finally
                {
                    //Without disposing of the contextHandle, the runtime *will* crash the process.
                    contextHandle.Dispose();
                }
            }

            Assert.True(true);
        }

        [Fact]
        public void BaristaContextThrowsIfConversionStrategyNotSpecified()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                var valueFactoryBuilder = m_provider.GetRequiredService<IBaristaValueFactoryBuilder>();
                var moduleRecordFactory = m_provider.GetRequiredService<IBaristaModuleRecordFactory>();
                var taskQueue = m_provider.GetRequiredService<IPromiseTaskQueue>();

                var contextHandle = rt.Engine.JsCreateContext(rt.Handle);

                try
                {
                    Assert.Throws<ArgumentNullException>(() =>
                    {
                        var ctx = new BaristaContext(rt.Engine, valueFactoryBuilder, null, moduleRecordFactory, taskQueue, contextHandle);
                    });
                }
                finally
                {
                    //Without disposing of the contextHandle, the runtime *will* crash the process.
                    contextHandle.Dispose();
                }
            }

            Assert.True(true);
        }

        [Fact]
        public void MultipleJavaScriptContextsCanBeCreated()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                var ctx1 = rt.CreateContext();
                var ctx2 = rt.CreateContext();

                Assert.NotEqual(ctx1, ctx2);
            }
        }

        [Fact]
        public void BeforeCollectCallbackIsCalled()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                bool hasRaisedBeforeCollect = false;
                EventHandler<BaristaObjectBeforeCollectEventArgs> beforeCollect = (sender, e) =>
                {
                    hasRaisedBeforeCollect = true;
                };

                var ctx1 = rt.CreateContext();
                ctx1.BeforeCollect += beforeCollect;

                //Manually dispose of the value handle and trigger a garbage collect to trigger the beforeCollect event.
                ctx1.Handle.Dispose();
                rt.CollectGarbage();

                Assert.True(hasRaisedBeforeCollect);
            }
        }

        [Fact]
        public void NestedScopesAreNotAllowed()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.Throws<BaristaException>(() =>
                        {
                            ctx.Scope();
                        });
                    }
                }
            }
        }

        #region Barista Properties
        [Fact]
        public void JavaScriptContextShouldGetTaskFactory()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.TaskFactory);
                    }

                    ctx.Dispose();
                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        var foo = ctx.TaskFactory;
                    });
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetValueFactory()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.ValueFactory);
                    }

                    ctx.Dispose();
                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        var foo = ctx.ValueFactory;
                    });
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetCurrentScope()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.CurrentScope);
                        Assert.True(ctx.HasCurrentScope);
                    }

                    Assert.Null(ctx.CurrentScope);
                    Assert.False(ctx.HasCurrentScope);

                    ctx.Dispose();
                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        var foo = ctx.CurrentScope;
                    });

                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        var foo = ctx.HasCurrentScope;
                    });
                }
            }
        }
        #endregion

        #region Built-In Objects
        [Fact]
        public void JavaScriptContextShouldGetFalse()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.False);
                        Assert.False(ctx.False);
                    }

                    ctx.Dispose();
                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        var foo = ctx.False;
                    });
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetTrue()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.True);
                        Assert.True(ctx.True);
                    }

                    ctx.Dispose();
                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        var foo = ctx.True;
                    });
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetNull()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.Null);
                    }

                    ctx.Dispose();
                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        var foo = ctx.Null;
                    });
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetUndefined()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.Undefined);
                    }

                    ctx.Dispose();
                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        var foo = ctx.Undefined;
                    });
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetGlobalObject()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.GlobalObject);
                    }

                    ctx.Dispose();
                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        var foo = ctx.GlobalObject;
                    });
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetJSONBuiltIn()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.JSON);
                    }

                    ctx.Dispose();
                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        var foo = ctx.JSON;
                    });
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetObjectBuiltIn()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.Object);
                    }

                    ctx.Dispose();
                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        var foo = ctx.Object;
                    });
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetPromiseBuiltIn()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.Promise);
                    }

                    ctx.Dispose();
                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        var foo = ctx.Promise;
                    });
                }
            }
        }

        [Fact]
        public void JavaScriptContextsShouldHaveUniqueUndefineds()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
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
        #endregion

        [Fact]
        public void JsPropertyCanBeRetrievedByName()
        {
            var script = @"var fooObj = {
    foo: 'bar',
    baz: 'qix'
};

export default fooObj;";

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fooObj = ctx.EvaluateModule(script) as JsObject;
                        Assert.NotNull(fooObj);

                        dynamic fooValue = fooObj.GetProperty<JsString>("foo");
                        Assert.Equal("bar", (string)fooValue);
                    }
                }
            }
        }
    }
}
