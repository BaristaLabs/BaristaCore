namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Xunit;

    [Collection("BaristaCore Tests")]
    public partial class BaristaTypeConversionStrategy_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public BaristaTypeConversionStrategy_Facts()
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
        public void CallingTryFromObjectWithNullArgumentsThrows()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var typeConversionStrategy = m_provider.GetRequiredService<IBaristaTypeConversionStrategy>();

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            typeConversionStrategy.TryCreatePrototypeFunction(null, typeof(Foo), out JsFunction value);
                        });

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            typeConversionStrategy.TryCreatePrototypeFunction(ctx, null, out JsFunction value);
                        });
                    }
                }
            }
        }

        #region Constructor Tests

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
        public void ConverterCanConstructObjectsWithMultipleNativeConstructors()
        {
            var script = @"
var myFoo = new Foo(1);
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleConstructors), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.NotNull(result);
                        Assert.Equal("Single double", result["whichConstructor"].ToString());
                    }
                }
            }
        }

        [Fact]
        public void ConverterCanConstructObjectsIfExcessArgumentsSupplied()
        {
            var script = @"
var myFoo = new Foo(1, 'foo');
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleConstructors), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.NotNull(result);
                        Assert.Equal("Single double", result["whichConstructor"].ToString());
                    }
                }
            }
        }

        [Fact]
        public void ConverterIgnoresConstructorsWithBaristaIgnore()
        {
            var script = @"
var myFoo = new Foo(1, 'foo');
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasIgnoredConstructors), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.NotNull(result);
                        Assert.Equal("Default Constructor", result["whichConstructor"].ToString());
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
        public void TypeConstructorsThatThrowNativelyRethrow()
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
                        ctx.Converter.TryFromObject(ctx, typeof(HasFawltyConstructor), out JsValue value);
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
        public void TypesWithoutAPublicConstructorCannotBeConstructed()
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
                        ctx.Converter.TryFromObject(ctx, typeof(HasNoPublicConstructor), out JsValue value);
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
        public void TypeConstuctorsWithUnconvertableParametersCannotBeConstructed()
        {
            var script = @"
var myFoo = new Foo('#fakehummingbird');
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasUnconvertableConstuctorParameter), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule<JsObject>(script);
                        });
                    }
                }
            }
        }
        #endregion

        #region Hierarchy Tests

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

                        var result = ctx.EvaluateModule<JsBoolean>(script);
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

                        var result = ctx.EvaluateModule<JsBoolean>(script);
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

                        var result = ctx.EvaluateModule<JsBoolean>(script);
                        Assert.True(result.ToBoolean());
                    }
                }
            }
        }
        #endregion

        #region IEnumerableTests

        [Fact]
        public void ConverterProvidesIteratorPropertyWhenTypeIsIEnumerable()
        {
            var script = @"
var myFoo = new Foo();
var stuff = [];
for(const foo of myFoo) {
  stuff.push(foo);
}
export default stuff;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(ImplementsIEnumerable), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsArray>(script);
                        Assert.NotNull(result);
                        Assert.Equal(3, result.Length);
                        Assert.Equal("apple", result[0].ToString());
                        Assert.Equal("banana", result[1].ToString());
                        Assert.Equal("cherry", result[2].ToString());
                    }
                }
            }
        }

        [Fact]
        public void ConverterIteratesOverMethodsThatReturnIEnumerable()
        {
            var script = @"
var myFoo = new Foo();
var stuff = [];
for(const foo of myFoo.getFroot()) {
  stuff.push(foo);
}
export default stuff;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasIEnumerableMethod), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsArray>(script);
                        Assert.NotNull(result);
                        Assert.Equal(3, result.Length);
                        Assert.Equal("apple", result[0].ToString());
                        Assert.Equal("banana", result[1].ToString());
                        Assert.Equal("cherry", result[2].ToString());
                    }
                }
            }
        }

        [Fact]
        public void CallingIteratorFunctionWithoutThisThrows()
        {
            var script = @"
var myFoo = new Foo();
myFoo[Symbol.iterator].call(null);
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(ImplementsIEnumerable), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule<JsArray>(script);
                        });
                    }
                }
            }
        }

        #region Test Classes
        private class ImplementsIEnumerable : IEnumerable<string>
        {
            private IList<string> froot = new List<string>() { "apple", "banana", "cherry" };

            public IEnumerator<string> GetEnumerator()
            {
                return froot.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        private class HasIEnumerableMethod
        {
            private IList<string> froot = new List<string>() { "apple", "banana", "cherry" };

            public IEnumerable<string> GetFroot()
            {
                return froot;
            }
        }
        #endregion

        #endregion

        #region Test Classes

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

        private class HasMultipleConstructors
        {
            public HasMultipleConstructors()
            {
                WhichConstructor = "Default Constructor";
            }

            public HasMultipleConstructors(int a)
            {
                WhichConstructor = "Single int";
            }

            public HasMultipleConstructors(double a)
            {
                WhichConstructor = "Single double";
            }

            public HasMultipleConstructors(string a)
            {
                WhichConstructor = "Single string";
            }

            public string WhichConstructor
            {
                get;
                private set;
            }
        }

        private class HasIgnoredConstructors
        {
            public HasIgnoredConstructors()
            {
                WhichConstructor = "Default Constructor";
            }

            [BaristaIgnore]
            public HasIgnoredConstructors(int a)
            {
                WhichConstructor = "Single int";
            }

            [BaristaIgnore]
            public HasIgnoredConstructors(double a)
            {
                WhichConstructor = "Single double";
            }

            [BaristaIgnore]
            public HasIgnoredConstructors(string a)
            {
                WhichConstructor = "Single string";
            }

            public string WhichConstructor
            {
                get;
                private set;
            }
        }

        private class HasFawltyConstructor
        {
            public HasFawltyConstructor()
            {
                throw new InvalidOperationException("Boom!");
            }
        }

        private class HasNoPublicConstructor
        {
            private HasNoPublicConstructor()
            {

            }
        }

        private class HasUnconvertableConstuctorParameter
        {
            private HasUnconvertableConstuctorParameter()
            {
            }

            public HasUnconvertableConstuctorParameter(Hummingbird hummingbird)
            {
            }

            public class Hummingbird
            {
                public string HumTone
                {
                    get;
                    set;
                }
            }
        }
        #endregion
    }
}
