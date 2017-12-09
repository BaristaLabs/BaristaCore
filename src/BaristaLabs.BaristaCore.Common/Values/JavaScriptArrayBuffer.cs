namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public class JavaScriptArrayBuffer : JavaScriptValue
    {
        public JavaScriptArrayBuffer(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }
    }
}
