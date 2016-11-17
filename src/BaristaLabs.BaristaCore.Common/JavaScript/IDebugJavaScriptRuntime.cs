namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

    using System;

    /// <summary>
    /// ChakraDebug.h interface
    /// </summary>
    public interface IDebugJavaScriptRuntime
	{
		/// <summary>
		///     Starts debugging in the given runtime.
		/// </summary>
		/// <param name="runtimeHandle">Runtime to put into debug mode.</param>
		/// <param name="debugEventCallback">Registers a callback to be called on every JsDiagDebugEvent.</param>
		/// <param name="callbackState">User provided state that will be passed back to the callback.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		/// <remarks>
		///     The runtime should be active on the current thread and should not be in debug state.
		/// </remarks>
		JavaScriptErrorCode JsDiagStartDebugging(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagDebugEventCallback debugEventCallback, IntPtr callbackState);

	}
}