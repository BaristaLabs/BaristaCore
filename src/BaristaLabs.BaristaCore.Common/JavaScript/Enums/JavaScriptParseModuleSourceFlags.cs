namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    [Flags]
    public enum JavaScriptParseModuleSourceFlags
    {
        DataIsUTF16LE = 0x00000000,
        DataIsUTF8 = 0x00000001
    }
}
