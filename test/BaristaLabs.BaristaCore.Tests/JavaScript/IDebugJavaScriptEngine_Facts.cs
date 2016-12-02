namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
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
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, IntPtr eventData, IntPtr callbackState) =>
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
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, IntPtr eventData, IntPtr callbackState) =>
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
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, IntPtr eventData, IntPtr callbackState) =>
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
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, IntPtr eventData, IntPtr callbackState) =>
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

        /// <summary>
        /// End-to-end test of debugging
        /// </summary>
        [Fact]
        public void JsCanBeDebugged()
        {
            string fibonacci = @"
function fibonacci(num){
  var a = 1, b = 0, temp;

  while (num >= 0){
    temp = a;
    a = a + b;
    b = temp;
    num--;
  }

  return b;
};

fibonacci(50);
";
            using (var runtimeHandle = Engine.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null))
            {
                using (var contextHandle = Engine.JsCreateContext(runtimeHandle))
                {
                    Engine.JsSetCurrentContext(contextHandle);

                    bool called = false;
                    JavaScriptDiagDebugEventCallback callback = (JavaScriptDiagDebugEventType eventType, IntPtr eventData, IntPtr callbackState) =>
                    {
                        called = true;
                        return true;
                    };

                    using (var ss = new ScriptSource(Engine, fibonacci))
                    {
                        Engine.JsDiagStartDebugging(runtimeHandle, callback, IntPtr.Zero);

                        var scripts = Engine.JsDiagGetScripts();
                        Assert.NotEqual(JavaScriptValueSafeHandle.Invalid, scripts);

                        var ix = Engine.JsIntToNumber(0);
                        var objScript = Engine.JsGetIndexedProperty(scripts, ix);

                        var handleType = Engine.JsGetValueType(objScript);
                        Assert.True(handleType == JavaScriptValueType.Object);

                        //Not sure if the ScriptId varies independently of the ScriptContext cookie
                        var scriptIdPropertyHandle = Engine.JsCreatePropertyIdUtf8("scriptId", new UIntPtr((uint)"scriptId".Length));
                        var scriptIdHandle = Engine.JsGetProperty(objScript, scriptIdPropertyHandle);
                        var scriptId = Engine.JsNumberToInt(scriptIdHandle);
                        var breakPointHandle = Engine.JsDiagSetBreakpoint((uint)scriptId, 5, 0);

                        //Call the function
                        Engine.JsCallFunction(ss.FunctionHandle, new IntPtr[] { ss.FunctionHandle.DangerousGetHandle() }, 1);

                        Assert.True(called);
                        Engine.JsDiagStopDebugging(runtimeHandle);
                    }
                }
            }
        }

        #region Nested Class: ScriptSource
        /// <summary>
        /// Represents a 'scriptsource' implementation.
        /// </summary>
        private sealed class ScriptSource : IDisposable
        {
            private IntPtr m_ptrScript;
            private readonly string m_script;
            private readonly JavaScriptSourceContext m_sourceContext;

            private JavaScriptValueSafeHandle m_scriptHandle, m_sourceUrlHandle, m_fnScript;

            public string Script
            {
                get { return m_script; }
            }

            /// <summary>
            /// Gets the handle of the script
            /// </summary>
            public JavaScriptValueSafeHandle FunctionHandle
            {
                get { return m_fnScript; }
            }

            public JavaScriptSourceContext SourceContext
            {
                get { return m_sourceContext; }
            }

            public ScriptSource(IJavaScriptEngine engine, string script, string sourceUrl = "[eval source]")
            {
                if (engine == null)
                    throw new ArgumentNullException(nameof(engine));

                if (script == null)
                    throw new ArgumentNullException(nameof(script));
                
                m_script = script;
                m_ptrScript = Marshal.StringToHGlobalAnsi(script);
                m_scriptHandle = engine.JsCreateExternalArrayBuffer(m_ptrScript, (uint)script.Length, null, IntPtr.Zero);
                m_sourceUrlHandle = engine.JsCreateStringUtf8(sourceUrl, new UIntPtr((uint)sourceUrl.Length));
                m_sourceContext = JavaScriptSourceContext.GetNextSourceContext();

                m_fnScript = engine.JsParse(m_scriptHandle, m_sourceContext, m_sourceUrlHandle, JavaScriptParseScriptAttributes.None);
            }

            #region IDisposable
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (m_fnScript != null)
                    {
                        m_fnScript.Dispose();
                        m_fnScript = null;
                    }

                    if (m_sourceUrlHandle != null)
                    {
                        m_sourceUrlHandle.Dispose();
                        m_sourceUrlHandle = null;
                    }

                    if (m_scriptHandle != null)
                    {
                        m_scriptHandle.Dispose();
                        m_scriptHandle = null;
                    }
                }

                Marshal.FreeHGlobal(m_ptrScript);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            ~ScriptSource()
            {
                Dispose(false);
            }
            #endregion
        }
        #endregion
    }
}
