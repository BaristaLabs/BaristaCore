namespace BaristaLabs.BaristaCore
{
    using System;

    public sealed class BaristaExecutionScope : IDisposable
    {
        private Action m_release;

        public BaristaExecutionScope(Action release)
        {
            m_release = release ?? throw new ArgumentNullException(nameof(release));
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

        ~BaristaExecutionScope()
        {
            Dispose(false);
        }

        #endregion
    }
}
