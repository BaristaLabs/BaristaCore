namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    public sealed class JavaScriptBoolean : JavaScriptValue
    {
        public JavaScriptBoolean(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }

        public override bool ToBoolean()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JavaScriptValue));

            return Engine.JsBooleanToBool(Handle);
        }

        public static implicit operator bool(JavaScriptBoolean jsBool)
        {
            return jsBool.ToBoolean();
        }
    }
}
