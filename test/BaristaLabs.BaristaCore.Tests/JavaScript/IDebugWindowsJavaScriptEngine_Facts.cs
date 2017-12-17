namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class IDebugWindowsJavaScriptEngine_Facts
    {
        private IJavaScriptEngine Engine;

        public IDebugWindowsJavaScriptEngine_Facts()
        {
            Engine = JavaScriptEngineFactory.CreateChakraEngine();
        }

        public ICommonWindowsJavaScriptEngine CommonWindowsEngine
        {
            get
            {
                return Engine as ICommonWindowsJavaScriptEngine;
            }
        }

        public IDebugWindowsJavaScriptEngine DebugWindowsEngine
        {
            get
            {
                return Engine as IDebugWindowsJavaScriptEngine;
            }
        }

        /// <summary>
        /// End-to-end test of debugging
        /// </summary>
        [Fact]
        public void JsDiagEvaluateReturnsAValue()
        {
            if (DebugWindowsEngine == null)
                return;

            string iterate = @"
var moose = 0;
for(var i = 0; i < 50; i++)
{
    moose++;
}

moose;
";
            int iPod = 0;

            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    //Callback that is run for each breakpoint.
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, IntPtr eventData, IntPtr callbackState) =>
                    {
                        var evalScript = Engine.JsCreateString("i", (ulong)"i".Length);
                        var evalResultHandle = Engine.JsDiagEvaluate(evalScript, 0, JavaScriptParseScriptAttributes.None, false);
                        
                        var handleType = Engine.JsGetValueType(evalResultHandle);
                        Assert.Equal(JavaScriptValueType.Object, handleType);

                        var valuePropertyHandle = CommonWindowsEngine.JsGetPropertyIdFromName("value");
                        var valueHandle = Engine.JsGetProperty(evalResultHandle, valuePropertyHandle);
                        iPod = Engine.JsNumberToInt(valueHandle);
                        evalScript.Dispose();
                        return true;
                    };

                    using (var ss = new ScriptSource(Engine, iterate))
                    {
                        Engine.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                        var scripts = Engine.JsDiagGetScripts();
                        Assert.NotEqual(JavaScriptValueSafeHandle.Invalid, scripts);

                        var ix = Engine.JsIntToNumber(0);
                        var objScriptHandle = Engine.JsGetIndexedProperty(scripts, ix);
                        var scriptIdPropertyHandle = CommonWindowsEngine.JsGetPropertyIdFromName("scriptId");
                        var scriptIdHandle = Engine.JsGetProperty(objScriptHandle, scriptIdPropertyHandle);

                        var scriptId = Engine.JsNumberToInt(scriptIdHandle);

                        //Set a breakpoint with a knkown position
                        var breakPointHandle = Engine.JsDiagSetBreakpoint((uint)scriptId, 4, 0);
                        Assert.NotEqual(JavaScriptValueSafeHandle.Invalid, breakPointHandle);

                        var finalResult = Engine.JsCallFunction(ss.FunctionHandle, new IntPtr[] { ss.FunctionHandle.DangerousGetHandle() }, 1);
                        var handleType = Engine.JsGetValueType(finalResult);
                        Assert.Equal(JavaScriptValueType.Number, handleType);
                        
                        Engine.JsDiagStopDebugging(runtimeHandle);
                    }

                    Assert.Equal(49, iPod);
                }
            }
        }
    }
}
