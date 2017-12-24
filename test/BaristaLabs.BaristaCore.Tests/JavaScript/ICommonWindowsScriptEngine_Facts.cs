namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Text;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class ICommonWindowsScriptEngine_Facts
    {
        private IJavaScriptEngine Engine;

        public ICommonWindowsScriptEngine_Facts()
        {
            var chakraCoreFactory = new ChakraCoreFactory();
            Engine = chakraCoreFactory.CreateJavaScriptEngine();
        }

        public ICommonWindowsJavaScriptEngine CommonWindowsEngine
        {
            get
            {
                return Engine as ICommonWindowsJavaScriptEngine;
            }
        }

        [Fact]
        public void JsParseScriptParsesAndReturnsAFunction()
        {
            if (CommonWindowsEngine == null)
                return;

            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var resultHandle = CommonWindowsEngine.JsParseScript(script, JavaScriptSourceContext.GetNextSourceContext(), sourceUrl);

                    Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(resultHandle);
                    Assert.True(handleType == JsValueType.Function);

                    resultHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsParseScriptWithAttributeParsesAndReturnsAFunction()
        {
            if (CommonWindowsEngine == null)
                return;

            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var resultHandle = CommonWindowsEngine.JsParseScriptWithAttributes(script, JavaScriptSourceContext.GetNextSourceContext(), sourceUrl, JavaScriptParseScriptAttributes.None);

                    Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(resultHandle);
                    Assert.True(handleType == JsValueType.Function);

                    resultHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsRunScriptExecutesAndReturnsAValue()
        {
            if (CommonWindowsEngine == null)
                return;

            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var resultHandle = CommonWindowsEngine.JsRunScript(script, JavaScriptSourceContext.GetNextSourceContext(), sourceUrl);

                    Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(resultHandle);
                    Assert.True(handleType == JsValueType.Number);

                    Assert.Equal(42, Engine.JsNumberToInt(resultHandle));

                    resultHandle.Dispose();
                }
            }
        }

        //Skip JsExperimentalApiRunModule

        [Fact]
        public void JsScriptCanBeSerialized()
        {
            if (CommonWindowsEngine == null)
                return;

            var script = "(()=>{return 6*7;})()";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    byte[] buffer = new byte[1024];
                    uint bufferSize = (uint)buffer.Length;

                    CommonWindowsEngine.JsSerializeScript(script, buffer, ref bufferSize);

                    Assert.True(bufferSize != (ulong)buffer.Length);
                    Assert.True(buffer[0] != 0);
                }
            }
        }

        [Fact]
        public void JsSerializedScriptCanBeParsedWithCallbacks()
        {
            if (CommonWindowsEngine == null)
                return;

            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            bool loaded = false;
            bool unloaded = false;

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    byte[] buffer = new byte[1024];
                    uint bufferSize = (uint)buffer.Length;

                    CommonWindowsEngine.JsSerializeScript(script, buffer, ref bufferSize);

                    JavaScriptSerializedScriptLoadSourceCallback loadCallback = (JavaScriptSourceContext sourceContext, out StringBuilder scriptBuffer) =>
                    {
                        loaded = true;
                        scriptBuffer = new StringBuilder(script);
                        return true;
                    };

                    JavaScriptSerializedScriptUnloadCallback unloadCallback = (JavaScriptSourceContext sourceContext) =>
                    {
                        unloaded = true;
                    };

                    var fnHandle = CommonWindowsEngine.JsParseSerializedScriptWithCallback(loadCallback, unloadCallback, buffer, JavaScriptSourceContext.GetNextSourceContext(), sourceUrl);
                    Assert.NotEqual(JavaScriptValueSafeHandle.Invalid, fnHandle);

                    var handleType = Engine.JsGetValueType(fnHandle);
                    Assert.True(handleType == JsValueType.Function);

                    //Get the string representation of the function. This triggers the load callback.
                    var fnStringHandle = Engine.JsConvertValueToString(fnHandle);
                    var stringPtr = CommonWindowsEngine.JsStringToPointer(fnStringHandle, out ulong length);
                    Assert.True(stringPtr != IntPtr.Zero);
                    Assert.True(length > 0);

                    fnStringHandle.Dispose();
                    fnHandle.Dispose();
                }
                Engine.JsCollectGarbage(runtimeHandle);
            }

            Assert.True(loaded);
            Assert.True(unloaded);
        }

        [Fact]
        public void JsSerializedScriptCanBeRunWithCallbacks()
        {
            if (CommonWindowsEngine == null)
                return;

            var script = @"
    var moose = function() {
        //Trigger script loading
        var str = arguments.callee.toString();
        return 6*7;
    };
    moose();
";
            string sourceUrl = "[eval code]";

            bool loaded = false;
            bool unloaded = false;

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    byte[] buffer = new byte[1024];
                    uint bufferSize = (uint)buffer.Length;

                    CommonWindowsEngine.JsSerializeScript(script, buffer, ref bufferSize);

                    JavaScriptSerializedScriptLoadSourceCallback loadCallback = (JavaScriptSourceContext sourceContext, out StringBuilder scriptBuffer) =>
                    {
                        loaded = true;
                        scriptBuffer = new StringBuilder(script);
                        return true;
                    };

                    JavaScriptSerializedScriptUnloadCallback unloadCallback = (JavaScriptSourceContext sourceContext) =>
                    {
                        unloaded = true;
                    };

                    var resultHandle = CommonWindowsEngine.JsRunSerializedScriptWithCallback(loadCallback, unloadCallback, buffer, JavaScriptSourceContext.GetNextSourceContext(), sourceUrl);
                    Assert.NotEqual(JavaScriptValueSafeHandle.Invalid, resultHandle);

                    var handleType = Engine.JsGetValueType(resultHandle);
                    Assert.True(handleType == JsValueType.Number);

                    Assert.Equal(42, Engine.JsNumberToInt(resultHandle));

                    resultHandle.Dispose();
                }
                Engine.JsCollectGarbage(runtimeHandle);
            }

            Assert.True(loaded);
            Assert.True(unloaded);
        }

        [Fact]
        public void JsSerializedScriptCanBeParsed()
        {
            if (CommonWindowsEngine == null)
                return;

            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    byte[] buffer = new byte[1024];
                    uint bufferSize = (uint)buffer.Length;

                    CommonWindowsEngine.JsSerializeScript(script, buffer, ref bufferSize);
                    
                    var fnHandle = CommonWindowsEngine.JsParseSerializedScript(script, buffer, JavaScriptSourceContext.GetNextSourceContext(), sourceUrl);
                    Assert.NotEqual(JavaScriptValueSafeHandle.Invalid, fnHandle);

                    var handleType = Engine.JsGetValueType(fnHandle);
                    Assert.True(handleType == JsValueType.Function);

                    fnHandle.Dispose();
                }
                Engine.JsCollectGarbage(runtimeHandle);
            }
        }

        [Fact]
        public void JsSerializedScriptCanBeRun()
        {
            if (CommonWindowsEngine == null)
                return;

            var script = @"
    var moose = function() {
        //Trigger script loading
        var str = arguments.callee.toString();
        return 6*7;
    };
    moose();
";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    byte[] buffer = new byte[1024];
                    uint bufferSize = (uint)buffer.Length;

                    CommonWindowsEngine.JsSerializeScript(script, buffer, ref bufferSize);

                    var resultHandle = CommonWindowsEngine.JsRunSerializedScript(script, buffer, JavaScriptSourceContext.GetNextSourceContext(), sourceUrl);
                    Assert.NotEqual(JavaScriptValueSafeHandle.Invalid, resultHandle);

                    var handleType = Engine.JsGetValueType(resultHandle);
                    Assert.True(handleType == JsValueType.Number);

                    Assert.Equal(42, Engine.JsNumberToInt(resultHandle));
                    resultHandle.Dispose();
                }
                Engine.JsCollectGarbage(runtimeHandle);
            }
        }

        [Fact]
        public void JsCanRetrievePropertyIdByName()
        {
            if (CommonWindowsEngine == null)
                return;

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
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var objHandle = CommonWindowsEngine.JsRunScript(script, JavaScriptSourceContext.GetNextSourceContext(), sourceUrl);
                    var propertyIdHandle = CommonWindowsEngine.JsGetPropertyIdFromName("waldo");
                    Assert.NotEqual(JavaScriptPropertyIdSafeHandle.Invalid, propertyIdHandle);

                    //Try to get a property value with the handle.
                    var resultHandle = Engine.JsGetProperty(objHandle, propertyIdHandle);
                    Assert.NotEqual(JavaScriptValueSafeHandle.Invalid, resultHandle);

                    var stringPtr = CommonWindowsEngine.JsStringToPointer(resultHandle, out ulong length);
                    Assert.True(stringPtr != IntPtr.Zero);
                    Assert.True(length > 0);
                    var str = Marshal.PtrToStringUni(stringPtr, (int)length);
                    Assert.Equal("fred", str);

                    resultHandle.Dispose();
                    propertyIdHandle.Dispose();
                    objHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanGetPropertyIdFromName()
        {
            if (CommonWindowsEngine == null)
                return;

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);
                    var propertyIdHandle = CommonWindowsEngine.JsGetPropertyIdFromName("lorax");
                    Assert.NotEqual(JavaScriptPropertyIdSafeHandle.Invalid, propertyIdHandle);

                    propertyIdHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanGetNameFromPropertyId()
        {
            if (CommonWindowsEngine == null)
                return;

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);
                    var propertyName = "onefishtwofishredfishbluefish";
                    var propertyIdHandle = CommonWindowsEngine.JsGetPropertyIdFromName(propertyName);

                    var ptr = CommonWindowsEngine.JsGetPropertyNameFromId(propertyIdHandle);
                    var name = Marshal.PtrToStringUni(ptr);
                    Assert.Equal(name, propertyName);

                    propertyIdHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanGetPointerFromString()
        {
            if (CommonWindowsEngine == null)
                return;

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var str = "Hi there, world!";
                    var stringHandle = CommonWindowsEngine.JsPointerToString(str, (ulong)str.Length);
                    Assert.NotEqual(stringHandle, JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(stringHandle);
                    Assert.True(handleType == JsValueType.String);
                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCanGetPointerToString()
        {
            if (CommonWindowsEngine == null)
                return;

            var script = "(()=>{return 'bpafree';})()";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var resultHandle = CommonWindowsEngine.JsRunScript(script, JavaScriptSourceContext.GetNextSourceContext(), sourceUrl);
                    var stringPtr = CommonWindowsEngine.JsStringToPointer(resultHandle, out ulong length);
                    Assert.True(stringPtr != IntPtr.Zero);
                    Assert.True(length > 0);
                    var str = Marshal.PtrToStringUni(stringPtr, (int)length);
                    Assert.Equal("bpafree", str);
                }
            }
        }
    }
}
