namespace BaristaLabs.BaristaCore.JavaScript.Interop.Interfaces
{
    using Callbacks;
    using SafeHandles;

    using System;

    /// <summary>
    /// ChakraCore.h interface
    /// </summary>
    internal interface IChakraCore
	{
		/// <summary>
		///     Initialize a ModuleRecord from host
		/// </summary>
		/// <remarks>
		///     Bootstrap the module loading process by creating a new module record.
		/// </remarks>
		/// <param name="referencingModule">The referencingModule as in HostResolveImportedModule (15.2.1.17). nullptr if this is the top level module.</param>
		/// <param name="normalizedSpecifier">The host normalized specifier. This is the key to a unique ModuleRecord.</param>
		/// <param name="moduleRecord">The new ModuleRecord created. The host should not try to call this API twice with the same normalizedSpecifier.
		///                           chakra will return an existing ModuleRecord if the specifier was passed in before.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsInitializeModuleRecord(IntPtr referencingModule, JavaScriptValueSafeHandle normalizedSpecifier, out IntPtr moduleRecord);

		/// <summary>
		///     Parse the module source
		/// </summary>
		/// <remarks>
		/// This is basically ParseModule operation in ES6 spec. It is slightly different in that the ModuleRecord was initialized earlier, and passed in as an argument.
		/// </remarks>
		/// <param name="requestModule">The ModuleRecord that holds the parse tree of the source code.</param>
		/// <param name="sourceContext">A cookie identifying the script that can be used by debuggable script contexts.</param>
		/// <param name="script">The source script to be parsed, but not executed in this code.</param>
		/// <param name="scriptLength">The source length of sourceText. The input might contain embedded null.</param>
		/// <param name="sourceFlag">The type of the source code passed in. It could be UNICODE or utf8 at this time.</param>
		/// <param name="exceptionValueRef">The error object if there is parse error.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsParseModuleSource(IntPtr requestModule, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags sourceFlag, out JavaScriptValueSafeHandle exceptionValueRef);

		/// <summary>
		///     Execute module code.
		/// </summary>
		/// <remarks>
		///     This method implements 15.2.1.1.6.5, "ModuleEvaluation" concrete method.
		///     When this methid is called, the chakra engine should have notified the host that the module and all its dependent are ready to be executed.
		///     One moduleRecord will be executed only once. Additional execution call on the same moduleRecord will fail.
		/// </remarks>
		/// <param name="requestModule">The module to be executed.</param>
		/// <param name="result">The return value of the module.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsModuleEvaluation(IntPtr requestModule, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Set the host info for the specified module.
		/// </summary>
		/// <param name="requestModule">The request module.</param>
		/// <param name="moduleHostInfo">The type of host info to be set.</param>
		/// <param name="hostInfo">The host info to be set.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsSetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, IntPtr hostInfo);

		/// <summary>
		///     Retrieve the host info for the specified module.
		/// </summary>
		/// <param name="requestModule">The request module.</param>
		/// <param name="moduleHostInfo">The type of host info to get.</param>
		/// <param name="hostInfo">The host info to be retrieved.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsGetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, out IntPtr hostInfo);

		/// <summary>
		///     Create JavascriptString variable from C string
		/// </summary>
		/// <remarks>
		///     <para>
		///         C string is expected to be ASCII
		///     </para>
		/// </remarks>
		/// <param name="content">Pointer to string memory.</param>
		/// <param name="length">Number of bytes within the string</param>
		/// <param name="value">JsValueRef representing the JavascriptString</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsCreateString(string content, UIntPtr length, out JavaScriptValueSafeHandle value);

		/// <summary>
		///     Create JavascriptString variable from Utf8 string
		/// </summary>
		/// <remarks>
		///     <para>
		///         Input string can be either ASCII or Utf8
		///     </para>
		/// </remarks>
		/// <param name="content">Pointer to string memory.</param>
		/// <param name="length">Number of bytes within the string</param>
		/// <param name="value">JsValueRef representing the JavascriptString</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsCreateStringUtf8(string content, UIntPtr length, out JavaScriptValueSafeHandle value);

