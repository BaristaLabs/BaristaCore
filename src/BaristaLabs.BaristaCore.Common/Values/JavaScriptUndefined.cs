namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System.Diagnostics;

    /// <summary>
    /// Represents a JavaScript 'undefined'
    /// </summary>
    public sealed class JavaScriptUndefined : JavaScriptValue
    {
        public JavaScriptUndefined(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle handle)
            : base(engine, context, handle)
        {
        }
    }
}
