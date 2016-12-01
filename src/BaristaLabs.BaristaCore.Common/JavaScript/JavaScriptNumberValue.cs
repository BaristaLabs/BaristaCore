namespace BaristaLabs.BaristaCore.JavaScript
{
    public sealed class JavaScriptNumberValue : JavaScriptValue
    {
        internal JavaScriptNumberValue(IJavaScriptEngine engine, JavaScriptContext context, JavaScriptValueSafeHandle value) : base(engine, context, value)
        {
        }
    }
}
