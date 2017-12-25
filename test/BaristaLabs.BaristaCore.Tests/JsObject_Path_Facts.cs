namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    [Collection("BaristaCore Tests")]
    public class JsObject_Path_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public JsObject_Path_Facts()
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
        public void JsObjectPropertyCanBeRetrievedByPath()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: { bar: { baz: 'qix' } } };";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var qix = result.SelectValue("foo.bar.baz");
                        Assert.True(qix != null);
                        Assert.Equal("qix", qix.ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsObjectSelectThrowsIfNoPathSpecified()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: { bar: { baz: 'qix' } } };";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            var qix = result.SelectValue("");
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsObjectSelectThrowsIfNoScope()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsObject obj;
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: { bar: { baz: 'qix' } } };";
                        obj = ctx.EvaluateModule<JsObject>(script);
                    }
                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        var qix = obj.SelectValue("foo");
                    });
                }
            }
        }

        [Fact]
        public void JsObjectSelectDoesNotThrowIfUndefined()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: { bar: { baz: 'qix' } } };";
                        var obj = ctx.EvaluateModule<JsObject>(script);
                        var qix = obj.SelectValue("foo.bat", true);
                        Assert.Equal(ctx.Undefined, qix);
                    }
                }
            }
        }

        [Fact]
        public void JsObjectSelectDoesNotThrowIfNoMatchAndNoThrowIsSpecified()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: { bar: { baz: 'qix' } } };";
                        var obj = ctx.EvaluateModule<JsObject>(script);
                        var qix = obj.SelectValue("foo.bat.man");
                        Assert.Equal(ctx.Undefined, qix);
                    }
                }
            }
        }

        [Fact]
        public void JsObjectSelectThrowsIfNoMatch()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: { bar: { baz: 'qix' } } };";
                        var obj = ctx.EvaluateModule<JsObject>(script);
                        Assert.Throws<IndexOutOfRangeException>(() =>
                        {
                            var qix = obj.SelectValue("foo.bat.man", true);
                        });
                    }
                }
            }
        }
    }
}