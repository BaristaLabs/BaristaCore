namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Internal;
    using System;
    using System.Collections.Concurrent;
    using Xunit;

    public class JavaScriptReference_Facts
    {
        private IJavaScriptEngine Jsrt;

        public JavaScriptReference_Facts()
        {
            Jsrt = JavaScriptEngineFactory.CreateChakraRuntime();
        }

        [Fact]
        public void JsContextsAreSetInvalidWhenHandleIsCollected()
        {
            JavaScriptContextSafeHandle contextHandle, anotherContextHandle;
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                contextHandle = Jsrt.JsCreateContext(runtimeHandle);
                JavaScriptObjectManager.MonitorJavaScriptObjectLifetime(contextHandle);

                Jsrt.JsSetCurrentContext(contextHandle);

                anotherContextHandle = Jsrt.JsGetCurrentContext();
                JavaScriptObjectManager.MonitorJavaScriptObjectLifetime(anotherContextHandle);

                Assert.Equal(contextHandle, anotherContextHandle);
                Assert.NotSame(contextHandle, anotherContextHandle);
            }

            Assert.True(contextHandle.IsInvalid);
            Assert.True(contextHandle.IsClosed);

            Assert.True(anotherContextHandle.IsInvalid);
            Assert.True(anotherContextHandle.IsClosed);
        }

        [Fact]
        public void JsValuesAreSetInvalidWhenHandleIsCollected()
        {
            JavaScriptValueSafeHandle valueHandle, anotherValueHandle;

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var superman = "superman";
                    valueHandle = Jsrt.JsCreateStringUtf8(superman, new UIntPtr((uint)superman.Length));
                    JavaScriptObjectManager.MonitorJavaScriptObjectLifetime(valueHandle);
                    anotherValueHandle = Jsrt.JsCreateStringUtf8(superman, new UIntPtr((uint)superman.Length));
                    JavaScriptObjectManager.MonitorJavaScriptObjectLifetime(anotherValueHandle);
                }

                Jsrt.JsCollectGarbage(runtimeHandle);
            }

            Assert.True(valueHandle.IsInvalid);
            Assert.True(valueHandle.IsClosed);

            Assert.True(anotherValueHandle.IsInvalid);
            Assert.True(anotherValueHandle.IsClosed);
        }

        [Fact]
        public void JsValuesAreSetInvalidWhenHandleIsCollectedWithDispose()
        {
            JavaScriptValueSafeHandle valueHandle;

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    valueHandle = Jsrt.JsCreateStringUtf8("superman", new UIntPtr((uint)"superman".Length));
                    JavaScriptObjectManager.MonitorJavaScriptObjectLifetime(valueHandle);
                    valueHandle.Dispose();
                }

                Jsrt.JsCollectGarbage(runtimeHandle);
            }

            Assert.True(valueHandle.IsInvalid);
            Assert.True(valueHandle.IsClosed);
        }

        [Fact]
        public void JsValueEqualSafeHandlesAreSetInvalidWhenCollected()
        {
            JavaScriptValueSafeHandle trueHandle, anotherTrueHandle;

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    trueHandle = Jsrt.JsGetTrueValue();
                    JavaScriptObjectManager.MonitorJavaScriptObjectLifetime(trueHandle);
                    anotherTrueHandle = Jsrt.JsBoolToBoolean(true);
                    JavaScriptObjectManager.MonitorJavaScriptObjectLifetime(anotherTrueHandle);

                    Assert.Equal(trueHandle, anotherTrueHandle);
                }
            }

            Assert.True(trueHandle.IsInvalid);
            Assert.True(trueHandle.IsClosed);

            Assert.True(anotherTrueHandle.IsInvalid);
            Assert.True(anotherTrueHandle.IsClosed);
        }

        [Fact]
        public void JsValueEqualSafeHandlesAreSetInvalidWhenDisposed()
        {
            JavaScriptValueSafeHandle trueHandle, anotherTrueHandle;

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    trueHandle = Jsrt.JsGetTrueValue();
                    JavaScriptObjectManager.MonitorJavaScriptObjectLifetime(trueHandle);
                    anotherTrueHandle = Jsrt.JsBoolToBoolean(true);
                    JavaScriptObjectManager.MonitorJavaScriptObjectLifetime(anotherTrueHandle);

                    Assert.Equal(trueHandle, anotherTrueHandle);
                    trueHandle.Dispose();
                    anotherTrueHandle.Dispose();
                }
            }

            Assert.True(trueHandle.IsInvalid);
            Assert.True(trueHandle.IsClosed);

            Assert.True(anotherTrueHandle.IsInvalid);
            Assert.True(anotherTrueHandle.IsClosed);
        }

        [Fact]
        public void JsValueSafeHandlesAreNotSingletons()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var trueHandle = Jsrt.JsGetTrueValue();
                    var anotherTrueHandle = Jsrt.JsBoolToBoolean(true);

                    Assert.Equal(trueHandle, anotherTrueHandle);
                    Assert.NotSame(trueHandle, anotherTrueHandle);
                }
            }
        }
    }
}
