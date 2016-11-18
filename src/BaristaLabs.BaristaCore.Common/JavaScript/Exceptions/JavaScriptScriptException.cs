namespace BaristaLabs.BaristaCore.JavaScript
{
    using Internal;
    using System;

    /// <summary>
    ///     A script exception.
    /// </summary>
    public sealed class JavaScriptScriptException : JavaScriptException, IDisposable
    {
        /// <summary>
        /// The error.
        /// </summary>
        private readonly JavaScriptValueSafeHandle m_error;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JavaScriptScriptException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        /// <param name="error">The JavaScript error object.</param>
        public JavaScriptScriptException(JavaScriptErrorCode code, JavaScriptValueSafeHandle error) :
            this(code, error, "JavaScript Exception")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JavaScriptScriptException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        /// <param name="error">The JavaScript error object.</param>
        /// <param name="message">The error message.</param>
        public JavaScriptScriptException(JavaScriptErrorCode code, JavaScriptValueSafeHandle error, string message) :
            base(code, message)
        {
            m_error = error;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JavaScriptScriptException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        private JavaScriptScriptException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        /// <summary>
        ///     Gets a JavaScript object representing the script error.
        /// </summary>
        internal JavaScriptValueSafeHandle Error
        {
            get
            {
                return m_error;
            }
        }


        /// <summary>
        /// Gets the column number in the line of the file that raised this error.
        /// </summary>
        public int ColumnNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the column number in the line of the file that raised this error.
        /// </summary>
        public int LineNumber
        {
            get;
            set;
        }

        public int Length
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a name for the type of error. The initial value is "Error".
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a trace of which functions were called, in what order, from which line and file, and with what arguments. The stack string proceeds from the most recent calls to earlier ones, leading back to the original global scope call.
        /// </summary>
        public string Stack
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the source of the error.
        /// </summary>
        public string ScriptSource
        {
            get;
            set;
        }

        #region IDisposable implementation
        private bool m_disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!m_disposedValue)
            {
                if (disposing)
                {
                    m_error.Dispose();
                    m_disposedValue = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
