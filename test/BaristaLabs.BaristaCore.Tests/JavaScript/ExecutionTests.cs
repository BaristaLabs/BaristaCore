namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Interfaces;
    using SafeHandles;
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
                    using (var asdf = ctx.AcquireExecutionContext())
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

                        byte[] result = new byte[(int)size];
                        UIntPtr written;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCopyString(handle, 0, -1, result, out written));
                        resultStr = Encoding.ASCII.GetString(result, 0, (int)written);
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
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCopyString(handle, 0, -1, null, out size));

                        byte[] result = new byte[(int)size];
                        UIntPtr written;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf8(handle, result, new UIntPtr((uint)result.Length), out written));
                        resultStr = Encoding.UTF8.GetString(result, 0, (int)written);
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
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf16(str, new UIntPtr((uint)str.Length * 2), out handle));

                        UIntPtr size;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCopyString(handle, 0, -1, null, out size));

                        byte[] result = new byte[(int)size];
                        UIntPtr written;
                        Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf16(handle, 0, -1, result, out written));
                        resultStr = Encoding.Unicode.GetString(result, 0, (int)written);
                    }
                }
            }

            
            Assert.True(str == resultStr);
        }
    }
}
