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

		/// <summary>
		///		Stops debugging in the given runtime.
		/// </summary>
		/// <remarks>
		///		The runtime should be active on the current thread and in debug state.
		/// </remarks>
		/// <param name="runtimeHandle">
		///		Runtime to stop debugging.
		/// </param>
		/// <returns>
		///		User provided state that was passed in JsDiagStartDebugging.
		/// </returns>
		IntPtr JsDiagStopDebugging(JavaScriptRuntimeSafeHandle runtimeHandle);

		/// <summary>
		///		Request the runtime to break on next JavaScript statement.
		/// </summary>
		/// <remarks>
		///		The runtime should be in debug state. This API can be called from another runtime.
		/// </remarks>
		/// <param name="runtimeHandle">
		///		Runtime to request break.
		/// </param>
		void JsDiagRequestAsyncBreak(JavaScriptRuntimeSafeHandle runtimeHandle);

		/// <summary>
		///		List all breakpoints in the current runtime.
		/// </summary>
		/// <remarks>
		///		[{
		///		"breakpointId" : 1,
		///		"scriptId" : 1,
		///		"line" : 0,
		///		"column" : 62
		///		}]
		/// </remarks>
		/// <param name="breakpoints">
		///		Array of breakpoints.
		/// </param>
		void JsDiagGetBreakpoints(JavaScriptValueSafeHandle breakpoints);

		/// <summary>
		///		Sets breakpoint in the specified script at give location.
		/// </summary>
		/// <remarks>
		///		{
		///		"breakpointId" : 1,
		///		"line" : 2,
		///		"column" : 4
		///		}
		/// </remarks>
		/// <param name="scriptId">
		///		Id of script from JsDiagGetScripts or JsDiagGetSource to put breakpoint.
		/// </param>
		/// <param name="lineNumber">
		///		0 based line number to put breakpoint.
		/// </param>
		/// <param name="columnNumber">
		///		0 based column number to put breakpoint.
		/// </param>
		/// <returns>
		///		Breakpoint object with id, line and column if success.
		/// </returns>
		JavaScriptValueSafeHandle JsDiagSetBreakpoint(uint scriptId, uint lineNumber, uint columnNumber);

		/// <summary>
		///		Remove a breakpoint.
		/// </summary>
		/// <remarks>
		///		The current runtime should be in debug state. This API can be called when runtime is at a break or running.
		/// </remarks>
		/// <param name="breakpointId">
		///		Breakpoint id returned from JsDiagSetBreakpoint.
		/// </param>
		void JsDiagRemoveBreakpoint(uint breakpointId);

		/// <summary>
		///		Sets break on exception handling.
		/// </summary>
		/// <remarks>
		///		If this API is not called the default value is set to JsDiagBreakOnExceptionAttributeUncaught in the runtime.
		/// </remarks>
		/// <param name="runtimeHandle">
		///		Runtime to set break on exception attributes.
		/// </param>
		/// <param name="exceptionAttributes">
		///		Mask of JsDiagBreakOnExceptionAttributes to set.
		/// </param>
		void JsDiagSetBreakOnException(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagBreakOnExceptionAttributes exceptionAttributes);

		/// <summary>
		///		Gets break on exception setting.
		/// </summary>
		/// <remarks>
		///		The runtime should be in debug state. This API can be called from another runtime.
		/// </remarks>
		/// <param name="runtimeHandle">
		///		Runtime from which to get break on exception attributes, should be in debug mode.
		/// </param>
		/// <returns>
		///		Mask of JsDiagBreakOnExceptionAttributes.
		/// </returns>
		JavaScriptDiagBreakOnExceptionAttributes JsDiagGetBreakOnException(JavaScriptRuntimeSafeHandle runtimeHandle);

		/// <summary>
		///		Sets the step type in the runtime after a debug break.
		/// </summary>
		/// <remarks>
		///		Requires to be at a debug break.
		/// </remarks>
		/// <param name="stepType">
		///		NO DESCRIPTION PROVIDED
		/// </param>
		void JsDiagSetStepType(JavaScriptDiagStepType stepType);

		/// <summary>
		///		Gets list of scripts.
		/// </summary>
		/// <remarks>
		///		[{
		///		"scriptId" : 2,
		///		"fileName" : "c:\\Test\\Test.js",
		///		"lineCount" : 4,
		///		"sourceLength" : 111
		///		}, {
		///		"scriptId" : 3,
		///		"parentScriptId" : 2,
		///		"scriptType" : "eval code",
		///		"lineCount" : 1,
		///		"sourceLength" : 12
		///		}]
		/// </remarks>
		/// <returns>
		///		Array of script objects.
		/// </returns>
		JavaScriptValueSafeHandle JsDiagGetScripts();

		/// <summary>
		///		Gets source for a specific script identified by scriptId from JsDiagGetScripts.
		/// </summary>
		/// <remarks>
		///		{
		///		"scriptId" : 1,
		///		"fileName" : "c:\\Test\\Test.js",
		///		"lineCount" : 12,
		///		"sourceLength" : 15154,
		///		"source" : "var x = 1;"
		///		}
		/// </remarks>
		/// <param name="scriptId">
		///		Id of the script.
		/// </param>
		/// <returns>
		///		Source object.
		/// </returns>
		JavaScriptValueSafeHandle JsDiagGetSource(uint scriptId);

		/// <summary>
		///		Gets the source information for a function object.
		/// </summary>
		/// <remarks>
		///		{
		///		"scriptId" : 1,
		///		"fileName" : "c:\\Test\\Test.js",
		///		"line" : 1,
		///		"column" : 2,
		///		"firstStatementLine" : 6,
		///		"firstStatementColumn" : 0
		///		}
		/// </remarks>
		/// <param name="function">
		///		JavaScript function.
		/// </param>
		/// <returns>
		///		Function position - scriptId, start line, start column, line number of first statement, column number of first statement.
		/// </returns>
		JavaScriptValueSafeHandle JsDiagGetFunctionPosition(JavaScriptValueSafeHandle function);

		/// <summary>
		///		Gets the stack trace information.
		/// </summary>
		/// <remarks>
		///		[{
		///		"index" : 0,
		///		"scriptId" : 2,
		///		"line" : 3,
		///		"column" : 0,
		///		"sourceLength" : 9,
		///		"sourceText" : "var x = 1",
		///		"functionHandle" : 1
		///		}]
		/// </remarks>
		/// <returns>
		///		NO DESCRIPTION PROVIDED
		/// </returns>
		JavaScriptValueSafeHandle JsDiagGetStackTrace();

		/// <summary>
		///		Gets the list of properties corresponding to the frame.
		/// </summary>
		/// <remarks>
		///		propertyAttributes is a bit mask of
		///		NONE = 0x1,
		///		HAVE_CHILDRENS = 0x2,
		///		READ_ONLY_VALUE = 0x4,
		/// </remarks>
		/// <param name="stackFrameIndex">
		///		Index of stack frame from JsDiagGetStackTrace.
		/// </param>
		/// <returns>
		///		Object of properties array (properties, scopes and globals).
		/// </returns>
		JavaScriptValueSafeHandle JsDiagGetStackProperties(uint stackFrameIndex);

		/// <summary>
		///		Gets the list of children of a handle.
		/// </summary>
		/// <remarks>
		///		Handle should be from objects returned from call to JsDiagGetStackProperties.
		/// </remarks>
		/// <param name="objectHandle">
		///		Handle of object.
		/// </param>
		/// <param name="fromCount">
		///		0-based from count of properties, usually 0.
		/// </param>
		/// <param name="totalCount">
		///		Number of properties to return.
		/// </param>
		/// <returns>
		///		Array of properties.
		/// </returns>
		JavaScriptValueSafeHandle JsDiagGetProperties(uint objectHandle, uint fromCount, uint totalCount);

		/// <summary>
		///		Gets the object corresponding to handle.
		/// </summary>
		/// <remarks>
		///		{
		///		"scriptId" : 24,
		///		"line" : 1,
		///		"column" : 63,
		///		"name" : "foo",
		///		"type" : "function",
		///		"handle" : 2
		///		}
		/// </remarks>
		/// <param name="objectHandle">
		///		Handle of object.
		/// </param>
		/// <returns>
		///		Object corresponding to the handle.
		/// </returns>
		JavaScriptValueSafeHandle JsDiagGetObjectFromHandle(uint objectHandle);

	}
}