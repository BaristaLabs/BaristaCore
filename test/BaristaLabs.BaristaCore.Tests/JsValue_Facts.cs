namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class JsValue_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public JsValue_Facts()
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
        public void JsValueCanBeCreated()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var str = "Test 123";
                        var valueHandle = rt.Engine.JsCreateString(str, (ulong)str.Length);

                        try
                        {
                            var jsValue = new JsString(rt.Engine, ctx, valueHandle);
                            Assert.Equal(str, jsValue.ToString());
                        }
                        finally
                        {
                            valueHandle.Dispose();
                        }
                    }
                }
            }
        }

        [Fact]
        public void JsValueConstructorThrowsIfArgumentsNull()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var str = "Test 123";
                        var valueHandle = rt.Engine.JsCreateString(str, (ulong)str.Length);

                        try
                        {
                            Assert.Throws<ArgumentNullException>(() =>
                            {
                                var jsValue = new JsString(null, ctx, valueHandle);
                            });

                            Assert.Throws<ArgumentNullException>(() =>
                            {
                                var jsValue = new JsString(rt.Engine, null, valueHandle);
                            });
                        }
                        finally
                        {
                            valueHandle.Dispose();
                        }
                    }
                }
            }
        }

        [Fact]
        public void JsValueFunctionsWithoutBeforeCollectEventAttached()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var str = "Test 123";
                        var valueHandle = rt.Engine.JsCreateString(str, (ulong)str.Length);
                        
                        try
                        {
                            var jsValue = new JsString(rt.Engine, ctx, valueHandle);
                            valueHandle.Dispose();

                            rt.CollectGarbage();
                            Assert.True(true);
                        }
                        finally
                        {
                            valueHandle.Dispose();
                        }
                    }
                }
            }
        }

        [Fact]
        public void CanGetBoolean()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsValue zip;
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: true, bar: false, baz: 'true', qix: 'false', zip: 1 };";
                        var result = ctx.EvaluateModule<JsObject>(script);
                        var value = result["foo"];

                        Assert.True(result["foo"].ToBoolean());
                        Assert.False(result["bar"].ToBoolean());
                        Assert.True(result["baz"].ToBoolean());
                        Assert.True(result["qix"].ToBoolean());

                        zip = result["zip"];
                    }

                    Assert.True(zip.ToBoolean());
                    zip.Dispose();
                    Assert.Throws<ObjectDisposedException>(() =>
                    {
                        zip.ToBoolean();
                    });
                }
            }
        }

        [Fact]
        public void CanGetDouble()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsValue bar;
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: '1234', bar: '5678' };";
                        var result = ctx.EvaluateModule<JsObject>(script);
                        var value = result["foo"];

                        Assert.Equal(1234, value.ToDouble());
                        bar = result["bar"];
                    }

                    Assert.Equal(5678, bar.ToDouble());

                    using (ctx.Scope())
                    {
                        bar.Dispose();

                        Assert.Throws<ObjectDisposedException>(() =>
                        {
                            bar.ToDouble();
                        });
                    }
                }
            }
        }

        [Fact]
        public void CanGetInt32()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsValue bar;
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: '1234', bar: '5678' };";
                        var result = ctx.EvaluateModule<JsObject>(script);
                        var value = result["foo"];

                        Assert.Equal(1234, value.ToInt32());
                        bar = result["bar"];
                    }

                    Assert.Equal(5678, bar.ToInt32());

                    using (ctx.Scope())
                    {
                        bar.Dispose();

                        Assert.Throws<ObjectDisposedException>(() =>
                        {
                            bar.ToInt32();
                        });
                    }
                }
            }
        }

        [Fact]
        public void CanGetString()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsValue bar;
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 1234, bar: 5678 };";
                        var result = ctx.EvaluateModule<JsObject>(script);
                        var value = result["foo"];

                        Assert.Equal("1234", value.ToString());
                        bar = result["bar"];
                    }

                    Assert.Equal("5678", bar.ToString());

                    using (ctx.Scope())
                    {
                        bar.Dispose();

                        Assert.Throws<ObjectDisposedException>(() =>
                        {
                            bar.ToString();
                        });
                    }
                }
            }
        }

        [Fact]
        public void AccessingHandleAfterDisposeThrows()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsValue bar;
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar' };";
                        var result = ctx.EvaluateModule<JsObject>(script);
                        var value = result["foo"];
                        value.Dispose();

                        Assert.Throws<ObjectDisposedException>(() =>
                        {
                            var hanDeeHugo = value.Handle;
                        });
                    }
                }
            }
        }

        [Fact]
        public void CanRepresentTypedArray()
        {
            var script = @"
var int16 = new Int16Array(2);
int16[0] = 42;

export default int16;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        JsValue result = ctx.EvaluateModule(script);

                        Assert.IsType<JsTypedArray>(result);

                        var typedArray = result as JsTypedArray;
                        Assert.Equal(JavaScriptTypedArrayType.Int16, typedArray.ArrayType);
                    }
                }
            }

        }
    }
}
