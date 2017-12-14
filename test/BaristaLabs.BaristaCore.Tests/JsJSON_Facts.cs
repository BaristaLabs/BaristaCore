namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class JsJSON_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public JsJSON_Facts()
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
        public void JsJSONCanBeCreated()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.JSON);
                        Assert.Equal(JavaScript.JavaScriptValueType.Object, ctx.JSON.Type);
                    }
                }
            }
        }

        [Fact]
        public void JsJSONCanParse()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var str = ctx.ValueService.CreateString("{ \"foo\": \"bar\" }");

                        var jsObject = ctx.JSON.Parse(str) as JsObject;
                        Assert.NotNull(jsObject);
                        var bar = jsObject["foo"];
                        Assert.Equal("bar", bar.ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsJSONCanStringify()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var str = ctx.EvaluateModule<JsObject>("export default { 'hello': 'world' };");

                        var json = ctx.JSON.Stringify(str);
                        Assert.NotNull(json);
                        Assert.Equal("{\"hello\":\"world\"}", json);
                    }
                }
            }
        }
    }
}
