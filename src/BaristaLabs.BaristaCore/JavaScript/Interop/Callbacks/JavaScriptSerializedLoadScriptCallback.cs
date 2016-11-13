namespace BaristaLabs.BaristaCore.JavaScript.Interop.Callbacks
{
    using SafeHandles;

    /// <summary>
    ///     Called by the runtime to load the source code of the serialized script.
    /// </summary>
    /// <param name="sourceContext">The context passed to Js[Parse|Run]SerializedScriptCallback</param>
    /// <param name="script">The script returned.</param>
    /// <returns>
    ///     true if the operation succeeded, false otherwise.
    /// </returns>
    internal delegate bool JavaScriptSerializedLoadScriptCallback(JavaScriptSourceContext sourceContext, out JavaScriptValueSafeHandle value, out JsParseScriptAttributes parseAttributes);
}
