namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Diagnostics;

    public sealed class JavaScriptExecutionContext : IDisposable
    {
        private JavaScriptContext m_context;
        private Action m_release;

        internal JavaScriptExecutionContext(JavaScriptContext context, Action release)
        {
            Debug.Assert(context != null);
            Debug.Assert(release != null);

            m_context = context;
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
            m_release?.Invoke();

            if (disposing)
            {
                m_context = null;
                m_release = null;
            }
        }
    }
}
