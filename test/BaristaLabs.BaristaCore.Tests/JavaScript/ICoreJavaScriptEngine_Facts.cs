namespace BaristaLabs.BaristaCore.JavaScript
{
    using BaristaLabs.BaristaCore.Tests.Extensions;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Text;
    using Xunit;

    [ExcludeFromCodeCoverage]
    [Collection("BaristaCore Tests")]
    public class ICoreJavaScriptEngine_Facts
    {
        private IJavaScriptEngine Engine;

        public ICoreJavaScriptEngine_Facts()
        {
            var chakraCoreFactory = new ChakraCoreFactory();
            Engine = chakraCoreFactory.CreateJavaScriptEngine();
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
                    Assert.True(handleType == JsValueType.String);

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
                    Assert.True(handleType == JsValueType.String);

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
        public void JsCopyStringOneByteTest()
        {
            var str = "Hello, World!";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var stringHandle = Engine.JsCreateString(str, (ulong)str.Length);

                    var size = Engine.JsCopyString(stringHandle, null, 0);
                    if ((int)size > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    byte[] result = new byte[(int)size];
                    Engine.JsCopyStringOneByte(stringHandle, 0, result.Length, result);

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
                        Assert.True(handleType == JsValueType.Function);

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
                        Assert.True(handleType == JsValueType.Number);

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
                        Assert.True(handleType == JsValueType.ArrayBuffer);

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
                        Assert.True(handleType == JsValueType.Function);

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
                        Assert.True(handleType == JsValueType.Number);

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
const global = (new Function('return this;'))();
global.$EXPORTS = cube(3);
";

            var fooModuleSource = @"
export default function cube(x) {
  return x * x * x;
}
";
            var mainModuleReady = false;
            JavaScriptModuleRecord childModuleHandle = JavaScriptModuleRecord.Invalid;

            JavaScriptFetchImportedModuleCallback fetchCallback = (IntPtr referencingModule, IntPtr specifier, out IntPtr dependentModuleRecord) =>
            {
                var moduleName = Engine.GetStringUtf8(new JavaScriptValueSafeHandle(specifier));
                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    dependentModuleRecord = referencingModule;
                    return false;
                }

                Assert.True(moduleName == "foo");
                var moduleRecord = Engine.JsInitializeModuleRecord(new JavaScriptModuleRecord(referencingModule), new JavaScriptValueSafeHandle(specifier));
                dependentModuleRecord = moduleRecord.DangerousGetHandle();
                childModuleHandle = moduleRecord;
                return false;
            };

            JavaScriptFetchImportedModuleFromScriptCallback fetchFromScriptCallback = (IntPtr referencingModule, IntPtr specifier, out IntPtr dependentModuleRecord) =>
            {
                var moduleName = Engine.GetStringUtf8(new JavaScriptValueSafeHandle(specifier));
                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    dependentModuleRecord = referencingModule;
                    return false;
                }

                Assert.True(moduleName == "foo");
                var moduleRecord = Engine.JsInitializeModuleRecord(new JavaScriptModuleRecord(referencingModule), new JavaScriptValueSafeHandle((IntPtr)specifier));
                dependentModuleRecord = moduleRecord.DangerousGetHandle();
                childModuleHandle = moduleRecord;
                return false;
            };

            JavaScriptNotifyModuleReadyCallback notifyCallback = (IntPtr referencingModule, IntPtr exceptionVar) =>
            {
                Assert.Equal(IntPtr.Zero, exceptionVar);
                mainModuleReady = true;
                return false;
            };

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    //Initialize the "main" module (Empty-string specifier).
                    var moduleNameHandle = Engine.JsCreateString(mainModuleName, (ulong)mainModuleName.Length);
                    var mainModuleHandle = Engine.JsInitializeModuleRecord(JavaScriptModuleRecord.Invalid, moduleNameHandle);
                    Assert.True(mainModuleHandle != JavaScriptModuleRecord.Invalid);

                    //Set the fetch callback.
                    var fetchCallbackDelegateHandle = GCHandle.Alloc(fetchCallback);
                    IntPtr fetchCallbackPtr = Marshal.GetFunctionPointerForDelegate(fetchCallback);
                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback, fetchCallbackPtr);

                    //Ensure the callback was set properly.
                    var moduleHostPtr = Engine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback);
                    Assert.Equal(fetchCallbackPtr, moduleHostPtr);

                    //Set the fetchScript callback
                    var fetchFromScriptCallbackDelegateHandle = GCHandle.Alloc(fetchCallback);
                    IntPtr fetchFromScriptCallbackPtr = Marshal.GetFunctionPointerForDelegate(fetchFromScriptCallback);
                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleFromScriptCallback, fetchFromScriptCallbackPtr);

                    //Ensure the callback was set properly.
                    moduleHostPtr = Engine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleFromScriptCallback);
                    Assert.Equal(fetchFromScriptCallbackPtr, moduleHostPtr);

                    //Set the notify callback
                    var notifyCallbackDelegateHandle = GCHandle.Alloc(fetchCallback);
                    IntPtr notifyCallbackPtr = Marshal.GetFunctionPointerForDelegate(notifyCallback);
                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback, notifyCallbackPtr);

                    //Ensure the callback was set properly.
                    moduleHostPtr = Engine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback);
                    Assert.Equal(notifyCallbackPtr, moduleHostPtr);


                    //Indicate the host-defined, main module.
                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.HostDefined, moduleNameHandle.DangerousGetHandle());

                    //Ensure the callback was set properly.
                    moduleHostPtr = Engine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.HostDefined);
                    Assert.Equal(moduleNameHandle.DangerousGetHandle(), moduleHostPtr);

                    // ParseModuleSource is sync, while additional fetch & evaluation are async.
                    var scriptBuffer = Encoding.UTF8.GetBytes(mainModuleSource);
                    var errorHandle = Engine.JsParseModuleSource(mainModuleHandle, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)mainModuleSource.Length, JavaScriptParseModuleSourceFlags.DataIsUTF8);
                    Assert.True(errorHandle == JavaScriptValueSafeHandle.Invalid);
                    Assert.True(childModuleHandle != JavaScriptModuleRecord.Invalid);
                    Assert.False(mainModuleReady);

                    //Parse the foo now.
                    scriptBuffer = Encoding.UTF8.GetBytes(fooModuleSource);
                    errorHandle = Engine.JsParseModuleSource(childModuleHandle, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)fooModuleSource.Length, JavaScriptParseModuleSourceFlags.DataIsUTF8);
                    Assert.True(errorHandle == JavaScriptValueSafeHandle.Invalid);

                    Assert.True(mainModuleReady);

                    //Now we're ready, evaluate the main module.
                    var evalResultHandle = Engine.JsModuleEvaluation(mainModuleHandle);
                    Assert.True(evalResultHandle != JavaScriptValueSafeHandle.Invalid);

                    //Result type of a module is always undefined per spec.
                    var evalResultType = Engine.JsGetValueType(evalResultHandle);
                    Assert.True(evalResultType == JsValueType.Undefined);

                    var resultHandle = Engine.GetGlobalVariable("$EXPORTS");
                    var handleType = Engine.JsGetValueType(resultHandle);
                    Assert.True(handleType == JsValueType.Number);

                    var result = Engine.JsNumberToInt(resultHandle);
                    Assert.Equal(27, result);


                    //Cleanup
                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback, IntPtr.Zero);
                    fetchCallbackDelegateHandle.Free();
                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleFromScriptCallback, IntPtr.Zero);
                    fetchFromScriptCallbackDelegateHandle.Free();
                    Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback, IntPtr.Zero);
                    notifyCallbackDelegateHandle.Free();
                }
            }
        }

        [Fact]
        public void JsPromiseCanBeCreated()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    //This SEEMS backwards, regarding the out parameters, but the promise serves as a breakpoint until
                    //CallFunction is invoked on the resolve/reject handles.
                    var promiseHandle = Engine.JsCreatePromise(out JavaScriptValueSafeHandle resolveHandle, out JavaScriptValueSafeHandle rejectHandle);

                    Assert.True(promiseHandle != JavaScriptValueSafeHandle.Invalid);
                    Assert.True(resolveHandle != JavaScriptValueSafeHandle.Invalid);
                    Assert.True(rejectHandle != JavaScriptValueSafeHandle.Invalid);

                    promiseHandle.Dispose();
                    resolveHandle.Dispose();
                    rejectHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsWeakReferenceCanBeCreated()
        {
            var str = "Hello, World!";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var stringHandle = Engine.JsCreateString(str, (ulong)str.Length);
                    Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

                    var weakRef = Engine.JsCreateWeakReference(stringHandle);
                    Assert.True(weakRef != JavaScriptWeakReferenceSafeHandle.Invalid);

                    stringHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsWeakReferenceValueBeRetrieved()
        {
            var str = "Hello, World!";
            var weakRef = JavaScriptWeakReferenceSafeHandle.Invalid;
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var stringHandle = Engine.JsCreateString(str, (ulong)str.Length);
                    Assert.True(stringHandle != JavaScriptValueSafeHandle.Invalid);

                    weakRef = Engine.JsCreateWeakReference(stringHandle);
                    Assert.True(weakRef != JavaScriptWeakReferenceSafeHandle.Invalid);

                    var valueHandle = Engine.JsGetWeakReferenceValue(weakRef);
                    Assert.True(valueHandle != JavaScriptValueSafeHandle.Invalid);

                    Assert.True(valueHandle == stringHandle);

                    valueHandle.Dispose();
                    stringHandle.Dispose();
                }

                //TODO: even after a collect, JsGetWeakReferenceValue still returns a handle.
                //Engine.JsCollectGarbage(runtimeHandle);
                //var outOfScopeValueHandle = Engine.JsGetWeakReferenceValue(weakRef);
                //Assert.True(outOfScopeValueHandle == JavaScriptValueSafeHandle.Invalid);
            }
        }

        //TODO: Keeping the SharedContents an IntPtr for now, as a safehandle seems to corrupt the runtime until a hard reboot.

        [Fact]
        public void JsSharedArrayBufferWithSharedContentCanBeRetrieved()
        {
            var source = @"(() => {
        return new SharedArrayBuffer(50);
        })();
        ";
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var sharedArrayBufferHandle = Engine.JsRunScript(source);
                    var handleType = Engine.JsGetValueType(sharedArrayBufferHandle);

                    //Apparently the type is object for now.
                    Assert.True(handleType == JsValueType.Object);

                    Internal.LibChakraCore.JsGetSharedArrayBufferContent(sharedArrayBufferHandle, out IntPtr sharedContents);
                    Assert.True(sharedContents != IntPtr.Zero);

                    Internal.LibChakraCore.JsReleaseSharedArrayBufferContentHandle(sharedContents);

                    sharedArrayBufferHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsSharedArrayBufferWithSharedContentCanBeCreated()
        {
            var source = @"(() => {
        return new SharedArrayBuffer(50);
        })();
        ";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var sharedArrayBufferHandle = Engine.JsRunScript(source);
                    var handleType = Engine.JsGetValueType(sharedArrayBufferHandle);

                    //Apparently the type is object for now.
                    Assert.True(handleType == JsValueType.Object);

                    var sharedBufferContentHandle = Engine.JsGetSharedArrayBufferContent(sharedArrayBufferHandle);
                    Assert.True(sharedBufferContentHandle != IntPtr.Zero);

                    var sharedArrayHandle = Engine.JsCreateSharedArrayBufferWithSharedContent(sharedBufferContentHandle);
                    Assert.True(sharedArrayHandle != JavaScriptValueSafeHandle.Invalid);

                    handleType = Engine.JsGetValueType(sharedArrayHandle);
                    Assert.True(handleType == JsValueType.Object);

                    Internal.LibChakraCore.JsReleaseSharedArrayBufferContentHandle(sharedBufferContentHandle);

                    sharedArrayBufferHandle.Dispose();
                    sharedArrayHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsSharedArrayBufferWithSharedContentCanBeReleased()
        {
            var source = @"(() => {
        return new SharedArrayBuffer(50);
        })();
        ";

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    var sharedArrayBufferHandle = Engine.JsRunScript(source);
                    var handleType = Engine.JsGetValueType(sharedArrayBufferHandle);

                    //Apparently the type is object for now.
                    Assert.True(handleType == JsValueType.Object);

                    var sharedBufferContentHandle = Engine.JsGetSharedArrayBufferContent(sharedArrayBufferHandle);
                    Assert.True(sharedBufferContentHandle != IntPtr.Zero);

                    Engine.JsReleaseSharedArrayBufferContentHandle(sharedBufferContentHandle);

                    //TODO: we called it, but unsure how to verify that it has been released -- calling Get again still returns the obj.

                    sharedArrayBufferHandle.Dispose();
                }
            }
        }

        [Fact]
        public void JsDataViewInfoCanBeRetrieved()
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
                    Assert.True(handleType == JsValueType.DataView);

                    var dataViewInfoHandle = Engine.JsGetDataViewInfo(dataViewHandle, out uint byteOffset, out uint byteLength);
                    Assert.True(byteOffset == 0);
                    Assert.True(byteLength == 50);
                    Assert.True(dataViewInfoHandle != JavaScriptValueSafeHandle.Invalid);
                    handleType = Engine.JsGetValueType(dataViewInfoHandle);
                    Assert.True(handleType == JsValueType.ArrayBuffer);

                    dataViewInfoHandle.Dispose();
                    arrayBufferHandle.Dispose();
                    dataViewHandle.Dispose();
                }
            }
        }
    }
}
