namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Xunit;

    public class ICoreJavaScriptEngine_Facts
    {
        private IJavaScriptEngine Engine;

        public ICoreJavaScriptEngine_Facts()
        {
            Engine = JavaScriptEngineFactory.CreateChakraEngine();
        }

        [Fact]
        public void JsCreateStringTest()
        {
            var str = "Hello, World!";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var stringHandle = Engine.JsCreateString(str, (ulong)str.Length);

                    Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(stringHandle);
                    Assert.True(handleType == JavaScriptValueType.String);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCreateStringUtf16Test()
        {
            var str = "Hello, World!";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var stringHandle = Engine.JsCreateStringUtf16(str, (ulong)str.Length);

                    Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(stringHandle);
                    Assert.True(handleType == JavaScriptValueType.String);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCopyStringTest()
        {
            var str = "Hello, World!";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var stringHandle = Engine.JsCreateString(str, (ulong)str.Length);

                    //Get the size
                    var size = Engine.JsCopyString(stringHandle, null, 0);
                    if ((int)size > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    byte[] result = new byte[(int)size];
                    var written = Engine.JsCopyString(stringHandle, result, (ulong)result.Length);
                    string resultStr = Encoding.UTF8.GetString(result, 0, result.Length);

                    Assert.True(str == resultStr);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsCopyStringUtf16Test()
        {
            var str = "Hello, World!";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var stringHandle = Engine.JsCreateStringUtf16(str, (ulong)str.Length);

                    //Get the size
                    var size = Engine.JsCopyStringUtf16(stringHandle, 0, -1, null);
                    if ((int)size * 2 > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    byte[] result = new byte[(int)size * 2];
                    var written = Engine.JsCopyStringUtf16(stringHandle, 0, -1, result);
                    string resultStr = Encoding.Unicode.GetString(result, 0, result.Length);

                    Assert.True(str == resultStr);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsParseDoesParse()
        {
            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
                    try
                    {
                        var scriptHandle = Engine.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero);
                        var sourceContext = JavaScriptSourceContext.None;
                        var sourceUrlHandle = Engine.JsCreateString(sourceUrl, (ulong)sourceUrl.Length);
                        var resultHandle = Engine.JsParse(scriptHandle, sourceContext, sourceUrlHandle, JavaScriptParseScriptAttributes.None);

                        Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                        var handleType = Engine.JsGetValueType(resultHandle);
                        Assert.True(handleType == JavaScriptValueType.Function);

                        resultHandle.Dispose();
                        sourceUrlHandle.Dispose();
                        scriptHandle.Dispose();
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptrScript);
                    }
                }
            }
        }

        [Fact]
        public void JsRunDoesRun()
        {
            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
                    try
                    {
                        var scriptHandle = Engine.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero);

                        JavaScriptSourceContext sourceContext = JavaScriptSourceContext.None;

                        var sourceUrlHandle = Engine.JsCreateString(sourceUrl, (ulong)sourceUrl.Length);

                        var resultHandle = Engine.JsRun(scriptHandle, sourceContext, sourceUrlHandle, JavaScriptParseScriptAttributes.None);
                        Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                        var handleType = Engine.JsGetValueType(resultHandle);
                        Assert.True(handleType == JavaScriptValueType.Number);

                        var resultValue = Engine.JsNumberToInt(resultHandle);
                        Assert.True(resultValue == 42);

                        resultHandle.Dispose();
                        sourceUrlHandle.Dispose();
                        scriptHandle.Dispose();
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptrScript);
                    }
                }
            }
        }

        [Fact]
        public void JsPropertyIdCanBeCreated()
        {
            var str = "foo";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var propertyHandle = Engine.JsCreatePropertyId(str, (ulong)str.Length);

                    Assert.True(propertyHandle != JavaScriptPropertyIdSafeHandle.Invalid);

                    propertyHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsPropertyIdCanBeCopied()
        {
            var str = "foo";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var propertyHandle = Engine.JsCreatePropertyId(str, (ulong)str.Length);

                    //Get the size
                    var size = Engine.JsCopyPropertyId(propertyHandle, null, 0);
                    if ((int)size > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    byte[] result = new byte[(int)size];
                    var written = Engine.JsCopyPropertyId(propertyHandle, result, (ulong)result.Length);
                    string resultStr = Encoding.UTF8.GetString(result, 0, result.Length);

                    Assert.True(str == resultStr);

                    propertyHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsScriptCanBeSerialized()
        {
            var script = "(()=>{return 6*7;})()";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
                    try
                    {
                        var scriptHandle = Engine.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero);
                        var bufferHandle = Engine.JsSerialize(scriptHandle, JavaScriptParseScriptAttributes.None);

                        var handleType = Engine.JsGetValueType(bufferHandle);
                        Assert.True(handleType == JavaScriptValueType.ArrayBuffer);

                        scriptHandle.Dispose();
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptrScript);
                    }
                }
            }
        }

        [Fact]
        public void JsSerializedScriptCanBeParsed()
        {
            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptSerializedLoadScriptCallback callback = (JavaScriptSourceContext sourceContext, out JavaScriptValueSafeHandle value, out JavaScriptParseScriptAttributes parseAttributes) =>
                    {
                        value = null;
                        parseAttributes = JavaScriptParseScriptAttributes.None;
                        return true;
                    };

                    IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
                    try
                    {
                        var scriptHandle = Engine.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero);
                        var bufferHandle = Engine.JsSerialize(scriptHandle, JavaScriptParseScriptAttributes.None);

                        var sourceUrlHandle = Engine.JsCreateString(sourceUrl, (ulong)sourceUrl.Length);
                        var resultHandle = Engine.JsParseSerialized(bufferHandle, callback, JavaScriptSourceContext.None, sourceUrlHandle);

                        Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                        var handleType = Engine.JsGetValueType(resultHandle);
                        Assert.True(handleType == JavaScriptValueType.Function);

                        resultHandle.Dispose();
                        sourceUrlHandle.Dispose();
                        scriptHandle.Dispose();
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptrScript);
                    }
                }
            }
        }

        [Fact]
        public void JsSerializedScriptCanBeRun()
        {
            var script = "(()=>{return 6*7;})()";
            string sourceUrl = "[eval code]";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    JavaScriptSerializedLoadScriptCallback callback = (JavaScriptSourceContext sourceContext, out JavaScriptValueSafeHandle value, out JavaScriptParseScriptAttributes parseAttributes) =>
                    {
                        value = null;
                        parseAttributes = JavaScriptParseScriptAttributes.None;
                        return true;
                    };

                    IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
                    try
                    {
                        var scriptHandle = Engine.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero);
                        var bufferHandle = Engine.JsSerialize(scriptHandle, JavaScriptParseScriptAttributes.None);

                        var sourceUrlHandle = Engine.JsCreateString(sourceUrl, (ulong)sourceUrl.Length);
                        var resultHandle = Engine.JsRunSerialized(bufferHandle, callback, JavaScriptSourceContext.None, sourceUrlHandle);

                        Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                        var handleType = Engine.JsGetValueType(resultHandle);
                        Assert.True(handleType == JavaScriptValueType.Number);

                        var resultValue = Engine.JsNumberToInt(resultHandle);

                        Assert.True(resultValue == 42);

                        sourceUrlHandle.Dispose();
                        resultHandle.Dispose();
                        scriptHandle.Dispose();
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptrScript);
                    }
                }
            }
        }

        [Fact]
        public void JsModuleCanBeImportedAndExecuted()
        {
            var mainModuleName = "";
            var mainModuleSource = @"
import x from 'foo.js'
return x();
";
            var fooModuleSource = @"
export default function() { return ""Hello, World.""; }
";
            var mainModuleReady = false;
            IntPtr childModuleHandle = IntPtr.Zero;

            JavaScriptFetchImportedModuleCallback fetchCallback = (IntPtr referencingModule, IntPtr specifier, out IntPtr dependentModuleRecord) =>
            {
                var moduleName = Extensions.IJavaScriptEngineExtensions.GetStringUtf8(Engine, new JavaScriptValueSafeHandle(specifier));
                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    dependentModuleRecord = referencingModule;
                    return false;
                }

                Assert.True(moduleName == "foo.js");
                var moduleRecord = Engine.JsInitializeModuleRecord(referencingModule, new JavaScriptValueSafeHandle((IntPtr)specifier));
                dependentModuleRecord = moduleRecord;
                childModuleHandle = moduleRecord;
                return false;
            };

            JavaScriptNotifyModuleReadyCallback notifyCallback = (IntPtr referencingModule, IntPtr exceptionVar) =>
            {
                mainModuleReady = true;
                return false;
            };

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.EnableExperimentalFeatures, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    //Initialize the "main" module (Empty-string specifier.
                    var moduleNameHandle = Engine.JsCreateString(mainModuleName, (ulong)mainModuleName.Length);
                    var mainModuleHandle = Engine.JsInitializeModuleRecord(IntPtr.Zero, moduleNameHandle);

                    IntPtr fetchCallbackPtr = Marshal.GetFunctionPointerForDelegate(fetchCallback);
                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback, fetchCallbackPtr);

                    //Ensure the callback was set properly.
                    var moduleHostPtr = Engine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback);
                    Assert.Equal(fetchCallbackPtr, moduleHostPtr);

                    IntPtr notifyCallbackPtr = Marshal.GetFunctionPointerForDelegate(notifyCallback);
                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback, notifyCallbackPtr);

                    //Ensure the callback was set properly.
                    moduleHostPtr = Engine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback);
                    Assert.Equal(notifyCallbackPtr, moduleHostPtr);

                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.HostDefined, moduleNameHandle.DangerousGetHandle());

                    //Ensure the callback was set properly.
                    moduleHostPtr = Engine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.HostDefined);
                    Assert.Equal(moduleNameHandle.DangerousGetHandle(), moduleHostPtr);

                    // ParseModuleSource is sync, while additional fetch & evaluation are async.
                    var scriptBuffer = Encoding.UTF8.GetBytes(mainModuleSource);
                    var errorHandle = Engine.JsParseModuleSource(mainModuleHandle, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)mainModuleSource.Length, JavaScriptParseModuleSourceFlags.JsParseModuleSourceFlags_DataIsUTF8);
                    Assert.True(errorHandle == JavaScriptValueSafeHandle.Invalid);
                    Assert.True(childModuleHandle != IntPtr.Zero);
                    Assert.False(mainModuleReady);

                    //Parse the foo now.
                    scriptBuffer = Encoding.UTF8.GetBytes(fooModuleSource);
                    errorHandle = Engine.JsParseModuleSource(childModuleHandle, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)fooModuleSource.Length, JavaScriptParseModuleSourceFlags.JsParseModuleSourceFlags_DataIsUTF8);
                    Assert.True(errorHandle == JavaScriptValueSafeHandle.Invalid);

                    Assert.True(mainModuleReady);

                    //Now we're ready, evaluate the main module.
                    var resultHandle = Engine.JsModuleEvaluation(mainModuleHandle);
                    Assert.True(resultHandle != JavaScriptValueSafeHandle.Invalid);

                    var handleType = Engine.JsGetValueType(resultHandle);
                    Assert.True(handleType == JavaScriptValueType.String);

                    var result = Extensions.IJavaScriptEngineExtensions.GetStringUtf8(Engine, resultHandle);
                    Assert.Equal("Hello, World.", result);
                }
            }
        }
    }
}
