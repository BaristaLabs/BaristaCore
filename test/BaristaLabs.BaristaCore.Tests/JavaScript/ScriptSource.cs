namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a 'scriptsource' implementation.
    /// </summary>
    public sealed class ScriptSource : IDisposable
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
            m_sourceUrlHandle = engine.JsCreateString(sourceUrl, new UIntPtr((uint)sourceUrl.Length));
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
}
