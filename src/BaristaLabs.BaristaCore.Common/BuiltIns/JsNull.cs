namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    /// <summary>
    /// Represents a JavaScript 'null'
    /// </summary>
    public sealed class JsNull : JsValue
    {
        public JsNull(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
        }

        public override JsValueType Type
        {
            get { return JsValueType.Null; }
        }
    }
}
