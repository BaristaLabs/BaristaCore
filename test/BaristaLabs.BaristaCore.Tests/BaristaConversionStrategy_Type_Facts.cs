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

        [Fact]
        public void ConverterExposesTypeHierarchies()
        {
            var script = @"
var rect = new Rectangle();
rect.move(1, 1);
export default (rect instanceof Rectangle);
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(Rectangle), out JsValue jsRectangle);
                        ctx.GlobalObject["Rectangle"] = jsRectangle;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.True(result.ToBoolean());
                    }
                }
            }
        }

        [Fact]
        public void ConverterExposesTypeHierarchiesDepth3()
        {
            var script = @"
var square = new Square();
square.move(1, 1);
square.width = 15;
square.height = 15;
export default (square instanceof Square);
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(Square), out JsValue jsSquare);
                        ctx.GlobalObject["Square"] = jsSquare;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.True(result.ToBoolean());
                    }
                }
            }
        }

        [Fact]
        public void ConverterExposesTypeHierarchiesInheritanceIsPreserved()
        {
            var script = @"
var square = new Square();
square.move(1, 1);
square.width = 15;
square.height = 15;
export default (square instanceof Square) && (square instanceof Rectangle) && (square instanceof Shape);
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(Square), out JsValue jsSquare);
                        ctx.Converter.TryFromObject(ctx, typeof(Rectangle), out JsValue jsRectangle);
                        ctx.Converter.TryFromObject(ctx, typeof(Shape), out JsValue jsShape);
                        ctx.GlobalObject["Square"] = jsSquare;
                        ctx.GlobalObject["Rectangle"] = jsRectangle;
                        ctx.GlobalObject["Shape"] = jsShape;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.True(result.ToBoolean());
                    }
                }
            }
        }

        #region Test Classes
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

        private class Shape
        {
            public Shape()
            {
                X = 0;
                Y = 0;
            }

            public int X
            {
                get;
                set;
            }

            public int Y
            {
                get;
                set;
            }

            public void Move(int x, int y)
            {
                X += x;
                Y += y;
            }
        }

        private class Rectangle : Shape
        {
            public int Width
            {
                get;
                set;
            }

            public int Height
            {
                get;
                set;
            }
        }

        private class Square : Rectangle
        {

        }
        #endregion
    }
}
