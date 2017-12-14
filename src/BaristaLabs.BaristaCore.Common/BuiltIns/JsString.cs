namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public class JsString : JsObject
    {
        public JsString(IJavaScriptEngine engine, BaristaContext context, IBaristaValueFactory valueFactory, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueFactory, valueHandle)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.String; }
        }
    }
}
