namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    /// <summary>
    /// Represents a JavaScript Numeric Value
    /// </summary>
    public class JsNumber : JsObject
    {
        public JsNumber(IJavaScriptEngine engine, BaristaContext context,JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
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

        /// <summary>
        /// Returns the long representation of the numeric value.
        /// </summary>
        /// <returns></returns>
        public long ToInt64()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JsValue));

            throw new NotImplementedException();
        }
    }
}
