namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Internal;
    using System;
    using System.Collections.Concurrent;
    using Xunit;

    public class JavaScriptSafeHandle_Facts
    {
        private IJavaScriptRuntime Jsrt;

        public JavaScriptSafeHandle_Facts()
        {
            Jsrt = JavaScriptRuntimeFactory.CreateChakraRuntime();
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
                    anotherValueHandle = Jsrt.JsCreateStringUtf8(superman, new UIntPtr((uint)superman.Length));
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
                    anotherTrueHandle = Jsrt.JsBoolToBoolean(true);

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
                    anotherTrueHandle = Jsrt.JsBoolToBoolean(true);

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

        ConcurrentDictionary<IntPtr, WeakCollection<JavaScriptObjectBeforeCollectCallback>> m_objectBeforeCollect = new ConcurrentDictionary<IntPtr, WeakCollection<JavaScriptObjectBeforeCollectCallback>> ();

        
        private void MonitorJavaScriptSafeHandle(JavaScriptValueSafeHandle handle)
        {
            IntPtr handlePtr = handle.DangerousGetHandle();
            
            if (m_objectBeforeCollect.ContainsKey(handlePtr))
            {
                WeakCollection<JavaScriptObjectBeforeCollectCallback> callbacks;
                if (m_objectBeforeCollect.TryGetValue(handlePtr, out callbacks))
                {
                    if (!callbacks.Contains(handle.ObjectBeforeCollectCallback))
                    {
                        callbacks.Add(handle.ObjectBeforeCollectCallback);
                    }
                }
            }
            else
            {
                var callbacks = new WeakCollection<JavaScriptObjectBeforeCollectCallback>();
                callbacks.Add(handle.ObjectBeforeCollectCallback);
                m_objectBeforeCollect.TryAdd(handlePtr, callbacks);
            }
        }

        private void OnObjectBeforeCollect(IntPtr handle, IntPtr callbackState)
        {
            WeakCollection<JavaScriptObjectBeforeCollectCallback> callbacks;
            if (m_objectBeforeCollect.TryGetValue(handle, out callbacks))
            {
                foreach (var objectBeforeCollectCallback in callbacks)
                {
                    objectBeforeCollectCallback.Invoke(handle, callbackState);
                }
                callbacks.Clear();
            }
        }
    }
}
