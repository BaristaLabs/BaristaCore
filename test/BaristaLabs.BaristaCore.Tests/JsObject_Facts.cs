namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class JsObject_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public JsObject_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            m_provider = ServiceCollection.BuildServiceProvider();
        }

        public IBaristaRuntimeFactory BaristaRuntimeFactory
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeFactory>(); }
        }

        #region Constructor Facts
        [Fact]
        public void JsObjectCanBeCreated()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var obj = ctx.ValueFactory.CreateObject();
                        Assert.True(obj != null);
                        Assert.Equal(JsValueType.Object, obj.Type);
                    }
                }
            }
        }

        [Fact]
        public void JsObjectConstructorThrowsWhenHandleIsNull()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            new JsObject(rt.Engine, ctx, null);
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsObjectConstructorThrowsWhenValueFactoryIsNull()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            new JsObject(rt.Engine, ctx, null);
                        });
                    }
                }
            }
        }
        #endregion

        #region HasProperty Facts
        [Fact]
        public void JsObjectReportsHasProperty()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.True(result.HasProperty("foo"));
                        Assert.True(result.HasProperty("toString"));
                    }
                }
            }
        }

        [Fact]
        public void JsObjectReportsHasOwnProperty()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.True(result.HasOwnProperty("foo"));
                        Assert.False(result.HasOwnProperty("toString"));
                    }
                }
            }
        }

        [Fact]
        public void JsObjectKeysCanBeRetrieved()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var keys = result.Keys;
                        Assert.True(keys != null);
                        Assert.Equal(1, keys.Length);
                        Assert.Equal("foo", keys[0].ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsObjectIsExtensible()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        Assert.True(result.IsExtensible);

                        ctx.Object.Freeze(result);

                        Assert.False(result.IsExtensible);
                    }
                }
            }
        }
        #endregion

        #region Property Retrieval Facts (by index value type)
        [Fact]
        public void JsObjectPropertyCanBeRetrievedByStringIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsObject result;
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar'};";
                        result = ctx.EvaluateModule<JsObject>(script);

                        var bar = result["foo"];
                        Assert.True(bar != null);
                        Assert.Equal("bar", bar.ToString());

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            var foo = result[""];
                        });
                    }

                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        var foo = result["foo"];
                    });
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeRetrievedByNumericIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsObject result;
                    using (ctx.Scope())
                    {
                        var script = "export default { 0: 'bar'};";
                        result = ctx.EvaluateModule<JsObject>(script);

                        var bar = result[0];
                        Assert.True(bar != null);
                        Assert.Equal("bar", bar.ToString());
                    }

                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        var foo = result[0];
                    });
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeRetrievedBySymbolIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsObject result;
                    JsSymbol symbol;
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        result = ctx.EvaluateModule<JsObject>(script);

                        symbol = ctx.ValueFactory.CreateSymbol("baz");
                        result.SetProperty(symbol, ctx.ValueFactory.CreateString("qix"));

                        var qix = result[symbol];
                        Assert.True(qix != null);
                        Assert.Equal("qix", qix.ToString());

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            var foo = result[(JsSymbol)null];
                        });
                    }

                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        var foo = result[symbol];
                    });
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeRetrievedByValueIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsObject result;
                    JsValue value;
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar'};";
                        result = ctx.EvaluateModule<JsObject>(script);
                        value = ctx.ValueFactory.CreateString("foo");

                        var bar = result[value];
                        Assert.True(bar != null);
                        Assert.Equal("bar", bar.ToString());

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            var foo = result[(JsValue)null];
                        });
                    }

                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        var foo = result[value];
                    });
                }
            }
        }
        #endregion

        #region Property Strongly Typed Retrieval Facts

        [Fact]
        public void JsObjectGetPropertySymbolIsStronglyTyped()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var obj = ctx.EvaluateModule<JsObject>(script);
                        var symbol = ctx.ValueFactory.CreateSymbol("baz");
                        var val = ctx.ValueFactory.CreateString("football");

                        obj.SetProperty(symbol, val);

                        var result = obj.GetProperty<JsString>(symbol);
                        Assert.Equal(val, result);
                    }
                }
            }
        }

        [Fact]
        public void JsObjectGetPropertyValueIsStronglyTyped()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var obj = ctx.EvaluateModule<JsObject>(script);
                        var propertyName = ctx.ValueFactory.CreateString("baz");
                        var val = ctx.ValueFactory.CreateString("football");

                        obj.SetProperty(propertyName, val);

                        var result = obj.GetProperty<JsString>(propertyName);
                        Assert.Equal(val, result);
                    }
                }
            }
        }

        [Fact]
        public void JsObjectGetPropertyIntIsStronglyTyped()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var obj = ctx.EvaluateModule<JsObject>(script);
                        var val = ctx.ValueFactory.CreateString("football");

                        obj.SetProperty(1, val);

                        var result = obj.GetProperty<JsString>(1);
                        Assert.Equal(val, result);
                    }
                }
            }
        }
        #endregion

        #region Property Set Facts (by index value type)
        [Fact]
        public void JsObjectPropertyCanBeSetByStringIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var strValue = ctx.ValueFactory.CreateString("baz");
                        var qixValue = ctx.ValueFactory.CreateString("qix");

                        result[strValue] = qixValue;

                        Assert.True(result.HasOwnProperty("baz"));
                    }
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeSetByIntIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsObject result;
                    JsObject qixValue;
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        result = ctx.EvaluateModule<JsObject>(script);

                        var indexValue = ctx.ValueFactory.CreateNumber(0);
                        qixValue = ctx.ValueFactory.CreateString("qix");

                        result[indexValue] = qixValue;

                        Assert.Equal("qix", result[0].ToString());

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            result.SetProperty(0, (JsValue)null);
                        });
                    }

                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        result.SetProperty(0, qixValue);
                    });
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeSetBySymbolIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var symbol = ctx.ValueFactory.CreateSymbol("baz");
                        result[symbol] = ctx.ValueFactory.CreateString("qix");

                        Assert.Equal("qix", result[symbol].ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeSetByValueIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var jsVal = ctx.ValueFactory.CreateString("baz");
                        result[jsVal] = ctx.ValueFactory.CreateString("qix");

                        Assert.Equal("qix", result[jsVal].ToString());
                    }
                }
            }
        }
        #endregion

        #region Property Set Negative Facts

        [Fact]
        public void JsObjectSetPropertySymbolThrowsIfNull()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            result.SetProperty((JsSymbol)null, ctx.ValueFactory.CreateString("qix"));
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsObjectSetPropertySymbolThrowsIfValueNull()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var symbol = ctx.ValueFactory.CreateSymbol("baz");

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            result.SetProperty(symbol, (JsValue)null);
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsObjectSetPropertySymbolThrowsIfNotInScope()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsObject obj;
                    JsSymbol symbol;
                    JsValue val;
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        obj = ctx.EvaluateModule<JsObject>(script);
                        symbol = ctx.ValueFactory.CreateSymbol("baz");
                        val = ctx.ValueFactory.CreateString("football");
                    }

                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        obj.SetProperty(symbol, val);
                    });
                }
            }
        }

        [Fact]
        public void JsObjectSetPropertyValueThrowsIfNull()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            result.SetProperty((JsValue)null, ctx.ValueFactory.CreateString("qix"));
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsObjectSetPropertyValueThrowsIfValueNull()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var val = ctx.ValueFactory.CreateString("baz");

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            result.SetProperty(val, (JsValue)null);
                        });
                    }
                }
            }
        }

        [Fact]
        public void JsObjectSetPropertyValueThrowsIfNotInScope()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsObject obj;
                    JsValue propertyName;
                    JsValue val;
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        obj = ctx.EvaluateModule<JsObject>(script);
                        propertyName = ctx.ValueFactory.CreateSymbol("baz");
                        val = ctx.ValueFactory.CreateString("football");
                    }

                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        obj.SetProperty(propertyName, val);
                    });
                }
            }
        }
        #endregion

        #region Property Delete Facts (by index value type)

        [Fact]
        public void JsObjectPropertyCanBeDeletedByIntIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsObject result;
                    using (ctx.Scope())
                    {
                        var script = "export default { 0: 'bar'};";
                        result = ctx.EvaluateModule<JsObject>(script);

                        Assert.True(result.HasProperty("0"));

                        result.DeleteProperty(0);
                        
                        Assert.False(result.HasProperty("0"));
                    }

                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        result.DeleteProperty(0);
                    });
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeDeletedByStringIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsObject result;
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        result = ctx.EvaluateModule<JsObject>(script);

                        Assert.True(result.HasProperty("foo"));

                        result.DeleteProperty("foo");

                        Assert.False(result.HasProperty("foo"));

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            result.DeleteProperty("");
                        });
                    }

                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        result.DeleteProperty("foo");
                    });
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeDeletedBySymbolIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsSymbol symbol;
                    JsObject result;
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        result = ctx.EvaluateModule<JsObject>(script);

                        symbol = ctx.ValueFactory.CreateSymbol("baz");
                        result.SetProperty(symbol, ctx.ValueFactory.CreateString("qix"));

                        Assert.Equal("qix", result.GetProperty(symbol).ToString());

                        result.DeleteProperty(symbol);

                        Assert.Equal(ctx.Undefined, result.GetProperty(symbol));

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            result.DeleteProperty((JsSymbol)null);
                        });
                    }

                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        result.DeleteProperty(symbol);
                    });
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeDeletedByJsValueIndexer()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    JsValue val;
                    JsObject result;
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        result = ctx.EvaluateModule<JsObject>(script);

                        val = ctx.ValueFactory.CreateString("foo");
                        Assert.True(result.HasProperty("foo"));

                        result.DeleteProperty(val);

                        Assert.False(result.HasProperty("foo"));

                        Assert.Throws<ArgumentNullException>(() =>
                        {
                            result.DeleteProperty((JsValue)null);
                        });
                    }

                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        result.DeleteProperty(val);
                    });
                }
            }
        }

        #endregion

    }
}
