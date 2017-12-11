namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    public class JsBoolean : JsObject
    {
        public JsBoolean(IJavaScriptEngine engine, BaristaContext context, IBaristaValueService valueService, JavaScriptValueSafeHandle value)
            : base(engine, context, valueService, value)
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
