namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    [Collection("BaristaCore Tests")]
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
            var converter = new BaristaConversionStrategy(new JsonNetConverter());
        }

        #region FromObject
        [Fact]
        public void ConverterCanConvertFromNull()
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
        public void ConverterCanConvertFromUndefined()
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
        public void ConverterCanConvertFromDouble()
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
        public void ConverterCanConvertFromUInt()
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
        public void ConverterCanConvertFromLong()
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
        public void ConverterCanConvertFromBool()
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
        public void ConverterCanConvertFromDelegate()
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

                        fn.Call(null, ctx.CreateString("bananas"));

                        Assert.Equal("bananas", thingy);
                    }
                }
            }
        }

        [Fact]
        public void ConverterCanConvertFromTask()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, ctx.TaskFactory.StartNew(async () => {
                            await Task.Delay(500);
                            return 42;
                        }), out JsValue value);
                        var promise = value as JsObject;
                        Assert.NotNull(promise);

                        var result = ctx.Promise.Wait(promise);
                        Assert.Equal(42, result.ToInt32());
                    }
                }
            }
        }

        [Fact]
        public void ConverterConvertsFromValueTypes_NullWithoutJsonConverter()
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
        public void ConverterConvertsFromValueTypes_ObjectWithJsonConverter()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {

                        var converter = new BaristaConversionStrategy(new JsonNetConverter());

                        converter.TryFromObject(ctx, new MyStruct("bar", 42), out JsValue value);
                        var obj = value as JsObject;
                        Assert.NotNull(obj);

                        Assert.Equal("bar", obj["Foo"].ToString());
                        Assert.Equal(42, obj["Bar"].ToDouble());
                    }
                }
            }
        }

        [Fact]
        public void ConverterCanConvertIEnumerable()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var froot = new List<string>() { "apple", "banana", "cherry" };
                        ctx.Converter.TryFromObject(ctx, froot, out JsValue value);
                        var objList = value as JsObject;
                        Assert.NotNull(objList);

                        var fnIteratorGenerator = objList.GetProperty(ctx.Symbol.Iterator) as JsFunction;
                        Assert.NotNull(fnIteratorGenerator);

                        var iterator = fnIteratorGenerator.Call(value as JsObject) as JsObject;

                        var fnNext = iterator["next"] as JsFunction;
                        Assert.NotNull(fnNext);

                        var currentValue = fnNext.Call(null) as JsObject;
                        Assert.False(currentValue["done"].ToBoolean());
                        Assert.Equal("apple", currentValue["value"].ToString());

                        currentValue = fnNext.Call(null) as JsObject;
                        Assert.False(currentValue["done"].ToBoolean());
                        Assert.Equal("banana", currentValue["value"].ToString());

                        currentValue = fnNext.Call(null) as JsObject;
                        Assert.False(currentValue["done"].ToBoolean());
                        Assert.Equal("cherry", currentValue["value"].ToString());

                        //We're done.
                        currentValue = fnNext.Call(null) as JsObject;
                        Assert.True(currentValue["done"].ToBoolean());
                        Assert.Same(ctx.Undefined, currentValue["value"]);

                        //Still done.
                        currentValue = fnNext.Call(null) as JsObject;
                        Assert.True(currentValue["done"].ToBoolean());
                        Assert.Same(ctx.Undefined, currentValue["value"]);
                    }
                }
            }
        }
        #endregion

        #region ToObject
        [Fact]
        public void ConverterConvertsToNull()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                BaristaContext myContext;
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryToObject(ctx, ctx.Null, out object value);
                        Assert.Null(value);

                        ctx.Converter.TryToObject(ctx, null, out object value1);
                    }
                    myContext = ctx;
                }

                Assert.Throws<ObjectDisposedException>(() => {
                    myContext.Converter.TryToObject(myContext, null, out object value);
                });
            }
        }

        [Fact]
        public void ConverterConvertsToUndefined()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                BaristaContext myContext;
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryToObject(ctx, ctx.Undefined, out object value);
                        Assert.Equal(Undefined.Value, value);
                    }
                    myContext = ctx;
                }
            }
        }

        [Fact]
        public void ConverterConvertsToBoolean()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                BaristaContext myContext;
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryToObject(ctx, ctx.True, out object value);
                        Assert.True((bool)value);
                    }
                    myContext = ctx;
                }
            }
        }

        [Fact]
        public void ConverterConvertsToArray()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                BaristaContext myContext;
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var myArray = ctx.CreateArray(0);
                        myArray.Push(ctx.CreateString("Foo"));

                        ctx.Converter.TryToObject(ctx, myArray, out object value);
                        Assert.NotNull(value);
                        var valueArr = value as Array;
                        Assert.NotNull(valueArr);
                        Assert.Single(valueArr);
                    }
                    myContext = ctx;
                }
            }
        }

        [Fact]
        public void ConverterConvertsToException()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                BaristaContext myContext;
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryToObject(ctx, ctx.CreateError("Err"), out object value);
                        Assert.NotNull(value);
                        Assert.IsType<JsScriptException>(value);
                        Assert.Equal("Err", ((JsScriptException)value).m_message);
                    }
                    myContext = ctx;
                }
            }
        }
        #endregion

        private struct MyStruct
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
