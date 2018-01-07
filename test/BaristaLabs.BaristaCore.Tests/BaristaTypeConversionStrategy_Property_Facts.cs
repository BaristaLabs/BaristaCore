namespace BaristaLabs.BaristaCore.Tests
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public partial class BaristaTypeConversionStrategy_Facts
    {
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
                        ctx.GlobalObject["Foo"] = (value as JsFunction);
                        var fnFoo = (value as JsFunction);
                        Assert.Equal(2, fnFoo.Keys.Length); //myStaticProperty, myStaticMethod.

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.Equal("test123", Foo.MyStaticProperty);
                        Assert.True(ctx.Object.InstanceOf(result, fnFoo));
                        Assert.Equal("123Test", result["myProperty"].ToString());

                        Assert.Equal(2, result.Keys.Length);
                        Assert.Equal("myProperty", result.Keys[0].ToString());
                        Assert.Equal("myMethod", result.Keys[1].ToString());
                        var stringified = ctx.JSON.Stringify(result);
                        dynamic json = JsonConvert.DeserializeObject(stringified);
                        Assert.Equal("123Test", (string)json.myProperty);
                    }
                }
            }
        }

        [Fact]
        public void ConverterExposesIndexerPropertiesAsMethods()
        {
            var script = @"
var myFoo = new Foo();
var thing = myFoo.getItemAt(1);
myFoo.setItemAt(2, 'cantaloupe');
thing = myFoo.getItemAt(2);
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasIndexerProperty), out JsValue value);
                        ctx.GlobalObject["Foo"] = (value as JsObject);

                        var objFoo = ctx.EvaluateModule<JsObject>(script);
                        Assert.Equal(2, objFoo.Keys.Length);
                        Assert.Equal("getItemAt", objFoo.Keys[0].ToString());
                        Assert.Equal("setItemAt", objFoo.Keys[1].ToString());

                        if (objFoo.TryGetBean(out JsExternalObject bean))
                        {
                            var obj = bean.Target as HasIndexerProperty;
                            Assert.Equal("cantaloupe", obj[2]);
                        }
                        else
                        {
                            Assert.True(false, "Should be able to retrieve the bean.");
                        }
                    }
                }
            }
        }

        [Fact]
        public void ConverterExposesIndexerPropertiesThatThrowIfInsufficientArguments()
        {
            var script = @"
var myFoo = new Foo();
var thing = myFoo.getItemAt();
export default myFoo;
";

            var script1 = @"
var myFoo = new Foo();
myFoo.setItemAt(1);
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasIndexerProperty), out JsValue value);
                        ctx.GlobalObject["Foo"] = (value as JsObject);

                        Assert.Throws<JsScriptException>(() =>
                        {
                            ctx.EvaluateModule<JsObject>(script);
                        });

                        ctx.CurrentScope.GetAndClearException();

                        Assert.Throws<JsScriptException>(() =>
                        {
                            ctx.EvaluateModule<JsObject>(script1);
                        });
                    }
                }
            }
        }

        [Fact]
        public void ConverterExposesIndexerPropertiesThatThrowIfIncorrectArguments()
        {
            var script = @"
var myFoo = new Foo();
var thing = myFoo.getItemAt('foo');
export default myFoo;
";

            var script1 = @"
var myFoo = new Foo();
myFoo.setItemAt('foo', 'bar');
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasIndexerProperty), out JsValue value);
                        ctx.GlobalObject["Foo"] = (value as JsObject);

                        Assert.Throws<JsScriptException>(() =>
                        {
                            ctx.EvaluateModule<JsObject>(script);
                        });

                        ctx.CurrentScope.GetAndClearException();

                        Assert.Throws<JsScriptException>(() =>
                        {
                            ctx.EvaluateModule<JsObject>(script1);
                        });
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
        public void ClassesThatExposeJsValuePropertiesCanBeSet()
        {
            var script = @"
var myFoo = new Foo();
myFoo.myJsValue = '123Test';
export default myFoo;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasHybridProperties), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.IsType<JsString>(result["myJsValue"]);
                        Assert.True(ctx.CreateString("123Test").StrictEquals(result["myJsValue"]));
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
        public void CallingASetterOrGetterWithoutThisThrows()
        {
            var script = @"
var myFoo = new Foo();
Object.getOwnPropertyDescriptor(myFoo, 'myProperty').get.call(null);
export default myFoo;
";
            var script1 = @"
var myFoo = new Foo();
Object.getOwnPropertyDescriptor(myFoo, 'myProperty').set.call(null);
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
        public void CallingAIndexerSetOrGetMethodWithoutThisThrows()
        {
            var script = @"
var myFoo = new Foo();
var thing = myFoo.getItemAt.call(null);
export default thing;
";
            var script1 = @"
var myFoo = new Foo();
var thing = myFoo.setItemAt.call(null);
export default thing;
";

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasIndexerProperty), out JsValue value);
                        ctx.GlobalObject["Foo"] = (value as JsObject);

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
        public void CallingAIndexerSetOrGetMethodWithNullArgsThrows()
        {
            var script = @"
var myFoo = new Foo();
var thing = myFoo.getItemAt.apply(myFoo, null);
export default thing;
";
            var script1 = @"
var myFoo = new Foo();
var thing = myFoo.setItemAt.apply(myFoo, null);
export default thing;
";

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasIndexerProperty), out JsValue value);
                        ctx.GlobalObject["Foo"] = (value as JsObject);

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

        private class HasIndexerProperty
        {
            private List<string> m_froot = new List<string>() { "apple", "banana", "cherry" };

            public string this[int ix]
            {
                get
                {
                    return m_froot[ix];
                }
                set
                {
                    m_froot[ix] = value;
                }
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

        private class HasHybridProperties
        {
            public JsObject MyJsValue
            {
                get;
                set;
            }
        }

        #endregion
    }
}
