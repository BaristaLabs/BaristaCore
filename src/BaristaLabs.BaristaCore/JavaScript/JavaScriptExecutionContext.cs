namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Diagnostics;

    public sealed class JavaScriptExecutionContext : IDisposable
    {
        private JavaScriptEngine m_engine;
        private Action m_release;

        internal JavaScriptExecutionContext(JavaScriptEngine engine, Action release)
        {
            Debug.Assert(engine != null);
            Debug.Assert(release != null);

            m_engine = engine;
            m_release = release;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~JavaScriptExecutionContext()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (m_release != null)
                m_release();

            if (disposing)
            {
                m_engine = null;
                m_release = null;
            }
        }
    }
}
