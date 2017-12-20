namespace BaristaLabs.BaristaCore
{
    using System;

    /// <summary>
    ///     An API usage exception occurred.
    /// </summary>
    public sealed class JsUsageException : JsException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="JsUsageException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        public JsUsageException(JsErrorCode code) :
            this(code, "A fatal exception has occurred in a JavaScript runtime")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsUsageException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        /// <param name="message">The error message.</param>
        public JsUsageException(JsErrorCode code, string message) :
            base(code, message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsUsageException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        private JsUsageException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
