namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class BaristaConversionStrategy_Type_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public BaristaConversionStrategy_Type_Facts()
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
        public void ConverterCanConvertsTypesIntoFunctions()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(Foo), out JsValue value);
                        Assert.IsType<JsNativeFunction>(value);
                    }
                }
            }
        }

        [Fact]
        public void ConverterCanConvertTypesThatCanBeConstructed()
        {
            var script = @"
var myFoo = new Foo();
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(Foo), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.NotNull(result);
                    }
                }
            }
        }

        [Fact]
        public void TypeConstructorsThrowIfCalledWithoutNew()
        {
            var script = @"
var myFoo = Foo();
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(Foo), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule<JsObject>(script);
                        });
                    }
                }
            }
        }

        [Fact]
        public void ConverterExposesStaticAndInstanceProperties()
        {
            var script = @"
Foo.myStaticProperty = 'test123';
var myFoo = new Foo();
myFoo.myProperty = '123Test';
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Foo.MyStaticProperty = null;
                        ctx.Converter.TryFromObject(ctx, typeof(Foo), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.Equal("test123", Foo.MyStaticProperty);
                        Assert.Equal("123Test", result["myProperty"].ToString());
                    }
                }
            }
        }

        [Fact]
        public void ConverterExposesStaticAndInstanceMethods()
        {
            var script = @"
Foo.myStaticMethod('test123');
var myFoo = new Foo();
myFoo.myMethod('123Test');
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Foo.MyStaticProperty = null;
                        ctx.Converter.TryFromObject(ctx, typeof(Foo), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.Equal("test123", Foo.MyStaticProperty);
                        Assert.Equal("123Test", result["myProperty"].ToString());
                    }
                }
            }
        }

        private class Foo
        {
            public Foo()
            {
            }

            public static string MyStaticProperty
            {
                get;
                set;
            }

            public string MyProperty
            {
                get;
                set;
            }

            public static void MyStaticMethod(string value)
            {
                MyStaticProperty = value;
            }

            public void MyMethod(string value)
            {
                MyProperty = value;
            }
        }
    }
}
