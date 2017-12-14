namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.JavaScript.Internal;
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class JavaScriptReference_Facts
    {
        #region Test Support
        private IJavaScriptEngine Engine;

        public JavaScriptReference_Facts()
        {
            Engine = JavaScriptEngineFactory.CreateChakraEngine();
        }

        /// <summary>
        /// Manages the lifecycle of JavaScriptReference objects.
        /// </summary>
        public static class JavaScriptObjectManager
        {
            private static ConcurrentDictionary<IntPtr, WeakCollection<JavaScriptObjectBeforeCollectCallback>> s_objectBeforeCollect = new ConcurrentDictionary<IntPtr, WeakCollection<JavaScriptObjectBeforeCollectCallback>>();

            /// <summary>
            /// Monitors the specified JavaScript Reference and executes the specified delegate on the before collect callback.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="handle"></param>
            /// <param name="callback"></param>
            /// <param name="callbackState"></param>
            public static void MonitorJavaScriptObjectLifetime<T>(T handle, JavaScriptObjectBeforeCollectCallback callback, IntPtr callbackState = default(IntPtr)) where T : JavaScriptReference<T>
            {
                if (callbackState == default(IntPtr))
                    callbackState = IntPtr.Zero;

                IntPtr handlePtr = handle.DangerousGetHandle();
                if (handlePtr == IntPtr.Zero)
                    return;

                //Certain values, such as undefined, null, true, false, etc, cannot have a callback set. InvalidArgument is returned in these situations.
                var errorCode = LibChakraCore.JsSetObjectBeforeCollectCallback(handle, callbackState, OnObjectBeforeCollect);
                if (errorCode == JavaScriptErrorCode.NoError)
                {

                    //If any other error occured, throw it.
                    Errors.ThrowIfError(errorCode);

                    if (s_objectBeforeCollect.ContainsKey(handlePtr))
                    {
                        if (s_objectBeforeCollect.TryGetValue(handlePtr, out WeakCollection<JavaScriptObjectBeforeCollectCallback> callbacks))
                        {
                            if (!callbacks.Contains(callback))
                            {
                                callbacks.Add(callback);
                            }
                        }
                    }
                    else
                    {
                        var callbacks = new WeakCollection<JavaScriptObjectBeforeCollectCallback>
                        {
                            callback
                        };
                        s_objectBeforeCollect.TryAdd(handlePtr, callbacks);
                    }
                }
            }

            /// <summary>
            /// Monitors the specified JavaScript Reference and disposes of the object on the before collect callback.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="handle"></param>
            public static void DisposeWhenCollected<T>(T handle) where T : JavaScriptReference<T>
            {
                IntPtr ptr = handle.DangerousGetHandle();
                MonitorJavaScriptObjectLifetime(handle, (IntPtr handlePtr, IntPtr callbackState) =>
                {
                    if (ptr == handlePtr)
                        handle.Dispose();
                });
            }

            private static void OnObjectBeforeCollect(IntPtr handle, IntPtr callbackState)
            {
                if (s_objectBeforeCollect.TryGetValue(handle, out WeakCollection<JavaScriptObjectBeforeCollectCallback> callbacks))
                {
                    foreach (var objectBeforeCollectCallback in callbacks)
                    {
                        objectBeforeCollectCallback.Invoke(handle, callbackState);
                    }
                    callbacks.Clear();
                }
            }
        }
        #endregion

        [Fact]
        public void JsReferencesIndicateNativeFunctionSource()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                Assert.Equal("JsCreateRuntime", runtimeHandle.NativeFunctionSource);
            }
        }

        [Fact]
        public void JsContextsAreClosedWhenHandleIsCollected()
        {
            JavaScriptContextSafeHandle contextHandle, anotherContextHandle;
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                contextHandle = Engine.JsCreateContext(runtimeHandle);
                JavaScriptObjectManager.DisposeWhenCollected(contextHandle);

                Engine.JsSetCurrentContext(contextHandle);

                anotherContextHandle = Engine.JsGetCurrentContext();
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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var superman = "superman";
                    valueHandle = Engine.JsCreateString(superman, (ulong)superman.Length);
                    JavaScriptObjectManager.DisposeWhenCollected(valueHandle);
                    anotherValueHandle = Engine.JsCreateString(superman, (ulong)superman.Length);
                    JavaScriptObjectManager.DisposeWhenCollected(anotherValueHandle);
                }

                Engine.JsCollectGarbage(runtimeHandle);
            }

            Assert.True(valueHandle.IsClosed);
            Assert.True(anotherValueHandle.IsClosed);
        }

        [Fact]
        public void JsValuesAreClosedWhenHandleIsCollectedWithDispose()
        {
            JavaScriptValueSafeHandle valueHandle;

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    valueHandle = Engine.JsCreateString("superman", (ulong)"superman".Length);
                    JavaScriptObjectManager.DisposeWhenCollected(valueHandle);
                    valueHandle.Dispose();
                }

                Engine.JsCollectGarbage(runtimeHandle);
            }

            Assert.True(valueHandle.IsClosed);
        }

        [Fact]
        public void JsValueEqualSafeHandlesAreClosedWhenCollected()
        {
            JavaScriptValueSafeHandle trueHandle, anotherTrueHandle;

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    trueHandle = Engine.JsGetTrueValue();
                    JavaScriptObjectManager.DisposeWhenCollected(trueHandle);
                    anotherTrueHandle = Engine.JsBoolToBoolean(true);
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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    trueHandle = Engine.JsGetTrueValue();
                    JavaScriptObjectManager.DisposeWhenCollected(trueHandle);
                    anotherTrueHandle = Engine.JsBoolToBoolean(true);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var trueHandle = Engine.JsGetTrueValue();
                    var anotherTrueHandle = Engine.JsBoolToBoolean(true);

                    Assert.Equal(trueHandle, anotherTrueHandle);
                    Assert.NotSame(trueHandle, anotherTrueHandle);
                }
            }
        }
    }
}
