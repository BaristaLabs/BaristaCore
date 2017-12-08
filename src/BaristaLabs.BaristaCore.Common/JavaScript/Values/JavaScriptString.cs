namespace BaristaLabs.BaristaCore.JavaScript
{
    public sealed class JavaScriptString : JavaScriptValue
    {
        internal JavaScriptString(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }
    }
}
