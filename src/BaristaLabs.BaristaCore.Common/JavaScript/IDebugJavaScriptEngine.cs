namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

    using System;
	using System.Runtime.InteropServices;

    /// <summary>
    /// ChakraDebug.h interface
    /// </summary>
    public interface IDebugJavaScriptEngine
	{
		/// <summary>
		///		Starts debugging in the given runtime.
		/// </summary>
		/// <remarks>
		///		The runtime should be active on the current thread and should not be in debug state.
		/// </remarks>
		/// <param name="runtimeHandle">
		///		Runtime to put into debug mode.
		/// </param>
		/// <param name="debugEventCallback">
		///		Registers a callback to be called on every JsDiagDebugEvent.
		/// </param>
		/// <param name="callbackState">
		///		User provided state that will be passed back to the callback.
		/// </param>
		void JsDiagStartDebugging(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagDebugEventCallback debugEventCallback, IntPtr callbackState);

	}
}