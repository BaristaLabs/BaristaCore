namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    ///     An exception returned from the Chakra engine.
    /// </summary>
    public class JavaScriptException : Exception
    {
        /// <summary>
        /// The error code.
        /// </summary>
        private readonly JavaScriptErrorCode m_code;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JavaScriptException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        public JavaScriptException(JavaScriptErrorCode code) :
            this(code, "A fatal exception has occurred in a JavaScript runtime")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JavaScriptException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        /// <param name="message">The error message.</param>
        public JavaScriptException(JavaScriptErrorCode code, string message) :
            base(message)
        {
            this.m_code = code;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JavaScriptException"/> class. 
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="innerException">An inner exception</param>
        protected JavaScriptException(string message, Exception innerException) :
            base(message, innerException)
        {
            if (message != null)
            {
                m_code = (JavaScriptErrorCode)HResult;
            }
        }

        /*
        /// <summary>
        ///     Serializes the exception information.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("code", (uint)code);
        }
        */
        /// <summary>
        ///     Gets the error code.
        /// </summary>
        public JavaScriptErrorCode ErrorCode
        {
            get { return m_code; }
        }
    }
}
