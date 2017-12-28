namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public sealed class JsString : JsValue
    {
        public JsString(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
        }

        public override JsValueType Type
        {
            get { return JsValueType.String; }
        }
    }
}
