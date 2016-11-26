namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Xunit;

    public class JavaScriptContext_Facts
    {
        private IJavaScriptEngine Engine;

        public JavaScriptContext_Facts()
        {
            Engine = JavaScriptEngineFactory.CreateChakraEngine();
        }

        //[Fact]
        //public void JavaScriptContextCanBeCreated()
        //{
        //    using (var rt = JavaScriptRuntime.CreateJavaScriptRuntime(Engine))
        //    {
        //        using (var ctx = rt.CreateContext())
        //        {

        //        }
        //    }
        //    Assert.True(true);
        //}

        [Fact]
        public void JavaScriptContextShouldEvaluateScriptText()
        {
            //dynamic result;
            //using (var rt = new JavaScriptRuntime())
            //{
            //    using (var ctx = rt.CreateContext())
            //    {
            //        using (var xc = ctx.AcquireExecutionContext())
            //        {
            //            var fn = ctx.EvaluateScriptText("41+1;");
            //            result = fn.Invoke();
            //        }
            //    }
            //}
            //Assert.True((int)result == 42);
        }

        [Fact]
        public void JsPropertyCanBeRetrievedByName()
        {
            //            var script = @"( () => { return {
            //    foo: 'bar',
            //    baz: 'qix'
            //  };
            //})()";
            //            string result;

            //using (var rt = new JavaScriptRuntime())
            //{
            //    using (var ctx = rt.CreateContext())
            //    {
            //        using (var xc = ctx.AcquireExecutionContext())
            //        {
            //            JavaScriptValueSafeHandle obj;
            //            Errors.ThrowIfIs(ChakraApi.Instance.JsRunScript(script, JavaScriptSourceContext.None, null, JsParseScriptAttributes.JsParseScriptAttributeNone, out obj));

            //            JavaScriptValueSafeHandle foo;
            //            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf16("foo", new UIntPtr((uint)"foo".Length), out foo));

            //            JavaScriptValueSafeHandle propertyHandle;
            //            Errors.ThrowIfIs(ChakraApi.Instance.JsGetIndexedProperty(obj, foo,out propertyHandle));
            //            UIntPtr size;
            //            Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf16(propertyHandle, 0, -1, null, out size));

            //            if ((int)size * 2 > int.MaxValue)
            //                throw new OutOfMemoryException("Exceeded maximum string length.");

            //            byte[] propertyValue = new byte[(int)size * 2];
            //            UIntPtr written;
            //            Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf16(propertyHandle, 0, -1, propertyValue, out written));
            //            result = Encoding.Unicode.GetString(propertyValue, 0, propertyValue.Length);
            //        }
            //    }
            //}

            //Assert.True("bar" == result);
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
