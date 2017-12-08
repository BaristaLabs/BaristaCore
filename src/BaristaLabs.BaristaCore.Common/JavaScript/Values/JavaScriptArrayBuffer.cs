namespace BaristaLabs.BaristaCore.JavaScript
{
    public class JavaScriptArrayBuffer : JavaScriptValue
    {
        internal JavaScriptArrayBuffer(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }
    }
}
