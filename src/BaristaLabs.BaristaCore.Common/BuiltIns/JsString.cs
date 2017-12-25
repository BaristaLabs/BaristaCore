namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public class JsString : JsObject
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
