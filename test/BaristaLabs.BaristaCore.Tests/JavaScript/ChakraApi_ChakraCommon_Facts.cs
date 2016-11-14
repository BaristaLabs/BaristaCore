namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Interop;
    using Interop.Callbacks;
    using Interop.SafeHandles;
    using Extensions;
    using System;
    using System.Text;
    using Xunit;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Direct tests against the IChakraApi layer
    /// </summary>
    public class ChakraApi_ChakraCommon_Facts
    {
        [Fact]
        public void JsRuntimeCanBeConstructed()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            Assert.True(runtimeHandle.IsClosed == false);
            Assert.True(runtimeHandle.IsInvalid == false);
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JSRuntimeCanBeDisposed()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));
            Assert.True(runtimeHandle.IsClosed == false);
            runtimeHandle.Dispose();
            Assert.True(runtimeHandle.IsClosed);
        }

        [Fact]
        public void JSCollectGarbageCanBeCalled()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsCollectGarbage(runtimeHandle));

            runtimeHandle.Dispose();
        }

        [Fact]
        public void JSRuntimeMemoryUsageCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            ulong usage;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetRuntimeMemoryUsage(runtimeHandle, out usage));

            Assert.True(usage > 0);
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JSRuntimeMemoryLimitCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            ulong limit;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetRuntimeMemoryLimit(runtimeHandle, out limit));

            Assert.True(limit == ulong.MaxValue);
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JSRuntimeMemoryLimitCanBeSet()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            Errors.ThrowIfIs(ChakraApi.Instance.JsSetRuntimeMemoryLimit(runtimeHandle, 64000));

            ulong limit;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetRuntimeMemoryLimit(runtimeHandle, out limit));

            Assert.True(64000 == limit);
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JSRuntimeMemoryAllocationCallbackIsCalled()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            bool called = false;
            JavaScriptMemoryAllocationCallback callback = (IntPtr callbackState, JavaScriptMemoryEventType allocationEvent, UIntPtr allocationSize) =>
            {
                called = true;
                return true;
            };

            Errors.ThrowIfIs(ChakraApi.Instance.JsSetRuntimeMemoryAllocationCallback(runtimeHandle, IntPtr.Zero, callback));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));

            contextHandle.Dispose();
            runtimeHandle.Dispose();

            Assert.True(called == true);
        }

        [Fact]
        public void JSRuntimeBeforeCollectCallbackIsCalled()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            bool called = false;
            JavaScriptBeforeCollectCallback callback = (IntPtr callbackState) =>
            {
                called = true;
            };

            Errors.ThrowIfIs(ChakraApi.Instance.JsSetRuntimeBeforeCollectCallback(runtimeHandle, IntPtr.Zero, callback));

            Errors.ThrowIfIs(ChakraApi.Instance.JsCollectGarbage(runtimeHandle));

            runtimeHandle.Dispose();

            Assert.True(called == true);
        }

        private struct MyPoint
        {
            public int x, y;
        }

        [Fact]
        public void JsRefCanBeAdded()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            var point = new MyPoint()
            {
                x = 64,
                y = 64
            };
            
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MyPoint>());
            try
            {
                Marshal.StructureToPtr(point, ptr, false);

                uint count;
                Errors.ThrowIfIs(ChakraApi.Instance.JsAddRef(ptr, out count));

                Assert.True(count == 0);

                Errors.ThrowIfIs(ChakraApi.Instance.JsCollectGarbage(runtimeHandle));
            }
            finally
            {
                Marshal.DestroyStructure<MyPoint>(ptr);
                Marshal.FreeHGlobal(ptr);
                contextHandle.Dispose();
                runtimeHandle.Dispose();
            }
        }

        [Fact]
        public void JsObjectBeforeCollectCallbackIsCalled()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            bool called = false;
            JavaScriptObjectBeforeCollectCallback callback = (IntPtr sender, IntPtr callbackState) =>
            {
                called = true;
            };

            JavaScriptValueSafeHandle valueHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8("superman", new UIntPtr((uint)"superman".Length), out valueHandle));

            Errors.ThrowIfIs(ChakraApi.Instance.JsSetObjectBeforeCollectCallback(valueHandle, IntPtr.Zero, callback));

            valueHandle.Dispose();
            Errors.ThrowIfIs(ChakraApi.Instance.JsCollectGarbage(runtimeHandle));

            Assert.True(called == true);
            
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsContextCanBeCreated()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));

            Assert.True(contextHandle.IsClosed == false);
            Assert.True(contextHandle.IsInvalid == false);

            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsContextCanBeReleased()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));

            Assert.True(contextHandle.IsClosed == false);
            contextHandle.Dispose();
            Assert.True(contextHandle.IsClosed);


            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCurrentContextCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetCurrentContext(out contextHandle));

            Assert.True(contextHandle.IsInvalid);

            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCurrentContextCanBeSet()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));

            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptContextSafeHandle currentContextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetCurrentContext(out currentContextHandle));
            Assert.True(currentContextHandle == contextHandle);
            
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsExternalArrayBufferCanBeCreated()
        {
            var data = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(data);
            try
            {
                JavaScriptValueSafeHandle externalArrayBufferHandle;
                Errors.ThrowIfIs(ChakraApi.Instance.JsCreateExternalArrayBuffer(ptrScript, (uint)data.Length, null, IntPtr.Zero, out externalArrayBufferHandle));
                Assert.True(externalArrayBufferHandle != JavaScriptValueSafeHandle.Invalid);
            }
            finally
            {
                contextHandle.Dispose();
                runtimeHandle.Dispose();
            }
        }
    }
}
