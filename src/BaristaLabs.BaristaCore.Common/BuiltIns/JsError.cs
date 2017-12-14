namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public class JsError : JsObject
    {
        public JsError(IJavaScriptEngine engine, BaristaContext context, IBaristaValueFactory valueFactory, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueFactory, valueHandle)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.Error; }
        }

        public string Message
        {
            get
            {
                var result = GetProperty<JsString>("message");
                return result.ToString();
            }
        }
    }
}
