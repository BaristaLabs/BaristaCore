namespace BaristaLabs.BaristaCore.JavaScript
{
    public enum JavaScriptModuleHostInfoKind
    {
        Exception = 0x01,
        HostDefined = 0x02,
        NotifyModuleReadyCallback = 0x3,
        FetchImportedModuleCallback = 0x4
    }
}
