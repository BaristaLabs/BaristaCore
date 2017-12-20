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
                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.Promise.Wait(jsPromise);
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsPromiseCanInvokeAll()
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
                        var allResultArray = ctx.Promise.All(p1, p2);
                        Assert.NotNull(allResultArray);
                        Assert.True(iRan1);
                        Assert.True(iRan2);

                        var rel = ctx.Promise.Wait<JsArray>(allResultArray);
                        Assert.NotNull(rel);
                        Assert.Equal(2, rel.Length);

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            ctx.Promise.All(null);
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

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            ctx.Promise.Race(null);
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsPromiseCanReject()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var rejectPromise = ctx.Promise.Reject(ctx.ValueFactory.CreateString("foo"));
                        Assert.Throws<JsScriptException>(() =>
                        {
                            try
                            {
                                ctx.Promise.Wait<JsString>(rejectPromise);
                            }
                            catch(JsScriptException ex)
                            {
                                Assert.Equal("foo", ex.Message);
                                throw;
                            }
                        });

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            ctx.Promise.Reject(null);
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsPromiseCanResolve()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var resolvePromise = ctx.Promise.Resolve(ctx.ValueFactory.CreateString("foo"));
                        var result = ctx.Promise.Wait<JsString>(resolvePromise);
                        Assert.Equal("foo", result.ToString());

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            ctx.Promise.Resolve(null);
                        });
                    }
                }
            }
        }
    }
}
