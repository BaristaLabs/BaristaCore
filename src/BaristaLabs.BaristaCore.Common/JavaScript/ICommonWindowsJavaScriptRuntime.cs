﻿namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

    using System;

    /// <summary>
    /// ChakraCommonWindows.h interface
    /// </summary>
    public interface ICommonWindowsJavaScriptRuntime
	{
		/// <summary>
		///     Parses a script and returns a function representing the script.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="script">The script to parse.</param>
		/// <param name="sourceContext">
		///     A cookie identifying the script that can be used by debuggable script contexts.
		/// </param>
		/// <param name="sourceUrl">The location the script came from.</param>
		/// <param name="result">A function representing the script code.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JavaScriptErrorCode JsParseScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Parses a script and returns a function representing the script.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="script">The script to parse.</param>
		/// <param name="sourceContext">
		///     A cookie identifying the script that can be used by debuggable script contexts.
		/// </param>
		/// <param name="sourceUrl">The location the script came from.</param>
		/// <param name="parseAttributes">Attribute mask for parsing the script</param>
		/// <param name="result">A function representing the script code.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JavaScriptErrorCode JsParseScriptWithAttributes(string script, JavaScriptSourceContext sourceContext, string sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Executes a script.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="script">The script to run.</param>
		/// <param name="sourceContext">
		///     A cookie identifying the script that can be used by debuggable script contexts.
		/// </param>
		/// <param name="sourceUrl">The location the script came from.</param>
		/// <param name="result">The result of the script, if any. This parameter can be null.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JavaScriptErrorCode JsRunScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Executes a module.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="script">The module script to parse and execute.</param>
		/// <param name="sourceContext">
		///     A cookie identifying the script that can be used by debuggable script contexts.
		/// </param>
		/// <param name="sourceUrl">The location the module script came from.</param>
		/// <param name="result">The result of executing the module script, if any. This parameter can be null.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JavaScriptErrorCode JsExperimentalApiRunModule(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Serializes a parsed script to a buffer than can be reused.
		/// </summary>
		/// <remarks>
		///     <para>
		///     <c>JsSerializeScript</c> parses a script and then stores the parsed form of the script in a
		///     runtime-independent format. The serialized script then can be deserialized in any
		///     runtime without requiring the script to be re-parsed.
		///     </para>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="script">The script to serialize.</param>
		/// <param name="buffer">The buffer to put the serialized script into. Can be null.</param>
		/// <param name="bufferSize">
		///     On entry, the size of the buffer, in bytes; on exit, the size of the buffer, in bytes,
		///     required to hold the serialized script.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JavaScriptErrorCode JsSerializeScript(string script, byte[] buffer, ref ulong bufferSize);

		/// <summary>
		///     Parses a serialized script and returns a function representing the script.
		///     Provides the ability to lazy load the script source only if/when it is needed.
		/// </summary>
		/// <remarks>
		///     <para>
		///     Requires an active script context.
		///     </para>
		///     <para>
		///     The runtime will hold on to the buffer until all instances of any functions created from
		///     the buffer are garbage collected.  It will then call scriptUnloadCallback to inform the
		///     caller it is safe to release.
		///     </para>
		/// </remarks>
		/// <param name="scriptLoadCallback">Callback called when the source code of the script needs to be loaded.</param>
		/// <param name="scriptUnloadCallback">Callback called when the serialized script and source code are no longer needed.</param>
		/// <param name="buffer">The serialized script.</param>
		/// <param name="sourceContext">
		///     A cookie identifying the script that can be used by debuggable script contexts.
		///     This context will passed into scriptLoadCallback and scriptUnloadCallback.
		/// </param>
		/// <param name="sourceUrl">The location the script came from.</param>
		/// <param name="result">A function representing the script code.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JavaScriptErrorCode JsParseSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Runs a serialized script.
		///     Provides the ability to lazy load the script source only if/when it is needed.
		/// </summary>
		/// <remarks>
		///     <para>
		///     Requires an active script context.
		///     </para>
		///     <para>
		///     The runtime will hold on to the buffer until all instances of any functions created from
		///     the buffer are garbage collected.  It will then call scriptUnloadCallback to inform the
		///     caller it is safe to release.
		///     </para>
		/// </remarks>
		/// <param name="scriptLoadCallback">Callback called when the source code of the script needs to be loaded.</param>
		/// <param name="scriptUnloadCallback">Callback called when the serialized script and source code are no longer needed.</param>
		/// <param name="buffer">The serialized script.</param>
		/// <param name="sourceContext">
		///     A cookie identifying the script that can be used by debuggable script contexts.
		///     This context will passed into scriptLoadCallback and scriptUnloadCallback.
		/// </param>
		/// <param name="sourceUrl">The location the script came from.</param>
		/// <param name="result">
		///     The result of running the script, if any. This parameter can be null.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JavaScriptErrorCode JsRunSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Parses a serialized script and returns a function representing the script.
		/// </summary>
		/// <remarks>
		///     <para>
		///     Requires an active script context.
		///     </para>
		///     <para>
		///     The runtime will hold on to the buffer until all instances of any functions created from
		///     the buffer are garbage collected.
		///     </para>
		/// </remarks>
		/// <param name="script">The script to parse.</param>
		/// <param name="buffer">The serialized script.</param>
		/// <param name="sourceContext">
		///     A cookie identifying the script that can be used by debuggable script contexts.
		/// </param>
		/// <param name="sourceUrl">The location the script came from.</param>
		/// <param name="result">A function representing the script code.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JavaScriptErrorCode JsParseSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Runs a serialized script.
		/// </summary>
		/// <remarks>
		///     <para>
		///     Requires an active script context.
		///     </para>
		///     <para>
		///     The runtime will hold on to the buffer until all instances of any functions created from
		///     the buffer are garbage collected.
		///     </para>
		/// </remarks>
		/// <param name="script">The source code of the serialized script.</param>
		/// <param name="buffer">The serialized script.</param>
		/// <param name="sourceContext">
		///     A cookie identifying the script that can be used by debuggable script contexts.
		/// </param>
		/// <param name="sourceUrl">The location the script came from.</param>
		/// <param name="result">
		///     The result of running the script, if any. This parameter can be null.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JavaScriptErrorCode JsRunSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Gets the property ID associated with the name.
		/// </summary>
		/// <remarks>
		///     <para>
		///     Property IDs are specific to a context and cannot be used across contexts.
		///     </para>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="name">
		///     The name of the property ID to get or create. The name may consist of only digits.
		/// </param>
		/// <param name="propertyId">The property ID in this runtime for the given name.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JavaScriptErrorCode JsGetPropertyIdFromName(string name, out JavaScriptPropertyIdSafeHandle propertyId);

		/// <summary>
		///     Gets the name associated with the property ID.
		/// </summary>
		/// <remarks>
		///     <para>
		///     Requires an active script context.
		///     </para>
		///     <para>
		///     The returned buffer is valid as long as the runtime is alive and cannot be used
		///     once the runtime has been disposed.
		///     </para>
		/// </remarks>
		/// <param name="propertyId">The property ID to get the name of.</param>
		/// <param name="name">The name associated with the property ID.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JavaScriptErrorCode JsGetPropertyNameFromId(JavaScriptPropertyIdSafeHandle propertyId, out string name);

		/// <summary>
		///     Creates a string value from a string pointer.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="stringValue">The string pointer to convert to a string value.</param>
		/// <param name="stringLength">The length of the string to convert.</param>
		/// <param name="value">The new string value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JavaScriptErrorCode JsPointerToString(string stringValue, ulong stringLength, out JavaScriptValueSafeHandle value);

		/// <summary>
		///     Retrieves the string pointer of a string value.
		/// </summary>
		/// <remarks>
		///     <para>
		///     This function retrieves the string pointer of a string value. It will fail with
		///     <c>JsErrorInvalidArgument</c> if the type of the value is not string. The lifetime
		///     of the string returned will be the same as the lifetime of the value it came from, however
		///     the string pointer is not considered a reference to the value (and so will not keep it
		///     from being collected).
		///     </para>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="value">The string value to convert to a string pointer.</param>
		/// <param name="stringValue">The string pointer.</param>
		/// <param name="stringLength">The length of the string.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JavaScriptErrorCode JsStringToPointer(JavaScriptValueSafeHandle value, out IntPtr stringValue, out ulong stringLength);

	}
}