namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Internal;

    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Xunit;

    public class IDebugJavaScriptEngine_Facts
    {
        private IJavaScriptEngine Engine;

        public IDebugJavaScriptEngine_Facts()
        {
            Engine = JavaScriptEngineFactory.CreateChakraEngine();
        }

        [Fact]
        public void JsCanStartDebugging()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    bool called = false;
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, JavaScriptValueSafeHandle eventData, IntPtr callbackState) =>
                    {
                        called = true;
                        return true;
                    };

                    Engine.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                    //We didn't specify any breakpoints so...
                    Assert.False(called);
                }
            }
        }

        [Fact]
        public void JsCanStopDebugging()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    bool called = false;
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, JavaScriptValueSafeHandle eventData, IntPtr callbackState) =>
                    {
                        called = true;
                        return true;
                    };

                    Engine.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                    Engine.JsDiagStopDebugging(runtimeHandle);

                    //We didn't specify any breakpoints so...
                    Assert.False(called);
                }
            }
        }

        [Fact]
        public void JsCanRequestAsyncBreak()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    bool called = false;
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, JavaScriptValueSafeHandle eventData, IntPtr callbackState) =>
                    {
                        called = true;
                        return true;
                    };

                    Engine.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                    Engine.JsDiagRequestAsyncBreak(runtimeHandle);

                    //We didn't specify any breakpoints so...
                    Assert.False(called);
                }
            }
        }

        [Fact]
        public void JsCanRetrieveBreakpoints()
        {
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    bool called = false;
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, JavaScriptValueSafeHandle eventData, IntPtr callbackState) =>
                    {
                        called = true;
                        return true;
                    };

                    Engine.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                    var breakpoints = Engine.JsDiagGetBreakpoints();
                    Assert.True(breakpoints != JavaScriptValueSafeHandle.Invalid);

                    Engine.JsDiagStopDebugging(runtimeHandle);

                    //We didn't specify any breakpoints so...
                    Assert.False(called);
                }
            }
        }
    }
}
