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

        //TODO: Need additional libChakraCore that marshals as a byte array, or UTF8 directly (which is coming in .net core https://github.com/dotnet/coreclr/issues/1012)
        //[Fact]
        //public void JsCopyStringHandlesUtf8Strings()
        //{
        //    var str = "いろはにほへとちりぬるをわかよたれそつねならむうゐのおくやまけふこえてあさきゆめみしゑひもせす";

        //    using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
        //    {
        //        using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
        //        {
        //            Engine.JsSetCurrentContext(contextHandle);

        //            var stringHandle = Engine.JsCreateString(str, (ulong)str.Length);

        //            //Get the size
        //            var size = Engine.JsCopyString(stringHandle, null, 0);
        //            if ((int)size > int.MaxValue)
        //                throw new OutOfMemoryException("Exceeded maximum string length.");

        //            byte[] result = new byte[(int)size];
        //            var written = Engine.JsCopyString(stringHandle, result, (ulong)result.Length);
        //            string resultStr = Encoding.UTF8.GetString(result, 0, result.Length);

        //            Assert.True(str == resultStr);

        //            stringHandle.Dispose();
        //        }
        //    }
        //}

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
        public void JsCopyStringUtf16HandlesUtf16Strings()
        {
            var str = "ㄅㄆㄇㄈㄉㄊㄋㄌㄍㄎㄏㄐㄑㄒㄓㄔㄕㄖㄗㄘㄙㄚㄛㄜㄝㄞㄟㄠㄡㄢㄣㄤㄥㄦㄧㄨㄩㄪㄫㄬ";

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
import cube from 'foo';
(function(global) {
  'use strict';
  global.result = cube(3)
}((1,eval)('this')));
";

            var fooModuleSource = @"
export default function cube(x) {
  return x * x * x;
}
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

                Assert.True(moduleName == "foo");
                var moduleRecord = Engine.JsInitializeModuleRecord(referencingModule, new JavaScriptValueSafeHandle(specifier));
                dependentModuleRecord = moduleRecord;
                childModuleHandle = moduleRecord;
                return false;
            };

            JavaScriptFetchImportedModuleFromScriptCallback fetchFromScriptCallback = (IntPtr referencingModule, IntPtr specifier, out IntPtr dependentModuleRecord) =>
            {
                var moduleName = Extensions.IJavaScriptEngineExtensions.GetStringUtf8(Engine, new JavaScriptValueSafeHandle(specifier));
                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    dependentModuleRecord = referencingModule;
                    return false;
                }

                Assert.True(moduleName == "foo");
                var moduleRecord = Engine.JsInitializeModuleRecord(referencingModule, new JavaScriptValueSafeHandle((IntPtr)specifier));
                dependentModuleRecord = moduleRecord;
                childModuleHandle = moduleRecord;
                return false;
            };

            JavaScriptNotifyModuleReadyCallback notifyCallback = (IntPtr referencingModule, IntPtr exceptionVar) =>
            {
                if (exceptionVar != IntPtr.Zero)
                {
                    var type = Engine.JsGetValueType(new JavaScriptValueSafeHandle(exceptionVar));
                    var ex = Extensions.IJavaScriptEngineExtensions.Stringify(Engine, new JavaScriptValueSafeHandle(exceptionVar));
                    Assert.True(ex == "{}", ex);
                }

                mainModuleReady = true;
                return false;
            };

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.EnableExperimentalFeatures, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    //Initialize the "main" module (Empty-string specifier).
                    var moduleNameHandle = Engine.JsCreateString(mainModuleName, (ulong)mainModuleName.Length);
                    var mainModuleHandle = Engine.JsInitializeModuleRecord(IntPtr.Zero, moduleNameHandle);
                    Assert.True(mainModuleHandle != IntPtr.Zero);

                    //Set the fetch callback.
                    IntPtr fetchCallbackPtr = Marshal.GetFunctionPointerForDelegate(fetchCallback);
                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback, fetchCallbackPtr);

                    //Ensure the callback was set properly.
                    var moduleHostPtr = Engine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback);
                    Assert.Equal(fetchCallbackPtr, moduleHostPtr);

                    //Set the fetchScript callback
                    IntPtr fetchFromScriptCallbackPtr = Marshal.GetFunctionPointerForDelegate(fetchFromScriptCallback);
                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleFromScriptCallback, fetchFromScriptCallbackPtr);

                    //Ensure the callback was set properly.
                    moduleHostPtr = Engine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleFromScriptCallback);
                    Assert.Equal(fetchFromScriptCallbackPtr, moduleHostPtr);

                    //Set the notify callback
                    IntPtr notifyCallbackPtr = Marshal.GetFunctionPointerForDelegate(notifyCallback);
                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback, notifyCallbackPtr);

                    //Ensure the callback was set properly.
                    moduleHostPtr = Engine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback);
                    Assert.Equal(notifyCallbackPtr, moduleHostPtr);


                    //Errrhmmm.. not sure what this is.
                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.HostDefined, moduleNameHandle.DangerousGetHandle());

                    //Ensure the callback was set properly.
                    moduleHostPtr = Engine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.HostDefined);
                    Assert.Equal(moduleNameHandle.DangerousGetHandle(), moduleHostPtr);

                    // ParseModuleSource is sync, while additional fetch & evaluation are async.
                    var scriptBuffer = Encoding.UTF8.GetBytes(mainModuleSource);
                    var errorHandle = Engine.JsParseModuleSource(mainModuleHandle, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)mainModuleSource.Length, JavaScriptParseModuleSourceFlags.DataIsUTF8);
                    Assert.True(errorHandle == JavaScriptValueSafeHandle.Invalid);
                    Assert.True(childModuleHandle != IntPtr.Zero);
                    Assert.False(mainModuleReady);

                    //Parse the foo now.
                    scriptBuffer = Encoding.UTF8.GetBytes(fooModuleSource);
                    errorHandle = Engine.JsParseModuleSource(childModuleHandle, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)fooModuleSource.Length, JavaScriptParseModuleSourceFlags.DataIsUTF8);
                    Assert.True(errorHandle == JavaScriptValueSafeHandle.Invalid);

                    Assert.True(mainModuleReady);

                    //Now we're ready, evaluate the main module.
                    var evalResultHandle = Engine.JsModuleEvaluation(mainModuleHandle);
                    Assert.True(evalResultHandle != JavaScriptValueSafeHandle.Invalid);

                    var resultHandle = Extensions.IJavaScriptEngineExtensions.GetGlobalVariable(Engine, "result");
                    var handleType = Engine.JsGetValueType(resultHandle);
                    Assert.True(handleType == JavaScriptValueType.Number);

                    var result = Engine.JsNumberToInt(resultHandle);
                    Assert.Equal(27, result);
                }
            }
        }
    }
}
