namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class JsObject_Dynamic_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public JsObject_Dynamic_Facts()
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
        public void JsValueConvertsToABoolDirectly()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default true";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.True((bool)result == true);
                    }
                }
            }
        }

        [Fact]
        public void JsValueConvertsToABoolWithCoersion()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default 1";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.True((bool)result == true);
                    }
                }
            }
        }

        [Fact]
        public void JsValueConvertsToAnIntDirectly()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default 41+1";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.True((int)result == 42);
                    }
                }
            }
        }

        [Fact]
        public void JsValueConvertsToAnIntWithCoersion()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default '42'";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.True((int)result == 42);
                    }
                }
            }
        }

        [Fact]
        public void JsValueConvertsToADoubleDirectly()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default 41.0+1.1";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.True((double)result == 42.1);
                    }
                }
            }
        }

        [Fact]
        public void JsValueConvertsToADoubleWithCoersion()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default '42.1'";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.True((double)result == 42.1);
                    }
                }
            }
        }

        [Fact]
        public void JsValueThrowsWhenCoersingToSomethingElse()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default 1";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() =>
                        {
                            var foo = (StringBuilder)result;
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsObjectCanGetDynamicProperty()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar' }";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.Equal("bar", (string)result.foo);

                        Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() =>
                        {
                            var foo = result.test;
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsObjectCanSetDynamicProperty()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar' }";
                        dynamic result = ctx.EvaluateModule(script);

                        result.baz = "qix";

                        Assert.Equal("qix", (string)result.baz);

                        Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() =>
                        {
                            result.test = new StringBuilder("test");
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsObjectCanInvokeDynamicFunction()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: (x) => 'bar' + x, baz: 'twist' }";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.Equal("bard", (string)result.foo("d"));

                        //Can't invoke undefines nor non-functions nor using unconvertable objects.
                        Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() =>
                        {
                            result.test();
                        });

                        Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() =>
                        {
                            result.baz();
                        });

                        //Can't invoke undefines nor values.
                        Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() =>
                        {
                            result.foo(new StringBuilder("test"));
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsObjectCanGetDynamicByIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar' }";
                        dynamic result = ctx.EvaluateModule(script);

                        var stringVal = result[(object)"goofy"];
                        Assert.Equal(ctx.Undefined, stringVal);

                        var intVal = result[(object)0];
                        Assert.Equal(ctx.Undefined, intVal);

                        var symbol = ctx.ValueFactory.CreateSymbol("broom");
                        var symbolVal = result[(object)symbol];
                        Assert.Equal(ctx.Undefined, symbolVal);

                        var val = ctx.ValueFactory.CreateString("window");
                        var jsVal = result[(object)val];
                        Assert.Equal(ctx.Undefined, jsVal);

                        Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() =>
                        {
                            var foo = result["123", "456"];
                        });

                        Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() =>
                        {
                            var foo = result[new StringBuilder("test")];
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsObjectCanSetDynamicByIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar' }";
                        dynamic result = ctx.EvaluateModule(script);

                        var symbol = ctx.ValueFactory.CreateSymbol("broom");
                        var jsVal = ctx.ValueFactory.CreateString("window");

                        result["baz"] = "qix";
                        result[0] = "qix";
                        result[symbol] = "qix";
                        result[jsVal] = "qix";

                        Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() =>
                        {
                            result["123","456"] = "qix";
                        });
                        Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() =>
                        {
                            result[new StringBuilder("test")] = "qix";
                        });

                        Assert.Equal("qix", (string)result.baz);
                        Assert.Equal("qix", (string)result[0]);
                        Assert.Equal("qix", (string)result[symbol]);
                        Assert.Equal("qix", (string)result[jsVal]);
                    }
                }
            }
        }

        [Fact]
        public void JsObjectCanDeleteDynamicPropertyByIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar' }";
                        dynamic result = ctx.EvaluateModule(script);

                        var symbol = ctx.ValueFactory.CreateSymbol("broom");
                        var jsVal = ctx.ValueFactory.CreateString("window");
                        result[0] = "qix";
                        result[symbol] = "qix";
                        result[jsVal] = "qix";

                        //Welp, there isn't currently C# syntax, nor a runtime binder to delete a property, so we'll do it the fun way.
                        ((DynamicObject)result).TryDeleteIndex(new MyDeleteIndexBinder(new CallInfo(0)), new object[] { "foo" });
                        Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() =>
                        {
                            var foo = result.foo;
                        });

                        ((DynamicObject)result).TryDeleteIndex(new MyDeleteIndexBinder(new CallInfo(0)), new object[] { 0 });
                        Assert.Equal(ctx.Undefined, (JsValue)result[(object)0]);

                        ((DynamicObject)result).TryDeleteIndex(new MyDeleteIndexBinder(new CallInfo(0)), new object[] { symbol });
                        Assert.Equal(ctx.Undefined, (JsValue)result[(object)symbol]);

                        ((DynamicObject)result).TryDeleteIndex(new MyDeleteIndexBinder(new CallInfo(0)), new object[] { jsVal });
                        Assert.Equal(ctx.Undefined, (JsValue)result[(object)jsVal]);

                        //Since C# doesn't implement these, it actually doesn't care when it goes to the base.TryDeleteIndex implementation.
                        ((DynamicObject)result).TryDeleteIndex(new MyDeleteIndexBinder(new CallInfo(0)), new object[] { "foo", "bar" });
                        ((DynamicObject)result).TryDeleteIndex(new MyDeleteIndexBinder(new CallInfo(0)), new object[] { new StringBuilder() });
                    }
                }
            }
        }

        [Fact]
        public void JsObjectCanInvokeDynamicProperty()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar', baz: () => 'qix' }";
                        dynamic result = ctx.EvaluateModule(script);

                        var qix = (string)result.baz();

                        Assert.Equal("qix", qix);
                    }
                }
            }
        }

        [Fact]
        public void JsObjectCanDeleteDynamicProperty()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar' }";
                        dynamic result = ctx.EvaluateModule(script);

                        //Welp, there isn't currently C# syntax, nor a runtime binder to delete a property, so we'll do it the fun way.
                        ((DynamicObject)result).TryDeleteMember(new MyDeleteMemberBinder("foo"));
                        Assert.False(((JsObject)result).HasProperty("foo"));
                    }
                }
            }
        }

        [Fact]
        public void JsObjectCanGetDynamicPropertyNames()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar' }";
                        dynamic result = ctx.EvaluateModule(script);

                        //We're kinda cheating here too, but there isn't a built-in get all names.
                        var names = ((DynamicObject)result).GetDynamicMemberNames().ToArray();
                        Assert.Single(names);
                        Assert.Equal("foo", names[0]);
                    }
                }
            }
        }

        [Fact]
        public void JsArrayCanGetDynamicProperty()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default [ 1, 3, 5 ]";
                        dynamic result = ctx.EvaluateModule(script);

                        Assert.Equal(3, (int)result[1]);
                    }
                }
            }
        }

        class MyDeleteMemberBinder : DeleteMemberBinder
        {
            public MyDeleteMemberBinder(string name)
                : base(name, false)
            {

            }

            public override DynamicMetaObject FallbackDeleteMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                throw new NotImplementedException();
            }
        }

        class MyDeleteIndexBinder : DeleteIndexBinder
        {
            public MyDeleteIndexBinder(CallInfo callInfo)
                : base(callInfo)
            {

            }

            public override DynamicMetaObject FallbackDeleteIndex(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject errorSuggestion)
            {
                throw new NotImplementedException();
            }
        }
    }
}
