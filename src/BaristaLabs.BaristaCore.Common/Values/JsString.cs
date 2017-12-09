namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public sealed class JsString : JsValue
    {
        public JsString(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.String; }
        }
    }
}
