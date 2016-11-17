namespace BaristaLabs.BaristaCore.JavaScript
{
    public enum JavaScriptModuleHostInfoKind
    {
        JsModuleHostInfo_Exception = 0x01,
        JsModuleHostInfo_HostDefined = 0x02,
        JsModuleHostInfo_NotifyModuleReadyCallback = 0x3,
        JsModuleHostInfo_FetchImportedModuleCallback = 0x4
    }
}
