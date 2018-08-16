namespace BaristaLabs.BaristaCore.JavaScript
{
    /// <summary>
    ///     The types of host info that can be set on a module record with JsSetModuleHostInfo.
    /// </summary>
    /// <remarks>
    ///     For more information see JsSetModuleHostInfo.
    /// </remarks>
    public enum JavaScriptModuleHostInfoKind
    {
        /// <summary>
        ///     An exception object - e.g. if the module file cannot be found.
        /// </summary>
        Exception = 0x01,
        /// <summary>
        ///     Host defined info.
        /// </summary>
        HostDefined = 0x02,
        /// <summary>
        ///     Callback for receiving notification when module is ready.
        /// </summary>
        NotifyModuleReadyCallback = 0x3,
        /// <summary>
        ///     Callback for receiving notification to fetch a dependent module.
        /// </summary>
        FetchImportedModuleCallback = 0x4,
        /// <summary>
        ///     Callback for receiving notification for calls to ```import()```
        /// </summary>
        FetchImportedModuleFromScriptCallback = 0x5,
        /// <summary>
        ///     URL for use in error stack traces and debugging.
        /// </summary>
        Url = 0x6
    }
}
