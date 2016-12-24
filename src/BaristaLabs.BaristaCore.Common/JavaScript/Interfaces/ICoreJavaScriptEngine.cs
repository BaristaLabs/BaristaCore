namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

    using System;
	using System.Runtime.InteropServices;

    /// <summary>
    /// ChakraCore.h interface
    /// </summary>
    public interface ICoreJavaScriptEngine
	{
		/// <summary>
		///		Create JavascriptString variable from ASCII or Utf8 string
		/// </summary>
		/// <remarks>
		///		Input string can be either ASCII or Utf8
		/// </remarks>
		/// <param name="content">
		///		Pointer to string memory.
		/// </param>
		/// <param name="length">
		///		Number of bytes within the string
		/// </param>
		/// <returns>
		///		JsValueRef representing the JavascriptString
		/// </returns>
		JavaScriptValueSafeHandle JsCreateString(string content, UIntPtr length);

		/// <summary>
		///		Create JavascriptString variable from Utf8 string
		/// </summary>
		/// <remarks>
		///		Input string can be either ASCII or Utf8
		/// </remarks>
		/// <param name="content">
		///		Pointer to string memory.
		/// </param>
		/// <param name="length">
		///		Number of bytes within the string
		/// </param>
		/// <returns>
		///		JsValueRef representing the JavascriptString
		/// </returns>
		JavaScriptValueSafeHandle JsCreateStringUtf16(string content, UIntPtr length);

		/// <summary>
		///		Write JavascriptString value into C string buffer (Utf8)
		/// </summary>
		/// <remarks>
		///		When size of the `buffer` is unknown,
		///		`buffer` argument can be nullptr.
		///		In that case, `written` argument will return the length needed.
		/// </remarks>
		/// <param name="value">
		///		JavascriptString value
		/// </param>
		/// <param name="buffer">
		///		Pointer to buffer
		/// </param>
		/// <param name="bufferSize">
		///		Buffer size
		/// </param>
		/// <returns>
		///		Total number of characters written
		/// </returns>
		UIntPtr JsCopyString(JavaScriptValueSafeHandle value, byte[] buffer, UIntPtr bufferSize);

		/// <summary>
		///		Write string value into Utf16 string buffer
		/// </summary>
		/// <remarks>
		///		When size of the `buffer` is unknown,
		///		`buffer` argument can be nullptr.
		///		In that case, `written` argument will return the length needed.
		///		when start is out of range or < 0, returns JsErrorInvalidArgument
		///		and `written` will be equal to 0.
		///		If calculated length is 0 (It can be due to string length or `start`
		///		and length combination), then `written` will be equal to 0 and call
		///		returns JsNoError
		/// </remarks>
		/// <param name="value">
		///		JavascriptString value
		/// </param>
		/// <param name="start">
		///		start offset of buffer
		/// </param>
		/// <param name="length">
		///		length to be written
		/// </param>
		/// <param name="buffer">
		///		Pointer to buffer
		/// </param>
		/// <returns>
		///		Total number of characters written
		/// </returns>
		UIntPtr JsCopyStringUtf16(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer);

		/// <summary>
		///		Parses a script and returns a function representing the script.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		///		Script source can be either JavascriptString or JavascriptExternalArrayBuffer.
		///		In case it is an ExternalArrayBuffer, and the encoding of the buffer is Utf16,
		///		JsParseScriptAttributeArrayBufferIsUtf16Encoded is expected on parseAttributes.
		///		Use JavascriptExternalArrayBuffer with Utf8/ASCII script source
		///		for better performance and smaller memory footprint.
		/// </remarks>
		/// <param name="script">
		///		The script to run.
		/// </param>
		/// <param name="sourceContext">
		///		A cookie identifying the script that can be used by debuggable script contexts.
		/// </param>
		/// <param name="sourceUrl">
		///		The location the script came from.
		/// </param>
		/// <param name="parseAttributes">
		///		Attribute mask for parsing the script
		/// </param>
		/// <returns>
		///		The result of the compiled script.
		/// </returns>
		JavaScriptValueSafeHandle JsParse(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes);

		/// <summary>
		///		Executes a script.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		///		Script source can be either JavascriptString or JavascriptExternalArrayBuffer.
		///		In case it is an ExternalArrayBuffer, and the encoding of the buffer is Utf16,
		///		JsParseScriptAttributeArrayBufferIsUtf16Encoded is expected on parseAttributes.
		///		Use JavascriptExternalArrayBuffer with Utf8/ASCII script source
		///		for better performance and smaller memory footprint.
		/// </remarks>
		/// <param name="script">
		///		The script to run.
		/// </param>
		/// <param name="sourceContext">
		///		A cookie identifying the script that can be used by debuggable script contexts.
		/// </param>
		/// <param name="sourceUrl">
		///		The location the script came from
		/// </param>
		/// <param name="parseAttributes">
		///		Attribute mask for parsing the script
		/// </param>
		/// <returns>
		///		The result of the script, if any. This parameter can be null.
		/// </returns>
		JavaScriptValueSafeHandle JsRun(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes);

		/// <summary>
		///		Creates the property ID associated with the name.
		/// </summary>
		/// <remarks>
		///		Property IDs are specific to a context and cannot be used across contexts.
		///		Requires an active script context.
		/// </remarks>
		/// <param name="name">
		///		The name of the property ID to get or create. The name may consist of only digits.
		///		The string is expected to be ASCII / utf8 encoded.
		/// </param>
		/// <param name="length">
		///		length of the name in bytes
		/// </param>
		/// <returns>
		///		The property ID in this runtime for the given name.
		/// </returns>
		JavaScriptPropertyIdSafeHandle JsCreatePropertyId(string name, UIntPtr length);

		/// <summary>
		///		Copies the name associated with the property ID into a buffer.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		///		When size of the `buffer` is unknown,
		///		`buffer` argument can be nullptr.
		///		`length` argument will return the size needed.
		/// </remarks>
		/// <param name="propertyId">
		///		The property ID to get the name of.
		/// </param>
		/// <param name="buffer">
		///		The buffer holding the name associated with the property ID, encoded as utf8
		/// </param>
		/// <param name="bufferSize">
		///		Size of the buffer.
		/// </param>
		/// <returns>
		///		Total number of characters written or to be written
		/// </returns>
		UIntPtr JsCopyPropertyId(JavaScriptPropertyIdSafeHandle propertyId, byte[] buffer, UIntPtr bufferSize);

		/// <summary>
		///		Serializes a parsed script to a buffer than can be reused.
		/// </summary>
		/// <remarks>
		///		JsSerializeScript parses a script and then stores the parsed form of the script in a
		///		runtime-independent format. The serialized script then can be deserialized in any
		///		runtime without requiring the script to be re-parsed.
		///		Requires an active script context.
		///		Script source can be either JavascriptString or JavascriptExternalArrayBuffer.
		///		In case it is an ExternalArrayBuffer, and the encoding of the buffer is Utf16,
		///		JsParseScriptAttributeArrayBufferIsUtf16Encoded is expected on parseAttributes.
		///		Use JavascriptExternalArrayBuffer with Utf8/ASCII script source
		///		for better performance and smaller memory footprint.
		/// </remarks>
		/// <param name="script">
		///		The script to serialize
		/// </param>
		/// <param name="parseAttributes">
		///		Encoding for the script.
		/// </param>
		/// <returns>
		///		The buffer to put the serialized script into. Can be null.
		/// </returns>
		JavaScriptValueSafeHandle JsSerialize(JavaScriptValueSafeHandle script, JavaScriptParseScriptAttributes parseAttributes);

		/// <summary>
		///		Parses a serialized script and returns a function representing the script. Provides the ability to lazy load the script source only if/when it is needed.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="buffer">
		///		The serialized script.
		/// </param>
		/// <param name="scriptLoadCallback">
		///		Callback called when the source code of the script needs to be loaded.
		///		This is an optional parameter, set to null if not needed.
		/// </param>
		/// <param name="sourceContext">
		///		A cookie identifying the script that can be used by debuggable script contexts.
		///		This context will passed into scriptLoadCallback.
		/// </param>
		/// <param name="sourceUrl">
		///		The location the script came from.
		/// </param>
		/// <returns>
		///		A function representing the script code.
		/// </returns>
		JavaScriptValueSafeHandle JsParseSerialized(JavaScriptValueSafeHandle buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl);

		/// <summary>
		///		Runs a serialized script. Provides the ability to lazy load the script source only if/when it is needed.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		///		The runtime will hold on to the buffer until all instances of any functions created from
		///		the buffer are garbage collected.
		/// </remarks>
		/// <param name="buffer">
		///		The serialized script.
		/// </param>
		/// <param name="scriptLoadCallback">
		///		Callback called when the source code of the script needs to be loaded.
		/// </param>
		/// <param name="sourceContext">
		///		A cookie identifying the script that can be used by debuggable script contexts.
		///		This context will passed into scriptLoadCallback.
		/// </param>
		/// <param name="sourceUrl">
		///		The location the script came from.
		/// </param>
		/// <returns>
		///		The result of running the script, if any. This parameter can be null.
		/// </returns>
		JavaScriptValueSafeHandle JsRunSerialized(JavaScriptValueSafeHandle buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl);

	}
}