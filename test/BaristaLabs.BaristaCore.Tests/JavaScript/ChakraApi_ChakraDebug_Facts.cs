namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Internal;

    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Xunit;

    public class ChakraApi_ChakraDebug_Facts
    {
        private IJavaScriptEngine Jsrt;

        public ChakraApi_ChakraDebug_Facts()
        {
            Jsrt = JavaScriptEngineFactory.CreateChakraEngine();
        }

        [Fact]
        public void JsCanStartDebugging()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    bool called = false;
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, JavaScriptValueSafeHandle eventData, IntPtr callbackState) =>
                    {
                        called = true;
                        return true;
                    };

                    Jsrt.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                    //We didn't specify any breakpoints so...
                    Assert.False(called);
                }
            }
        }

        [Fact]
        public void JsCanStopDebugging()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    bool called = false;
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, JavaScriptValueSafeHandle eventData, IntPtr callbackState) =>
                    {
                        called = true;
                        return true;
                    };

                    Jsrt.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                    Jsrt.JsDiagStopDebugging(runtimeHandle);

                    //We didn't specify any breakpoints so...
                    Assert.False(called);
                }
            }
        }

        [Fact]
        public void JsCanRequestAsyncBreak()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    bool called = false;
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, JavaScriptValueSafeHandle eventData, IntPtr callbackState) =>
                    {
                        called = true;
                        return true;
                    };

                    Jsrt.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                    Jsrt.JsDiagRequestAsyncBreak(runtimeHandle);

                    //We didn't specify any breakpoints so...
                    Assert.False(called);
                }
            }
        }

        [Fact]
        public void JsCanRetrieveBreakpoints()
        {
            using (var runtimeHandle = Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Jsrt.JsCreateContext(runtimeHandle))
                {
                    Jsrt.JsSetCurrentContext(contextHandle);

                    bool called = false;
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, JavaScriptValueSafeHandle eventData, IntPtr callbackState) =>
                    {
                        called = true;
                        return true;
                    };

                    Jsrt.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                    var breakpoints = Jsrt.JsDiagGetBreakpoints();
                    Assert.True(breakpoints != JavaScriptValueSafeHandle.Invalid);

                    Jsrt.JsDiagStopDebugging(runtimeHandle);

                    //We didn't specify any breakpoints so...
                    Assert.False(called);
                }
            }
        }
    }
}
