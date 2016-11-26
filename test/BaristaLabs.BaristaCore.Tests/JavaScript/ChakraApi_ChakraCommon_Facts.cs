namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Internal;

    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using Xunit;

    /// <summary>
    /// Direct tests against the IChakraApi layer
    /// </summary>
    public class ChakraApi_ChakraCommon_Facts
    {
        private IJavaScriptEngine Jsrt;

        public ChakraApi_ChakraCommon_Facts()
        {
            Jsrt = JavaScriptEngineFactory.CreateChakraEngine();
        }

        [Fact]
        public void JsRuntimeCanBeConstructed()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                Assert.True(runtimeHandle != JavaScriptRuntimeSafeHandle.Invalid);
                Assert.False(runtimeHandle.IsClosed);
                Assert.False(runtimeHandle.IsInvalid);
            }
        }

        [Fact]
        public void JsRuntimeCanBeDisposed()
        {
            var runtimeHandle = JavaScriptRuntimeSafeHandle.Invalid;
            try
            {
                runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null);
                Assert.False(runtimeHandle.IsClosed);
            }
            finally
            {
                runtimeHandle.Dispose();
                Assert.True(runtimeHandle.IsClosed);
            }
        }

        [Fact]
        public void JsCollectGarbageCanBeCalled()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                Jsrt.JsCollectGarbage(runtimeHandle);
            }
        }

        [Fact]
        public void JsRuntimeMemoryUsageCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                var usage = Jsrt.JsGetRuntimeMemoryUsage(runtimeHandle);

                Assert.True(usage > 0);
            }
        }

        [Fact]
        public void JsRuntimeMemoryLimitCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                var limit = Jsrt.JsGetRuntimeMemoryLimit(runtimeHandle);

                Assert.True(limit == ulong.MaxValue);
            }
        }

        [Fact]
        public void JsRuntimeMemoryLimitCanBeSet()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {

                Jsrt.JsSetRuntimeMemoryLimit(runtimeHandle, 64000);

                var limit = Jsrt.JsGetRuntimeMemoryLimit(runtimeHandle);

                Assert.True(64000 == limit);
            }
        }

        [Fact]
        public void JsRuntimeMemoryAllocationCallbackIsCalled()
        {
            bool called = false;
            JavaScriptMemoryAllocationCallback callback = (IntPtr callbackState, JavaScriptMemoryEventType allocationEvent, UIntPtr allocationSize) =>
            {
                called = true;
                return true;
            };

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                Jsrt.JsSetRuntimeMemoryAllocationCallback(runtimeHandle, IntPtr.Zero, callback);

                //Bounce a context to get an allocation
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                }
            }

            Assert.True(called);
        }

        [Fact]
        public void JsRuntimeBeforeCollectCallbackIsCalled()
        {
            bool called = false;
            JavaScriptBeforeCollectCallback callback = (IntPtr callbackState) =>
            {
                called = true;
            };

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                Jsrt.JsSetRuntimeBeforeCollectCallback(runtimeHandle, IntPtr.Zero, callback);

                Jsrt.JsCollectGarbage(runtimeHandle);
            }

            Assert.True(called);
        }

        [Fact]
        public void JsValueRefCanBeAdded()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var myString = "Have you ever questioned the nature of your reality?";
                    var stringHandle = Jsrt.JsCreateStringUtf8(myString, new UIntPtr((uint)myString.Length));

                    var count = Jsrt.JsAddRef(stringHandle);

                    Assert.Equal((uint)1, count);

                    Jsrt.JsCollectGarbage(runtimeHandle);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsObjectBeforeCollectCallbackIsCalled()
        {
            bool called = false;
            JavaScriptObjectBeforeCollectCallback callback = (IntPtr sender, IntPtr callbackState) =>
            {
                called = true;
            };

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var valueHandle = Jsrt.JsCreateStringUtf8("superman", new UIntPtr((uint)"superman".Length));

                    Jsrt.JsSetObjectBeforeCollectCallback(valueHandle, IntPtr.Zero, callback);

                    //The callback is executed once the context is released and garbage is collected.
                    valueHandle.Dispose();
                }

                Jsrt.JsCollectGarbage(runtimeHandle);
                Assert.True(called);
            }
        }

        [Fact]
        public void JsObjectBeforeCollectCallbackIsCalledIfObjectIsNotDisposed()
        {
            bool called = false;
            JavaScriptObjectBeforeCollectCallback callback = (IntPtr sender, IntPtr callbackState) =>
            {
                called = true;
            };

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var valueHandle = Jsrt.JsCreateStringUtf8("superman", new UIntPtr((uint)"superman".Length));

                    Jsrt.JsSetObjectBeforeCollectCallback(valueHandle, IntPtr.Zero, callback);
                }

                Jsrt.JsCollectGarbage(runtimeHandle);
                Assert.True(called);
            }
        }

        [Fact]
        public void JsContextCanBeCreated()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    Assert.False(contextHandle == JavaScriptContext.Invalid);
                    Assert.False(contextHandle.IsClosed);
                    Assert.False(contextHandle.IsInvalid);
                }
            }
        }

        [Fact]
        public void JsContextCanBeReleased()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                var contextHandle = Jsrt.JsCreateContext(runtimeHandle);
                try
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    Assert.False(contextHandle.IsClosed);
                }
                finally
                {
                    contextHandle.Dispose();
                    Assert.True(contextHandle.IsClosed);
                }
            }
        }

        [Fact]
        public void JsCurrentContextCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                var contextHandle = Jsrt.JsGetCurrentContext();

                Assert.True(contextHandle.IsInvalid);
            }
        }

        [Fact]
        public void JsCurrentContextCanBeSet()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var currentContextHandle = Jsrt.JsGetCurrentContext();
                    Assert.True(currentContextHandle == contextHandle);
                }
            }
        }

        [Fact]
        public void JsContextOfObjectCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    string str = "I do not fear computers. I fear the lack of them.";
                    var stringHandle = Jsrt.JsCreateStringUtf8(str, new UIntPtr((uint)str.Length));

                    var objectContextHandle = Jsrt.JsGetContextOfObject(stringHandle);

                    Assert.True(objectContextHandle == contextHandle);

                    stringHandle.Dispose();
                    objectContextHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JSContextDataCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var contextData = Jsrt.JsGetContextData(contextHandle);

                    Assert.True(contextData == IntPtr.Zero);
                }
            }
        }

        [Fact]
        public void JSContextDataCanBeSet()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    string myString = "How inappropriate to call this planet 'Earth', when it is clearly 'Ocean'.";
                    var strPtr = Marshal.StringToHGlobalAnsi(myString);
                    try
                    {
                        Jsrt.JsSetContextData(contextHandle, strPtr);

                        var contextData = Jsrt.JsGetContextData(contextHandle);

                        Assert.True(contextData == strPtr);
                        Assert.True(myString == Marshal.PtrToStringAnsi(contextData));
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(strPtr);
                    }

                }
            }
        }

        [Fact]
        public void JsRuntimeCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var contextRuntimeHandle = Jsrt.JsGetRuntime(contextHandle);

                    Assert.True(contextRuntimeHandle == runtimeHandle);
                }
            }
        }

        [Fact]
        public void JsIdleCanBeCalled()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.EnableIdleProcessing, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var nextIdleTick = Jsrt.JsIdle();

                    var nextTickTime = new DateTime(DateTime.Now.Ticks + nextIdleTick);
                    Assert.True(nextTickTime > DateTime.Now);

                }
            }
        }

        [Fact]
        public void JsSymbolCanBeRetrievedFromPropertyId()
        {
            string propertyName = "foo";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var propertyNameHandle = Jsrt.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length));
                    var symbolHandle = Jsrt.JsCreateSymbol(propertyNameHandle);
                    var propertyIdHandle = Jsrt.JsGetPropertyIdFromSymbol(symbolHandle);
                    var retrievedSymbolHandle = Jsrt.JsGetSymbolFromPropertyId(propertyIdHandle);

                    Assert.True(retrievedSymbolHandle != JavaScriptValueSafeHandle.Invalid);

                    retrievedSymbolHandle.Dispose();
                    propertyIdHandle.Dispose();
                    symbolHandle.Dispose();
                    propertyNameHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsPropertyIdTypeCanBeDetermined()
        {
            string propertyName = "foo";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var propertyNameHandle = Jsrt.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length));
                    var symbolHandle = Jsrt.JsCreateSymbol(propertyNameHandle);
                    var symbolPropertyIdHandle = Jsrt.JsGetPropertyIdFromSymbol(symbolHandle);
                    var stringPropertyIdHandle = Jsrt.JsCreatePropertyIdUtf8(propertyName, new UIntPtr((uint)propertyName.Length));

                    var symbolPropertyType = Jsrt.JsGetPropertyIdType(symbolPropertyIdHandle);
                    Assert.True(symbolPropertyType == JavaScriptPropertyIdType.Symbol);

                    var stringPropertyType = Jsrt.JsGetPropertyIdType(stringPropertyIdHandle);
                    Assert.True(stringPropertyType == JavaScriptPropertyIdType.String);

                    stringPropertyIdHandle.Dispose();
                    symbolPropertyIdHandle.Dispose();
                    symbolHandle.Dispose();
                    propertyNameHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsPropertyIdCanBeRetrievedFromASymbol()
        {
            string propertyName = "foo";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var propertyNameHandle = Jsrt.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length));
                    var symbolHandle = Jsrt.JsCreateSymbol(propertyNameHandle);
                    var propertyIdHandle = Jsrt.JsGetPropertyIdFromSymbol(symbolHandle);

                    Assert.True(propertyIdHandle != JavaScriptPropertyId.Invalid);

                    propertyIdHandle.Dispose();
                    symbolHandle.Dispose();
                    propertyNameHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsSymbolCanBeCreated()
        {
            string propertyName = "foo";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var propertyNameHandle = Jsrt.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length));
                    var symbolHandle = Jsrt.JsCreateSymbol(propertyNameHandle);

                    Assert.True(symbolHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(symbolHandle);
                    Assert.True(handleType == JavaScriptValueType.Symbol);

                    symbolHandle.Dispose();
                    propertyNameHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsSymbolDescriptionCanBeRetrieved()
        {
            string propertyName = "foo";
            string toStringPropertyName = "toString";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var propertyNameHandle = Jsrt.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length));
                    var symbolHandle = Jsrt.JsCreateSymbol(propertyNameHandle);


                    var toStringFunctionPropertyIdHandle = Jsrt.JsCreatePropertyIdUtf8(toStringPropertyName, new UIntPtr((uint)toStringPropertyName.Length));

                    var symbolObjHandle = Jsrt.JsConvertValueToObject(symbolHandle);
                    var symbolToStringFnHandle = Jsrt.JsGetProperty(symbolObjHandle, toStringFunctionPropertyIdHandle);

                    var resultHandle = Jsrt.JsCallFunction(symbolToStringFnHandle, new IntPtr[] { symbolObjHandle.DangerousGetHandle() }, 1);

                    var size = Jsrt.JsCopyStringUtf8(resultHandle, null, UIntPtr.Zero);
                    if ((int)size > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    byte[] result = new byte[(int)size];
                    var written = Jsrt.JsCopyStringUtf8(resultHandle, result, size);
                    string resultStr = Encoding.UTF8.GetString(result, 0, result.Length);

                    Assert.True(resultStr == "Symbol(foo)");

                    toStringFunctionPropertyIdHandle.Dispose();
                    symbolObjHandle.Dispose();
                    symbolToStringFnHandle.Dispose();
                    resultHandle.Dispose();

                    symbolHandle.Dispose();
                    propertyNameHandle.Dispose();
                }
            }
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

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, script);

                    var propertySymbols = Jsrt.JsGetOwnPropertySymbols(objHandle);

                    Assert.True(propertySymbols != JavaScriptValueSafeHandle.Invalid);

                    var propertySymbolsType = Jsrt.JsGetValueType(propertySymbols);
                    Assert.True(propertySymbolsType == JavaScriptValueType.Array);

                    propertySymbols.Dispose();
                    objHandle.Dispose();

                }
            }
        }

        [Fact]
        public void JsUndefinedValueCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var undefinedHandle = Jsrt.JsGetUndefinedValue();

                    Assert.True(undefinedHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(undefinedHandle);
                    Assert.True(handleType == JavaScriptValueType.Undefined);

                    undefinedHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsNullValueCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var nullHandle = Jsrt.JsGetNullValue();

                    Assert.True(nullHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(nullHandle);
                    Assert.True(handleType == JavaScriptValueType.Null);

                    nullHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsTrueValueCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var trueHandle = Jsrt.JsGetTrueValue();

                    Assert.True(trueHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(trueHandle);
                    Assert.True(handleType == JavaScriptValueType.Boolean);

                    trueHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsFalseValueCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var falseHandle = Jsrt.JsGetFalseValue();

                    Assert.True(falseHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(falseHandle);
                    Assert.True(handleType == JavaScriptValueType.Boolean);

                    falseHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertBoolValueToBoolean()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var trueHandle = Jsrt.JsBoolToBoolean(true);
                    Assert.True(trueHandle != JavaScriptValueSafeHandle.Invalid);

                    trueHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertBooleanValueToBool()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var trueHandle = Jsrt.JsGetTrueValue();
                    var falseHandle = Jsrt.JsGetFalseValue();

                    var result = Jsrt.JsBooleanToBool(trueHandle);
                    Assert.True(result);

                    result = Jsrt.JsBooleanToBool(falseHandle);
                    Assert.False(result);

                    trueHandle.Dispose();
                    falseHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertValueToBoolean()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var stringValue = "true";
                    var stringHandle = Jsrt.JsCreateStringUtf8(stringValue, new UIntPtr((uint)stringValue.Length));

                    var boolHandle = Jsrt.JsConvertValueToBoolean(stringHandle);

                    var handleType = Jsrt.JsGetValueType(boolHandle);
                    Assert.True(handleType == JavaScriptValueType.Boolean);

                    var result = Jsrt.JsBooleanToBool(boolHandle);
                    Assert.True(result);

                    stringHandle.Dispose();
                    boolHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanGetValueType()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var stringValue = "Dear future generations: Please accept our apologies. We were rolling drunk on petroleum.";
                    var stringHandle = Jsrt.JsCreateStringUtf8(stringValue, new UIntPtr((uint)stringValue.Length));

                    var handleType = Jsrt.JsGetValueType(stringHandle);
                    Assert.True(handleType == JavaScriptValueType.String);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertDoubleValueToNumber()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var doubleHandle = Jsrt.JsDoubleToNumber(3.14156);

                    Assert.True(doubleHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(doubleHandle);
                    Assert.True(handleType == JavaScriptValueType.Number);

                    doubleHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertIntValueToNumber()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var intHandle = Jsrt.JsIntToNumber(3);

                    Assert.True(intHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(intHandle);
                    Assert.True(handleType == JavaScriptValueType.Number);

                    intHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertNumberToDouble()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var doubleHandle = Jsrt.JsDoubleToNumber(3.14159);

                    var result = Jsrt.JsNumberToDouble(doubleHandle);

                    Assert.True(result == 3.14159);

                    doubleHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertNumberToInt()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var intHandle = Jsrt.JsIntToNumber(3);
                    var result = Jsrt.JsNumberToInt(intHandle);

                    Assert.True(result == 3);

                    intHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertValueToNumber()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var stringValue = "2.71828";
                    var stringHandle = Jsrt.JsCreateStringUtf8(stringValue, new UIntPtr((uint)stringValue.Length));
                    var numberHandle = Jsrt.JsConvertValueToNumber(stringHandle);

                    var handleType = Jsrt.JsGetValueType(numberHandle);
                    Assert.True(handleType == JavaScriptValueType.Number);

                    var result = Jsrt.JsNumberToDouble(numberHandle);
                    Assert.True(result == 2.71828);

                    stringHandle.Dispose();
                    numberHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanGetStringLength()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var stringValue = "If your brains were dynamite there wouldn't be enough to blow your hat off.";
                    var stringHandle = Jsrt.JsCreateStringUtf8(stringValue, new UIntPtr((uint)stringValue.Length));

                    var result = Jsrt.JsGetStringLength(stringHandle);
                    Assert.True(stringValue.Length == result);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertValueToString()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var numberHandle = Jsrt.JsDoubleToNumber(2.71828);
                    var stringHandle = Jsrt.JsConvertValueToString(numberHandle);

                    Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(stringHandle);
                    Assert.True(handleType == JavaScriptValueType.String);

                    //Get the size
                    var size = Jsrt.JsCopyStringUtf8(stringHandle, null, UIntPtr.Zero);
                    if ((int)size > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    byte[] result = new byte[(int)size];
                    var written = Jsrt.JsCopyStringUtf8(stringHandle, result, new UIntPtr((uint)result.Length));
                    string resultStr = Encoding.UTF8.GetString(result, 0, result.Length);

                    Assert.True(resultStr == "2.71828");

                    stringHandle.Dispose();
                    numberHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanRetrieveGlobalObject()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var objectHandle = Jsrt.JsGetGlobalObject();

                    Assert.True(objectHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(objectHandle);
                    Assert.True(handleType == JavaScriptValueType.Object);

                    objectHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanCreateObject()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var objectHandle = Jsrt.JsCreateObject();

                    Assert.True(objectHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(objectHandle);
                    Assert.True(handleType == JavaScriptValueType.Object);

                    objectHandle.Dispose();
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class Foo
        {
            public string Bar
            {
                get;
                set;
            }
        }

        [Fact]
        public void JsCanCreateExternalObject()
        {
            var myFoo = new Foo();
            int size = Marshal.SizeOf(myFoo);
            IntPtr myFooPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(myFoo, myFooPtr, false);

            bool called = false;
            JavaScriptObjectFinalizeCallback callback = (IntPtr ptr) =>
            {
                called = true;
                Assert.True(myFooPtr == ptr);
                Marshal.FreeHGlobal(ptr);
            };

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var objectHandle = Jsrt.JsCreateExternalObject(myFooPtr, callback);

                    Assert.True(objectHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(objectHandle);
                    Assert.True(handleType == JavaScriptValueType.Object);

                    //The callback is executed during runtime release.
                    objectHandle.Dispose();
                    Jsrt.JsCollectGarbage(runtimeHandle);

                    //Commenting this as apparently on linux/osx, JsCollectGarbage does call the callback,
                    //while on windows it does not. Might be related to timing, garbage collection, or idle.
                    //Assert.False(called);

                }
            }

            Assert.True(called);
        }

        [Fact]
        public void JsCanConvertValueToObject()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var numberHandle = Jsrt.JsDoubleToNumber(2.71828);
                    var objectHandle = Jsrt.JsConvertValueToObject(numberHandle);

                    Assert.True(objectHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(objectHandle);
                    Assert.True(handleType == JavaScriptValueType.Object);

                    objectHandle.Dispose();
                    numberHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanGetObjectPrototype()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    string stringValue = "Just what do you think you’re doing, Dave?";
                    var stringHandle = Jsrt.JsCreateStringUtf8(stringValue, new UIntPtr((uint)stringValue.Length));
                    var objectHandle = Jsrt.JsConvertValueToObject(stringHandle);
                    var prototypeHandle = Jsrt.JsGetPrototype(objectHandle);

                    Assert.True(prototypeHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(prototypeHandle);
                    Assert.True(handleType == JavaScriptValueType.Object);

                    prototypeHandle.Dispose();
                    objectHandle.Dispose();
                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanSetObjectPrototype()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    //Create a new mammal object to use as a prototype.
                    var mammalHandle = Jsrt.JsCreateObject();

                    string isMammal = "isMammal";
                    var isMammalPropertyHandle = Jsrt.JsCreatePropertyIdUtf8(isMammal, new UIntPtr((uint)isMammal.Length));

                    var trueHandle = Jsrt.JsGetTrueValue();

                    //Set the prototype of cat to be mammal.
                    Jsrt.JsSetProperty(mammalHandle, isMammalPropertyHandle, trueHandle, false);

                    var catHandle = Jsrt.JsCreateObject();

                    Jsrt.JsSetPrototype(catHandle, mammalHandle);

                    //Assert that the prototype of cat is mammal, and that cat now contains a isMammal property set to true.
                    var catPrototypeHandle = Jsrt.JsGetPrototype(catHandle);

                    Assert.True(catPrototypeHandle == mammalHandle);

                    var catIsMammalHandle = Jsrt.JsGetProperty(catHandle, isMammalPropertyHandle);

                    var handleType = Jsrt.JsGetValueType(catIsMammalHandle);
                    Assert.True(handleType == JavaScriptValueType.Boolean);

                    var catIsMammal = Jsrt.JsBooleanToBool(catIsMammalHandle);
                    Assert.True(catIsMammal);

                    mammalHandle.Dispose();
                    isMammalPropertyHandle.Dispose();
                    trueHandle.Dispose();
                    catHandle.Dispose();
                    catPrototypeHandle.Dispose();
                    catIsMammalHandle.Dispose();
                    //Whew!

                }
            }
        }

        [Fact]
        public void JsCanDetermineInstanceOf()
        {
            var script = @"
function Mammal() {
  this.isMammal = 'yes';
}

function MammalSpecies(sMammalSpecies) {
  this.species = sMammalSpecies;
}

MammalSpecies.prototype = new Mammal();
MammalSpecies.prototype.constructor = MammalSpecies;

var oCat = new MammalSpecies('Felis');
";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, script);

                    var oCatPropertyHandle = Jsrt.JsCreatePropertyIdUtf8("oCat", new UIntPtr((uint)"oCat".Length));
                    var fnMammalSpeciesPropertyHandle = Jsrt.JsCreatePropertyIdUtf8("MammalSpecies", new UIntPtr((uint)"MammalSpecies".Length));


                    var globalHandle = Jsrt.JsGetGlobalObject();
                    var fnMammalSpeciesHandle = Jsrt.JsGetProperty(globalHandle, fnMammalSpeciesPropertyHandle);
                    var oCatHandle = Jsrt.JsGetProperty(globalHandle, oCatPropertyHandle);

                    var result = Jsrt.JsInstanceOf(oCatHandle, fnMammalSpeciesHandle);
                    Assert.True(result);

                    oCatPropertyHandle.Dispose();
                    fnMammalSpeciesPropertyHandle.Dispose();

                    oCatHandle.Dispose();
                    fnMammalSpeciesHandle.Dispose();
                    globalHandle.Dispose();

                }
            }
        }

        [Fact]
        public void JsCanDetermineIfObjectIsExtensible()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var objectHandle = Jsrt.JsCreateObject();

                    var isExtensible = Jsrt.JsGetExtensionAllowed(objectHandle);
                    Assert.True(isExtensible);

                    objectHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanMakeObjectNonExtensible()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var objectHandle = Jsrt.JsCreateObject();

                    Jsrt.JsPreventExtension(objectHandle);

                    var isExtensible = Jsrt.JsGetExtensionAllowed(objectHandle);
                    Assert.False(isExtensible);

                    objectHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanGetProperty()
        {
            var script = @"(() => {
var obj = {
    'foo': 'bar',
    'baz': 'qux',
    'quux': 'quuz',
    'corge': 'grault',
    'waldo': 'fred',
    'plugh': 'xyzzy',
    'lol': 'kik'
};
return obj;
})();
";
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, script);

                    var propertyName = "plugh";
                    var propertyIdHandle = Jsrt.JsCreatePropertyIdUtf8(propertyName, new UIntPtr((uint)propertyName.Length));
                    var propertyHandle = Jsrt.JsGetProperty(objHandle, propertyIdHandle);

                    Assert.True(propertyHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(propertyHandle);
                    Assert.True(handleType == JavaScriptValueType.String);

                    propertyIdHandle.Dispose();
                    propertyHandle.Dispose();
                    objHandle.Dispose();

                }
            }
        }

        [Fact]
        public void JsCanGetOwnPropertyDescriptor()
        {
            var script = @"(() => {
var obj = {
    'foo': 'bar',
    'baz': 'qux',
    'quux': 'quuz',
    'corge': 'grault',
    'waldo': 'fred',
    'plugh': 'xyzzy',
    'lol': 'kik'
};
return obj;
})();
";
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, script);

                    var propertyName = "corge";
                    var propertyIdHandle = Jsrt.JsCreatePropertyIdUtf8(propertyName, new UIntPtr((uint)propertyName.Length));
                    var propertyDescriptorHandle = Jsrt.JsGetOwnPropertyDescriptor(objHandle, propertyIdHandle);

                    Assert.True(propertyDescriptorHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(propertyDescriptorHandle);
                    Assert.True(handleType == JavaScriptValueType.Object);

                    propertyIdHandle.Dispose();
                    propertyDescriptorHandle.Dispose();
                    objHandle.Dispose();

                }
            }
        }

        [Fact]
        public void JsCanGetOwnPropertyNames()
        {
            var script = @"(() => {
var obj = {
    'foo': 'bar',
    'baz': 'qux',
    'quux': 'quuz',
    'corge': 'grault',
    'waldo': 'fred',
    'plugh': 'xyzzy',
    'lol': 'kik'
};
return obj;
})();
";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, script);

                    var propertySymbols = Jsrt.JsGetOwnPropertyNames(objHandle);

                    Assert.True(propertySymbols != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(propertySymbols);
                    Assert.True(handleType == JavaScriptValueType.Array);

                    propertySymbols.Dispose();
                    objHandle.Dispose();

                }
            }
        }

        [Fact]
        public void JsCanSetProperty()
        {
            var script = @"(() => {
var obj = {
    'foo': 'bar',
    'baz': 'qux',
    'quux': 'quuz',
    'corge': 'grault',
    'waldo': 'fred',
    'plugh': 'xyzzy',
    'lol': 'kik'
};
return obj;
})();
";
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, script);

                    var propertyName = "baz";
                    var propertyIdHandle = Jsrt.JsCreatePropertyIdUtf8(propertyName, new UIntPtr((uint)propertyName.Length));
                    var newPropertyValue = Jsrt.JsDoubleToNumber(3.14159);

                    //Set the property
                    Jsrt.JsSetProperty(objHandle, propertyIdHandle, newPropertyValue, true);

                    var propertyHandle = Jsrt.JsGetProperty(objHandle, propertyIdHandle);

                    Assert.True(propertyHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(propertyHandle);
                    Assert.True(handleType == JavaScriptValueType.Number);

                    propertyIdHandle.Dispose();
                    propertyHandle.Dispose();
                    objHandle.Dispose();

                }
            }
        }

        [Fact]
        public void JsCanDetermineIfPropertyExists()
        {
            var script = @"(() => {
var obj = {
    'foo': 'bar',
    'baz': 'qux',
    'quux': 'quuz',
    'corge': 'grault',
    'waldo': 'fred',
    'plugh': 'xyzzy',
    'lol': 'kik'
};
return obj;
})();
";
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, script);

                    var propertyName = "lol";
                    var propertyIdHandle = Jsrt.JsCreatePropertyIdUtf8(propertyName, new UIntPtr((uint)propertyName.Length));

                    var propertyExists = Jsrt.JsHasProperty(objHandle, propertyIdHandle);
                    Assert.True(propertyExists);

                    propertyName = "asdf";
                    propertyIdHandle.Dispose();
                    propertyIdHandle = Jsrt.JsCreatePropertyIdUtf8(propertyName, new UIntPtr((uint)propertyName.Length));

                    propertyExists = Jsrt.JsHasProperty(objHandle, propertyIdHandle);
                    Assert.False(propertyExists);


                    propertyIdHandle.Dispose();
                    objHandle.Dispose();

                }
            }
        }

        [Fact]
        public void JsCanDeleteProperty()
        {
            var script = @"(() => {
var obj = {
    'foo': 'bar',
    'baz': 'qux',
    'quux': 'quuz',
    'corge': 'grault',
    'waldo': 'fred',
    'plugh': 'xyzzy',
    'lol': 'kik'
};
return obj;
})();
";
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, script);

                    var propertyName = "waldo";
                    var propertyIdHandle = Jsrt.JsCreatePropertyIdUtf8(propertyName, new UIntPtr((uint)propertyName.Length));
                    var propertyDeletedHandle = Jsrt.JsDeleteProperty(objHandle, propertyIdHandle, true);

                    var wasPropertyDeleted = Jsrt.JsBooleanToBool(propertyDeletedHandle);
                    Assert.True(wasPropertyDeleted);

                    var propertyExists = Jsrt.JsHasProperty(objHandle, propertyIdHandle);
                    Assert.False(propertyExists);

                    propertyDeletedHandle.Dispose();
                    propertyIdHandle.Dispose();
                    objHandle.Dispose();

                }
            }
        }

        [Fact]
        public void JsCanDefineProperty()
        {
            var script = @"(() => {
var obj = {
    'foo': 'bar'
};
return obj;
})();
";
            var propertyDef = @"(() => {
