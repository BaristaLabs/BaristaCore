namespace BaristaLabs.BaristaCore
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class BaristaConversionStrategy_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public BaristaConversionStrategy_Facts()
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
        public void ConverterCanBeConstructedWithJsonConverter()
        {
            var converter = new BaristaConversionStrategy(new MockJVert());
        }

        [Fact]
        public void ConverterCanConvertNulls()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                BaristaContext myContext;
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, null, out JsValue value);
                        Assert.Equal(ctx.Null, value);

                        Assert.Throws<ArgumentNullException>(() => {
                            ctx.Converter.TryFromObject(null, null, out JsValue value1);
                        });
                    }
                    myContext = ctx;
                }

                Assert.Throws<ObjectDisposedException>(() => {
                    myContext.Converter.TryFromObject(myContext, null, out JsValue value1);
                });
            }
        }

        [Fact]
        public void ConverterCanConvertUndefined()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, Undefined.Value, out JsValue value);
                        Assert.Equal(ctx.Undefined, value);
                    }
                }
            }
        }

        [Fact]
        public void ConverterCanConvertDouble()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, 42.42, out JsValue value);
                        Assert.Equal(42.42, value.ToDouble());
                    }
                }
            }
        }

        [Fact]
        public void ConverterCanConvertUInt()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, (uint)42, out JsValue value);
                        Assert.Equal(42, value.ToDouble());
                    }
                }
            }
        }

        [Fact]
        public void ConverterCanConvertLong()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, (long)42, out JsValue value);
                        Assert.Equal(42, value.ToDouble());
                    }
                }
            }
        }

        [Fact]
        public void ConverterCanConvertBool()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, true, out JsValue value);
                        Assert.Equal(ctx.True, value);

                        ctx.Converter.TryFromObject(ctx, false, out value);
                        Assert.Equal(ctx.False, value);
                    }
                }
            }
        }

        [Fact]
        public void ConverterCanConvertDelegate()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        string thingy = null;
                        ctx.Converter.TryFromObject(ctx, new Action<JsObject, string>((t, s) => thingy = s), out JsValue value);
                        var fn = value as JsFunction;
                        Assert.NotNull(fn);

                        fn.Call(null, ctx.ValueFactory.CreateString("bananas"));

                        Assert.Equal("bananas", thingy);
                    }
                }
            }
        }

        [Fact]
        public void ConverterReturnsNullOnValueTypesWithoutJsonConverter()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        
                        var result = ctx.Converter.TryFromObject(ctx, new MyStruct("bar", 42), out JsValue value);
                        Assert.False(result);
                        Assert.Null(value);
                    }
                }
            }
        }

        [Fact]
        public void ConverterConvertsValueTypeWhenJsonConverterIsSpecified()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {

                        var converter = new BaristaConversionStrategy(new MockJVert());

                        converter.TryFromObject(ctx, new MyStruct("bar", 42), out JsValue value);
                        var obj = value as JsObject;
                        Assert.NotNull(obj);

                        Assert.Equal("bar", obj["Foo"].ToString());
                        Assert.Equal(42, obj["Bar"].ToDouble());
                    }
                }
            }
        }

        class MockJVert : IJsonConverter
        {
            public object Parse(string json)
            {
                return JsonConvert.DeserializeObject(json);
            }

            public string Stringify(object obj)
            {
                return JsonConvert.SerializeObject(obj);
            }
        }

        struct MyStruct
        {
            public MyStruct(string foo, int bar)
            {
                Foo = foo;
                Bar = bar;
            }

            public string Foo;
            public int Bar;
        }
    }
}
