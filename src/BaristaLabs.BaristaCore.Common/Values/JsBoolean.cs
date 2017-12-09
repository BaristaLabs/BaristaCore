namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    public sealed class JsBoolean : JsValue
    {
        public JsBoolean(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.Boolean; }
        }

        public override bool ToBoolean()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JsValue));

            return Engine.JsBooleanToBool(Handle);
        }

        public static implicit operator bool(JsBoolean jsBool)
        {
            return jsBool.ToBoolean();
        }
    }
}
