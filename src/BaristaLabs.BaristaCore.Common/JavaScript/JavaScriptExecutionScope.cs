namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    public sealed class JavaScriptExecutionScope : IDisposable
    {
        private Action m_release;

        internal JavaScriptExecutionScope(Action release)
        {
            if (release == null)
                throw new ArgumentNullException(nameof(release));

            m_release = release;
        }

        #region Disposable
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_release();
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        ~JavaScriptExecutionScope()
        {
            Dispose(false);
        }

        #endregion
    }
}
