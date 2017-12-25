namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    /// <summary>
    /// Represents a JavaScript 'undefined'
    /// </summary>
    public sealed class JsUndefined : JsValue
    {
        public JsUndefined(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle handle)
            : base(engine, context, handle)
        {
        }

        public override JsValueType Type
        {
            get { return JsValueType.Undefined; }
        }
    }
}
