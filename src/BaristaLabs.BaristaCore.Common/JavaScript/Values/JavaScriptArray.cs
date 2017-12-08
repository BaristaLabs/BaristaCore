namespace BaristaLabs.BaristaCore.JavaScript
{
    public sealed class JavaScriptArray : JavaScriptValue
    {
        internal JavaScriptArray(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }
    }
}
