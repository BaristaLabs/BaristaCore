namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public class JsString : JsObject
    {
        public JsString(IJavaScriptEngine engine, BaristaContext context, IBaristaValueService valueService, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueService, valueHandle)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.String; }
        }
    }
}
