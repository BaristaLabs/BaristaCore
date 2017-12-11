namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public class JsArrayBuffer : JsObject
    {
        public JsArrayBuffer(IJavaScriptEngine engine, BaristaContext context, IBaristaValueFactory valueFactory, JavaScriptValueSafeHandle value)
            : base(engine, context, valueFactory, value)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.ArrayBuffer; }
        }
    }
}
