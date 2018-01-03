namespace BaristaLabs.BaristaCore.Tests
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public partial class BaristaTypeConversionStrategy_Facts
    {
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
        public void CallingAMethodWithoutThisThrows()
        {
            var script = @"
var myFoo = new Foo();
var thing = myFoo.myMethod.call(null);
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

        #region Test Classes
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
