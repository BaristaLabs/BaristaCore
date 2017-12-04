namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Direct tests against the IChakraApi layer
    /// </summary>
    public class ICommonJavaScriptEngine_Facts
    {
        #region Test Support
        private IJavaScriptEngine Engine;
        private readonly ITestOutputHelper Output;

        public ICommonJavaScriptEngine_Facts(ITestOutputHelper output)
        {
            Engine = JavaScriptEngineFactory.CreateChakraEngine();
            Output = output;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class Point
        {
            public int x;
            public int y;
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

        public IntPtr GetPtr<T>(T data)
        {
            //Pin data to unmanaged memory;
            IntPtr dataPtr = Marshal.AllocHGlobal(Marshal.SizeOf(data));
            Marshal.StructureToPtr(data, dataPtr, true);
            return dataPtr;
        }
        #endregion

        [Fact]
        public void JsRuntimeCanBeConstructed()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
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
            runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null);
            Assert.NotEqual(runtimeHandle, JavaScriptRuntimeSafeHandle.Invalid);
            Assert.False(runtimeHandle.IsClosed);

            Engine.JsDisposeRuntime(runtimeHandle.DangerousGetHandle());
        }

        [Fact]
        public void JsCollectGarbageCanBeCalled()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                Engine.JsCollectGarbage(runtimeHandle);
            }
        }

        [Fact]
        public void JsRuntimeMemoryUsageCanBeRetrieved()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                var usage = Engine.JsGetRuntimeMemoryUsage(runtimeHandle);

                Assert.True(usage > 0);
            }
        }

        [Fact]
        public void JsRuntimeMemoryLimitCanBeRetrieved()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                var limit = Engine.JsGetRuntimeMemoryLimit(runtimeHandle);

                Assert.True(limit == ulong.MaxValue);
            }
        }

        [Fact]
        public void JsRuntimeMemoryLimitCanBeSet()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {

                Engine.JsSetRuntimeMemoryLimit(runtimeHandle, 64000);

                var limit = Engine.JsGetRuntimeMemoryLimit(runtimeHandle);

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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                Engine.JsSetRuntimeMemoryAllocationCallback(runtimeHandle, IntPtr.Zero, callback);

                //Bounce a context to get an allocation
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                Engine.JsSetRuntimeBeforeCollectCallback(runtimeHandle, IntPtr.Zero, callback);

                Engine.JsCollectGarbage(runtimeHandle);
            }

            Assert.True(called);
        }

        [Fact]
        public void JsValueRefCanBeAdded()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var myString = "Have you ever questioned the nature of your reality?";
                    var stringHandle = Engine.JsCreateString(myString, (ulong)myString.Length);

                    var count = Engine.JsAddRef(stringHandle);

                    Assert.Equal((uint)2, count);

                    Engine.JsCollectGarbage(runtimeHandle);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsValueRefCanBeReleased()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var myString = "Have you ever questioned the nature of your reality?";
                    var stringHandle = Engine.JsCreateString(myString, (ulong)myString.Length);

                    var count = Engine.JsAddRef(stringHandle);

                    //ChakraEngine implementation automatically adds a reference, thus the count is now 2.
                    Assert.Equal((uint)2, count);

                    count = Engine.JsRelease(stringHandle);

                    Assert.Equal((uint)1, count);

                    Engine.JsCollectGarbage(runtimeHandle);

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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var valueHandle = Engine.JsCreateString("superman", (ulong)"superman".Length);

                    Engine.JsSetObjectBeforeCollectCallback(valueHandle, IntPtr.Zero, callback);

                    //The callback is executed once the context is released and garbage is collected.
                    valueHandle.Dispose();
                }

                Engine.JsCollectGarbage(runtimeHandle);
                Assert.True(called);
            }
        }

        [Fact]
        public void JsObjectBeforeCollectCallbackMustHaveACurrentContext()
        {
            bool called = false;
            JavaScriptObjectBeforeCollectCallback callback = (IntPtr sender, IntPtr callbackState) =>
            {
                called = true;
            };

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);
                    var valueHandle = Engine.JsCreateString("superman", (ulong)"superman".Length);
                    Engine.JsSetCurrentContext(JavaScriptContextSafeHandle.Invalid);

                    try
                    {
                        Engine.JsSetObjectBeforeCollectCallback(valueHandle, IntPtr.Zero, callback);
                        Assert.False(true);
                    }
                    catch (JavaScriptUsageException ex)
                    {
                        Assert.Equal("No current context.", ex.Message);
                    }

                    Engine.JsSetCurrentContext(contextHandle);
                    valueHandle.Dispose();
                }
            }

            Assert.False(called);
        }

        [Fact]
        public void JsObjectBeforeCollectCallbackIsCalledIfObjectIsNotDisposed()
        {
            bool called = false;
            JavaScriptObjectBeforeCollectCallback callback = (IntPtr sender, IntPtr callbackState) =>
            {
                called = true;
            };

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var valueHandle = Engine.JsCreateString("superman", (ulong)"superman".Length);

                    Engine.JsSetObjectBeforeCollectCallback(valueHandle, IntPtr.Zero, callback);
                }

                Engine.JsCollectGarbage(runtimeHandle);
            }

            //Since the object was not disposed, and a reference still exists,
            //the callback is called during runtime collection
            Assert.True(called);
        }

        [Fact]
        public void JsObjectBeforeCollectCallbackCannotBeSetOnARuntime()
        {
            bool called = false;
            JavaScriptObjectBeforeCollectCallback callback = (IntPtr sender, IntPtr callbackState) =>
            {
                called = true;
            };

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    try
                    {
                        Engine.JsSetObjectBeforeCollectCallback(runtimeHandle, IntPtr.Zero, callback);
                        Assert.False(true);
                    }
                    catch (JavaScriptUsageException ex)
                    {
                        Assert.Equal("Invalid argument.", ex.Message);
                    }

                }
            }

            Assert.False(called);
        }

        [Fact]
        public void JsObjectBeforeCollectCallbackCanBeSetOnAPropertyId()
        {
            bool called = false;
            JavaScriptObjectBeforeCollectCallback callback = (IntPtr sender, IntPtr callbackState) =>
            {
                called = true;
            };

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);
                    string name = "oof";
                    var propertyHandle = Engine.JsCreatePropertyId(name, (ulong)name.Length);
                    Engine.JsSetObjectBeforeCollectCallback(propertyHandle, IntPtr.Zero, callback);
                    propertyHandle.Dispose();
                }

                Engine.JsCollectGarbage(runtimeHandle);
                Assert.True(called);
            }
        }

        [Fact]
        public void JsObjectBeforeCollectCallbackCanBeSetOnAContext()
        {
            bool called = false;
            JavaScriptObjectBeforeCollectCallback callback = (IntPtr sender, IntPtr callbackState) =>
            {
                called = true;
            };

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetObjectBeforeCollectCallback(contextHandle, IntPtr.Zero, callback);
                }
            }
            Assert.True(called);
        }

        [Fact]
        public void JsObjectBeforeCollectCallbackCanBeSetOnUndefined()
        {
            bool called = false;
            JavaScriptObjectBeforeCollectCallback callback = (IntPtr sender, IntPtr callbackState) =>
            {
                called = true;
            };

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var valueHandle = Engine.JsGetUndefinedValue();

                    Engine.JsSetObjectBeforeCollectCallback(valueHandle, IntPtr.Zero, callback);

                    //The callback is executed once the context is released and garbage is collected.
                    valueHandle.Dispose();
                }
            }

            //As "Undefined" is a 'const' it disposes when the runtime itself cleans up.
            Assert.True(called);
        }

        [Fact]
        public void JsContextCanBeCreated()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    Assert.False(contextHandle == JavaScriptContextSafeHandle.Invalid);
                    Assert.False(contextHandle.IsClosed);
                    Assert.False(contextHandle.IsInvalid);
                }
            }
        }

        [Fact]
        public void JsContextCanBeReleased()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                var contextHandle = Engine.JsCreateContext(runtimeHandle);
                try
                {
                    Engine.JsSetCurrentContext(contextHandle);

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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                var contextHandle = Engine.JsGetCurrentContext();

                Assert.True(contextHandle.IsInvalid);
            }
        }

        [Fact]
        public void JsCurrentContextCanBeSet()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var currentContextHandle = Engine.JsGetCurrentContext();
                    Assert.True(currentContextHandle == contextHandle);
                }
            }
        }

        [Fact]
        public void JsContextOfObjectCanBeRetrieved()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    string str = "I do not fear computers. I fear the lack of them.";
                    var stringHandle = Engine.JsCreateString(str, (ulong)str.Length);

                    var objectContextHandle = Engine.JsGetContextOfObject(stringHandle);

                    Assert.True(objectContextHandle == contextHandle);

                    stringHandle.Dispose();
                    objectContextHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JSContextDataCanBeRetrieved()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var contextData = Engine.JsGetContextData(contextHandle);

                    Assert.True(contextData == IntPtr.Zero);
                }
            }
        }

        [Fact]
        public void JSContextDataCanBeSet()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    string myString = "How inappropriate to call this planet 'Earth', when it is clearly 'Ocean'.";
                    var strPtr = Marshal.StringToHGlobalAnsi(myString);
                    try
                    {
                        Engine.JsSetContextData(contextHandle, strPtr);

                        var contextData = Engine.JsGetContextData(contextHandle);

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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var contextRuntimeHandle = Engine.JsGetRuntime(contextHandle);

                    Assert.True(contextRuntimeHandle == runtimeHandle);
                }
            }
        }

        [Fact]
        public void JsIdleCanBeCalled()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.EnableIdleProcessing, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var nextIdleTick = Engine.JsIdle();

                    var nextTickTime = new DateTime(DateTime.Now.Ticks + nextIdleTick);
                    Assert.True(nextTickTime > DateTime.Now);
                }
            }
        }

        [Fact]
        public void JsSymbolCanBeRetrievedFromPropertyId()
        {
            string propertyName = "foo";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var propertyNameHandle = Engine.JsCreateString(propertyName, (ulong)propertyName.Length);
                    var symbolHandle = Engine.JsCreateSymbol(propertyNameHandle);
                    var propertyIdHandle = Engine.JsGetPropertyIdFromSymbol(symbolHandle);
                    var retrievedSymbolHandle = Engine.JsGetSymbolFromPropertyId(propertyIdHandle);

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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var propertyNameHandle = Engine.JsCreateString(propertyName, (ulong)propertyName.Length);
                    var symbolHandle = Engine.JsCreateSymbol(propertyNameHandle);
                    var symbolPropertyIdHandle = Engine.JsGetPropertyIdFromSymbol(symbolHandle);
                    var stringPropertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length);

                    var symbolPropertyType = Engine.JsGetPropertyIdType(symbolPropertyIdHandle);
                    Assert.True(symbolPropertyType == JavaScriptPropertyIdType.Symbol);

                    var stringPropertyType = Engine.JsGetPropertyIdType(stringPropertyIdHandle);
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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var propertyNameHandle = Engine.JsCreateString(propertyName, (ulong)propertyName.Length);
                    var symbolHandle = Engine.JsCreateSymbol(propertyNameHandle);
                    var propertyIdHandle = Engine.JsGetPropertyIdFromSymbol(symbolHandle);

                    Assert.True(propertyIdHandle != JavaScriptPropertyIdSafeHandle.Invalid);

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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var propertyNameHandle = Engine.JsCreateString(propertyName, (ulong)propertyName.Length);
                    var symbolHandle = Engine.JsCreateSymbol(propertyNameHandle);

                    Assert.True(symbolHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(symbolHandle);
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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var propertyNameHandle = Engine.JsCreateString(propertyName, (ulong)propertyName.Length);
                    var symbolHandle = Engine.JsCreateSymbol(propertyNameHandle);


                    var toStringFunctionPropertyIdHandle = Engine.JsCreatePropertyId(toStringPropertyName, (ulong)toStringPropertyName.Length);

                    var symbolObjHandle = Engine.JsConvertValueToObject(symbolHandle);
                    var symbolToStringFnHandle = Engine.JsGetProperty(symbolObjHandle, toStringFunctionPropertyIdHandle);

                    var resultHandle = Engine.JsCallFunction(symbolToStringFnHandle, new IntPtr[] { symbolObjHandle.DangerousGetHandle() }, 1);

                    var size = Engine.JsCopyString(resultHandle, null, 0);
                    if ((int)size > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    byte[] result = new byte[(int)size];
                    var written = Engine.JsCopyString(resultHandle, result, size);
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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);

                    var propertySymbols = Engine.JsGetOwnPropertySymbols(objHandle);

                    Assert.True(propertySymbols != JavaScriptValueSafeHandle.Invalid);

                    var propertySymbolsType = Engine.JsGetValueType(propertySymbols);
                    Assert.True(propertySymbolsType == JavaScriptValueType.Array);

                    propertySymbols.Dispose();
                    objHandle.Dispose();

                }
            }
        }

        [Fact]
        public void JsUndefinedValueCanBeRetrieved()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var undefinedHandle = Engine.JsGetUndefinedValue();

                    Assert.True(undefinedHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(undefinedHandle);
                    Assert.True(handleType == JavaScriptValueType.Undefined);

                    undefinedHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsUndefinedHandlesFromDifferentContextsAreDifferent()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                JavaScriptValueSafeHandle undefinedHandle1;
                JavaScriptValueSafeHandle undefinedHandle2;
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    undefinedHandle1 = Engine.JsGetUndefinedValue();
                }

                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    undefinedHandle2 = Engine.JsGetUndefinedValue();
                }

                Assert.NotEqual(undefinedHandle1, undefinedHandle2);
            }
        }

        [Fact]
        public void JsNullValueCanBeRetrieved()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var nullHandle = Engine.JsGetNullValue();

                    Assert.True(nullHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(nullHandle);
                    Assert.True(handleType == JavaScriptValueType.Null);

                    nullHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsTrueValueCanBeRetrieved()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var trueHandle = Engine.JsGetTrueValue();

                    Assert.True(trueHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(trueHandle);
                    Assert.True(handleType == JavaScriptValueType.Boolean);

                    trueHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsFalseValueCanBeRetrieved()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var falseHandle = Engine.JsGetFalseValue();

                    Assert.True(falseHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(falseHandle);
                    Assert.True(handleType == JavaScriptValueType.Boolean);

                    falseHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertBoolValueToBoolean()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var trueHandle = Engine.JsBoolToBoolean(true);
                    Assert.True(trueHandle != JavaScriptValueSafeHandle.Invalid);

                    trueHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertBooleanValueToBool()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var trueHandle = Engine.JsGetTrueValue();
                    var falseHandle = Engine.JsGetFalseValue();

                    var result = Engine.JsBooleanToBool(trueHandle);
                    Assert.True(result);

                    result = Engine.JsBooleanToBool(falseHandle);
                    Assert.False(result);

                    trueHandle.Dispose();
                    falseHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertValueToBoolean()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var stringValue = "true";
                    var stringHandle = Engine.JsCreateString(stringValue, (ulong)stringValue.Length);

                    var boolHandle = Engine.JsConvertValueToBoolean(stringHandle);

                    var handleType = Engine.JsGetValueType(boolHandle);
                    Assert.True(handleType == JavaScriptValueType.Boolean);

                    var result = Engine.JsBooleanToBool(boolHandle);
                    Assert.True(result);

                    stringHandle.Dispose();
                    boolHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanGetValueType()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var stringValue = "Dear future generations: Please accept our apologies. We were rolling drunk on petroleum.";
                    var stringHandle = Engine.JsCreateString(stringValue, (ulong)stringValue.Length);

                    var handleType = Engine.JsGetValueType(stringHandle);
                    Assert.True(handleType == JavaScriptValueType.String);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertDoubleValueToNumber()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var doubleHandle = Engine.JsDoubleToNumber(3.14156);

                    Assert.True(doubleHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(doubleHandle);
                    Assert.True(handleType == JavaScriptValueType.Number);

                    doubleHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertIntValueToNumber()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var intHandle = Engine.JsIntToNumber(3);

                    Assert.True(intHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(intHandle);
                    Assert.True(handleType == JavaScriptValueType.Number);

                    intHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertNumberToDouble()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var doubleHandle = Engine.JsDoubleToNumber(3.14159);

                    var result = Engine.JsNumberToDouble(doubleHandle);

                    Assert.True(result == 3.14159);

                    doubleHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertNumberToInt()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var intHandle = Engine.JsIntToNumber(3);
                    var result = Engine.JsNumberToInt(intHandle);

                    Assert.True(result == 3);

                    intHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertValueToNumber()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var stringValue = "2.71828";
                    var stringHandle = Engine.JsCreateString(stringValue, (ulong)stringValue.Length);
                    var numberHandle = Engine.JsConvertValueToNumber(stringHandle);

                    var handleType = Engine.JsGetValueType(numberHandle);
                    Assert.True(handleType == JavaScriptValueType.Number);

                    var result = Engine.JsNumberToDouble(numberHandle);
                    Assert.True(result == 2.71828);

                    stringHandle.Dispose();
                    numberHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanGetStringLength()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var stringValue = "If your brains were dynamite there wouldn't be enough to blow your hat off.";
                    var stringHandle = Engine.JsCreateString(stringValue, (ulong)stringValue.Length);

                    var result = Engine.JsGetStringLength(stringHandle);
                    Assert.True(stringValue.Length == result);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConvertValueToString()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var numberHandle = Engine.JsDoubleToNumber(2.71828);
                    var stringHandle = Engine.JsConvertValueToString(numberHandle);

                    Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(stringHandle);
                    Assert.True(handleType == JavaScriptValueType.String);

                    //Get the size
                    var size = Engine.JsCopyString(stringHandle, null, 0);
                    if ((int)size > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    byte[] result = new byte[(int)size];
                    var written = Engine.JsCopyString(stringHandle, result, (ulong)result.Length);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var objectHandle = Engine.JsGetGlobalObject();

                    Assert.True(objectHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(objectHandle);
                    Assert.True(handleType == JavaScriptValueType.Object);

                    objectHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanCreateObject()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var objectHandle = Engine.JsCreateObject();

                    Assert.True(objectHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(objectHandle);
                    Assert.True(handleType == JavaScriptValueType.Object);

                    objectHandle.Dispose();
                }
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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var objectHandle = Engine.JsCreateExternalObject(myFooPtr, callback);

                    Assert.True(objectHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(objectHandle);
                    Assert.True(handleType == JavaScriptValueType.Object);

                    //The callback is executed during runtime release.
                    objectHandle.Dispose();
                    Engine.JsCollectGarbage(runtimeHandle);

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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var numberHandle = Engine.JsDoubleToNumber(2.71828);
                    var objectHandle = Engine.JsConvertValueToObject(numberHandle);

                    Assert.True(objectHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(objectHandle);
                    Assert.True(handleType == JavaScriptValueType.Object);

                    objectHandle.Dispose();
                    numberHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanGetObjectPrototype()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    string stringValue = "Just what do you think you’re doing, Dave?";
                    var stringHandle = Engine.JsCreateString(stringValue, (ulong)stringValue.Length);
                    var objectHandle = Engine.JsConvertValueToObject(stringHandle);
                    var prototypeHandle = Engine.JsGetPrototype(objectHandle);

                    Assert.True(prototypeHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(prototypeHandle);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    //Create a new mammal object to use as a prototype.
                    var mammalHandle = Engine.JsCreateObject();

                    string isMammal = "isMammal";
                    var isMammalPropertyHandle = Engine.JsCreatePropertyId(isMammal, (ulong)isMammal.Length);

                    var trueHandle = Engine.JsGetTrueValue();

                    //Set the prototype of cat to be mammal.
                    Engine.JsSetProperty(mammalHandle, isMammalPropertyHandle, trueHandle, false);

                    var catHandle = Engine.JsCreateObject();

                    Engine.JsSetPrototype(catHandle, mammalHandle);

                    //Assert that the prototype of cat is mammal, and that cat now contains a isMammal property set to true.
                    var catPrototypeHandle = Engine.JsGetPrototype(catHandle);

                    Assert.True(catPrototypeHandle == mammalHandle);

                    var catIsMammalHandle = Engine.JsGetProperty(catHandle, isMammalPropertyHandle);

                    var handleType = Engine.JsGetValueType(catIsMammalHandle);
                    Assert.True(handleType == JavaScriptValueType.Boolean);

                    var catIsMammal = Engine.JsBooleanToBool(catIsMammalHandle);
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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);

                    var oCatPropertyHandle = Engine.JsCreatePropertyId("oCat", (ulong)"oCat".Length);
                    var fnMammalSpeciesPropertyHandle = Engine.JsCreatePropertyId("MammalSpecies", (ulong)"MammalSpecies".Length);


                    var globalHandle = Engine.JsGetGlobalObject();
                    var fnMammalSpeciesHandle = Engine.JsGetProperty(globalHandle, fnMammalSpeciesPropertyHandle);
                    var oCatHandle = Engine.JsGetProperty(globalHandle, oCatPropertyHandle);

                    var result = Engine.JsInstanceOf(oCatHandle, fnMammalSpeciesHandle);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var objectHandle = Engine.JsCreateObject();

                    var isExtensible = Engine.JsGetExtensionAllowed(objectHandle);
                    Assert.True(isExtensible);

                    objectHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanMakeObjectNonExtensible()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var objectHandle = Engine.JsCreateObject();

                    Engine.JsPreventExtension(objectHandle);

                    var isExtensible = Engine.JsGetExtensionAllowed(objectHandle);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);

                    var propertyName = "plugh";
                    var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length);
                    var propertyHandle = Engine.JsGetProperty(objHandle, propertyIdHandle);

                    Assert.True(propertyHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(propertyHandle);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);

                    var propertyName = "corge";
                    var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length);
                    var propertyDescriptorHandle = Engine.JsGetOwnPropertyDescriptor(objHandle, propertyIdHandle);

                    Assert.True(propertyDescriptorHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(propertyDescriptorHandle);
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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);

                    var propertySymbols = Engine.JsGetOwnPropertyNames(objHandle);

                    Assert.True(propertySymbols != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(propertySymbols);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);

                    var propertyName = "baz";
                    var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length);
                    var newPropertyValue = Engine.JsDoubleToNumber(3.14159);

                    //Set the property
                    Engine.JsSetProperty(objHandle, propertyIdHandle, newPropertyValue, true);

                    var propertyHandle = Engine.JsGetProperty(objHandle, propertyIdHandle);

                    Assert.True(propertyHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(propertyHandle);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);

                    var propertyName = "lol";
                    var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length);

                    var propertyExists = Engine.JsHasProperty(objHandle, propertyIdHandle);
                    Assert.True(propertyExists);

                    propertyName = "asdf";
                    propertyIdHandle.Dispose();
                    propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length);

                    propertyExists = Engine.JsHasProperty(objHandle, propertyIdHandle);
                    Assert.False(propertyExists);


                    propertyIdHandle.Dispose();
                    objHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanDetermineIfOwnPropertyExists()
        {
            var script = @"(() => {
var o = new Object();
o.prop = 'exists';

return o;
})();
";
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);

                    var propertyName = "prop";
                    var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length);

                    var propertyExists = Engine.JsHasOwnProperty(objHandle, propertyIdHandle);
                    Assert.True(propertyExists);

                    propertyName = "toString";
                    propertyIdHandle.Dispose();
                    propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length);

                    propertyExists = Engine.JsHasProperty(objHandle, propertyIdHandle);
                    Assert.True(propertyExists);

                    propertyExists = Engine.JsHasOwnProperty(objHandle, propertyIdHandle);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);

                    var propertyName = "waldo";
                    var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length);
                    var propertyDeletedHandle = Engine.JsDeleteProperty(objHandle, propertyIdHandle, true);

                    var wasPropertyDeleted = Engine.JsBooleanToBool(propertyDeletedHandle);
                    Assert.True(wasPropertyDeleted);

                    var propertyExists = Engine.JsHasProperty(objHandle, propertyIdHandle);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle objHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);
                    JavaScriptValueSafeHandle propertyDefHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, propertyDef);

                    var propertyName = "rico";
                    var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length);

                    var result = Engine.JsDefineProperty(objHandle, propertyIdHandle, propertyDefHandle);
                    Assert.True(result);

                    var propertyExists = Engine.JsHasProperty(objHandle, propertyIdHandle);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    //Test on array
                    var arrayHandle = Engine.JsCreateArray(10);
                    var arrayIndexHandle = Engine.JsIntToNumber(0);

                    //array[0] = 0;
                    Engine.JsSetIndexedProperty(arrayHandle, arrayIndexHandle, arrayIndexHandle);


                    var hasArrayIndex = Engine.JsHasIndexedProperty(arrayHandle, arrayIndexHandle);
                    Assert.True(hasArrayIndex);

                    arrayIndexHandle = Engine.JsIntToNumber(10);

                    hasArrayIndex = Engine.JsHasIndexedProperty(arrayHandle, arrayIndexHandle);
                    Assert.False(hasArrayIndex);

                    arrayIndexHandle.Dispose();
                    arrayHandle.Dispose();

                    //Test on object as associative array.
                    var objectHandle = Engine.JsCreateObject();

                    string propertyName = "foo";
                    var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length);
                    var propertyNameHandle = Engine.JsCreateString(propertyName, (ulong)propertyName.Length);

                    string notAPropertyName = "bar";
                    var notAPropertyNameHandle = Engine.JsCreateString(notAPropertyName, (ulong)notAPropertyName.Length);

                    string propertyValue = "Some people choose to see the ugliness in this world. The disarray. I choose to see the beauty.";
                    var propertyValueHandle = Engine.JsCreateString(propertyValue, (ulong)propertyValue.Length);

                    Engine.JsSetProperty(objectHandle, propertyIdHandle, propertyValueHandle, true);

                    var hasObjectIndex = Engine.JsHasIndexedProperty(objectHandle, propertyNameHandle);
                    Assert.True(hasObjectIndex);

                    hasObjectIndex = Engine.JsHasIndexedProperty(objectHandle, notAPropertyNameHandle);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle arrayHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);

                    var arrayIndexHandle = Engine.JsIntToNumber(10);
                    var valueHandle = Engine.JsGetIndexedProperty(arrayHandle, arrayIndexHandle);
                    Assert.True(valueHandle != JavaScriptValueSafeHandle.Invalid);

                    var result = Extensions.IJavaScriptEngineExtensions.GetStringUtf8(Engine, valueHandle);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var arrayHandle = Engine.JsCreateArray(50);

                    var value = "The Bicameral Mind";
                    var valueHandle = Engine.JsCreateString(value, (ulong)value.Length);

                    var arrayIndexHandle = Engine.JsIntToNumber(42);

                    Engine.JsSetIndexedProperty(arrayHandle, arrayIndexHandle, valueHandle);

                    var resultHandle = Engine.JsGetIndexedProperty(arrayHandle, arrayIndexHandle);
                    Assert.True(valueHandle != JavaScriptValueSafeHandle.Invalid);

                    var result = Extensions.IJavaScriptEngineExtensions.GetStringUtf8(Engine, valueHandle);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptValueSafeHandle arrayHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);

                    var arrayIndexHandle = Engine.JsIntToNumber(12);

                    Engine.JsDeleteIndexedProperty(arrayHandle, arrayIndexHandle);

                    var valueHandle = Engine.JsGetIndexedProperty(arrayHandle, arrayIndexHandle);
                    Assert.True(valueHandle != JavaScriptValueSafeHandle.Invalid);

                    var undefinedHandle = Engine.JsGetUndefinedValue();

                    Assert.Equal(undefinedHandle, valueHandle);

                    undefinedHandle.Dispose();
                    valueHandle.Dispose();
                    arrayIndexHandle.Dispose();
                    arrayHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanDetermineIfIndexedPropertiesExternalDataExists()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    //Test on array
                    var arrayHandle = Engine.JsCreateArray(10);

                    var result = Engine.JsHasIndexedPropertiesExternalData(arrayHandle);
                    Assert.False(result);

                    arrayHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanRetrieveIndexedPropertiesExternalData()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    byte[] data = new byte[10];
                    //Populate the array with data.
                    for (byte i = 0; i < 10; i++)
                        data[i] = (byte)(42 + i);

                    //Pin data to unmanaged memory;
                    IntPtr dataPtr = Marshal.AllocHGlobal(sizeof(byte) * data.Length);
                    Marshal.Copy(data, 0, dataPtr, data.Length);

                    try
                    {
                        //Test on object
                        var objectHandle = Engine.JsCreateObject();

                        Engine.JsSetIndexedPropertiesToExternalData(objectHandle, dataPtr, JavaScriptTypedArrayType.Int8, (uint)data.Length);

                        IntPtr externalDataPtr = Engine.JsGetIndexedPropertiesExternalData(objectHandle, out JavaScriptTypedArrayType arrayType, out uint length);

                        Assert.Equal(JavaScriptTypedArrayType.Int8, arrayType);
                        Assert.Equal((uint)data.Length, length);
                        Assert.Equal(externalDataPtr, dataPtr);

                        objectHandle.Dispose();
                    }
                    finally
                    {
                        //Free our pinned memory.
                        Marshal.FreeHGlobal(dataPtr);
                    }
                }
            }
        }

        [Fact]
        public void JsCanSetIndexedPropertiesExternalData()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    byte[] data = new byte[10];
                    //Populate the array with data.
                    for (byte i = 0; i < 10; i++)
                        data[i] = (byte)(42 + i);

                    //Pin data to unmanaged memory;
                    IntPtr dataPtr = Marshal.AllocHGlobal(sizeof(byte) * data.Length);
                    Marshal.Copy(data, 0, dataPtr, data.Length);

                    try
                    {
                        //Test on object
                        var objectHandle = Engine.JsCreateObject();

                        Engine.JsSetIndexedPropertiesToExternalData(objectHandle, dataPtr, JavaScriptTypedArrayType.Int8, (uint)data.Length);
                        var hasIndexedPropertiesExternalData = Engine.JsHasIndexedPropertiesExternalData(objectHandle);
                        Assert.True(hasIndexedPropertiesExternalData);

                        var arrayIndexHandle = Engine.JsIntToNumber(5);
                        var propertyHandle = Engine.JsGetIndexedProperty(objectHandle, arrayIndexHandle);
                        Assert.True(propertyHandle != JavaScriptValueSafeHandle.Invalid);

                        var handleType = Engine.JsGetValueType(propertyHandle);
                        Assert.True(handleType == JavaScriptValueType.Number);

                        var value = Engine.JsNumberToInt(propertyHandle);
                        Assert.Equal(42 + 5, value);

                        arrayIndexHandle.Dispose();
                        propertyHandle.Dispose();
                        objectHandle.Dispose();
                    }
                    finally
                    {
                        //Free our pinned memory.
                        Marshal.FreeHGlobal(dataPtr);
                    }
                }
            }
        }

        [Fact]
        public void JsEqualsIsEquals()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var name = "The Well-Tempered Clavier";
                    var nameHandle = Engine.JsCreateString(name, (ulong)name.Length);
                    var name2Handle = Engine.JsCreateString(name, (ulong)name.Length);
                    var areEqual = Engine.JsEquals(nameHandle, name2Handle);
                    Assert.True(areEqual);

                    nameHandle.Dispose();
                    name2Handle.Dispose();

                    var stringValue = "42";
                    var stringValueHandle = Engine.JsCreateString(stringValue, (ulong)stringValue.Length);
                    var intValueHandle = Engine.JsIntToNumber(42);
                    areEqual = Engine.JsEquals(stringValueHandle, intValueHandle);
                    Assert.True(areEqual);

                    stringValueHandle.Dispose();
                    intValueHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsStrictEqualsIsStrictEquals()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var name = "Trace Decay";
                    var nameHandle = Engine.JsCreateString(name, (ulong)name.Length);
                    var name2Handle = Engine.JsCreateString(name, (ulong)name.Length);
                    var areEqual = Engine.JsStrictEquals(nameHandle, name2Handle);
                    Assert.True(areEqual);

                    nameHandle.Dispose();
                    name2Handle.Dispose();

                    var stringValue = "8";
                    var stringValueHandle = Engine.JsCreateString(stringValue, (ulong)stringValue.Length);
                    var intValueHandle = Engine.JsIntToNumber(8);
                    areEqual = Engine.JsStrictEquals(stringValueHandle, intValueHandle);
                    Assert.False(areEqual);

                    stringValueHandle.Dispose();
                    intValueHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanDetermineIfIsExternalData()
        {
            var myPoint = new Point()
            {
                x = 123,
                y = 456
            };

            var myPointPtr = GetPtr(myPoint);

            bool called = false;
            JavaScriptObjectFinalizeCallback callback = (IntPtr ptr) =>
            {
                called = true;
                Assert.True(myPointPtr == ptr);
                Marshal.FreeHGlobal(myPointPtr);
            };

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    //Test on external object
                    var objectHandle = Engine.JsCreateExternalObject(myPointPtr, callback);

                    var result = Engine.JsHasExternalData(objectHandle);
                    Assert.True(result);

                    objectHandle.Dispose();
                }
            }

            Assert.True(called);
        }

        [Fact]
        public void JsCanRetrieveExternalData()
        {
            var myPoint = new Point()
            {
                x = 123,
                y = 456
            };

            var myPointPtr = GetPtr(myPoint);

            bool called = false;
            JavaScriptObjectFinalizeCallback callback = (IntPtr ptr) =>
            {
                called = true;
                Assert.True(myPointPtr == ptr);
                Marshal.FreeHGlobal(myPointPtr);
            };

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    //Test on external object
                    var objectHandle = Engine.JsCreateExternalObject(myPointPtr, callback);

                    IntPtr externalDataPtr = Engine.JsGetExternalData(objectHandle);
                    Assert.Equal(externalDataPtr, myPointPtr);
                    objectHandle.Dispose();
                }
            }

            Assert.True(called);
        }

        [Fact]
        public void JsCanSetExternalData()
        {
            var myPoint = new Point()
            {
                x = 123,
                y = 456
            };

            var myPoint2 = new Point()
            {
                x = 789,
                y = 123
            };

            var myPointPtr = GetPtr(myPoint);
            var myPointPtr2 = GetPtr(myPoint2);

            int calledCount = 0;
            JavaScriptObjectFinalizeCallback callback = (IntPtr ptr) =>
            {
                calledCount++;
                Marshal.FreeHGlobal(ptr);
            };

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    //Test on external object
                    var objectHandle = Engine.JsCreateExternalObject(myPointPtr, callback);

                    Engine.JsSetExternalData(objectHandle, myPointPtr2);
                    callback(myPointPtr); //Since we set it, I guess we're responsible for clearing it.

                    IntPtr externalDataPtr = Engine.JsGetExternalData(objectHandle);
                    Assert.Equal(externalDataPtr, myPointPtr2);
                    objectHandle.Dispose();
                }
            }

            Assert.Equal(2, calledCount);
        }

        [Fact]
        public void JsArrayCanBeCreated()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var arrayHandle = Engine.JsCreateArray(50);

                    Assert.True(arrayHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(arrayHandle);
                    Assert.True(handleType == JavaScriptValueType.Array);

                    arrayHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsArrayBufferCanBeCreated()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var arrayBufferHandle = Engine.JsCreateArrayBuffer(50);
                    Assert.True(arrayBufferHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(arrayBufferHandle);
                    Assert.True(handleType == JavaScriptValueType.ArrayBuffer);

                    arrayBufferHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsExternalArrayBufferCanBeCreated()
        {
            var data = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    IntPtr ptrScript = Marshal.StringToHGlobalAnsi(data);

                    try
                    {
                        var externalArrayBufferHandle = Engine.JsCreateExternalArrayBuffer(ptrScript, (uint)data.Length, null, IntPtr.Zero);
                        Assert.True(externalArrayBufferHandle != JavaScriptValueSafeHandle.Invalid);

                        var handleType = Engine.JsGetValueType(externalArrayBufferHandle);
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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var typedArrayHandle = Engine.JsCreateTypedArray(JavaScriptTypedArrayType.Int8, JavaScriptValueSafeHandle.Invalid, 0, 50);

                    Assert.True(typedArrayHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(typedArrayHandle);
                    Assert.True(handleType == JavaScriptValueType.TypedArray);

                    typedArrayHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsDataViewCanBeCreated()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var arrayBufferHandle = Engine.JsCreateArrayBuffer(50);
                    var dataViewHandle = Engine.JsCreateDataView(arrayBufferHandle, 0, 50);

                    Assert.True(dataViewHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(dataViewHandle);
                    Assert.True(handleType == JavaScriptValueType.DataView);

                    arrayBufferHandle.Dispose();
                    dataViewHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsTypedArrayInfoCanBeRetrieved()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var typedArrayHandle = Engine.JsCreateTypedArray(JavaScriptTypedArrayType.Int8, JavaScriptValueSafeHandle.Invalid, 0, 50);

                    var typedArrayType = Engine.JsGetTypedArrayInfo(typedArrayHandle, out JavaScriptValueSafeHandle arrayBufferHandle, out uint byteOffset, out uint byteLength);

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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var arrayBufferHandle = Engine.JsCreateArrayBuffer(50);

                    var ptrBuffer = Engine.JsGetArrayBufferStorage(arrayBufferHandle, out uint bufferLength);

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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var typedArrayHandle = Engine.JsCreateTypedArray(JavaScriptTypedArrayType.Int8, JavaScriptValueSafeHandle.Invalid, 0, 50);

                    var ptrBuffer = Engine.JsGetTypedArrayStorage(typedArrayHandle, out uint bufferLength, out JavaScriptTypedArrayType typedArrayType, out int elementSize);

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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var arrayBufferHandle = Engine.JsCreateArrayBuffer(50);
                    var dataViewHandle = Engine.JsCreateDataView(arrayBufferHandle, 0, 50);

                    var ptrBuffer = Engine.JsGetDataViewStorage(dataViewHandle, out uint bufferLength);

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
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    try
                    {
                        var promiseHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);
                        Assert.True(false, "Promises should not be able to be resolved without a promise continuation callback defined.");
                    }
                    catch (JavaScriptScriptException ex)
                    {
                        Assert.Equal("Host may not have set any promise continuation callback. Promises may not be executed.", ex.ErrorMessage);
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
                var task = new JavaScriptValueSafeHandle(taskHandle);
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

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    Engine.JsSetPromiseContinuationCallback(promiseContinuationCallback, IntPtr.Zero);

                    var promiseHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);
                    Assert.True(promiseHandle != JavaScriptValueSafeHandle.Invalid);

                    var undefinedHandle = Engine.JsGetUndefinedValue();
                    var trueHandle = Engine.JsGetTrueValue();
                    var falseHandle = Engine.JsGetFalseValue();

                    var allDonePropertyName = "allDone";
                    var allDonePropertyIdHandle = Engine.JsCreatePropertyId(allDonePropertyName, (ulong)allDonePropertyName.Length);

                    var globalObjectHandle = Engine.JsGetGlobalObject();
                    var allDoneHandle = Engine.JsGetProperty(globalObjectHandle, allDonePropertyIdHandle);
                    Assert.True(allDoneHandle == falseHandle);


                    var args = new IntPtr[] { undefinedHandle.DangerousGetHandle() };

                    do
                    {
                        var task = taskQueue.Pop();
                        try
                        {
                            var handleType = Engine.JsGetValueType(task);
                            Assert.True(handleType == JavaScriptValueType.Function);
                            var result = Engine.JsCallFunction(task, args, (ushort)args.Length);
                            result.Dispose();
                        }
                        finally
                        {
                            task.Dispose();
                        }
                    } while (taskQueue.Count > 0);

                    allDoneHandle.Dispose();
                    allDoneHandle = Engine.JsGetProperty(globalObjectHandle, allDonePropertyIdHandle);
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

        [Fact]
        public void JsCallFunctionCallsAndReturnsAValue()
        {
            string toStringPropertyName = "toString";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var toStringFunctionPropertyIdHandle = Engine.JsCreatePropertyId(toStringPropertyName, (ulong)toStringPropertyName.Length);

                    var objectHandle = Engine.JsCreateObject();
                    var objectToStringFnHandle = Engine.JsGetProperty(objectHandle, toStringFunctionPropertyIdHandle);

                    var resultHandle = Engine.JsCallFunction(objectToStringFnHandle, new IntPtr[] { objectHandle.DangerousGetHandle() }, 1);

                    Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(resultHandle);
                    Assert.True(handleType == JavaScriptValueType.String);

                    var size = Engine.JsCopyString(resultHandle, null, 0);
                    if ((int)size > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    byte[] result = new byte[(int)size];
                    var written = Engine.JsCopyString(resultHandle, result, size);
                    string resultStr = Encoding.UTF8.GetString(result, 0, result.Length);

                    Assert.True(resultStr == "[object Object]");

                    resultHandle.Dispose();
                    objectToStringFnHandle.Dispose();
                    objectHandle.Dispose();
                    toStringFunctionPropertyIdHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanConstructAnObjectFromAConstructorFunction()
        {
            var script = @"(() => {
var fn = function Tree(name) {
  this.name = name;
};
return fn;
})()";

            var name = "Redwood";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var fnHandle = Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);
                    var nameHandle = Engine.JsCreateString(name, (ulong)name.Length);

                    var handleType = Engine.JsGetValueType(fnHandle);
                    Assert.True(handleType == JavaScriptValueType.Function);

                    var objectHandle = Engine.JsConstructObject(fnHandle, new IntPtr[] { fnHandle.DangerousGetHandle(), nameHandle.DangerousGetHandle() }, 2);
                    
                    Assert.True(objectHandle != JavaScriptValueSafeHandle.Invalid);

                    handleType = Engine.JsGetValueType(objectHandle);
                    Assert.True(handleType == JavaScriptValueType.Object);

                    var namePropertyName = "name";
                    var namePropertyIdHandle = Engine.JsCreatePropertyId(namePropertyName, (ulong)namePropertyName.Length);
                    var namePropertyHandle = Engine.JsGetProperty(objectHandle, namePropertyIdHandle);
                    Assert.True(namePropertyHandle != JavaScriptValueSafeHandle.Invalid);

                    var propertyHandleType = Engine.JsGetValueType(namePropertyHandle);
                    Assert.True(propertyHandleType == JavaScriptValueType.String);

                    var resultStr = Extensions.IJavaScriptEngineExtensions.GetStringUtf8(Engine, namePropertyHandle);
                    Assert.True(resultStr == name);

                    namePropertyHandle.Dispose();
                    objectHandle.Dispose();
                    namePropertyHandle.Dispose();
                    namePropertyIdHandle.Dispose();
                    nameHandle.Dispose();
                    fnHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsToStringOnAFunctionOutputsTheCode()
        {
            string myCode = "'12345678'";
            string toStringPropertyName = "toString";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var toStringFunctionPropertyIdHandle = Engine.JsCreatePropertyId(toStringPropertyName, (ulong)toStringPropertyName.Length);

                    IntPtr ptrScript = Marshal.StringToHGlobalAnsi(myCode);
                    try
                    {
                        var scriptHandle = Engine.JsCreateExternalArrayBuffer(ptrScript, (uint)myCode.Length, null, IntPtr.Zero);
                        var sourceUrlHandle = Engine.JsCreateString(sourceUrl, (ulong)sourceUrl.Length);
                        var fnHandle = Engine.JsParse(scriptHandle, JavaScriptSourceContext.GetNextSourceContext(), sourceUrlHandle, JavaScriptParseScriptAttributes.None);
                        var fnToStringFnHandle = Engine.JsGetProperty(fnHandle, toStringFunctionPropertyIdHandle);
                        var resultHandle = Engine.JsCallFunction(fnToStringFnHandle, new IntPtr[] { fnHandle.DangerousGetHandle() }, 1);

                        Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                        var handleType = Engine.JsGetValueType(resultHandle);
                        Assert.True(handleType == JavaScriptValueType.String);

                        var size = Engine.JsCopyString(resultHandle, null, 0);
                        if ((int)size > int.MaxValue)
                            throw new OutOfMemoryException("Exceeded maximum string length.");

                        byte[] result = new byte[(int)size];
                        var written = Engine.JsCopyString(resultHandle, result, size);
                        string resultStr = Encoding.UTF8.GetString(result, 0, result.Length);

                        Assert.Equal(myCode, resultStr);

                        resultHandle.Dispose();
                        fnToStringFnHandle.Dispose();
                        fnHandle.Dispose();
                        scriptHandle.Dispose();
                        sourceUrlHandle.Dispose();
                        toStringFunctionPropertyIdHandle.Dispose();
                    }
                    finally
                    {
                        Marshal.ZeroFreeGlobalAllocAnsi(ptrScript);
                    }
                }
            }
        }

        [Fact]
        public void JsCanCreateAFunctionFromANativeFunction()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var called = false;
                    JavaScriptNativeFunction fn = (callee, isConstructCall, arguments, argumentCount, callbackData) => {
                        called = true;
                        return IntPtr.Zero;
                    };

                    var fnHandle = Engine.JsCreateFunction(fn, IntPtr.Zero);
                    Assert.True(fnHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(fnHandle);
                    Assert.True(handleType == JavaScriptValueType.Function);

                    var resultHandle = Engine.JsCallFunction(fnHandle, new IntPtr[] { fnHandle.DangerousGetHandle() }, 1);

                    handleType = Engine.JsGetValueType(resultHandle);
                    Assert.True(handleType == JavaScriptValueType.Undefined);
                    Assert.True(called);

                    resultHandle.Dispose();
                    fnHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsNativeFunctionsCanReturnValues()
        {
            string foo = "Contrapasso";
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);
                    var called = false;
                    JavaScriptNativeFunction fn = (callee, isConstructCall, arguments, argumentCount, callbackData) => {
                        called = true;
                        
                        var fooHandle = Engine.JsCreateString(foo, (ulong)foo.Length);
                        return fooHandle.DangerousGetHandle();
                    };

                    var fnHandle = Engine.JsCreateFunction(fn, IntPtr.Zero);
                    Assert.True(fnHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(fnHandle);
                    Assert.True(handleType == JavaScriptValueType.Function);

                    var resultHandle = Engine.JsCallFunction(fnHandle, new IntPtr[] { fnHandle.DangerousGetHandle() }, 1);

                    handleType = Engine.JsGetValueType(resultHandle);
                    Assert.True(handleType == JavaScriptValueType.String);
                    Assert.True(called);

                    var result = Extensions.IJavaScriptEngineExtensions.GetStringUtf8(Engine, resultHandle);
                    Assert.Equal(foo, result);

                    resultHandle.Dispose();
                    resultHandle.Dispose();
                    fnHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanCreateANamedFunctionFromANativeFunction()
        {
            string name = "hogwarts";
            string toStringPropertyName = "toString";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var called = false;
                    JavaScriptNativeFunction fn = (callee, isConstructCall, arguments, argumentCount, callbackData) => {
                        called = true;
                        return IntPtr.Zero;
                    };

                    var nameHandle = Engine.JsCreateString(name, (ulong)name.Length);

                    var fnHandle = Engine.JsCreateNamedFunction(nameHandle, fn, IntPtr.Zero);
                    Assert.True(fnHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(fnHandle);
                    Assert.True(handleType == JavaScriptValueType.Function);

                    var resultHandle = Engine.JsCallFunction(fnHandle, new IntPtr[] { fnHandle.DangerousGetHandle() }, 1);

                    handleType = Engine.JsGetValueType(resultHandle);
                    Assert.True(handleType == JavaScriptValueType.Undefined);
                    Assert.True(called);

                    var toStringFunctionPropertyIdHandle = Engine.JsCreatePropertyId(toStringPropertyName, (ulong)toStringPropertyName.Length);
                    var fnToStringFnHandle = Engine.JsGetProperty(fnHandle, toStringFunctionPropertyIdHandle);
                    var fnToStringResultHandle = Engine.JsCallFunction(fnToStringFnHandle, new IntPtr[] { fnHandle.DangerousGetHandle() }, 1);
                    var result = Extensions.IJavaScriptEngineExtensions.GetStringUtf8(Engine, fnToStringResultHandle);
                    Assert.Equal("function hogwarts() { [native code] }", result);

                    toStringFunctionPropertyIdHandle.Dispose();
                    fnToStringFnHandle.Dispose();
                    fnToStringResultHandle.Dispose();

                    resultHandle.Dispose();
                    fnHandle.Dispose();
                    nameHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanCreateErrors()
        {
            var message = "Dormammu, I've come to bargain.";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var messageHandle = Engine.JsCreateString(message, (ulong)message.Length);

                    var errorHandle = Engine.JsCreateError(messageHandle);
                    Assert.True(errorHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(errorHandle);
                    Assert.True(handleType == JavaScriptValueType.Error);

                    errorHandle.Dispose();
                    messageHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanCreateRangeErrors()
        {
            var message = "William of Occam was a 13th century monk. He can't help us now, Bernard. He would have us burned at the stake.";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var messageHandle = Engine.JsCreateString(message, (ulong)message.Length);

                    var errorHandle = Engine.JsCreateRangeError(messageHandle);
                    Assert.True(errorHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(errorHandle);
                    Assert.True(handleType == JavaScriptValueType.Error);

                    errorHandle.Dispose();
                    messageHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanCreateReferenceErrors()
        {
            var message = "Dormammu, I've come to bargain.";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var messageHandle = Engine.JsCreateString(message, (ulong)message.Length);

                    var errorHandle = Engine.JsCreateReferenceError(messageHandle);
                    Assert.True(errorHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(errorHandle);
                    Assert.True(handleType == JavaScriptValueType.Error);

                    errorHandle.Dispose();
                    messageHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanCreateSyntaxErrors()
        {
            var message = "Dormammu, I've come to bargain.";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var messageHandle = Engine.JsCreateString(message, (ulong)message.Length);

                    var errorHandle = Engine.JsCreateSyntaxError(messageHandle);
                    Assert.True(errorHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(errorHandle);
                    Assert.True(handleType == JavaScriptValueType.Error);

                    errorHandle.Dispose();
                    messageHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanCreateTypeErrors()
        {
            var message = "Dormammu, I've come to bargain.";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var messageHandle = Engine.JsCreateString(message, (ulong)message.Length);

                    var errorHandle = Engine.JsCreateTypeError(messageHandle);
                    Assert.True(errorHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(errorHandle);
                    Assert.True(handleType == JavaScriptValueType.Error);

                    errorHandle.Dispose();
                    messageHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanCreateUriErrors()
        {
            var message = "Dormammu, I've come to bargain.";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var messageHandle = Engine.JsCreateString(message, (ulong)message.Length);

                    var errorHandle = Engine.JsCreateURIError(messageHandle);
                    Assert.True(errorHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(errorHandle);
                    Assert.True(handleType == JavaScriptValueType.Error);

                    errorHandle.Dispose();
                    messageHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanDetermineIfHasException()
        {
            var message = "Dormammu, I've come to bargain.";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var hasException = Engine.JsHasException();
                    Assert.False(hasException);

                    var messageHandle = Engine.JsCreateString(message, (ulong)message.Length);
                    var errorHandle = Engine.JsCreateError(messageHandle);

                    Engine.JsSetException(errorHandle);

                    hasException = Engine.JsHasException();
                    Assert.True(hasException);

                    errorHandle.Dispose();
                    messageHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanGetAndClearException()
        {
            var message = "Dormammu, I've come to bargain.";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var messageHandle = Engine.JsCreateString(message, (ulong)message.Length);
                    var errorHandle = Engine.JsCreateError(messageHandle);

                    Engine.JsSetException(errorHandle);

                    var hasException = Engine.JsHasException();
                    Assert.True(hasException);

                    var exceptionHandle = Engine.JsGetAndClearException();
                    Assert.True(exceptionHandle != JavaScriptValueSafeHandle.Invalid);

                    hasException = Engine.JsHasException();
                    Assert.False(hasException);

                    errorHandle.Dispose();
                    messageHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanGetAndClearExceptionWithMetadata()
        {
            var script = @"
let foo = 'bar';
function throwAtHost() {
  //Throw an exception.
  throw new Error('throwing');
};
";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    Extensions.IJavaScriptEngineExtensions.JsRunScript(Engine, script);
                    var fnHandle = Extensions.IJavaScriptEngineExtensions.GetGlobalVariable(Engine, "throwAtHost");
                    Assert.True(fnHandle != JavaScriptValueSafeHandle.Invalid);

                    try
                    {
                        Engine.JsCallFunction(fnHandle, new IntPtr[] { fnHandle.DangerousGetHandle() }, 1);
                    }
                    catch(JavaScriptException ex)
                    {
                        Assert.True(ex.ErrorCode == JavaScriptErrorCode.ScriptException);
                    }

                    var hasException = Engine.JsHasException();
                    Assert.True(hasException);
                    
                    var exceptionMetadataHandle = Engine.JsGetAndClearExceptionWithMetadata();
                    Assert.True(exceptionMetadataHandle != JavaScriptValueSafeHandle.Invalid);

                    hasException = Engine.JsHasException();
                    Assert.False(hasException);

                    fnHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanSetException()
        {
            var message = "Dormammu, I've come to bargain.";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var messageHandle = Engine.JsCreateString(message, (ulong)message.Length);
                    var errorHandle = Engine.JsCreateError(messageHandle);

                    var hasException = Engine.JsHasException();
                    Assert.False(hasException);

                    Engine.JsSetException(errorHandle);

                    hasException = Engine.JsHasException();
                    Assert.True(hasException);

                    errorHandle.Dispose();
                    messageHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanDisableRuntimeExecution()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.AllowScriptInterrupt, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    Engine.JsDisableRuntimeExecution(runtimeHandle);
                }
            }
        }

        [Fact]
        public void JsCanEnableRuntimeExecution()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.AllowScriptInterrupt, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    bool isRuntimeExecutionDisabled = Engine.JsIsRuntimeExecutionDisabled(runtimeHandle);
                    Assert.False(isRuntimeExecutionDisabled);

                    Engine.JsDisableRuntimeExecution(runtimeHandle);

                    isRuntimeExecutionDisabled = Engine.JsIsRuntimeExecutionDisabled(runtimeHandle);
                    Assert.True(isRuntimeExecutionDisabled);

                    Engine.JsEnableRuntimeExecution(runtimeHandle);

                    isRuntimeExecutionDisabled = Engine.JsIsRuntimeExecutionDisabled(runtimeHandle);
                    Assert.False(isRuntimeExecutionDisabled);
                }
            }
        }

        [Fact]
        public void JsCanRetrieveRuntimeDisabledState()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.AllowScriptInterrupt, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    bool isRuntimeExecutionDisabled = Engine.JsIsRuntimeExecutionDisabled(runtimeHandle);
                    Assert.False(isRuntimeExecutionDisabled);

                    Engine.JsDisableRuntimeExecution(runtimeHandle);

                    isRuntimeExecutionDisabled = Engine.JsIsRuntimeExecutionDisabled(runtimeHandle);
                    Assert.True(isRuntimeExecutionDisabled);
                }
            }
        }
    }
}
