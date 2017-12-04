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
                throw new ObjectDisposedException(nameof(JavaScriptValue));

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
                throw new ObjectDisposedException(nameof(JavaScriptValue));

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
                throw new ObjectDisposedException(nameof(JavaScriptValue));

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
                throw new ObjectDisposedException(nameof(JavaScriptValue));

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

        /// <summary>
        /// Returns a new JavaScriptValue for the specified handle querying for the handle's value type.
        /// </summary>
        /// <returns>The JavaScript Value that represents the handle</returns>
        internal static JavaScriptValue CreateJavaScriptValueFromHandle(IJavaScriptEngine engine, JavaScriptContext context, JavaScriptValueSafeHandle valueHandle)
        {
            var valueType = engine.JsGetValueType(valueHandle);
            switch (valueType)
            {
                case JavaScriptValueType.Array:
                    return new JavaScriptArray(engine, context, valueHandle);
                case JavaScriptValueType.ArrayBuffer:
                    return new JavaScriptArrayBuffer(engine, context, valueHandle);
                case JavaScriptValueType.Boolean:
                    return new JavaScriptBoolean(engine, context, valueHandle);
                case JavaScriptValueType.DataView:
                    //TODO: Add a dataview
                    throw new NotImplementedException();
                case JavaScriptValueType.Error:
                    //TODO: Realign exception classes to be JavaScriptValues... or something.
                    throw new NotImplementedException();
                case JavaScriptValueType.Function:
                    return new JavaScriptFunction(engine, context, valueHandle);
                case JavaScriptValueType.Null:
                    return new JavaScriptNull(engine, context, valueHandle);
                case JavaScriptValueType.Number:
                    return new JavaScriptNumber(engine, context, valueHandle);
                case JavaScriptValueType.Object:
                    return new JavaScriptObject(engine, context, valueHandle);
                case JavaScriptValueType.String:
                    return new JavaScriptString(engine, context, valueHandle);
                case JavaScriptValueType.Symbol:
                    //TODO: add symbol class.
                    throw new NotImplementedException();
                case JavaScriptValueType.TypedArray:
                    return new JavaScriptTypedArray(engine, context, valueHandle);
                    throw new NotImplementedException();
                case JavaScriptValueType.Undefined:
                    return new JavaScriptUndefined(engine, context, valueHandle);
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
            switch(typeof(T))
            {
                case Type t when t == typeof(JavaScriptFunction):
                    return new JavaScriptFunction(engine, context, valueHandle) as T;
                default:
                    throw new NotImplementedException($"Error Creating JavaScript Value: The Type '{typeof(T).ToString()}' is unknown, invalid, or has not been implemented.");
            }
        }
    }
}
