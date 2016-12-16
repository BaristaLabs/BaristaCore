namespace BaristaLabs.BaristaCore.JavaScript
{
    public class JavaScriptArrayBuffer : JavaScriptValue
    {
        internal JavaScriptArrayBuffer(IJavaScriptEngine engine, JavaScriptContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }
    }
}
