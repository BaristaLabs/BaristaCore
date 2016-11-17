﻿namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    ///     Attribute mask for JsParseScriptWithAttributes
    /// </summary>
    [Flags]
    public enum JavaScriptParseScriptAttributes
    {
        /// <summary>
        ///     Default attribute
        /// </summary>
        None = 0x0,
        /// <summary>
        ///     Specified script is internal and non-user code. Hidden from debugger
        /// </summary>
        LibraryCode = 0x1,
        /// <summary>
        ///     ChakraCore assumes ExternalArrayBuffer is Utf8 by default.
        ///     This one needs to be set for Utf16
        /// </summary>
        ArrayBufferIsUtf16Encoded = 0x2,
    }
}