		/// <summary>
		///     Create JavascriptString variable from Utf8 string
		/// </summary>
		/// <remarks>
		///     <para>
		///         Input string can be either ASCII or Utf8
		///     </para>
		/// </remarks>
		/// <param name="content">Pointer to string memory.</param>
		/// <param name="length">Number of bytes within the string</param>
		/// <param name="value">JsValueRef representing the JavascriptString</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsCreateStringUtf16(string content, UIntPtr length, out JavaScriptValueSafeHandle value);

		/// <summary>
		///     Write JavascriptString value into C string buffer
		/// </summary>
		/// <remarks>
		///     <para>
		///         When size of the `buffer` is unknown,
		///         `buffer` argument can be nullptr.
		///         In that case, `written` argument will return the length needed.
		///     </para>
		///     <para>
		///         when start is out of range or < 0, returns JsErrorInvalidArgument
		///         and `written` will be equal to 0.
		///         If calculated length is 0 (It can be due to string length or `start`
		///         and length combination), then `written` will be equal to 0 and call
		///         returns JsNoError
		///     </para>
		/// </remarks>
		/// <param name="value">JavascriptString value</param>
		/// <param name="start">start offset of buffer</param>
		/// <param name="length">length to be written</param>
		/// <param name="buffer">Pointer to buffer</param>
		/// <param name="written">Total number of characters written</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsCopyString(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out UIntPtr written);

		/// <summary>
		///     Write JavascriptString value into Utf8 string buffer
		/// </summary>
		/// <remarks>
		///     <para>
		///         When size of the `buffer` is unknown,
		///         `buffer` argument can be nullptr.
		///         In that case, `written` argument will return the length needed.
		///     </para>
		/// </remarks>
		/// <param name="value">JavascriptString value</param>
		/// <param name="buffer">Pointer to buffer</param>
		/// <param name="bufferSize">Buffer size</param>
		/// <param name="written">Total number of characters written</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsCopyStringUtf8(JavaScriptValueSafeHandle value, byte[] buffer, UIntPtr bufferSize, out UIntPtr written);

		/// <summary>
		///     Write string value into Utf16 string buffer
		/// </summary>
		/// <remarks>
		///     <para>
		///         When size of the `buffer` is unknown,
		///         `buffer` argument can be nullptr.
		///         In that case, `written` argument will return the length needed.
		///     </para>
		///     <para>
		///         when start is out of range or < 0, returns JsErrorInvalidArgument
		///         and `written` will be equal to 0.
		///         If calculated length is 0 (It can be due to string length or `start`
		///         and length combination), then `written` will be equal to 0 and call
		///         returns JsNoError
		///     </para>
		/// </remarks>
		/// <param name="value">JavascriptString value</param>
		/// <param name="start">start offset of buffer</param>
		/// <param name="length">length to be written</param>
		/// <param name="buffer">Pointer to buffer</param>
		/// <param name="written">Total number of characters written</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsCopyStringUtf16(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out UIntPtr written);

		/// <summary>
		///     Parses a script and returns a function representing the script.
		/// </summary>
		/// <remarks>
		///     <para>
		///        Requires an active script context.
		///     </para>
		///     <para>
		///         Script source can be either JavascriptString or JavascriptExternalArrayBuffer.
		///         In case it is an ExternalArrayBuffer, and the encoding of the buffer is Utf16,
		///         JsParseScriptAttributeArrayBufferIsUtf16Encoded is expected on parseAttributes.
		///     </para>
		///     <para>
		///         Use JavascriptExternalArrayBuffer with Utf8/ASCII script source
		///         for better performance and smaller memory footprint.
		///     </para>
		/// </remarks>
		/// <param name="script">The script to run.</param>
		/// <param name="sourceContext">
		///     A cookie identifying the script that can be used by debuggable script contexts.
		/// </param>
		/// <param name="sourceUrl">The location the script came from.</param>
		/// <param name="parseAttributes">Attribute mask for parsing the script</param>
		/// <param name="result">The result of the compiled script.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsParse(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JsParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Executes a script.
		/// </summary>
		/// <remarks>
		///     <para>
		///        Requires an active script context.
		///     </para>
		///     <para>
		///         Script source can be either JavascriptString or JavascriptExternalArrayBuffer.
		///         In case it is an ExternalArrayBuffer, and the encoding of the buffer is Utf16,
		///         JsParseScriptAttributeArrayBufferIsUtf16Encoded is expected on parseAttributes.
		///     </para>
		///     <para>
		///         Use JavascriptExternalArrayBuffer with Utf8/ASCII script source
		///         for better performance and smaller memory footprint.
		///     </para>
		/// </remarks>
		/// <param name="script">The script to run.</param>
		/// <param name="sourceContext">
		///     A cookie identifying the script that can be used by debuggable script contexts.
		/// </param>
		/// <param name="sourceUrl">The location the script came from</param>
		/// <param name="parseAttributes">Attribute mask for parsing the script</param>
		/// <param name="result">The result of the script, if any. This parameter can be null.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsRun(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JsParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Creates the property ID associated with the name.
		/// </summary>
		/// <remarks>
		///     <para>
		///         Property IDs are specific to a context and cannot be used across contexts.
		///     </para>
		///     <para>
		///         Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="name">
		///     The name of the property ID to get or create. The name may consist of only digits.
		///     The string is expected to be ASCII / utf8 encoded.
		/// </param>
		/// <param name="length">length of the name in bytes</param>
		/// <param name="propertyId">The property ID in this runtime for the given name.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsCreatePropertyIdUtf8(string name, UIntPtr length, out JavaScriptPropertyIdSafeHandle propertyId);