var obj = {
  enumerable: false,
  configurable: false,
  writable: false,
  value: 'static'
};
return obj;
})();
";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, script);
                    JavaScriptValueSafeHandle propertyDefHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, propertyDef);

                    var propertyName = "rico";
                    var propertyIdHandle = Jsrt.JsCreatePropertyIdUtf8(propertyName, new UIntPtr((uint)propertyName.Length));

                    var result = Jsrt.JsDefineProperty(objHandle, propertyIdHandle, propertyDefHandle);
                    Assert.True(result);

                    var propertyExists = Jsrt.JsHasProperty(objHandle, propertyIdHandle);
                    Assert.True(propertyExists);

                    propertyDefHandle.Dispose();
                    propertyIdHandle.Dispose();
                    objHandle.Dispose();

                }
            }
        }

        [Fact]
        public void JsCanDetermineIfIndexedPropertyExists()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    //Test on array
                    var arrayHandle = Jsrt.JsCreateArray(10);
                    var arrayIndexHandle = Jsrt.JsIntToNumber(0);

                    //array[0] = 0;
                    Jsrt.JsSetIndexedProperty(arrayHandle, arrayIndexHandle, arrayIndexHandle);


                    var hasArrayIndex = Jsrt.JsHasIndexedProperty(arrayHandle, arrayIndexHandle);
                    Assert.True(hasArrayIndex);

                    arrayIndexHandle = Jsrt.JsIntToNumber(10);

                    hasArrayIndex = Jsrt.JsHasIndexedProperty(arrayHandle, arrayIndexHandle);
                    Assert.False(hasArrayIndex);

                    arrayIndexHandle.Dispose();
                    arrayHandle.Dispose();

                    //Test on object as associative array.
                    var objectHandle = Jsrt.JsCreateObject();

                    string propertyName = "foo";
                    var propertyIdHandle = Jsrt.JsCreatePropertyIdUtf8(propertyName, new UIntPtr((uint)propertyName.Length));
                    var propertyNameHandle = Jsrt.JsCreateStringUtf8(propertyName, new UIntPtr((uint)propertyName.Length));

                    string notAPropertyName = "bar";
                    var notAPropertyNameHandle = Jsrt.JsCreateStringUtf8(notAPropertyName, new UIntPtr((uint)notAPropertyName.Length));

                    string propertyValue = "Some people choose to see the ugliness in this world. The disarray. I choose to see the beauty.";
                    var propertyValueHandle = Jsrt.JsCreateStringUtf8(propertyValue, new UIntPtr((uint)propertyValue.Length));

                    Jsrt.JsSetProperty(objectHandle, propertyIdHandle, propertyValueHandle, true);

                    var hasObjectIndex = Jsrt.JsHasIndexedProperty(objectHandle, propertyNameHandle);
                    Assert.True(hasObjectIndex);

                    hasObjectIndex = Jsrt.JsHasIndexedProperty(objectHandle, notAPropertyNameHandle);
                    Assert.False(hasObjectIndex);

                    propertyIdHandle.Dispose();
                    propertyNameHandle.Dispose();
                    notAPropertyNameHandle.Dispose();
                    propertyValueHandle.Dispose();
                    objectHandle.Dispose();


                }
            }
        }

        [Fact]
        public void JsCanRetrieveIndexedProperty()
        {
            var script = @"(() => {
var arr = ['Arnold', 'Bernard', 'Charlotte', 'Delores', 'Elsie', 'Felix', 'Hector', 'Lee', 'Maeve', 'Peter', 'Robert', 'Sylvester', 'Teddy', 'Wyatt'];
return arr;
})();
";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle arrayHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, script);

                    var arrayIndexHandle = Jsrt.JsIntToNumber(10);
                    var valueHandle = Jsrt.JsGetIndexedProperty(arrayHandle, arrayIndexHandle);

                    Assert.True(valueHandle != JavaScriptValueSafeHandle.Invalid);

                    var result = Extensions.IJavaScriptEngineExtensions.GetStringUtf8(Jsrt, valueHandle);

                    Assert.Equal("Robert", result);

                    valueHandle.Dispose();
                    arrayIndexHandle.Dispose();
                    arrayHandle.Dispose();

                }
            }
        }

        [Fact]
        public void JsCanSetIndexedProperty()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var arrayHandle = Jsrt.JsCreateArray(50);

                    var value = "The Bicameral Mind";
                    var valueHandle = Jsrt.JsCreateStringUtf8(value, new UIntPtr((uint)value.Length));

                    var arrayIndexHandle = Jsrt.JsIntToNumber(42);

                    Jsrt.JsSetIndexedProperty(arrayHandle, arrayIndexHandle, valueHandle);

                    var resultHandle = Jsrt.JsGetIndexedProperty(arrayHandle, arrayIndexHandle);
                    Assert.True(valueHandle != JavaScriptValueSafeHandle.Invalid);

                    var result = Extensions.IJavaScriptEngineExtensions.GetStringUtf8(Jsrt, valueHandle);

                    Assert.Equal("The Bicameral Mind", result);

                    resultHandle.Dispose();
                    valueHandle.Dispose();
                    arrayIndexHandle.Dispose();
                    arrayHandle.Dispose();

                }
            }
        }

        [Fact]
        public void JsCanDeleteIndexedProperty()
        {
            var script = @"(() => {
var arr = ['Arnold', 'Bernard', 'Charlotte', 'Delores', 'Elsie', 'Felix', 'Hector', 'Lee', 'Maeve', 'Peter', 'Robert', 'Sylvester', 'Teddy', 'Wyatt'];
return arr;
})();
";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle arrayHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, script);

                    var arrayIndexHandle = Jsrt.JsIntToNumber(12);

                    Jsrt.JsDeleteIndexedProperty(arrayHandle, arrayIndexHandle);

                    var valueHandle = Jsrt.JsGetIndexedProperty(arrayHandle, arrayIndexHandle);
                    Assert.True(valueHandle != JavaScriptValueSafeHandle.Invalid);

                    var undefinedHandle = Jsrt.JsGetUndefinedValue();

                    Assert.Equal(undefinedHandle, valueHandle);

                    undefinedHandle.Dispose();
                    valueHandle.Dispose();
                    arrayIndexHandle.Dispose();
                    arrayHandle.Dispose();

                }
            }
        }

        [Fact]
        public void JsArrayCanBeCreated()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var arrayHandle = Jsrt.JsCreateArray(50);

                    Assert.True(arrayHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(arrayHandle);
                    Assert.True(handleType == JavaScriptValueType.Array);

                    arrayHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsArrayBufferCanBeCreated()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var arrayBufferHandle = Jsrt.JsCreateArrayBuffer(50);
                    Assert.True(arrayBufferHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(arrayBufferHandle);
                    Assert.True(handleType == JavaScriptValueType.ArrayBuffer);

                    arrayBufferHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsExternalArrayBufferCanBeCreated()
        {
            var data = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    IntPtr ptrScript = Marshal.StringToHGlobalAnsi(data);

                    try
                    {
                        var externalArrayBufferHandle = Jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)data.Length, null, IntPtr.Zero);
                        Assert.True(externalArrayBufferHandle != JavaScriptValueSafeHandle.Invalid);

                        var handleType = Jsrt.JsGetValueType(externalArrayBufferHandle);
                        Assert.True(handleType == JavaScriptValueType.ArrayBuffer);

                        externalArrayBufferHandle.Dispose();
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptrScript);
                    }
                }
            }
        }

        [Fact]
        public void JsTypedArrayCanBeCreated()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var typedArrayHandle = Jsrt.JsCreateTypedArray(JavaScriptTypedArrayType.Int8, JavaScriptValueSafeHandle.Invalid, 0, 50);

                    Assert.True(typedArrayHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(typedArrayHandle);
                    Assert.True(handleType == JavaScriptValueType.TypedArray);

                    typedArrayHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsDataViewCanBeCreated()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var arrayBufferHandle = Jsrt.JsCreateArrayBuffer(50);
                    var dataViewHandle = Jsrt.JsCreateDataView(arrayBufferHandle, 0, 50);

                    Assert.True(dataViewHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Jsrt.JsGetValueType(dataViewHandle);
                    Assert.True(handleType == JavaScriptValueType.DataView);

                    arrayBufferHandle.Dispose();
                    dataViewHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsTypedArrayInfoCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var typedArrayHandle = Jsrt.JsCreateTypedArray(JavaScriptTypedArrayType.Int8, JavaScriptValueSafeHandle.Invalid, 0, 50);

                    JavaScriptValueSafeHandle arrayBufferHandle;
                    uint byteOffset, byteLength;
                    var typedArrayType = Jsrt.JsGetTypedArrayInfo(typedArrayHandle, out arrayBufferHandle, out byteOffset, out byteLength);

                    Assert.True(typedArrayType == JavaScriptTypedArrayType.Int8);
                    Assert.True(arrayBufferHandle != JavaScriptValueSafeHandle.Invalid);
                    Assert.True(byteOffset == 0);
                    Assert.True(byteLength == 50);

                    arrayBufferHandle.Dispose();
                    typedArrayHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsArrayBufferStorageCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var arrayBufferHandle = Jsrt.JsCreateArrayBuffer(50);

                    uint bufferLength;
                    var ptrBuffer = Jsrt.JsGetArrayBufferStorage(arrayBufferHandle, out bufferLength);

                    byte[] buffer = new byte[bufferLength];
                    Marshal.Copy(ptrBuffer, buffer, 0, (int)bufferLength);

                    Assert.True(bufferLength == 50);
                    Assert.True(buffer.Length == 50);

                    arrayBufferHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsTypedArrayStorageCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var typedArrayHandle = Jsrt.JsCreateTypedArray(JavaScriptTypedArrayType.Int8, JavaScriptValueSafeHandle.Invalid, 0, 50);

                    uint bufferLength;
                    JavaScriptTypedArrayType typedArrayType;
                    int elementSize;
                    var ptrBuffer = Jsrt.JsGetTypedArrayStorage(typedArrayHandle, out bufferLength, out typedArrayType, out elementSize);

                    //Normally, we'd create an appropriately typed buffer based on elementsize.
                    Assert.True(elementSize == 1); //byte

                    byte[] buffer = new byte[bufferLength];
                    Marshal.Copy(ptrBuffer, buffer, 0, (int)bufferLength);

                    Assert.True(bufferLength == 50);
                    Assert.True(buffer.Length == 50);
                    Assert.True(typedArrayType == JavaScriptTypedArrayType.Int8);


                    typedArrayHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsDataViewStorageCanBeRetrieved()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    var arrayBufferHandle = Jsrt.JsCreateArrayBuffer(50);
                    var dataViewHandle = Jsrt.JsCreateDataView(arrayBufferHandle, 0, 50);

                    uint bufferLength;
                    var ptrBuffer = Jsrt.JsGetDataViewStorage(dataViewHandle, out bufferLength);

                    byte[] buffer = new byte[bufferLength];
                    Marshal.Copy(ptrBuffer, buffer, 0, (int)bufferLength);

                    Assert.True(bufferLength == 50);
                    Assert.True(buffer.Length == 50);

                    dataViewHandle.Dispose();
                    arrayBufferHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsPromisesCannotResolveWithoutCallback()
        {
            var script = @"
var allDone = false;
new Promise(function(resolve, reject) {
        resolve('basic:success');
    }).then(function() {
        return new Promise(function(resolve, reject) {
            resolve('second:success');
        });
    }).then(function() { 
        allDone = true
    });";
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    try
                    {
                        var promiseHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, script);
                        Assert.True(false, "Promises should not be able to be resolved without a promise continuation callback defined.");
                    }
                    catch(JavaScriptScriptException ex)
                    {
                        Assert.Equal("Object doesn't support this action", ex.ErrorMessage);
                    }
                }
            }
        }

        [Fact]
        public void JsPromisesContinuationCallbacksAreExecuted()
        {
            int calledCount = 0;
            var taskQueue = new Stack<JavaScriptValueSafeHandle>();

            JavaScriptPromiseContinuationCallback promiseContinuationCallback = (IntPtr taskHandle, IntPtr callbackState) =>
            {
                calledCount++;
                var task = JavaScriptValueSafeHandle.CreateJavaScriptValueFromHandle(taskHandle);
                taskQueue.Push(task);
            };

            var script = @"
var allDone = false;
new Promise(function(resolve, reject) {
        resolve('basic:success');
    }).then(function() {
        return new Promise(function(resolve, reject) {
            resolve('second:success');
        });
    }).then(function() { 
        allDone = true
    });";

            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    Jsrt.JsSetPromiseContinuationCallback(promiseContinuationCallback, IntPtr.Zero);

                    var promiseHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Jsrt, script);
                    Assert.True(promiseHandle != JavaScriptValueSafeHandle.Invalid);

                    var undefinedHandle = Jsrt.JsGetUndefinedValue();
                    var trueHandle = Jsrt.JsGetTrueValue();
                    var falseHandle = Jsrt.JsGetFalseValue();

                    var allDonePropertyName = "allDone";
                    var allDonePropertyIdHandle = Jsrt.JsCreatePropertyIdUtf8(allDonePropertyName, new UIntPtr((uint)allDonePropertyName.Length));

                    var globalObjectHandle = Jsrt.JsGetGlobalObject();
                    var allDoneHandle = Jsrt.JsGetProperty(globalObjectHandle, allDonePropertyIdHandle);
                    Assert.True(allDoneHandle == falseHandle);

                    
                    var args = new IntPtr[] { undefinedHandle.DangerousGetHandle() };

                    do
                    {
                        var task = taskQueue.Pop();
                        try
                        {
                            var handleType = Jsrt.JsGetValueType(task);
                            Assert.True(handleType == JavaScriptValueType.Function);
                            var result = Jsrt.JsCallFunction(task, args, (ushort)args.Length);
                            result.Dispose();
                        }
                        finally
                        {
                            task.Dispose();
                        }
                    } while (taskQueue.Count > 0);

                    allDoneHandle.Dispose();
                    allDoneHandle = Jsrt.JsGetProperty(globalObjectHandle, allDonePropertyIdHandle);
                    Assert.True(allDoneHandle == trueHandle);

                    allDoneHandle.Dispose();
                    allDonePropertyIdHandle.Dispose();
                    trueHandle.Dispose();
                    falseHandle.Dispose();
                    undefinedHandle.Dispose();
                    globalObjectHandle.Dispose();
                    promiseHandle.Dispose();
                }
            }

            Assert.True(calledCount > 0);
        }
    }
}
