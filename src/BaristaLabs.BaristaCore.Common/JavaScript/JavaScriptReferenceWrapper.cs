namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    public abstract class JavaScriptReferenceWrapper<T> : IDisposable where T : JavaScriptReference<T>
    {
        private readonly IJavaScriptEngine m_javaScriptEngine;
        private T m_javaScriptReference;

        /// <summary>
        /// Gets the JavaScript Engine associated with the JavaScript Runtime.
        /// </summary>
        public IJavaScriptEngine Engine
        {
            get { return m_javaScriptEngine; }
        }

        /// <summary>
        /// Gets the underlying JavaScript Reference
        /// </summary>
        protected T Handle
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(T));

                return m_javaScriptReference;
            }
        }

        /// <summary>
        /// Gets a value that indicates if this runtime has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_javaScriptReference == null || m_javaScriptReference.IsClosed;
            }
        }

        /// <summary>
        /// Creates a new JavaScript reference wrapper with the specified engine and JavaScript Reference.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="javaScriptReference"></param>
        protected JavaScriptReferenceWrapper(IJavaScriptEngine engine, T javaScriptReference)
        {
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));

            if (javaScriptReference == null || javaScriptReference.IsClosed || javaScriptReference.IsInvalid )
                throw new ArgumentNullException(nameof(javaScriptReference));

            m_javaScriptEngine = engine;
            m_javaScriptReference = javaScriptReference;
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                //Dispose of the handle
                m_javaScriptReference.Dispose();
                m_javaScriptReference = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~JavaScriptReferenceWrapper()
        {
            Dispose(false);
        }
        #endregion;

    }
}