		/// <summary>
		///     Copies the name associated with the property ID into a buffer.
		/// </summary>
		/// <remarks>
		///     <para>
		///         Requires an active script context.
		///     </para>
		///     <para>
		///         When size of the `buffer` is unknown,
		///         `buffer` argument can be nullptr.
		///         `length` argument will return the size needed.
		///     </para>
		/// </remarks>
		/// <param name="propertyId">The property ID to get the name of.</param>
		/// <param name="buffer">The buffer holding the name associated with the property ID, encoded as utf8</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		/// <param name="written">Total number of characters written or to be written</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsCopyPropertyIdUtf8(JavaScriptPropertyIdSafeHandle propertyId, byte[] buffer, UIntPtr bufferSize, out UIntPtr length);

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
		///     <para>
		///         Script source can be either JavascriptString or JavascriptExternalArrayBuffer.
		///         In case it is an ExternalArrayBuffer, and the encoding of the buffer is Utf16,
		///         JsParseScriptAttributeArrayBufferIsUtf16Encoded is expected on parseAttributes.
		///     </para>
		///     <para>
		///         Use JavascriptExternalArrayBuffer with Utf8/ASCII script source
		///         for better performance and smaller memory footprint.
		///     </para>
		/// </remarks>
		/// <param name="script">The script to serialize</param>
		/// <param name="buffer">The buffer to put the serialized script into. Can be null.</param>
		/// <param name="bufferSize">
		///     On entry, the size of the buffer, in bytes; on exit, the size of the buffer, in bytes,
		///     required to hold the serialized script.
		/// </param>
		/// <param name="parseAttributes">Encoding for the script.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsSerialize(JavaScriptValueSafeHandle script, byte[] buffer, ref ulong bufferSize, JsParseScriptAttributes parseAttributes);

		/// <summary>
		///     Parses a serialized script and returns a function representing the script.
		///     Provides the ability to lazy load the script source only if/when it is needed.
		/// </summary>
		/// <remarks>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="buffer">The serialized script.</param>
		/// <param name="scriptLoadCallback">
		///     Callback called when the source code of the script needs to be loaded.
		///     This is an optional parameter, set to null if not needed.
		/// </param>
		/// <param name="sourceContext">
		///     A cookie identifying the script that can be used by debuggable script contexts.
		///     This context will passed into scriptLoadCallback.
		/// </param>
		/// <param name="sourceUrl">The location the script came from.</param>
		/// <param name="result">A function representing the script code.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsParseSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result);

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
		///     the buffer are garbage collected.
		///     </para>
		/// </remarks>
		/// <param name="buffer">The serialized script.</param>
		/// <param name="scriptLoadCallback">Callback called when the source code of the script needs to be loaded.</param>
		/// <param name="sourceContext">
		///     A cookie identifying the script that can be used by debuggable script contexts.
		///     This context will passed into scriptLoadCallback.
		/// </param>
		/// <param name="sourceUrl">The location the script came from.</param>
		/// <param name="result">
		///     The result of running the script, if any. This parameter can be null.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsRunSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result);

	}
}