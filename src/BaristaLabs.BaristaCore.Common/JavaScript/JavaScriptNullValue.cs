namespace BaristaLabs.BaristaCore.JavaScript
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a JavaScript 'null'
    /// </summary>
    public sealed class JavaScriptNullValue : JavaScriptValue
    {
        internal JavaScriptNullValue(IJavaScriptEngine engine, JavaScriptContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
#if DEBUG
            var valueType = GetValueType();
            Debug.Assert(valueType == JavaScriptValueType.Undefined);
#endif
        }
    }
}
