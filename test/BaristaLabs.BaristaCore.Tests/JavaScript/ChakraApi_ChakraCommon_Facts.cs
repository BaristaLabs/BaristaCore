namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Interop;
    using Interop.Callbacks;
    using Interop.SafeHandles;
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
        public void JsRuntimeCanBeDisposed()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));
            Assert.True(runtimeHandle.IsClosed == false);
            runtimeHandle.Dispose();
            Assert.True(runtimeHandle.IsClosed);
        }

        [Fact]
        public void JsCollectGarbageCanBeCalled()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsCollectGarbage(runtimeHandle));

            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsRuntimeMemoryUsageCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            ulong usage;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetRuntimeMemoryUsage(runtimeHandle, out usage));

            Assert.True(usage > 0);
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsRuntimeMemoryLimitCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            ulong limit;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetRuntimeMemoryLimit(runtimeHandle, out limit));

            Assert.True(limit == ulong.MaxValue);
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsRuntimeMemoryLimitCanBeSet()
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
        public void JsRuntimeMemoryAllocationCallbackIsCalled()
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
        public void JsRuntimeBeforeCollectCallbackIsCalled()
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
        public void JsContextOfObjectCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            string str = "I do not fear computers. I fear the lack of them.";
            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(str, new UIntPtr((uint)str.Length), out stringHandle));

            JavaScriptContextSafeHandle objectContextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetContextOfObject(stringHandle, out objectContextHandle));

            Assert.True(objectContextHandle == contextHandle);

            stringHandle.Dispose();
            objectContextHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JSContextDataCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            IntPtr contextData;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetContextData(contextHandle, out contextData));

            Assert.True(contextData == IntPtr.Zero);

            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JSContextDataCanBeSet()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            string myString = "How inappropriate to call this planet 'Earth', when it is clearly 'Ocean'.";
            var strPtr = Marshal.StringToHGlobalAnsi(myString);
            try
            {
                Errors.ThrowIfIs(ChakraApi.Instance.JsSetContextData(contextHandle, strPtr));

                IntPtr contextData;
                Errors.ThrowIfIs(ChakraApi.Instance.JsGetContextData(contextHandle, out contextData));

                Assert.True(contextData == strPtr);
                Assert.True(myString == Marshal.PtrToStringAnsi(contextData));
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocAnsi(strPtr);
            }

            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsRuntimeCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptRuntimeSafeHandle contextRuntimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetRuntime(contextHandle, out contextRuntimeHandle));

            Assert.True(contextRuntimeHandle == runtimeHandle);

            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsIdleCanBeCalled()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.EnableIdleProcessing, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            uint nextIdleTick;
            Errors.ThrowIfIs(ChakraApi.Instance.JsIdle(out nextIdleTick));

            var nextTickTime = new DateTime(DateTime.Now.Ticks + nextIdleTick);
            Assert.True(nextTickTime > DateTime.Now);

            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsSymbolCanBeRetrievedFromPropertyId()
        {
            string propertyName = "foo";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.EnableIdleProcessing, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle propertyNameHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length), out propertyNameHandle));

            JavaScriptValueSafeHandle symbolHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateSymbol(propertyNameHandle, out symbolHandle));

            JavaScriptPropertyIdSafeHandle propertyIdHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetPropertyIdFromSymbol(symbolHandle, out propertyIdHandle));

            JavaScriptValueSafeHandle retrievedSymbolHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetSymbolFromPropertyId(propertyIdHandle, out retrievedSymbolHandle));

            Assert.True(retrievedSymbolHandle != JavaScriptValueSafeHandle.Invalid);

            retrievedSymbolHandle.Dispose();
            propertyIdHandle.Dispose();
            symbolHandle.Dispose();
            propertyNameHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsPropertyIdTypeCanBeDetermined()
        {
            string propertyName = "foo";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.EnableIdleProcessing, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle propertyNameHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length), out propertyNameHandle));

            JavaScriptValueSafeHandle symbolHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateSymbol(propertyNameHandle, out symbolHandle));

            JavaScriptPropertyIdSafeHandle symbolPropertyIdHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetPropertyIdFromSymbol(symbolHandle, out symbolPropertyIdHandle));

            JavaScriptPropertyIdSafeHandle stringPropertyIdHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreatePropertyIdUtf8(propertyName, new UIntPtr((uint)propertyName.Length), out stringPropertyIdHandle));

            JavaScriptPropertyIdType symbolPropertyType;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetPropertyIdType(symbolPropertyIdHandle, out symbolPropertyType));

            Assert.True(symbolPropertyType == JavaScriptPropertyIdType.Symbol);

            JavaScriptPropertyIdType stringPropertyType;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetPropertyIdType(stringPropertyIdHandle, out stringPropertyType));

            Assert.True(stringPropertyType == JavaScriptPropertyIdType.String);

            stringPropertyIdHandle.Dispose();
            symbolPropertyIdHandle.Dispose();
            symbolHandle.Dispose();
            propertyNameHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsPropertyIdCanBeRetrievedFromASymbol()
        {
            string propertyName = "foo";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.EnableIdleProcessing, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle propertyNameHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length), out propertyNameHandle));

            JavaScriptValueSafeHandle symbolHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateSymbol(propertyNameHandle, out symbolHandle));

            JavaScriptPropertyIdSafeHandle propertyIdHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetPropertyIdFromSymbol(symbolHandle, out propertyIdHandle));
            
            Assert.True(propertyIdHandle != JavaScriptPropertyIdSafeHandle.Invalid);

            propertyIdHandle.Dispose();
            symbolHandle.Dispose();
            propertyNameHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsSymbolCanBeCreated()
        {
            string propertyName = "foo";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.EnableIdleProcessing, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle propertyNameHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length), out propertyNameHandle));

            JavaScriptValueSafeHandle symbolHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateSymbol(propertyNameHandle, out symbolHandle));
            
            Assert.True(symbolHandle != JavaScriptValueSafeHandle.Invalid);

            symbolHandle.Dispose();
            propertyNameHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsSymbolDescriptionCanBeRetrieved()
        {
            string propertyName = "foo";
            string toStringPropertyName = "toString";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.EnableIdleProcessing, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle propertyNameHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length), out propertyNameHandle));

            JavaScriptValueSafeHandle symbolHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateSymbol(propertyNameHandle, out symbolHandle));


            JavaScriptPropertyIdSafeHandle toStringFunctionPropertyIdHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreatePropertyIdUtf8(toStringPropertyName, new UIntPtr((uint)toStringPropertyName.Length), out toStringFunctionPropertyIdHandle));

            JavaScriptValueSafeHandle symbolObjHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsConvertValueToObject(symbolHandle, out symbolObjHandle));

            JavaScriptValueSafeHandle symbolToStringFnHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetProperty(symbolObjHandle, toStringFunctionPropertyIdHandle, out symbolToStringFnHandle));

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCallFunction(symbolToStringFnHandle, new IntPtr[] { symbolObjHandle.DangerousGetHandle() }, 1, out resultHandle));

            UIntPtr size;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf8(resultHandle, null, UIntPtr.Zero, out size));
            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            UIntPtr written;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCopyStringUtf8(resultHandle, result, size, out written));
            string resultStr = Encoding.UTF8.GetString(result, 0, result.Length);

            Assert.True(resultStr == "Symbol(foo)");

            toStringFunctionPropertyIdHandle.Dispose();
            symbolObjHandle.Dispose();
            symbolToStringFnHandle.Dispose();
            resultHandle.Dispose();

            symbolHandle.Dispose();
            propertyNameHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }


        [Fact]
        public void JsGetOwnPropertySymbols()
        {
            var script = @"(() => {
var sym = Symbol('foo');
var obj = {
    [sym]: 'bar',
    'baz': 'qix'
};
return obj;
})();
";
            var sourceUrl = "[eval code]";
            JavaScriptValueSafeHandle objHandle;

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateRuntime(JsRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfIs(ChakraApi.Instance.JsSetCurrentContext(contextHandle));

            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            try
            {
                JavaScriptValueSafeHandle scriptHandle;
                Errors.ThrowIfIs(ChakraApi.Instance.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

                JavaScriptSourceContext sourceContext = new JavaScriptSourceContext();

                JavaScriptValueSafeHandle sourceUrlHandle;
                Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length), out sourceUrlHandle));

                Errors.ThrowIfIs(ChakraApi.Instance.JsRun(scriptHandle, sourceContext, sourceUrlHandle, JsParseScriptAttributes.JsParseScriptAttributeNone, out objHandle));

                scriptHandle.Dispose();
                sourceUrlHandle.Dispose();
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocAnsi(ptrScript);
            }

            JavaScriptValueSafeHandle propertySymbols;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetOwnPropertySymbols(objHandle, out propertySymbols));

            JsValueType propertySymbolsType;
            Errors.ThrowIfIs(ChakraApi.Instance.JsGetValueType(propertySymbols, out propertySymbolsType));

            Assert.True(propertySymbols != JavaScriptValueSafeHandle.Invalid);
            Assert.True(propertySymbolsType == JsValueType.JsArray);

            propertySymbols.Dispose();
            objHandle.Dispose();

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
