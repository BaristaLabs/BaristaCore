namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    /// Represents a JavaScript Numeric Value
    /// </summary>
    public sealed class JavaScriptNumber : JavaScriptValue
    {
        internal JavaScriptNumber(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }

        /// <summary>
        /// Returns the double representation of the numeric value.
        /// </summary>
        /// <returns></returns>
        public override double ToDouble()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JavaScriptValue));

            return Engine.JsNumberToDouble(Handle);
        }

        /// <summary>
        /// Returns the integer representation of the numeric value.
        /// </summary>
        /// <returns></returns>
        public override int ToInt32()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JavaScriptValue));

            return Engine.JsNumberToInt(Handle);
        }
    }
}
