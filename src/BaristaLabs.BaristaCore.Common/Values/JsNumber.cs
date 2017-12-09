namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    /// <summary>
    /// Represents a JavaScript Numeric Value
    /// </summary>
    public sealed class JsNumber : JsValue
    {
        public JsNumber(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.Number; }
        }

        /// <summary>
        /// Returns the double representation of the numeric value.
        /// </summary>
        /// <returns></returns>
        public override double ToDouble()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JsValue));

            return Engine.JsNumberToDouble(Handle);
        }

        /// <summary>
        /// Returns the integer representation of the numeric value.
        /// </summary>
        /// <returns></returns>
        public override int ToInt32()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JsValue));

            return Engine.JsNumberToInt(Handle);
        }
    }
}
