namespace BaristaLabs.BaristaCore.JavaScript
{
    public sealed class JavaScriptString : JavaScriptValue
    {
        internal JavaScriptString(IJavaScriptEngine engine, JavaScriptContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }
    }
}
