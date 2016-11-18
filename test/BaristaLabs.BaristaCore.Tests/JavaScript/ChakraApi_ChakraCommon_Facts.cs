namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Internal;

    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Xunit;

    /// <summary>
    /// Direct tests against the IChakraApi layer
    /// </summary>
    public class ChakraApi_ChakraCommon_Facts
    {
        private IJavaScriptRuntime Jsrt;

        public ChakraApi_ChakraCommon_Facts()
        {
            Jsrt = JavaScriptRuntimeFactory.CreateChakraRuntime();
        }

        [Fact]
        public void JsRuntimeCanBeConstructed()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            Assert.True(runtimeHandle.IsClosed == false);
            Assert.True(runtimeHandle.IsInvalid == false);
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsRuntimeCanBeDisposed()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));
            Assert.True(runtimeHandle.IsClosed == false);
            runtimeHandle.Dispose();
            Assert.True(runtimeHandle.IsClosed);
        }

        [Fact]
        public void JsCollectGarbageCanBeCalled()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));
            Errors.ThrowIfError(Jsrt.JsCollectGarbage(runtimeHandle));

            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsRuntimeMemoryUsageCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            ulong usage;
            Errors.ThrowIfError(Jsrt.JsGetRuntimeMemoryUsage(runtimeHandle, out usage));

            Assert.True(usage > 0);
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsRuntimeMemoryLimitCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            ulong limit;
            Errors.ThrowIfError(Jsrt.JsGetRuntimeMemoryLimit(runtimeHandle, out limit));

            Assert.True(limit == ulong.MaxValue);
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsRuntimeMemoryLimitCanBeSet()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            Errors.ThrowIfError(Jsrt.JsSetRuntimeMemoryLimit(runtimeHandle, 64000));

            ulong limit;
            Errors.ThrowIfError(Jsrt.JsGetRuntimeMemoryLimit(runtimeHandle, out limit));

            Assert.True(64000 == limit);
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsRuntimeMemoryAllocationCallbackIsCalled()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            bool called = false;
            JavaScriptMemoryAllocationCallback callback = (IntPtr callbackState, JavaScriptMemoryEventType allocationEvent, UIntPtr allocationSize) =>
            {
                called = true;
                return true;
            };

            Errors.ThrowIfError(Jsrt.JsSetRuntimeMemoryAllocationCallback(runtimeHandle, IntPtr.Zero, callback));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));

            contextHandle.Dispose();
            runtimeHandle.Dispose();

            Assert.True(called == true);
        }

        [Fact]
        public void JsRuntimeBeforeCollectCallbackIsCalled()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            bool called = false;
            JavaScriptBeforeCollectCallback callback = (IntPtr callbackState) =>
            {
                called = true;
            };

            Errors.ThrowIfError(Jsrt.JsSetRuntimeBeforeCollectCallback(runtimeHandle, IntPtr.Zero, callback));

            Errors.ThrowIfError(Jsrt.JsCollectGarbage(runtimeHandle));

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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

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
                Errors.ThrowIfError(Jsrt.JsAddRef(ptr, out count));

                Assert.True(count == 0);

                Errors.ThrowIfError(Jsrt.JsCollectGarbage(runtimeHandle));
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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            bool called = false;
            JavaScriptObjectBeforeCollectCallback callback = (IntPtr sender, IntPtr callbackState) =>
            {
                called = true;
            };

            JavaScriptValueSafeHandle valueHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8("superman", new UIntPtr((uint)"superman".Length), out valueHandle));

            Errors.ThrowIfError(Jsrt.JsSetObjectBeforeCollectCallback(valueHandle, IntPtr.Zero, callback));

            //Apparently if the handle is released prior to garbage collection, the callback isn't run.
            //Thus, the following is commented out.
            //valueHandle.Dispose();

            Errors.ThrowIfError(Jsrt.JsCollectGarbage(runtimeHandle));

            Assert.True(called == true);
            
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsContextCanBeCreated()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));

            Assert.True(contextHandle.IsClosed == false);
            Assert.True(contextHandle.IsInvalid == false);

            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsContextCanBeReleased()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));

            Assert.True(contextHandle.IsClosed == false);
            contextHandle.Dispose();
            Assert.True(contextHandle.IsClosed);

            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCurrentContextCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsGetCurrentContext(out contextHandle));

            Assert.True(contextHandle.IsInvalid);

            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCurrentContextCanBeSet()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));

            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptContextSafeHandle currentContextHandle;
            Errors.ThrowIfError(Jsrt.JsGetCurrentContext(out currentContextHandle));
            Assert.True(currentContextHandle == contextHandle);
            
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsContextOfObjectCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            string str = "I do not fear computers. I fear the lack of them.";
            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(str, new UIntPtr((uint)str.Length), out stringHandle));

            JavaScriptContextSafeHandle objectContextHandle;
            Errors.ThrowIfError(Jsrt.JsGetContextOfObject(stringHandle, out objectContextHandle));

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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            IntPtr contextData;
            Errors.ThrowIfError(Jsrt.JsGetContextData(contextHandle, out contextData));

            Assert.True(contextData == IntPtr.Zero);

            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JSContextDataCanBeSet()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            string myString = "How inappropriate to call this planet 'Earth', when it is clearly 'Ocean'.";
            var strPtr = Marshal.StringToHGlobalAnsi(myString);
            try
            {
                Errors.ThrowIfError(Jsrt.JsSetContextData(contextHandle, strPtr));

                IntPtr contextData;
                Errors.ThrowIfError(Jsrt.JsGetContextData(contextHandle, out contextData));

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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptRuntimeSafeHandle contextRuntimeHandle;
            Errors.ThrowIfError(Jsrt.JsGetRuntime(contextHandle, out contextRuntimeHandle));

            Assert.True(contextRuntimeHandle == runtimeHandle);

            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsIdleCanBeCalled()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.EnableIdleProcessing, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            uint nextIdleTick;
            Errors.ThrowIfError(Jsrt.JsIdle(out nextIdleTick));

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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.EnableIdleProcessing, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle propertyNameHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length), out propertyNameHandle));

            JavaScriptValueSafeHandle symbolHandle;
            Errors.ThrowIfError(Jsrt.JsCreateSymbol(propertyNameHandle, out symbolHandle));

            JavaScriptPropertyIdSafeHandle propertyIdHandle;
            Errors.ThrowIfError(Jsrt.JsGetPropertyIdFromSymbol(symbolHandle, out propertyIdHandle));

            JavaScriptValueSafeHandle retrievedSymbolHandle;
            Errors.ThrowIfError(Jsrt.JsGetSymbolFromPropertyId(propertyIdHandle, out retrievedSymbolHandle));

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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.EnableIdleProcessing, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle propertyNameHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length), out propertyNameHandle));

            JavaScriptValueSafeHandle symbolHandle;
            Errors.ThrowIfError(Jsrt.JsCreateSymbol(propertyNameHandle, out symbolHandle));

            JavaScriptPropertyIdSafeHandle symbolPropertyIdHandle;
            Errors.ThrowIfError(Jsrt.JsGetPropertyIdFromSymbol(symbolHandle, out symbolPropertyIdHandle));

            JavaScriptPropertyIdSafeHandle stringPropertyIdHandle;
            Errors.ThrowIfError(Jsrt.JsCreatePropertyIdUtf8(propertyName, new UIntPtr((uint)propertyName.Length), out stringPropertyIdHandle));

            JavaScriptPropertyIdType symbolPropertyType;
            Errors.ThrowIfError(Jsrt.JsGetPropertyIdType(symbolPropertyIdHandle, out symbolPropertyType));

            Assert.True(symbolPropertyType == JavaScriptPropertyIdType.Symbol);

            JavaScriptPropertyIdType stringPropertyType;
            Errors.ThrowIfError(Jsrt.JsGetPropertyIdType(stringPropertyIdHandle, out stringPropertyType));

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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.EnableIdleProcessing, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle propertyNameHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length), out propertyNameHandle));

            JavaScriptValueSafeHandle symbolHandle;
            Errors.ThrowIfError(Jsrt.JsCreateSymbol(propertyNameHandle, out symbolHandle));

            JavaScriptPropertyIdSafeHandle propertyIdHandle;
            Errors.ThrowIfError(Jsrt.JsGetPropertyIdFromSymbol(symbolHandle, out propertyIdHandle));
            
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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.EnableIdleProcessing, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle propertyNameHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length), out propertyNameHandle));

            JavaScriptValueSafeHandle symbolHandle;
            Errors.ThrowIfError(Jsrt.JsCreateSymbol(propertyNameHandle, out symbolHandle));
            
            Assert.True(symbolHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(symbolHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.Symbol);

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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.EnableIdleProcessing, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle propertyNameHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length), out propertyNameHandle));

            JavaScriptValueSafeHandle symbolHandle;
            Errors.ThrowIfError(Jsrt.JsCreateSymbol(propertyNameHandle, out symbolHandle));


            JavaScriptPropertyIdSafeHandle toStringFunctionPropertyIdHandle;
            Errors.ThrowIfError(Jsrt.JsCreatePropertyIdUtf8(toStringPropertyName, new UIntPtr((uint)toStringPropertyName.Length), out toStringFunctionPropertyIdHandle));

            JavaScriptValueSafeHandle symbolObjHandle;
            Errors.ThrowIfError(Jsrt.JsConvertValueToObject(symbolHandle, out symbolObjHandle));

            JavaScriptValueSafeHandle symbolToStringFnHandle;
            Errors.ThrowIfError(Jsrt.JsGetProperty(symbolObjHandle, toStringFunctionPropertyIdHandle, out symbolToStringFnHandle));

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfError(Jsrt.JsCallFunction(symbolToStringFnHandle, new IntPtr[] { symbolObjHandle.DangerousGetHandle() }, 1, out resultHandle));

            UIntPtr size;
            Errors.ThrowIfError(Jsrt.JsCopyStringUtf8(resultHandle, null, UIntPtr.Zero, out size));
            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            UIntPtr written;
            Errors.ThrowIfError(Jsrt.JsCopyStringUtf8(resultHandle, result, size, out written));
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
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            try
            {
                JavaScriptValueSafeHandle scriptHandle;
                Errors.ThrowIfError(Jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

                JavaScriptSourceContext sourceContext = new JavaScriptSourceContext();

                JavaScriptValueSafeHandle sourceUrlHandle;
                Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length), out sourceUrlHandle));

                Errors.ThrowIfError(Jsrt.JsRun(scriptHandle, sourceContext, sourceUrlHandle, JavaScriptParseScriptAttributes.None, out objHandle));

                scriptHandle.Dispose();
                sourceUrlHandle.Dispose();
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocAnsi(ptrScript);
            }

            JavaScriptValueSafeHandle propertySymbols;
            Errors.ThrowIfError(Jsrt.JsGetOwnPropertySymbols(objHandle, out propertySymbols));

            JavaScriptValueType propertySymbolsType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(propertySymbols, out propertySymbolsType));

            Assert.True(propertySymbols != JavaScriptValueSafeHandle.Invalid);
            Assert.True(propertySymbolsType == JavaScriptValueType.Array);

            propertySymbols.Dispose();
            objHandle.Dispose();

            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsUndefinedValueCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle undefinedHandle;
            Errors.ThrowIfError(Jsrt.JsGetUndefinedValue(out undefinedHandle));

            Assert.True(undefinedHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(undefinedHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.Undefined);

            undefinedHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsNullValueCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle nullHandle;
            Errors.ThrowIfError(Jsrt.JsGetNullValue(out nullHandle));

            Assert.True(nullHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(nullHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.Null);

            nullHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsTrueValueCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle trueHandle;
            Errors.ThrowIfError(Jsrt.JsGetTrueValue(out trueHandle));

            Assert.True(trueHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(trueHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.Boolean);

            trueHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsFalseValueCanBeRetrieved()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle falseHandle;
            Errors.ThrowIfError(Jsrt.JsGetFalseValue(out falseHandle));

            Assert.True(falseHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(falseHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.Boolean);

            falseHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCanConvertBoolValueToBoolean()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle trueHandle;
            Errors.ThrowIfError(Jsrt.JsBoolToBoolean(true, out trueHandle));
            Assert.True(trueHandle != JavaScriptValueSafeHandle.Invalid);

            trueHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCanConvertBooleanValueToBool()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle trueHandle;
            Errors.ThrowIfError(Jsrt.JsGetTrueValue(out trueHandle));

            JavaScriptValueSafeHandle falseHandle;
            Errors.ThrowIfError(Jsrt.JsGetFalseValue(out falseHandle));

            bool result;
            Errors.ThrowIfError(Jsrt.JsBooleanToBool(trueHandle, out result));
            Assert.True(result);

            Errors.ThrowIfError(Jsrt.JsBooleanToBool(falseHandle, out result));
            Assert.False(result);

            trueHandle.Dispose();
            falseHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCanConvertValueToBoolean()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            var stringValue = "true";
            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(stringValue, new UIntPtr((uint)stringValue.Length), out stringHandle));

            JavaScriptValueSafeHandle boolHandle;
            Errors.ThrowIfError(Jsrt.JsConvertValueToBoolean(stringHandle, out boolHandle));

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(boolHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.Boolean);

            bool result;
            Errors.ThrowIfError(Jsrt.JsBooleanToBool(boolHandle, out result));
            Assert.True(result);

            stringHandle.Dispose();
            boolHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCanGetValueType()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            var stringValue = "Dear future generations: Please accept our apologies. We were rolling drunk on petroleum.";
            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(stringValue, new UIntPtr((uint)stringValue.Length), out stringHandle));

            JavaScriptValueType result;
            Errors.ThrowIfError(Jsrt.JsGetValueType(stringHandle, out result));

            Assert.True(result == JavaScriptValueType.String);

            stringHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCanConvertDoubleValueToNumber()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle doubleHandle;
            Errors.ThrowIfError(Jsrt.JsDoubleToNumber(3.14156, out doubleHandle));

            Assert.True(doubleHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(doubleHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.Number);

            doubleHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCanConvertIntValueToNumber()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle intHandle;
            Errors.ThrowIfError(Jsrt.JsIntToNumber(3, out intHandle));

            Assert.True(intHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(intHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.Number);

            intHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCanConvertNumberToDouble()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle doubleHandle;
            Errors.ThrowIfError(Jsrt.JsDoubleToNumber(3.14159, out doubleHandle));

            double result;
            Errors.ThrowIfError(Jsrt.JsNumberToDouble(doubleHandle, out result));

            Assert.True(result == 3.14159);

            doubleHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCanConvertNumberToInt()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle intHandle;
            Errors.ThrowIfError(Jsrt.JsIntToNumber(3, out intHandle));

            int result;
            Errors.ThrowIfError(Jsrt.JsNumberToInt(intHandle, out result));

            Assert.True(result == 3);

            intHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCanConvertValueToNumber()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            var stringValue = "2.71828";
            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(stringValue, new UIntPtr((uint)stringValue.Length), out stringHandle));

            JavaScriptValueSafeHandle numberHandle;
            Errors.ThrowIfError(Jsrt.JsConvertValueToNumber(stringHandle, out numberHandle));

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(numberHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.Number);

            double result;
            Errors.ThrowIfError(Jsrt.JsNumberToDouble(numberHandle, out result));

            Assert.True(result == 2.71828);

            stringHandle.Dispose();
            numberHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCanGetStringLength()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            var stringValue = "If your brains were dynamite there wouldn't be enough to blow your hat off.";
            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfError(Jsrt.JsCreateStringUtf8(stringValue, new UIntPtr((uint)stringValue.Length), out stringHandle));

            int result;
            Errors.ThrowIfError(Jsrt.JsGetStringLength(stringHandle, out result));

            Assert.True(stringValue.Length == result);

            stringHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCanConvertValueToString()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle numberHandle;
            Errors.ThrowIfError(Jsrt.JsDoubleToNumber(2.71828, out numberHandle));

            JavaScriptValueSafeHandle stringHandle;
            Errors.ThrowIfError(Jsrt.JsConvertValueToString(numberHandle, out stringHandle));

            Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(stringHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.String);

            //Get the size
            UIntPtr size;
            Errors.ThrowIfError(Jsrt.JsCopyStringUtf8(stringHandle, null, UIntPtr.Zero, out size));
            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            UIntPtr written;
            Errors.ThrowIfError(Jsrt.JsCopyStringUtf8(stringHandle, result, new UIntPtr((uint)result.Length), out written));
            string resultStr = Encoding.UTF8.GetString(result, 0, result.Length);

            Assert.True(resultStr == "2.71828");

            stringHandle.Dispose();
            numberHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsCanRetrieveGlobalObject()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            JavaScriptValueSafeHandle objectHandle;
            Errors.ThrowIfError(Jsrt.JsGetGlobalObject(out objectHandle));

            Assert.True(objectHandle != JavaScriptValueSafeHandle.Invalid);

            JavaScriptValueType handleType;
            Errors.ThrowIfError(Jsrt.JsGetValueType(objectHandle, out handleType));

            Assert.True(handleType == JavaScriptValueType.Object);

            objectHandle.Dispose();
            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }

        [Fact]
        public void JsExternalArrayBufferCanBeCreated()
        {
            var data = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(data);
            try
            {
                JavaScriptValueSafeHandle externalArrayBufferHandle;
                Errors.ThrowIfError(Jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)data.Length, null, IntPtr.Zero, out externalArrayBufferHandle));
                Assert.True(externalArrayBufferHandle != JavaScriptValueSafeHandle.Invalid);

                JavaScriptValueType handleType;
                Errors.ThrowIfError(Jsrt.JsGetValueType(externalArrayBufferHandle, out handleType));

                Assert.True(handleType == JavaScriptValueType.ArrayBuffer);
            }
            finally
            {
                contextHandle.Dispose();
                runtimeHandle.Dispose();
            }
        }
    }
}
