namespace BaristaLabs.BaristaCore.JavaScript
{
    public class JavaScriptObject : JavaScriptValue
    {
        internal JavaScriptObject(IJavaScriptEngine engine, JavaScriptContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }
    }
}
