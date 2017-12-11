namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents errors that occur during Barista execution.
    /// </summary>
    [Serializable]
    public class BaristaException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaristaException"/> class.
        /// </summary>
        public BaristaException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaristaException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public BaristaException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaristaException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.</param>
        public BaristaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is <c>null</c>.</exception>
        /// <exception cref="SerializationException">The class name is <c>null</c> or <see cref="Exception.HResult"/> is zero (0).</exception>
        public BaristaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
