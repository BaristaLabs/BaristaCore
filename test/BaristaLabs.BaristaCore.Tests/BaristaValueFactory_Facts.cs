namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaLabs.BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    [Collection("BaristaCore Tests")]
    public class BaristaValueFactory_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public BaristaValueFactory_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            m_provider = ServiceCollection.BuildServiceProvider();
        }

        public IBaristaRuntimeFactory BaristaRuntimeFactory
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeFactory>(); }
        }

        [Fact]
        public void BaristaValueFactoryConstructorThrowsOnNullArgs()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var bvf = new BaristaValueFactory(null, null);
            });

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                Assert.Throws<ArgumentNullException>(() =>
                {
                    var bvf = new BaristaValueFactory(rt.Engine, null);
                });
            }
        }

        [Fact]
        public void ValueFactoryCanCreateString()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var jsString = ctx.CreateString("Hello, world!");
                        Assert.NotNull(jsString);
                        jsString.Dispose();

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            ctx.CreateString(null);
                        });
                    }
                }
            }
        }

        [Fact]
        public void ValueFactoryCanCreateArrayBufferFromString()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var jsString = ctx.CreateArrayBuffer("Hello, world!");
                        Assert.NotNull(jsString);
                        Assert.Equal(13, jsString.GetArrayBufferStorage().Length);
                        jsString.Dispose();

                        jsString = ctx.CreateArrayBuffer((string)null);
                        Assert.NotNull(jsString);
                        Assert.Empty(jsString.GetArrayBufferStorage());
                        jsString.Dispose();
                    }
                }
            }
        }

        [Fact]
        public void ValueFactoryCanCreateAPromiseFromATask()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var iRan = false;
                        var myTask = ctx.TaskFactory.StartNew(() =>
                        {
                            Task.Delay(1000);
                            iRan = true;
                            return "foo";
                        });

                        var jsPromise = ctx.CreatePromise(myTask);
                        Assert.NotNull(jsPromise);
                        Assert.True(iRan);
                    }
                }
            }
        }

        [Fact]
        public void ValueFactoryCleansUpOnBeforeCollectCallbacks()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                bool hasRaisedBeforeCollect = false;
                JsString myValue;
                EventHandler<BaristaObjectBeforeCollectEventArgs> beforeCollect = (sender, e) =>
                {
                    hasRaisedBeforeCollect = true;
                };

                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        myValue = ctx.CreateString("Hello, World");
                        Assert.NotNull(myValue);

                        myValue.BeforeCollect += beforeCollect;

                        //Manually dispose of the value handle and trigger a garbage collect to trigger the beforeCollect event.
                        myValue.Handle.Dispose();
                        rt.CollectGarbage();

                        Assert.True(hasRaisedBeforeCollect);
                    }
                }
            }
        }
    }
}
