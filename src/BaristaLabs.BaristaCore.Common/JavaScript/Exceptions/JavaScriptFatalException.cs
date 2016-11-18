namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    ///     A fatal exception occurred.
    /// </summary>
    public sealed class JavaScriptFatalException : JavaScriptException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="JavaScriptFatalException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        public JavaScriptFatalException(JavaScriptErrorCode code) :
            this(code, "A fatal exception has occurred in a JavaScript runtime")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JavaScriptFatalException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        /// <param name="message">The error message.</param>
        public JavaScriptFatalException(JavaScriptErrorCode code, string message) :
            base(code, message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JavaScriptFatalException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        private JavaScriptFatalException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
