namespace BaristaLabs.BaristaCore.JavaScript
{
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    ///     Called by the runtime to load the source code of the serialized script.
    ///     The caller must keep the script buffer valid until the JsSerializedScriptUnloadCallback.
    /// </summary>
    /// <param name="sourceContext">The context passed to Js[Parse|Run]SerializedScriptWithCallback</param>
    /// <param name="scriptBuffer">The script returned.</param>
    /// <returns>
    ///     true if the operation succeeded, false otherwise.
    /// </returns>
    public delegate bool JavaScriptSerializedScriptLoadSourceCallback(JavaScriptSourceContext sourceContext, [MarshalAs(UnmanagedType.LPWStr)] out StringBuilder scriptBuffer);
}
