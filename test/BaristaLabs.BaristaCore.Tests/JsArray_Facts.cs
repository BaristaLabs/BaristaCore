namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using Xunit;

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

        public IBaristaRuntimeFactory BaristaRuntimeFactory
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeFactory>(); }
        }

        [Fact]
        public void JsArrayCanBeCreated()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var arr = ctx.ValueFactory.CreateArray(50);
                        Assert.True(arr != null);
                    }
                }
            }
        }

        [Fact]
        public void JsArrayLengthCanBeRetrieved()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var arr = ctx.ValueFactory.CreateArray(50);
                        Assert.Equal(50, arr.Length);
                    }
                }
            }
        }

        [Fact]
        public void JsArrayItemsCanBePopped()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
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
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default ['a', 'b', 'c'];";
                        var arr = ctx.EvaluateModule<JsArray>(script);
                        Assert.Equal(3, arr.Length);

                        arr.Push(ctx.ValueFactory.CreateString("d"));

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
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
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
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default ['a', 'b', 'c'];";
                        var arr = ctx.EvaluateModule<JsArray>(script);
                        Assert.Equal(3, arr.Length);

                        arr[0] = ctx.ValueFactory.CreateString("c");

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
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default ['a', 'b', 'c'];";
                        var arr = ctx.EvaluateModule<JsArray>(script);

                        var values = arr.ToArray();
                        var cVal = ctx.ValueFactory.CreateString("c");
                        Assert.Equal(values[2].Handle, cVal.Handle);
                        var ix = arr.IndexOf(cVal);
                        Assert.Equal(2, ix);
                    }
                }
            }
        }

        [Fact]
        public void JsArrayItemsCanBeEnumerated()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
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