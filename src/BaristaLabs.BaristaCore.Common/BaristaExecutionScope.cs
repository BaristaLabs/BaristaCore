namespace BaristaLabs.BaristaCore
{
    using System;

    public sealed class BaristaExecutionScope : IDisposable
    {
        private readonly Action m_release;

        internal BaristaExecutionScope(Action release)
        {
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

        ~BaristaExecutionScope()
        {
            Dispose(false);
        }

        #endregion
    }
}
