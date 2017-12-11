namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents errors that occur during the execution of barista scripts.
    /// </summary>
    [Serializable]
    public class BaristaScriptException : BaristaException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaristaScriptException"/> class.
        /// </summary>
        public BaristaScriptException()
        {
        }

        public BaristaScriptException(JsError error)
            : base(error.Message)
        {
            InitFromError(error);
        }

        // <summary>
        /// Initializes a new instance of the <see cref="BaristaException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public BaristaScriptException(string message)
            : base(message)
        {
        }

        // <summary>
        /// Initializes a new instance of the <see cref="BaristaException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public BaristaScriptException(string message, JsError error)
            : base(message)
        {
            InitFromError(error);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is <c>null</c>.</exception>
        /// <exception cref="SerializationException">The class name is <c>null</c> or <see cref="Exception.HResult"/> is zero (0).</exception>
        public BaristaScriptException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is <c>null</c>.</exception>
        /// <exception cref="SerializationException">The class name is <c>null</c> or <see cref="Exception.HResult"/> is zero (0).</exception>
        public BaristaScriptException(SerializationInfo info, StreamingContext context, JsError error)
            : base(info, context)
        {
            InitFromError(error);
        }

        [DebuggerStepThrough]
        private void InitFromError(JsError error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            Line = error.Line;
            Column = error.Column;
            Length = error.Length;
            Source = error.Source;
        }

        /// <summary>
        /// Gets or sets the line number where the script error occurred.
        /// </summary>
        public int Line
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the column number where the script error occurred.
        /// </summary>
        public int Column
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the length of the script error occurred.
        /// </summary>
        public int Length
        {
            get;
            set;
        }
    }
}
