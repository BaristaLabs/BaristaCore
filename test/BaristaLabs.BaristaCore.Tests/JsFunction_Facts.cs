namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
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

        public IBaristaRuntimeService BaristaRuntimeService
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeService>(); }
        }

        [Fact]
        public void JsFunctionCanBeExported()
        {
            var script = @"
export default () => 'hello, world';
";
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var result = ctx.EvaluateModule<JsFunction>(script);
                        Assert.NotNull(result);
                        Assert.Equal(JavaScript.JavaScriptValueType.Function, result.Type);
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
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fn = ctx.EvaluateModule<JsFunction>(script);
                        var result = fn.Invoke();
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
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fn = ctx.EvaluateModule<JsFunction>(script);

                        BaristaScriptException ex = null;
                        try
                        {
                            var result = fn.Invoke();
                        }
                        catch(BaristaScriptException invokeException)
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
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fnAdd = ctx.ValueService.CreateFunction(new Func<int, int, int>((a, b) =>
                        {
                            return a + b;
                        }));

                        Assert.NotNull(fnAdd);
                        var jsNumA = ctx.ValueService.CreateNumber(37);
                        var jsNumB = ctx.ValueService.CreateNumber(5);
                        var result = fnAdd.Invoke<JsNumber>(jsNumA, jsNumB);

                        Assert.Equal(42, result.ToInt32());
                    }
                }
            }
        }

        [Fact]
        public void JsNullNativeFunctionsCannotBeCreated()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            ctx.ValueService.CreateFunction(null);
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsNativeFunctionsThatThrowCanBeHandled()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fnBoom = ctx.ValueService.CreateFunction(new Action(() =>
                        {
                            throw new InvalidOperationException("A hole has been torn in the universe.");
                        }));
                        Assert.NotNull(fnBoom);

                        BaristaScriptException ex = null;
                        try
                        {
                            var result = fnBoom.Invoke();
                        }
                        catch(BaristaScriptException invokeException)
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