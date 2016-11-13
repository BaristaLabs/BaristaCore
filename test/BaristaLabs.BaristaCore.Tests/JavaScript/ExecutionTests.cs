namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Interop;
    using Interop.Callbacks;
    using Interop.SafeHandles;
    using Extensions;
    using System;
    using System.Text;
    using Xunit;

    public class BasicExecutionFacts
    {
        [Fact]
        public void JavaScriptRuntimeCanBeConstructed() 
        {
            using (var rt = new JavaScriptRuntime())
            {
            }
            Assert.True(true);
        }

        [Fact]
        public void JavaScriptRuntimeShouldReturnRuntimeMemoryUsage()
        {
            ulong memoryUsage;
            using (var rt = new JavaScriptRuntime())
            {
                memoryUsage = rt.RuntimeMemoryUsage;
            }
            Assert.True(memoryUsage > 0);
        }

        [Fact]
        public void JavaScriptContextCanBeCreated()
        {
            using (var rt = new JavaScriptRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    
                }
            }
            Assert.True(true);
        }

        [Fact]
        public void JavaScriptRuntimeShouldFireMemoryChangingCallbacks()
        {
            int changeCount = 0;
            EventHandler<JavaScriptMemoryEventArgs> handler = (sender, e) =>
            {
                changeCount++;
            };

            using (var rt = new JavaScriptRuntime())
            {
                rt.MemoryChanging += handler;
                changeCount = 0;
                using (var ctx = rt.CreateContext())
                {
                }
                rt.MemoryChanging -= handler;
            }

            Assert.True(changeCount > 0);
        }

        [Fact]
        public void JavaScriptContextShouldEvaluateScriptText()
        {
            dynamic result;
            using (var rt = new JavaScriptRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (var xc = ctx.AcquireExecutionContext())
                    {
                        var fn = ctx.EvaluateScriptText("41+1;");
                        result = fn.Invoke();
                    }
                }
            }
            Assert.True((int)result == 42);
        }

        [Fact]
        public void JsCreateStringCopyStringInvocationIsCorrect()
        {
            var str = "Hello, World!";
            string resultStr;

            using (var rt = new JavaScriptRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (var xc = ctx.AcquireExecutionContext())
                    {
                        JavaScriptValueSafeHandle handle;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCreateString(str, new UIntPtr((uint)str.Length), out handle));

                        UIntPtr size;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCopyString(handle, 0, -1, null, out size));
                        if ((int)size > int.MaxValue)
                            throw new OutOfMemoryException("Exceeded maximum string length.");

                        byte[] result = new byte[(int)size];
                        UIntPtr written;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCopyString(handle, 0, -1, result, out written));
                        resultStr = Encoding.ASCII.GetString(result, 0, result.Length);
                    }
                }
            }

            Assert.True(str == resultStr);
        }

        [Fact]
        public void JsCreateStringUtf8CopyStringUtf8InvocationIsCorrect()
        {
            var str = "Hello, World!";
            string resultStr;

            using (var rt = new JavaScriptRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (var xc = ctx.AcquireExecutionContext())
                    {
                        JavaScriptValueSafeHandle handle;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(str, new UIntPtr((uint)str.Length), out handle));

                        UIntPtr size;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf8(handle, null, UIntPtr.Zero, out size));
                        if ((int)size > int.MaxValue)
                            throw new OutOfMemoryException("Exceeded maximum string length.");

                        byte[] result = new byte[(int)size];
                        UIntPtr written;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf8(handle, result, new UIntPtr((uint)result.Length), out written));
                        resultStr = Encoding.UTF8.GetString(result, 0, result.Length);
                    }
                }
            }

            Assert.True(str == resultStr);
        }

        [Fact]
        public void JsCreateStringUtf16CopyStringUtf16InvocationIsCorrect()
        {
            var str = "Hello, World!";
            string resultStr;

            using (var rt = new JavaScriptRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (var xc = ctx.AcquireExecutionContext())
                    {
                        JavaScriptValueSafeHandle handle;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf16(str, new UIntPtr((uint)str.Length), out handle));

                        UIntPtr size;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf16(handle, 0, -1, null, out size));
                        if ((int)size * 2 > int.MaxValue)
                            throw new OutOfMemoryException("Exceeded maximum string length.");

                        byte[] result = new byte[(int)size * 2];
                        UIntPtr written;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf16(handle, 0, -1, result, out written));
                        resultStr = Encoding.Unicode.GetString(result, 0, result.Length);
                    }
                }
            }

            
            Assert.True(str == resultStr);
        }

        [Fact]
        public void JsParseInvocationIsCorrect()
        {
            var script = "(()=>{return 6*7;})()";
            int result;

            using (var rt = new JavaScriptRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (var xc = ctx.AcquireExecutionContext())
                    {
                        JavaScriptValueSafeHandle resultHandle;

                        try
                        {
                            Errors.ThrowIfIs(ChakraApi.Instance.JsParseScript(script, JavaScriptSourceContext.None, null, JsParseScriptAttributes.JsParseScriptAttributeNone, out resultHandle));
                            var fn = ctx.CreateValueFromHandle(resultHandle) as JavaScriptFunction;

                            dynamic resultValue = fn.Invoke();
                            result = (int)resultValue;
                        }
                        catch (Exception)
                        {
                            bool hasException;
                            ChakraApi.Instance.JsHasException(out hasException);

                            if (hasException)
                            {
                                JavaScriptValueSafeHandle jsEx;
                                ChakraApi.Instance.JsGetAndClearException(out jsEx);
                                dynamic ex = ctx.CreateValueFromHandle(jsEx);
                                throw new Exception(String.Format("{0} occured at {1} {2}", (string)ex, (int)ex.line, (int)ex.column));
                            }

                            throw;
                        }
                    }
                }
            }


            Assert.True(42 == result);
        }

        [Fact]
        public void JsRunInvocationIsCorrect()
        {
            var script = "(()=>{return 6*7;})()";
            int result;

            using (var rt = new JavaScriptRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (var xc = ctx.AcquireExecutionContext())
                    {
                        JavaScriptValueSafeHandle resultHandle;

                        try
                        {
                            Errors.ThrowIfIs(ChakraApi.Instance.JsRunScript(script, JavaScriptSourceContext.None, null, JsParseScriptAttributes.JsParseScriptAttributeNone, out resultHandle));
                            dynamic resultValue = ctx.CreateValueFromHandle(resultHandle);
                            result = (int)resultValue;
                        }
                        catch (Exception)
                        {
                            bool hasException;
                            ChakraApi.Instance.JsHasException(out hasException);

                            if (hasException) {
                                JavaScriptValueSafeHandle jsEx;
                                ChakraApi.Instance.JsGetAndClearException(out jsEx);
                                dynamic ex = ctx.CreateValueFromHandle(jsEx);
                                throw new Exception(String.Format("{0} occured at {1} {2}", (string)ex, (int)ex.line, (int)ex.column));
                            }

                            throw;
                        }
                    }
                }
            }


            Assert.True(42 == result);
        }

        [Fact]
        public void JsSerializeInvocationIsCorrect()
        {
            var script = "(()=>{return 6*7;})()";
            int result;

            using (var rt = new JavaScriptRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (var xc = ctx.AcquireExecutionContext())
                    {
                        JavaScriptValueSafeHandle resultHandle;

                        string source = "[test]";
                        JavaScriptValueSafeHandle sourceHandle;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(source, new UIntPtr((uint)source.Length), out sourceHandle));

                        byte[] buffer = new byte[1024];
                        ulong bufferSize = (ulong)buffer.Length;
                        try
                        {
                            Errors.ThrowIfIs(ChakraApi.Instance.JsSerializeScript(script, buffer, ref bufferSize, JsParseScriptAttributes.JsParseScriptAttributeNone));

                            JavaScriptSerializedLoadScriptCallback cb = (JavaScriptSourceContext sourceContext, out JavaScriptValueSafeHandle value, out JsParseScriptAttributes parseAttributes) =>
                            {
                                value = null;
                                parseAttributes = JsParseScriptAttributes.JsParseScriptAttributeNone;
                                return true;
                            };

                            Errors.ThrowIfIs(ChakraApi.Instance.JsRunSerialized(buffer, cb, JavaScriptSourceContext.None, sourceHandle, out resultHandle));
                            dynamic resultValue = ctx.CreateValueFromHandle(resultHandle);
                            result = (int)resultValue;
                        }
                        catch (Exception)
                        {
                            bool hasException;
                            ChakraApi.Instance.JsHasException(out hasException);

                            if (hasException)
                            {
                                JavaScriptValueSafeHandle jsEx;
                                ChakraApi.Instance.JsGetAndClearException(out jsEx);
                                dynamic ex = ctx.CreateValueFromHandle(jsEx);
                                throw new Exception(String.Format("{0} occured at {1} {2}", (string)ex, (int)ex.line, (int)ex.column));
                            }

                            throw;
                        }
                    }
                }
            }


            Assert.True(42 == result);
        }

        [Fact]
        public void JsPropertyCanBeRetrievedByName()
        {
            var script = @"( () => { return {
    foo: 'bar',
    baz: 'qix'
  };
})()";
            string result;

            using (var rt = new JavaScriptRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (var xc = ctx.AcquireExecutionContext())
                    {
                        JavaScriptValueSafeHandle obj;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsRunScript(script, JavaScriptSourceContext.None, null, JsParseScriptAttributes.JsParseScriptAttributeNone, out obj));

                        JavaScriptValueSafeHandle foo;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf16("foo", new UIntPtr((uint)"foo".Length), out foo));

                        JavaScriptValueSafeHandle propertyHandle;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsGetIndexedProperty(obj, foo,out propertyHandle));
                        UIntPtr size;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf16(propertyHandle, 0, -1, null, out size));
                        
                        if ((int)size * 2 > int.MaxValue)
                            throw new OutOfMemoryException("Exceeded maximum string length.");

                        byte[] propertyValue = new byte[(int)size * 2];
                        UIntPtr written;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf16(propertyHandle, 0, -1, propertyValue, out written));
                        result = Encoding.Unicode.GetString(propertyValue, 0, propertyValue.Length);
                    }
                }
            }

            Assert.True("bar" == result);
        }

        [Fact]
        public void JsPropertyDescriptorCanBeRetrieved()
        {
            var script = @"( () => { return {
    foo: 'bar',
    baz: 'qix'
  };
})()";
            string result;

            using (var rt = new JavaScriptRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (var xc = ctx.AcquireExecutionContext())
                    {
                        JavaScriptValueSafeHandle obj;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsRunScript(script, JavaScriptSourceContext.None, null, JsParseScriptAttributes.JsParseScriptAttributeNone, out obj));

                        JavaScriptValueSafeHandle propertyDescriptor;
                        var propertyId = JavaScriptPropertyIdSafeHandle.FromString("foo");
                        Errors.ThrowIfIs(ChakraApi.Instance.JsGetOwnPropertyDescriptor(obj, propertyId, out propertyDescriptor));

                        dynamic desc = ctx.CreateValueFromHandle(propertyDescriptor) as JavaScriptObject;
                        result = (string)desc.value;
                    }
                }
            }

            Assert.True("bar" == result);
        }
    }
}
