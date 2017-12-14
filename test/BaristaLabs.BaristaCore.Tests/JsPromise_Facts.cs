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
                        var myTask = new Task<string>(() =>
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

                        var t1 = new Task<string>(() =>
                        {
                            Task.Delay(250).GetAwaiter().GetResult();
                            iRan1 = true;
                            return "foo";
                        });

                        var t2 = new Task<string>(() =>
                        {
                            Task.Delay(500).GetAwaiter().GetResult();
                            iRan2 = true;
                            return "foo";
                        });


                        var p1 = ctx.ValueFactory.CreatePromise(t1);
                        var p2 = ctx.ValueFactory.CreatePromise(t2);
                        var raceResult = ctx.Promise.Race(p1, p2);
                        Assert.NotNull(raceResult);
                        Assert.True(iRan1);
                        Assert.True(iRan2);

                        //FIXME: Not sure why this is always undefined.
                        var rel = ctx.Promise.Wait(raceResult);
                    }
                }
            }
        }
    }
}
