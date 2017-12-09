namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     A function callback.
    /// </summary>
    /// <param name="callee">
    ///     A <c>Function</c> object that represents the function being invoked.
    /// </param>
    /// <param name="isConstructCall">Indicates whether this is a regular call or a 'new' call.</param>
    /// <param name="arguments">The arguments to the call.</param>
    /// <param name="argumentCount">The number of arguments.</param>
    /// <param name="callbackData">Callback data, if any.</param>
    /// <returns>The result of the call, if any.</returns>
    public delegate IntPtr JavaScriptNativeFunction(IntPtr callee, bool isConstructCall, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.SysUInt, SizeParamIndex = 3)] IntPtr[] arguments, ushort argumentCount, IntPtr callbackData);
}
