namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    public class JsBoolean : JsObject
    {
        public JsBoolean(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
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
