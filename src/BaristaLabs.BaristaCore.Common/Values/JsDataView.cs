namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public class JsDataView : JsObject
    {
        public JsDataView(IJavaScriptEngine engine, BaristaContext context, IBaristaValueFactory valueFactory, JavaScriptValueSafeHandle value)
            : base(engine, context, valueFactory, value)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.DataView; }
        }
    }
}
