namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Xunit;

    public class  ICoreWindowsJavaScriptEngine_Facts
    {
        private IJavaScriptEngine Engine;

        public ICoreWindowsJavaScriptEngine_Facts()
        {
            Engine = JavaScriptEngineFactory.CreateChakraEngine();
        }

        public ICoreWindowsJavaScriptEngine CoreWindowsEngine
        {
            get
            {
                return Engine as ICoreWindowsJavaScriptEngine;
            }
        }

        [Fact]
        public void JsModuleCanBeImportedAndExecuted()
        {
            if (CoreWindowsEngine == null)
                return;

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
                var moduleRecord = CoreWindowsEngine.JsInitializeModuleRecord(referencingModule, new JavaScriptValueSafeHandle((IntPtr)specifier));
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
                    var moduleNameHandle = Engine.JsCreateStringUtf8(mainModuleName, new UIntPtr((uint)mainModuleName.Length));
                    var mainModuleHandle = CoreWindowsEngine.JsInitializeModuleRecord(IntPtr.Zero, moduleNameHandle);

                    IntPtr fetchCallbackPtr = Marshal.GetFunctionPointerForDelegate(fetchCallback);
                    CoreWindowsEngine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback, fetchCallbackPtr);

                    //Ensure the callback was set properly.
                    var moduleHostPtr = CoreWindowsEngine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback);
                    Assert.Equal(fetchCallbackPtr, moduleHostPtr);
                    
                    IntPtr notifyCallbackPtr = Marshal.GetFunctionPointerForDelegate(notifyCallback);
                    CoreWindowsEngine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback, notifyCallbackPtr);

                    //Ensure the callback was set properly.
                    moduleHostPtr = CoreWindowsEngine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback);
                    Assert.Equal(notifyCallbackPtr, moduleHostPtr);

                    CoreWindowsEngine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.HostDefined, moduleNameHandle.DangerousGetHandle());

                    //Ensure the callback was set properly.
                    moduleHostPtr = CoreWindowsEngine.JsGetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.HostDefined);
                    Assert.Equal(moduleNameHandle.DangerousGetHandle(), moduleHostPtr);

                    // ParseModuleSource is sync, while additional fetch & evaluation are async.
                    var scriptBuffer = Encoding.UTF8.GetBytes(mainModuleSource);
                    var errorHandle = CoreWindowsEngine.JsParseModuleSource(mainModuleHandle, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)mainModuleSource.Length, JavaScriptParseModuleSourceFlags.JsParseModuleSourceFlags_DataIsUTF8);
                    Assert.True(errorHandle == JavaScriptValueSafeHandle.Invalid);
                    Assert.True(childModuleHandle != IntPtr.Zero);
                    Assert.False(mainModuleReady);

                    //Parse the foo now.
                    scriptBuffer = Encoding.UTF8.GetBytes(fooModuleSource);
                    errorHandle = CoreWindowsEngine.JsParseModuleSource(childModuleHandle, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)fooModuleSource.Length, JavaScriptParseModuleSourceFlags.JsParseModuleSourceFlags_DataIsUTF8);
                    Assert.True(errorHandle == JavaScriptValueSafeHandle.Invalid);

                    Assert.True(mainModuleReady);

                    //Now we're ready, evaluate the main module.
                    var resultHandle = CoreWindowsEngine.JsModuleEvaluation(mainModuleHandle);
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
