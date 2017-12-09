namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    /// <summary>
    /// Represents a JavaScript 'null'
    /// </summary>
    public sealed class JavaScriptNull : JavaScriptValue
    {
        public JavaScriptNull(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
        }
    }
}
