namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.JavaScript.Internal;
    using System;

    /// <summary>
    ///     A script exception.
    /// </summary>
    public sealed class JsScriptException : JsException, IDisposable
    {
        private const string MessagePropertyName = "message";
        private const string NamePropertyName = "name";
        private const string ColumnNumberPropertyName = "column";
        public const string LineNumberPropertyName = "line";

        public string m_message;

        /// <summary>
        /// The error.
        /// </summary>
        private readonly JavaScriptValueSafeHandle m_error;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsScriptException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        /// <param name="error">The JavaScript error object.</param>
        public JsScriptException(JsErrorCode code, JavaScriptValueSafeHandle error) :
            this(code, error, "JavaScript Exception")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsScriptException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        /// <param name="error">The JavaScript error object.</param>
        /// <param name="message">The error message.</param>
        public JsScriptException(JsErrorCode code, JavaScriptValueSafeHandle error, string message) :
            base(code, message)
        {
            m_error = error;
            m_message = message;

            //Don't use our helper errors class in order to prevent recursive errors.
            JsErrorCode innerError;

            //Get the error object type
            innerError = LibChakraCore.JsGetValueType(error, out JsValueType errorType);

            switch (errorType)
            {
                case JsValueType.Object:
                    if (TryGetErrorProperty(error, ColumnNumberPropertyName, out int metadataColumnValue))
                    {
                        ColumnNumber = metadataColumnValue;
                    }

                    if (TryGetErrorProperty(error, LineNumberPropertyName, out int metadataLineValue))
                    {
                        LineNumber = metadataLineValue;
                    }
                    if (TryGetErrorProperty(error, "source", out string metadataScriptSourceValue))
                    {
                        ScriptSource = metadataScriptSourceValue;
                    }
                    if (TryGetErrorProperty(error, "url", out string metadataUrlValue))
                    {
                        Name = metadataUrlValue;
                    }
                    break;
                case JsValueType.Error:
                    //Get the message of the Script Error.            
                    if (TryGetErrorProperty(error, MessagePropertyName, out string errrorMessageValue))
                    {
                        m_message = errrorMessageValue;
                    }

                    if (TryGetErrorProperty(error, ColumnNumberPropertyName, out int errorColumnValue))
                    {
                        ColumnNumber = errorColumnValue;
                    }

                    if (TryGetErrorProperty(error, LineNumberPropertyName, out int errorLineValue))
                    {
                        LineNumber = errorLineValue;
                    }

                    if (TryGetErrorProperty(error, NamePropertyName, out string errorNameValue))
                    {
                        Name = errorNameValue;
                    }

                    break;
                case JsValueType.String:
                    m_message = Helpers.GetStringUtf8(error);
                    break;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsScriptException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        private JsScriptException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        private bool TryGetErrorProperty<T>(JavaScriptValueSafeHandle error, string propertyName, out T value)
        {
            JsErrorCode innerError;
            innerError = LibChakraCore.JsCreatePropertyId(propertyName, (ulong)propertyName.Length, out JavaScriptPropertyIdSafeHandle propertyHandle);
            innerError = LibChakraCore.JsHasProperty(error, propertyHandle, out bool hasProperty);

            if (hasProperty == true)
            {
                innerError = LibChakraCore.JsGetProperty(error, propertyHandle, out JavaScriptValueSafeHandle propertyValue);
                innerError = LibChakraCore.JsGetValueType(propertyValue, out JsValueType propertyType);
                switch(propertyType)
                {
                    case JsValueType.Number:
                        innerError = LibChakraCore.JsNumberToDouble(propertyValue, out double doubleValue);
                        value = (T)Convert.ChangeType(doubleValue, typeof(T));
                        return true;
                    case JsValueType.String:
                        var strValue = Helpers.GetStringUtf8(propertyValue, releaseHandle: true);
                        value = (T)Convert.ChangeType(strValue, typeof(T));
                        return true;
                    default:
                        value = default(T);
                        return false;
                }

                
            }

            value = default(T);
            return false;
        }
        /// <summary>
        ///     Gets a JavaScript object representing the script error.
        /// </summary>
        public JavaScriptValueSafeHandle Error
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

        public override string Message
        {
            get { return m_message; }
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

        public override string ToString()
        {
            return $"{Name}: {m_message}";
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
