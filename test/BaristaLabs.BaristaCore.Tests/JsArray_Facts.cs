namespace BaristaLabs.BaristaCore.Tests
{
    using System.Collections.Generic;
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Xunit;
    using System.Collections;

    [ExcludeFromCodeCoverage]
    public class JsArray_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public JsArray_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            m_provider = ServiceCollection.BuildServiceProvider();
        }

        public IBaristaRuntimeService BaristaRuntimeService
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeService>(); }
        }

        [Fact]
        public void JsArrayCanBeCreated()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var arr = ctx.ValueService.CreateArray(50);
                        Assert.True(arr != null);
                        Assert.Equal(JavaScript.JavaScriptValueType.Array, arr.Type);
                    }
                }
            }
        }

        [Fact]
        public void JsArrayLengthCanBeRetrieved()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var arr = ctx.ValueService.CreateArray(50);
                        Assert.Equal(50, arr.Length);
                    }
                }
            }
        }

        [Fact]
        public void JsArrayItemsCanBePopped()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default ['a', 'b', 'c'];";
                        var arr = ctx.EvaluateModule<JsArray>(script);
                        Assert.Equal(3, arr.Length);

                        var value = arr.Pop() as JsString;
                        Assert.Equal("c", value.ToString());
                        Assert.Equal(2, arr.Length);
                    }
                }
            }
        }

        [Fact]
        public void JsArrayItemsCanBePushed()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default ['a', 'b', 'c'];";
                        var arr = ctx.EvaluateModule<JsArray>(script);
                        Assert.Equal(3, arr.Length);

                        arr.Push(ctx.ValueService.CreateString("d"));

                        var values = arr.ToArray();
                        Assert.Equal(4, values.Length);
                        Assert.Equal("a", values[0].ToString());
                        Assert.Equal("b", values[1].ToString());
                        Assert.Equal("c", values[2].ToString());
                        Assert.Equal("d", values[3].ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsArrayItemsCanGetByIndex()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default ['a', 'b', 'c'];";
                        var arr = ctx.EvaluateModule<JsArray>(script);
                        Assert.Equal(3, arr.Length);

                        var item = arr[2] as JsString;
                        Assert.NotNull(item);
                        Assert.Equal("c", item.ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsArrayItemsCanSetByIndex()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default ['a', 'b', 'c'];";
                        var arr = ctx.EvaluateModule<JsArray>(script);
                        Assert.Equal(3, arr.Length);

                        arr[0] = ctx.ValueService.CreateString("c");

                        var values = arr.ToArray();
                        Assert.Equal(3, values.Length);
                        Assert.Equal("c", values[0].ToString());
                        Assert.Equal("b", values[1].ToString());
                        Assert.Equal("c", values[2].ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsArrayIndexOfReturnsPosition()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default ['a', 'b', 'c'];";
                        var arr = ctx.EvaluateModule<JsArray>(script);

                        var values = arr.ToArray();
                        var cVal = ctx.ValueService.CreateString("c");
                        Assert.Equal(values[2].Handle, cVal.Handle);
                        var ix = arr.IndexOf(cVal);
                        Assert.Equal(2, ix);
                    }
                }
            }
        }

        [Fact]
        public void JsArrayIndexOfReturnsPositionWhenStartIndexSupplied()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default ['c', 'a', 'b', 'c'];";
                        var arr = ctx.EvaluateModule<JsArray>(script);

                        var values = arr.ToArray();
                        var cVal = ctx.ValueService.CreateString("c");
                        Assert.Equal(values[0].Handle, cVal.Handle);
                        Assert.Equal(values[3].Handle, cVal.Handle);
                        var ix = arr.IndexOf(cVal, 1);
                        Assert.Equal(3, ix);
                    }
                }
            }
        }

        [Fact]
        public void JsArrayItemsCanBeEnumerated()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default ['a', 'b', 'c'];";
                        var arr = ctx.EvaluateModule<JsArray>(script);

                        Assert.Equal(3, arr.Length);

                        var values = new List<string>();
                        var enumerator = ((IEnumerable)arr).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            values.Add(enumerator.Current.ToString());
                        }

                        Assert.Equal(3, values.Count);
                        Assert.Equal("a", values[0]);
                        Assert.Equal("b", values[1]);
                        Assert.Equal("c", values[2]);
                    }
                }
            }
        }

        [Fact]
        public void JsArrayItemsCanBeEnumeratedViaLinq()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default ['a', 'b', 'c'];";
                        var arr = ctx.EvaluateModule<JsArray>(script);

                        Assert.Equal(3, arr.Length);

                        var values = arr.ToArray();
                        Assert.Equal(3, values.Length);
                        Assert.Equal("a", values[0].ToString());
                        Assert.Equal("b", values[1].ToString());
                        Assert.Equal("c", values[2].ToString());
                    }
                }
            }
        }
    }
}