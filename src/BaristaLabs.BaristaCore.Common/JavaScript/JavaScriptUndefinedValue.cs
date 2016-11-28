namespace BaristaLabs.BaristaCore.JavaScript
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a JavaScript 'undefined'
    /// </summary>
    public sealed class JavaScriptUndefinedValue : JavaScriptValue
    {
        internal JavaScriptUndefinedValue(IJavaScriptEngine engine, JavaScriptValueSafeHandle handle)
            : base(engine, handle)
        {
#if DEBUG
            var valueType = GetValueType();
            Debug.Assert(valueType == JavaScriptValueType.Undefined);
#endif
        }
    }
}
