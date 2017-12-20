namespace BaristaLabs.BaristaCore
{
    using System;

    /// <summary>
    ///     An exception returned from a JavaScript engine.
    /// </summary>
    public abstract class JsException : Exception
    {
        /// <summary>
        /// The error code.
        /// </summary>
        private readonly JsErrorCode m_code;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        public JsException(JsErrorCode code) :
            this(code, "A fatal exception has occurred in a JavaScript runtime")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        /// <param name="message">The error message.</param>
        public JsException(JsErrorCode code, string message) :
            base(message)
        {
            m_code = code;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsException"/> class. 
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="innerException">An inner exception</param>
        protected JsException(string message, Exception innerException) :
            base(message, innerException)
        {
            if (message != null)
            {
                m_code = (JsErrorCode)HResult;
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
        public JsErrorCode ErrorCode
        {
            get { return m_code; }
        }
    }
}
