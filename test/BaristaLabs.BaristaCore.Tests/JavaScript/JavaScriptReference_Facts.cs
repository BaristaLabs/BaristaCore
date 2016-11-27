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
            Jsrt = JavaScriptEngineFactory.CreateChakraEngine();
        }

        [Fact]
        public void JsContextsAreClosedWhenHandleIsCollected()
        {
            JavaScriptContextSafeHandle contextHandle, anotherContextHandle;
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                contextHandle = Jsrt.JsCreateContext(runtimeHandle);
                JavaScriptObjectManager.DisposeWhenCollected(contextHandle);

                Jsrt.JsSetCurrentContext(contextHandle);

                anotherContextHandle = Jsrt.JsGetCurrentContext();
                JavaScriptObjectManager.DisposeWhenCollected(anotherContextHandle);

                Assert.Equal(contextHandle, anotherContextHandle);
                Assert.NotSame(contextHandle, anotherContextHandle);
            }

            Assert.True(contextHandle.IsClosed);
            Assert.True(anotherContextHandle.IsClosed);
        }

        [Fact]
        public void JsValuesAreClosedWhenHandleIsCollected()
        {
            JavaScriptValueSafeHandle valueHandle, anotherValueHandle;

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var superman = "superman";
                    valueHandle = Jsrt.JsCreateStringUtf8(superman, new UIntPtr((uint)superman.Length));
                    JavaScriptObjectManager.DisposeWhenCollected(valueHandle);
                    anotherValueHandle = Jsrt.JsCreateStringUtf8(superman, new UIntPtr((uint)superman.Length));
                    JavaScriptObjectManager.DisposeWhenCollected(anotherValueHandle);
                }

                Jsrt.JsCollectGarbage(runtimeHandle);
            }

            Assert.True(valueHandle.IsClosed);
            Assert.True(anotherValueHandle.IsClosed);
        }

        [Fact]
        public void JsValuesAreClosedWhenHandleIsCollectedWithDispose()
        {
            JavaScriptValueSafeHandle valueHandle;

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    valueHandle = Jsrt.JsCreateStringUtf8("superman", new UIntPtr((uint)"superman".Length));
                    JavaScriptObjectManager.DisposeWhenCollected(valueHandle);
                    valueHandle.Dispose();
                }

                Jsrt.JsCollectGarbage(runtimeHandle);
            }

            Assert.True(valueHandle.IsClosed);
        }

        [Fact]
        public void JsValueEqualSafeHandlesAreClosedWhenCollected()
        {
            JavaScriptValueSafeHandle trueHandle, anotherTrueHandle;

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    trueHandle = Jsrt.JsGetTrueValue();
                    JavaScriptObjectManager.DisposeWhenCollected(trueHandle);
                    anotherTrueHandle = Jsrt.JsBoolToBoolean(true);
                    JavaScriptObjectManager.DisposeWhenCollected(anotherTrueHandle);

                    Assert.Equal(trueHandle, anotherTrueHandle);
                }
            }

            Assert.True(trueHandle.IsClosed);
            Assert.True(anotherTrueHandle.IsClosed);
        }

        [Fact]
        public void JsValueEqualSafeHandlesAreClosedWhenDisposed()
        {
            JavaScriptValueSafeHandle trueHandle, anotherTrueHandle;

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    trueHandle = Jsrt.JsGetTrueValue();
                    JavaScriptObjectManager.DisposeWhenCollected(trueHandle);
                    anotherTrueHandle = Jsrt.JsBoolToBoolean(true);
                    JavaScriptObjectManager.DisposeWhenCollected(anotherTrueHandle);

                    Assert.Equal(trueHandle, anotherTrueHandle);
                    trueHandle.Dispose();
                    anotherTrueHandle.Dispose();
                }
            }

            Assert.True(trueHandle.IsClosed);
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
