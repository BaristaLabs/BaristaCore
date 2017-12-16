namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class JsPromise_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public JsPromise_Facts()
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
        public void JsPromiseCanBeCreated()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.Promise);
                        Assert.Equal(JavaScript.JavaScriptValueType.Object, ctx.Promise.Type);
                    }
                }
            }
        }

        [Fact]
        public void JsPromiseCanUnwrapAPromise()
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
                            Task.Delay(250).GetAwaiter().GetResult();
                            iRan = true;
                            return "foo";
                        });

                        var jsPromise = ctx.ValueFactory.CreatePromise(myTask);
                        var result = ctx.Promise.Wait(jsPromise);
                        Assert.True(iRan);
                        Assert.Equal("foo", result.ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsPromiseCanUnwrapAPromiseThrowingErrors()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var myTask = ctx.TaskFactory.StartNew(() =>
                        {
                            throw new InvalidOperationException("Boom");
                        });

                        var jsPromise = ctx.ValueFactory.CreatePromise(myTask);
                        Assert.Throws<BaristaScriptException>(() =>
                        {
                            var result = ctx.Promise.Wait(jsPromise);
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsPromiseCanRace()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var iRan1 = false;
                        var iRan2 = false;

                        var t1 = ctx.TaskFactory.StartNew(() =>
                        {
                            Task.Delay(250).GetAwaiter().GetResult();
                            iRan1 = true;
                            return "foo1";
                        });

                        var t2 = ctx.TaskFactory.StartNew(() =>
                        {
                            Task.Delay(500).GetAwaiter().GetResult();
                            iRan2 = true;
                            return "foo2";
                        });


                        var p1 = ctx.ValueFactory.CreatePromise(t1);
                        var p2 = ctx.ValueFactory.CreatePromise(t2);
                        var racePromise = ctx.Promise.Race(p1, p2);
                        Assert.NotNull(racePromise);
                        Assert.True(iRan1);
                        Assert.True(iRan2);

                        var rel = ctx.Promise.Wait(racePromise);
                        Assert.Equal("foo1", rel.ToString());
                    }
                }
            }
        }
    }
}
