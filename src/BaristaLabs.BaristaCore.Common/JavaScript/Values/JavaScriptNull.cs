namespace BaristaLabs.BaristaCore.JavaScript
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a JavaScript 'null'
    /// </summary>
    public sealed class JavaScriptNull : JavaScriptValue
    {
        internal JavaScriptNull(IJavaScriptEngine engine, JavaScriptContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
        }
    }
}
