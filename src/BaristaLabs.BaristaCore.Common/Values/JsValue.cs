namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Dynamic;
    using System.Text;

    /// <summary>
    /// Represents a JavaScript Value.
    /// </summary>
    public abstract class JsValue : DynamicObject, IBaristaObject<JavaScriptValueSafeHandle>
    {
        private readonly IJavaScriptEngine m_javaScriptEngine;
        private readonly BaristaContext m_context;
        private JavaScriptValueSafeHandle m_javaScriptReference;

        protected JsValue(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle value)
        {
            m_javaScriptEngine = engine ?? throw new ArgumentNullException(nameof(engine));
            m_context = context ?? throw new ArgumentNullException(nameof(context));
            m_javaScriptReference = value ?? throw new ArgumentNullException(nameof(value));
        }

        #region Properties
        /// <summary>
        /// Gets the context associated with the value.
        /// </summary>
        protected BaristaContext Context
        {
            get { return m_context; }
        }

        /// <summary>
        /// Gets the JavaScript Engine associated with the JavaScript object.
        /// </summary>
        public IJavaScriptEngine Engine
        {
            get { return m_javaScriptEngine; }
        }

        /// <summary>
        /// Gets the underlying JavaScript Reference
        /// </summary>
        public JavaScriptValueSafeHandle Handle
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(JsValue));

                return m_javaScriptReference;
            }
        }

        /// <summary>
        /// Gets the value type.
        /// </summary>
        public abstract JavaScriptValueType Type
        {
            get;
        }

        /// <summary>
        /// Gets a value that indicates if this reference has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_javaScriptReference == null || m_javaScriptReference.IsClosed;
            }
        }
        #endregion

        /// <summary>
        /// Gets the actual value type of the object.
        /// </summary>
        /// <returns></returns>
        protected JavaScriptValueType GetValueType()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JsValue));

            return Engine.JsGetValueType(Handle);
        }

        #region DynamicObject overrides
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof(bool))
            {
                result = ToBoolean();
                return true;
            }
            else if (binder.Type == typeof(int))
            {
                result = ToInt32();
                return true;
            }
            else if (binder.Type == typeof(double))
            {
                result = ToDouble();
                return true;
            }
            else if (binder.Type == typeof(string))
            {
                result = ToString();
                return true;
            }

            return base.TryConvert(binder, out result);
        }
        #endregion

        /// <summary>
        /// Converts the value to a bool and returns the boolean representation.
        /// </summary>
        /// <returns></returns>
        public virtual bool ToBoolean()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JsValue));

            using (var numberValueHandle = Engine.JsConvertValueToBoolean(Handle))
            {
                return Engine.JsBooleanToBool(numberValueHandle);
            }
        }

        /// <summary>
        /// Converts the value to a number and returns the double representation
        /// </summary>
        /// <returns></returns>
        public virtual double ToDouble()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JsValue));

            using (var numberValueHandle = Engine.JsConvertValueToNumber(Handle))
            {
                return Engine.JsNumberToDouble(numberValueHandle);
            }
        }

        /// <summary>
        /// Converts the value to a number and returns the integer representation.
        /// </summary>
        /// <returns></returns>
        public virtual int ToInt32()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JsValue));

            using (var numberValueHandle = Engine.JsConvertValueToNumber(Handle))
            {
                return Engine.JsNumberToInt(numberValueHandle);
            }
        }

        /// <summary>
        /// Converts the value to a string (using standard JavaScript semantics) and returns the result
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JsValue));

            using (var stringValueHandle = Engine.JsConvertValueToString(Handle))
            {
                //Get the size
                var size = Engine.JsCopyString(stringValueHandle, null, 0);
                if ((int)size > int.MaxValue)
                    throw new OutOfMemoryException("Exceeded maximum string length.");

                byte[] result = new byte[(int)size];
                var written = Engine.JsCopyString(stringValueHandle, result, (ulong)result.Length);
                return Encoding.UTF8.GetString(result, 0, result.Length);
            }
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

        ~JsValue()
        {
            Dispose(false);
        }
        #endregion;
    }
}
