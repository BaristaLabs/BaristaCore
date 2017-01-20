namespace BaristaLabs.BaristaCore.JavaScript
{
    using Internal;
    using System;
    using System.Diagnostics;

    /// <summary>
    ///     A script exception.
    /// </summary>
    public sealed class JavaScriptScriptException : JavaScriptException, IDisposable
    {
        private const string MessagePropertyName = "message";
        private const string NamePropertyName = "name";
        private const string ColumnNumberPropertyName = "column";

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

            //Don't use our helper errors class in order to prevent recursive errors.
            JavaScriptErrorCode innerError;

            //Get the message of the Script Error.            
            JavaScriptPropertyIdSafeHandle messagePropertyHandle;
            innerError = LibChakraCore.JsCreatePropertyId(MessagePropertyName, (ulong)MessagePropertyName.Length, out messagePropertyHandle);
            Debug.Assert(innerError == JavaScriptErrorCode.NoError);

            JavaScriptValueSafeHandle messageValue;
            innerError = LibChakraCore.JsGetProperty(error, messagePropertyHandle, out messageValue);
            Debug.Assert(innerError == JavaScriptErrorCode.NoError);

            ErrorMessage = Helpers.GetStringUtf8(messageValue, releaseHandle: true);
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
            private set;
        }

        /// <summary>
        /// Gets the column number in the line of the file that raised this error.
        /// </summary>
        public int LineNumber
        {
            get;
            private set;
        }

        public int Length
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a human-readable description of the error.
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a name for the type of error. The initial value is "Error".
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a trace of which functions were called, in what order, from which line and file, and with what arguments. The stack string proceeds from the most recent calls to earlier ones, leading back to the original global scope call.
        /// </summary>
        public string Stack
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the source of the error.
        /// </summary>
        public string ScriptSource
        {
            get;
            private set;
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
