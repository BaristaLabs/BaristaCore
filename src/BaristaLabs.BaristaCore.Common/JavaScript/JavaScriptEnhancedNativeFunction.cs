namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     A structure containing information about a native function callback.
    /// </summary>
    public struct JavaScriptNativeFunctionInfo
    {
        public IntPtr thisArg;
        public IntPtr newTargetArg;
        public bool isConstructCall;
    }

    /// <summary>
    ///     A function callback.
    /// </summary>
    /// <param name="callee">
    ///     A function object that represents the function being invoked.
    /// </param>
    /// <param name="arguments">The arguments to the call.</param>
    /// <param name="argumentCount">The number of arguments.</param>
    /// <param name="info">Additional information about this function call.</param>
    /// <param name="callbackState">
    ///     The state passed to <c>JsCreateFunction</c>.
    /// </param>
    /// <returns>The result of the call, if any.</returns>
    public delegate IntPtr JavaScriptEnhancedNativeFunction(IntPtr callee, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.SysUInt, SizeParamIndex = 3)] IntPtr[] arguments, ushort argumentCount, JavaScriptNativeFunctionInfo info, IntPtr callbackState);
}
