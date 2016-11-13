namespace BaristaLabs.BaristaCore.JavaScript.Interop.Callbacks
{
    using System;

    /// <summary>
    ///     A callback called before collection.
    /// </summary>
    /// <param name="callbackState">The state passed to SetBeforeCollectCallback.</param>
    internal delegate void JavaScriptBeforeCollectCallback(IntPtr callbackState);
}
