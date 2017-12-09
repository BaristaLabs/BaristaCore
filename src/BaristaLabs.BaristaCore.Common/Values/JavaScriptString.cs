namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public sealed class JavaScriptString : JavaScriptValue
    {
        public JavaScriptString(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }
    }
}
