namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Xunit;

    public class JavaScriptContext_Facts
    {
        private IServiceProvider Provider;

        public JavaScriptContext_Facts()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore();

            Provider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void JavaScriptContextCanBeCreated()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {

                }
            }
            Assert.True(true);
        }

        [Fact]
        public void MultipleJavaScriptContextsCanBeCreated()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                var ctx1 = rt.CreateContext();
                var ctx2 = rt.CreateContext();

                Assert.NotEqual(ctx1, ctx2);
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetFalse()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.False);
                        Assert.False(ctx.False);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetNull()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.Null);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetTrue()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.True);
                        Assert.True(ctx.True);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptContextShouldGetUndefined()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        Assert.NotNull(ctx.Undefined);
                    }
                }
            }
        }

        [Fact]
        public void JavaScriptContextCanCreateString()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var jsString = ctx.CreateString("Hello, world!");
                        Assert.NotNull(jsString);
                        jsString.Dispose();
                    }
                }
            }
        }

        

        [Fact]
        public void JavaScriptContextShouldParseAndInvokeScriptText()
        {
            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "41+1";
                        var fn = ctx.ParseScriptText(script);

                        //Assert that the function is the script we passed in.
                        var fnText = fn.ToString();
                        Assert.Equal(script, fnText);

                        //Invoke it.
                        JavaScriptNumber result = (JavaScriptNumber)fn.Invoke();
                        Assert.True(result.ToInt32() == 42);
                    }
                }
            }
        }

        [Fact]
        public void JsPropertyCanBeRetrievedByName()
        {
            var script = @"var fooObj = {
    foo: 'bar',
    baz: 'qix'
};

fooObj;";

            using (var rt = JavaScriptRuntime.CreateRuntime(Provider))
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var fn = ctx.ParseScriptText(script);
                        var fooObj = fn.Invoke() as JavaScriptObject;
                        Assert.NotNull(fooObj);

                        var fooValue = fooObj.GetPropertyByName<string>("foo");
                        Assert.Equal("bar", fooValue);
                    }
                }
            }
        }

        [Fact]
        public void JsPropertyDescriptorCanBeRetrieved()
        {
            //            var script = @"( () => { return {
            //    foo: 'bar',
            //    baz: 'qix'
            //  };
            //})()";
            //            string result;

            //            using (var rt = new JavaScriptRuntime())
            //            {
            //                using (var ctx = rt.CreateContext())
            //                {
            //                    using (var xc = ctx.AcquireExecutionContext())
            //                    {
            //                        JavaScriptValueSafeHandle obj;
            //                        Errors.ThrowIfIs(ChakraApi.Instance.JsRunScript(script, JavaScriptSourceContext.None, null, JsParseScriptAttributes.JsParseScriptAttributeNone, out obj));

            //                        JavaScriptValueSafeHandle propertyDescriptor;
            //                        var propertyId = JavaScriptPropertyIdSafeHandle.FromString("foo");
            //                        Errors.ThrowIfIs(ChakraApi.Instance.JsGetOwnPropertyDescriptor(obj, propertyId, out propertyDescriptor));

            //                        dynamic desc = ctx.CreateValueFromHandle(propertyDescriptor) as JavaScriptObject;
            //                        result = (string)desc.value;
            //                    }
            //                }
            //            }

            //            Assert.True("bar" == result);
        }
    }
}
