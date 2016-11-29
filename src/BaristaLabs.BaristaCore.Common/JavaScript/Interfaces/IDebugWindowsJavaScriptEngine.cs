namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

    using System;
	using System.Runtime.InteropServices;

    /// <summary>
    /// ChakraDebug.h interface
    /// </summary>
    public interface IDebugWindowsJavaScriptEngine
	{
		/// <summary>
		///		Evaluates an expression on given frame.
		/// </summary>
		/// <remarks>
		///		evalResult when evaluating 'this' and return is JsNoError
		///		{
		///		"name" : "this",
		///		"type" : "object",
		///		"className" : "Object",
		///		"display" : "{...}",
		///		"propertyAttributes" : 1,
		///		"handle" : 18
		///		}
		///		evalResult when evaluating a script which throws JavaScript error and return is JsErrorScriptException
		///		{
		///		"name" : "a.b.c",
		///		"type" : "object",
		///		"className" : "Error",
		///		"display" : "'a' is undefined",
		///		"propertyAttributes" : 1,
		///		"handle" : 18
		///		}
		/// </remarks>
		/// <param name="expression">
		///		A null-terminated expression to evaluate.
		/// </param>
		/// <param name="stackFrameIndex">
		///		Index of stack frame on which to evaluate the expression.
		/// </param>
		/// <returns>
		///		Result of evaluation.
		/// </returns>
		JavaScriptValueSafeHandle JsDiagEvaluate(string expression, uint stackFrameIndex);

	}
}