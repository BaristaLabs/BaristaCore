namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Text;

    public abstract class JavaScriptValue : JavaScriptReferenceWrapper<JavaScriptValueSafeHandle>
    {
        protected JavaScriptValue(IJavaScriptEngine engine, JavaScriptValueSafeHandle value)
            : base(engine, value)
        {
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
        /// Returns a new JavaScriptValueSafeHandle for the specified handle, ensuring that the handle's lifecycle is monitored.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static JavaScriptValue CreateJavaScriptValueFromHandle(IJavaScriptEngine engine, JavaScriptValueSafeHandle value)
        {
            var valueType = engine.JsGetValueType(value);
            JavaScriptValue result = null;
            switch (valueType)
            {
                case JavaScriptValueType.Object:
                    break;
                default:
                    throw new NotImplementedException($"The type {valueType} is unknown, invalid, or has not been implemented.");
            }

            return result;
        }

    }
}
