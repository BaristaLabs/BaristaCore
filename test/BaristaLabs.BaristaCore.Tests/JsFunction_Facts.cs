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
        public void JsFunctionCanBeCreated()
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
    }
}