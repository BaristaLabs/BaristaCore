namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    [Flags]
    public enum JavaScriptParseModuleSourceFlags
    {
        JsParseModuleSourceFlags_DataIsUTF16LE = 0x00000000,
        JsParseModuleSourceFlags_DataIsUTF8 = 0x00000001
    }
}
