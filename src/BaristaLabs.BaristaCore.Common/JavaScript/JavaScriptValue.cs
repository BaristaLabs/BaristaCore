namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    public abstract class JavaScriptValue
    {
        private readonly JavaScriptValueSafeHandle m_valueSafeHandle;

        protected JavaScriptValue(IJavaScriptEngine engine, JavaScriptValueSafeHandle value)
        {
            if (value == null || value == JavaScriptValueSafeHandle.Invalid)
                throw new ArgumentNullException(nameof(value));

            m_valueSafeHandle = value;
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
