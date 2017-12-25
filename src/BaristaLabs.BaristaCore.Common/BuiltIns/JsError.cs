namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public class JsError : JsObject
    {
        public JsError(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
        }

        public override JsValueType Type
        {
            get { return JsValueType.Error; }
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
