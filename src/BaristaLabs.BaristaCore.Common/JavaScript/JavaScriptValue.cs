namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Dynamic;
    using System.Text;

    public abstract class JavaScriptValue : JavaScriptReferenceFlyweight<JavaScriptValueSafeHandle>
    {
        private readonly JavaScriptContext m_context;

        protected JavaScriptValue(IJavaScriptEngine engine, JavaScriptContext context, JavaScriptValueSafeHandle value)
            : base(engine, value)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            m_context = context;
        }

        /// <summary>
        /// Gets the context associated with the value.
        /// </summary>
        protected JavaScriptContext Context
        {
            get { return m_context; }
        }

        /// <summary>
        /// Gets the actual value type of the object.
        /// </summary>
        /// <returns></returns>
        protected JavaScriptValueType GetValueType()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JavaScriptValue));

            return Engine.JsGetValueType(Handle);
        }

        #region DynamicObject overrides
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            //if (binder.Type == typeof(bool))
            //{
            //    result = ToBoolean();
            //    return true;
            //}
            if (binder.Type == typeof(int))
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

        public double ToDouble()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JavaScriptValue));

            if (this is JavaScriptNumberValue)
                return Engine.JsNumberToDouble(Handle);

            using (var numberValueHandle = Engine.JsConvertValueToNumber(Handle))
            {
                return Engine.JsNumberToDouble(numberValueHandle);
            }
        }

        public int ToInt32()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JavaScriptValue));

            if (this is JavaScriptNumberValue)
                return Engine.JsNumberToInt(Handle);

            using (var numberValueHandle = Engine.JsConvertValueToNumber(Handle))
            {
                return Engine.JsNumberToInt(numberValueHandle);
            }
        }

        public override string ToString()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JavaScriptValue));

            using (var stringValueHandle = Engine.JsConvertValueToString(Handle))
            {
                //Get the size
                var size = Engine.JsCopyStringUtf8(stringValueHandle, null, UIntPtr.Zero);
                if ((int)size > int.MaxValue)
                    throw new OutOfMemoryException("Exceeded maximum string length.");

                byte[] result = new byte[(int)size];
                var written = Engine.JsCopyStringUtf8(stringValueHandle, result, new UIntPtr((uint)result.Length));
                return Encoding.UTF8.GetString(result, 0, result.Length);
            }
        }

        /// <summary>
        /// Returns a new JavaScriptValue for the specified handle querying for the handle's value type.
        /// </summary>
        /// <returns>The JavaScript Value that represents the handle</returns>
        internal static JavaScriptValue CreateJavaScriptValueFromHandle(IJavaScriptEngine engine, JavaScriptContext context, JavaScriptValueSafeHandle valueHandle)
        {
            var valueType = engine.JsGetValueType(valueHandle);
            switch (valueType)
            {
                case JavaScriptValueType.Object:
                    return new JavaScriptObject(engine, context, valueHandle);
                case JavaScriptValueType.Function:
                    return new JavaScriptFunction(engine, context, valueHandle);
                case JavaScriptValueType.Number:
                    return new JavaScriptNumberValue(engine, context, valueHandle);
                case JavaScriptValueType.Null:
                    return new JavaScriptNullValue(engine, context, valueHandle);
                case JavaScriptValueType.Undefined:
                    return new JavaScriptUndefinedValue(engine, context, valueHandle);
                default:
                    throw new NotImplementedException($"Error Creating JavaScript Value: The JavaScript Value Type '{valueType}' is unknown, invalid, or has not been implemented.");
            }
        }

        // <summary>
        /// Returns a new JavaScriptValue for the specified handle using the supplied type information.
        /// </summary>
        /// <returns>The JavaScript Value that represents the Handle</returns>
        internal static T CreateJavaScriptValueFromHandle<T>(IJavaScriptEngine engine, JavaScriptContext context, JavaScriptValueSafeHandle valueHandle)
            where T : JavaScriptValue
        {
            switch(typeof(T).ToString())
            {
                case "JavaScriptFunction":
                    return new JavaScriptFunction(engine, context, valueHandle) as T;
                default:
                    throw new NotImplementedException($"Error Creating JavaScript Value: The Type '{typeof(T).ToString()}' is unknown, invalid, or has not been implemented.");

            }
        }
    }
}
