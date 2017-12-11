namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public class JsArrayBuffer : JsObject
    {
        public JsArrayBuffer(IJavaScriptEngine engine, BaristaContext context, IBaristaValueService valueService, JavaScriptValueSafeHandle value)
            : base(engine, context, valueService, value)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.ArrayBuffer; }
        }
    }
}
