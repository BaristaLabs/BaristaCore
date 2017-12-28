namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    public sealed class JsBoolean : JsValue
    {
        public JsBoolean(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
        }

        public override JsValueType Type
        {
            get { return JsValueType.Boolean; }
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
