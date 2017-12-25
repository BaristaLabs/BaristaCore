﻿namespace BaristaLabs.BaristaCore.Tests
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
        public void ConverterDoesNotIncludeIgnoredProperties()
        {
            var script = @"
Foo.myStaticProperty = 'test123';
var myFoo = new Foo();
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        HasIgnoredProperties.MyStaticProperty = "Not Set";
                        ctx.Converter.TryFromObject(ctx, typeof(HasIgnoredProperties), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.Equal("Not Set", HasIgnoredProperties.MyStaticProperty);
                        Assert.Equal("undefined", result["myProperty"].ToString());
                    }
                }
            }
        }

        [Fact]
        public void ConverterUsesPropertyAttributesForNaming()
        {
            var script = @"
Foo.mySuperStaticProperty = 'test123';
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
                        HasNamedProperties.MyStaticProperty = "Not Set";
                        ctx.Converter.TryFromObject(ctx, typeof(HasNamedProperties), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.Equal("test123", HasNamedProperties.MyStaticProperty);
                        Assert.Equal("Not Set", result["mySuperProperty"].ToString());
                    }
                }
            }
        }

        [Fact]
        public void FawltyPropertiesRethrow()
        {
            var script = @"
var myFoo = new Foo();
myFoo.fawlty = '123Test';
export default myFoo;
";
            var script1 = @"
var myFoo = new Foo();
var asdf = myFoo.fawlty;
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasFawltyProperty), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule<JsObject>(script);
                        });

                        var ex = ctx.CurrentScope.GetAndClearException();

                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule<JsObject>(script1);
                        });
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
        public void ConverterDoesNotIncludeIgnoredMethods()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasIgnoredMethods), out JsValue value);
                        var foo = value as JsFunction;
                        Assert.Equal("undefined", foo["myStaticMethod"].ToString());
                        var fooInstance = foo.Construct();
                        Assert.Equal("undefined", fooInstance["myMethod"].ToString());
                    }
                }
            }
        }

        [Fact]
        public void ConverterUsesMethodAttributesForNaming()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasNamedMethods), out JsValue value);
                        var foo = value as JsFunction;
                        Assert.Equal("function () { [native code] }", foo["mySuperStaticMethod"].ToString());
                        var fooInstance = foo.Construct();
                        Assert.Equal("function () { [native code] }", fooInstance["mySuperMethod"].ToString());
                    }
                }
            }
        }

        [Fact]
        public void FawltyMethodsRethrow()
        {
            var script = @"
var myFoo = new Foo();
myFoo.fawlty();
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasFawltyMethod), out JsValue value);
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
        public void ProjectedMethodsOfTheSameArityCanBeTypeDescriminated()
        {
            var script = @"
var myFoo = new Foo();
myFoo.callHome(123, 456);
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleMethods), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.Equal("Called Double", result["whichMethod"].ToString());
                    }
                }
            }
        }

        [Fact]
        public void ProjectedMethodsWillBeCalledEvenIfExcessArgumentsSupplied()
        {
            var script = @"
var myFoo = new Foo();
myFoo.callHome('123', '456', '789');
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleMethods), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.Equal("Called String", result["whichMethod"].ToString());
                    }
                }
            }
        }

        [Fact]
        public void ProjectedMethodsWithUnconvertableArgumentsCannotBeCalled()
        {
            var script = @"
var myFoo = new Foo();
myFoo.feedTheBirds('12345');
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleMethods), out JsValue value);
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

        private class HasIgnoredProperties
        {
            public HasIgnoredProperties()
            {
                MyProperty = "Not Set";
            }

            [BaristaIgnore]
            public static string MyStaticProperty
            {
                get;
                set;
            }

            [BaristaIgnore]
            public string MyProperty
            {
                get;
                set;
            }
        }

        private class HasIgnoredMethods
        {
            [BaristaIgnore]
            public static void MyStaticMethod(string value)
            {
            }

            [BaristaIgnore]
            public void MyMethod(string value)
            {
            }
        }

        private class HasNamedProperties
        {
            public HasNamedProperties()
            {
                MyProperty = "Not Set";
            }

            [BaristaProperty("mySuperStaticProperty")]
            public static string MyStaticProperty
            {
                get;
                set;
            }

            [BaristaProperty("mySuperProperty")]
            public string MyProperty
            {
                get;
                set;
            }
        }

        private class HasNamedMethods
        {
            [BaristaProperty("mySuperStaticMethod")]
            public static void MyStaticMethod(string value)
            {
            }

            [BaristaProperty("mySuperMethod")]
            public void MyMethod(string value)
            {
            }
        }

        private class HasFawltyProperty
        {
            public string Fawlty
            {
                get { throw new Exception("This is a fawlty getter"); }
                set { throw new Exception("This is a fawlty setter"); }
            }
        }

        private class HasFawltyMethod
        {
            public string Fawlty()
            {
                throw new Exception("This is a fawlty method");
            }
        }

        private class HasMultipleMethods
        {
            public string WhichMethod
            {
                get;
                set;
            }

            public void CallHome(int arg1, int arg2)
            {
                WhichMethod = "Called Int";
            }

            public void CallHome(double arg1, double arg2)
            {
                WhichMethod = "Called Double";
            }

            public void CallHome(string arg1, string arg2)
            {
                WhichMethod = "Called String";
            }

            public void FeedTheBirds(Hummingbird bird)
            {
                WhichMethod = "Hummingbird";
            }

            public class Hummingbird
            {
            }
        }
        #endregion
    }
}