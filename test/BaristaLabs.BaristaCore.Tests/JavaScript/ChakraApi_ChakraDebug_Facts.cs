namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Internal;

    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Xunit;

    public class ChakraApi_ChakraDebug_Facts
    {
        private IJavaScriptRuntime Jsrt;

        public ChakraApi_ChakraDebug_Facts()
        {
            Jsrt = JavaScriptRuntimeFactory.CreateChakraRuntime();
        }

        [Fact]
        public void JsCanStartDebugging()
        {
            JavaScriptRuntimeSafeHandle runtimeHandle;
            Errors.ThrowIfError(Jsrt.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtimeHandle));

            JavaScriptContextSafeHandle contextHandle;
            Errors.ThrowIfError(Jsrt.JsCreateContext(runtimeHandle, out contextHandle));
            Errors.ThrowIfError(Jsrt.JsSetCurrentContext(contextHandle));

            bool called = false;
            JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, JavaScriptValueSafeHandle eventData, IntPtr callbackState) =>
            {
                called = true;
                return true;
            };

            Errors.ThrowIfError(Jsrt.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero));

            //We didn't specify any breakpoints so...
            Assert.False(called);

            contextHandle.Dispose();
            runtimeHandle.Dispose();
        }
    }
}
