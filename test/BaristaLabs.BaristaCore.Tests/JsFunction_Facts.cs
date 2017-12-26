namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    [Collection("BaristaCore Tests")]
    public class JsFunction_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public JsFunction_Facts()
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
        public void JsFunctionCanBeExported()
        {
            var script = @"
export default () => 'hello, world';
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var result = ctx.EvaluateModule<JsFunction>(script);
                        Assert.NotNull(result);
                        Assert.Equal(JsValueType.Function, result.Type);
                        Assert.True(result.ToString() == "() => 'hello, world'");
                    }
                }
            }
        }

        [Fact]
        public void JsFunctionsCanBeInvoked()
        {
            var script = @"
export default () => 'hello, world';
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fn = ctx.EvaluateModule<JsFunction>(script);
                        var result = fn.Call();
                        Assert.Equal("hello, world", result.ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsFunctionsThatThrowCanBeCaught()
        {
            var script = @"
export default () => { throw new Error('That is quite illogical, captain.'); };
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fn = ctx.EvaluateModule<JsFunction>(script);

                        JsScriptException ex = null;
                        try
                        {
                            var result = fn.Call();
                        }
                        catch(JsScriptException invokeException)
                        {
                            ex = invokeException;
                        }

                        Assert.NotNull(ex);
                        Assert.Equal("That is quite illogical, captain.", ex.Message);
                    }
                }
            }
        }

        [Fact]
        public void JsNativeFunctionsCanBeCreated()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fnAdd = ctx.ValueFactory.CreateFunction(new Func<JsObject, int, int, int>((jsThis, a, b) =>
                        {
                            return a + b;
                        }));

                        Assert.NotNull(fnAdd);
                        var jsNumA = ctx.ValueFactory.CreateNumber(37);
                        var jsNumB = ctx.ValueFactory.CreateNumber(5);
                        var result = fnAdd.Call<JsNumber>(ctx.GlobalObject, jsNumA, jsNumB);

                        Assert.Equal(42, result.ToInt32());
                    }
                }
            }
        }

        [Fact]
        public void JsNullNativeFunctionsCannotBeCreated()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            ctx.ValueFactory.CreateFunction(null);
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsNativeFunctionsThatThrowCanBeHandled()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fnBoom = ctx.ValueFactory.CreateFunction(new Action(() =>
                        {
                            throw new InvalidOperationException("A hole has been torn in the universe.");
                        }));
                        Assert.NotNull(fnBoom);

                        JsScriptException ex = null;
                        try
                        {
                            var result = fnBoom.Call();
                        }
                        catch(JsScriptException invokeException)
                        {
                            ex = invokeException;
                        }

                        Assert.NotNull(ex);
                        Assert.Equal("A hole has been torn in the universe.", ex.Message);
                    }
                }
            }
        }
    }
}