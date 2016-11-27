namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    ///     Represents a JavaScript Context
    /// </summary>
    /// <remarks>
    ///     Each script context has its own global object that is isolated from all other script contexts.
    /// </remarks>
    public sealed class JavaScriptContext : IDisposable
    {
        private JavaScriptContextSafeHandle m_contextSafeHandle;

        public bool IsDisposed
        {
            get { return m_contextSafeHandle == null; }
        }

        #region IDisposable
        private void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                m_contextSafeHandle.Dispose();
                m_contextSafeHandle = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~JavaScriptContext()
        {
            Dispose(false);
        }
        #endregion

    }
}
