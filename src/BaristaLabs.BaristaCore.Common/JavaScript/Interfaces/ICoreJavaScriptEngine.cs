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
        ///   Initialize a ModuleRecord from host
        /// </summary>
        /// <remarks>
        ///     Bootstrap the module loading process by creating a new module record.
        /// </remarks>
        /// <param name="referencingModule">
        ///     The referencingModule as in HostResolveImportedModule (15.2.1.17). nullptr if this is the top level module.
        /// </param>
        /// <param name="normalizedSpecifier">
        ///     The host normalized specifier. This is the key to a unique ModuleRecord.
        /// </param>
        /// <returns>
        ///     The new ModuleRecord created. The host should not try to call this API twice with the same normalizedSpecifier.
        ///     chakra will return an existing ModuleRecord if the specifier was passed in before.
        /// </returns>
        IntPtr JsInitializeModuleRecord(IntPtr referencingModule, JavaScriptValueSafeHandle normalizedSpecifier);

        /// <summary>
        ///   Parse the module source
        /// </summary>
        /// <remarks>
        ///     This is basically ParseModule operation in ES6 spec. It is slightly different in that the ModuleRecord was initialized earlier, and passed in as an argument.
        /// </remarks>
        /// <param name="requestModule">
        ///     The ModuleRecord that holds the parse tree of the source code.
        /// </param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by debuggable script contexts.
        /// </param>
        /// <param name="script">
        ///     The source script to be parsed, but not executed in this code.
        /// </param>
        /// <param name="scriptLength">
        ///     The source length of sourceText. The input might contain embedded null.
        /// </param>
        /// <param name="sourceFlag">
        ///     The type of the source code passed in. It could be UNICODE or utf8 at this time.
        /// </param>
        /// <returns>
        ///     The error object if there is parse error.
        /// </returns>
        JavaScriptValueSafeHandle JsParseModuleSource(IntPtr requestModule, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags sourceFlag);

        /// <summary>
        ///   Execute module code.
        /// </summary>
        /// <remarks>
        ///     This method implements 15.2.1.1.6.5, "ModuleEvaluation" concrete method.
        ///     When this methid is called, the chakra engine should have notified the host that the module and all its dependent are ready to be executed.
        ///     One moduleRecord will be executed only once. Additional execution call on the same moduleRecord will fail.
        /// </remarks>
        /// <param name="requestModule">
        ///     The module to be executed.
        /// </param>
        /// <returns>
        ///     The return value of the module.
        /// </returns>
        JavaScriptValueSafeHandle JsModuleEvaluation(IntPtr requestModule);

        /// <summary>
        ///   Set the host info for the specified module.
        /// </summary>
        /// <param name="requestModule">
        ///     The request module.
        /// </param>
        /// <param name="moduleHostInfo">
        ///     The type of host info to be set.
        /// </param>
        /// <param name="hostInfo">
        ///     The host info to be set.
        /// </param>
        void JsSetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, IntPtr hostInfo);

        /// <summary>
        ///   Retrieve the host info for the specified module.
        /// </summary>
        /// <param name="requestModule">
        ///     The request module.
        /// </param>
        /// <param name="moduleHostInfo">
        ///     The type of host info to get.
        /// </param>
        /// <returns>
        ///     The host info to be retrieved.
        /// </returns>
        IntPtr JsGetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo);

        /// <summary>
        ///   Returns metadata relating to the exception that caused the runtime of the current context to be in the exception state and resets the exception state for that runtime. The metadata includes a reference to the exception itself.
        /// </summary>
        /// <remarks>
        ///     If the runtime of the current context is not in an exception state, this API will return
        ///     JsErrorInvalidArgument. If the runtime is disabled, this will return an exception
        ///     indicating that the script was terminated, but it will not clear the exception (the
        ///     exception will be cleared if the runtime is re-enabled using
        ///     JsEnableRuntimeExecution).
        ///     The metadata value is a javascript object with the following properties: exception, the
        ///     thrown exception object; line, the 0 indexed line number where the exception was thrown;
        ///     column, the 0 indexed column number where the exception was thrown; length, the
        ///     source-length of the cause of the exception; source, a string containing the line of
        ///     source code where the exception was thrown; and url, a string containing the name of
        ///     the script file containing the code that threw the exception.
        ///     Requires an active script context.
        /// </remarks>
        /// <returns>
        ///     The exception metadata for the runtime of the current context.
        /// </returns>
        JavaScriptValueSafeHandle JsGetAndClearExceptionWithMetadata();

        /// <summary>
        ///   Create JavascriptString variable from ASCII or Utf8 string
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        ///     Input string can be either ASCII or Utf8
        /// </remarks>
        /// <param name="content">
        ///     Pointer to string memory.
        /// </param>
        /// <param name="length">
        ///     Number of bytes within the string
        /// </param>
        /// <returns>
        ///     JsValueRef representing the JavascriptString
        /// </returns>
        JavaScriptValueSafeHandle JsCreateString(string content, ulong length);

        /// <summary>
        ///   Create JavascriptString variable from Utf16 string
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        ///     Expects Utf16 string
        /// </remarks>
        /// <param name="content">
        ///     Pointer to string memory.
        /// </param>
        /// <param name="length">
        ///     Number of characters within the string
        /// </param>
        /// <returns>
        ///     JsValueRef representing the JavascriptString
        /// </returns>
        JavaScriptValueSafeHandle JsCreateStringUtf16(string content, ulong length);

        /// <summary>
        ///   Write JavascriptString value into C string buffer (Utf8)
        /// </summary>
        /// <remarks>
        ///     When size of the `buffer` is unknown,
        ///     `buffer` argument can be nullptr.
        ///     In that case, `length` argument will return the length needed.
        /// </remarks>
        /// <param name="value">
        ///     JavascriptString value
        /// </param>
        /// <param name="buffer">
        ///     Pointer to buffer
        /// </param>
        /// <param name="bufferSize">
        ///     Buffer size
        /// </param>
        /// <returns>
        ///     Total number of characters needed or written
        /// </returns>
        ulong JsCopyString(JavaScriptValueSafeHandle value, byte[] buffer, ulong bufferSize);

        /// <summary>
        ///   Write string value into Utf16 string buffer
        /// </summary>
        /// <remarks>
        ///     When size of the `buffer` is unknown,
        ///     `buffer` argument can be nullptr.
        ///     In that case, `written` argument will return the length needed.
        ///     when start is out of range or < 0, returns JsErrorInvalidArgument
        ///     and `written` will be equal to 0.
        ///     If calculated length is 0 (It can be due to string length or `start`
        ///     and length combination), then `written` will be equal to 0 and call
        ///     returns JsNoError
        /// </remarks>
        /// <param name="value">
        ///     JavascriptString value
        /// </param>
        /// <param name="start">
        ///     start offset of buffer
        /// </param>
        /// <param name="length">
        ///     length to be written
        /// </param>
        /// <param name="buffer">
        ///     Pointer to buffer
        /// </param>
        /// <returns>
        ///     Total number of characters written
        /// </returns>
        ulong JsCopyStringUtf16(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer);

        /// <summary>
        ///   Parses a script and returns a function representing the script.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        ///     Script source can be either JavascriptString or JavascriptExternalArrayBuffer.
        ///     In case it is an ExternalArrayBuffer, and the encoding of the buffer is Utf16,
        ///     JsParseScriptAttributeArrayBufferIsUtf16Encoded is expected on parseAttributes.
        ///     Use JavascriptExternalArrayBuffer with Utf8/ASCII script source
        ///     for better performance and smaller memory footprint.
        /// </remarks>
        /// <param name="script">
        ///     The script to run.
        /// </param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by debuggable script contexts.
        /// </param>
        /// <param name="sourceUrl">
        ///     The location the script came from.
        /// </param>
        /// <param name="parseAttributes">
        ///     Attribute mask for parsing the script
        /// </param>
        /// <returns>
        ///     The result of the compiled script.
        /// </returns>
        JavaScriptValueSafeHandle JsParse(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes);

        /// <summary>
        ///   Executes a script.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        ///     Script source can be either JavascriptString or JavascriptExternalArrayBuffer.
        ///     In case it is an ExternalArrayBuffer, and the encoding of the buffer is Utf16,
        ///     JsParseScriptAttributeArrayBufferIsUtf16Encoded is expected on parseAttributes.
        ///     Use JavascriptExternalArrayBuffer with Utf8/ASCII script source
        ///     for better performance and smaller memory footprint.
        /// </remarks>
        /// <param name="script">
        ///     The script to run.
        /// </param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by debuggable script contexts.
        /// </param>
        /// <param name="sourceUrl">
        ///     The location the script came from
        /// </param>
        /// <param name="parseAttributes">
        ///     Attribute mask for parsing the script
        /// </param>
        /// <returns>
        ///     The result of the script, if any. This parameter can be null.
        /// </returns>
        JavaScriptValueSafeHandle JsRun(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes);

        /// <summary>
        ///   Creates the property ID associated with the name.
        /// </summary>
        /// <remarks>
        ///     Property IDs are specific to a context and cannot be used across contexts.
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="name">
        ///     The name of the property ID to get or create. The name may consist of only digits.
        ///     The string is expected to be ASCII / utf8 encoded.
        /// </param>
        /// <param name="length">
        ///     length of the name in bytes
        /// </param>
        /// <returns>
        ///     The property ID in this runtime for the given name.
        /// </returns>
        JavaScriptPropertyIdSafeHandle JsCreatePropertyId(string name, ulong length);

        /// <summary>
        ///   Copies the name associated with the property ID into a buffer.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        ///     When size of the `buffer` is unknown,
        ///     `buffer` argument can be nullptr.
        ///     `length` argument will return the size needed.
        /// </remarks>
        /// <param name="propertyId">
        ///     The property ID to get the name of.
        /// </param>
        /// <param name="buffer">
        ///     The buffer holding the name associated with the property ID, encoded as utf8
        /// </param>
        /// <param name="bufferSize">
        ///     Size of the buffer.
        /// </param>
        /// <returns>
        ///     NO DESCRIPTION PROVIDED
        /// </returns>
        ulong JsCopyPropertyId(JavaScriptPropertyIdSafeHandle propertyId, byte[] buffer, ulong bufferSize);

        /// <summary>
        ///   Serializes a parsed script to a buffer than can be reused.
        /// </summary>
        /// <remarks>
        ///     JsSerializeScript parses a script and then stores the parsed form of the script in a
        ///     runtime-independent format. The serialized script then can be deserialized in any
        ///     runtime without requiring the script to be re-parsed.
        ///     Requires an active script context.
        ///     Script source can be either JavascriptString or JavascriptExternalArrayBuffer.
        ///     In case it is an ExternalArrayBuffer, and the encoding of the buffer is Utf16,
        ///     JsParseScriptAttributeArrayBufferIsUtf16Encoded is expected on parseAttributes.
        ///     Use JavascriptExternalArrayBuffer with Utf8/ASCII script source
        ///     for better performance and smaller memory footprint.
        /// </remarks>
        /// <param name="script">
        ///     The script to serialize
        /// </param>
        /// <param name="parseAttributes">
        ///     Encoding for the script.
        /// </param>
        /// <returns>
        ///     ArrayBuffer
        /// </returns>
        JavaScriptValueSafeHandle JsSerialize(JavaScriptValueSafeHandle script, JavaScriptParseScriptAttributes parseAttributes);

        /// <summary>
        ///   Parses a serialized script and returns a function representing the script. Provides the ability to lazy load the script source only if/when it is needed.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="buffer">
        ///     The serialized script as an ArrayBuffer (preferably ExternalArrayBuffer).
        /// </param>
        /// <param name="scriptLoadCallback">
        ///     Callback called when the source code of the script needs to be loaded.
        ///     This is an optional parameter, set to null if not needed.
        /// </param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by debuggable script contexts.
        ///     This context will passed into scriptLoadCallback.
        /// </param>
        /// <param name="sourceUrl">
        ///     The location the script came from.
        /// </param>
        /// <returns>
        ///     A function representing the script code.
        /// </returns>
        JavaScriptValueSafeHandle JsParseSerialized(JavaScriptValueSafeHandle buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl);

        /// <summary>
        ///   Runs a serialized script. Provides the ability to lazy load the script source only if/when it is needed.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        ///     The runtime will hold on to the buffer until all instances of any functions created from
        ///     the buffer are garbage collected.
        /// </remarks>
        /// <param name="buffer">
        ///     The serialized script as an ArrayBuffer (preferably ExternalArrayBuffer).
        /// </param>
        /// <param name="scriptLoadCallback">
        ///     Callback called when the source code of the script needs to be loaded.
        /// </param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by debuggable script contexts.
        ///     This context will passed into scriptLoadCallback.
        /// </param>
        /// <param name="sourceUrl">
        ///     The location the script came from.
        /// </param>
        /// <returns>
        ///     The result of running the script, if any. This parameter can be null.
        /// </returns>
        JavaScriptValueSafeHandle JsRunSerialized(JavaScriptValueSafeHandle buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl);

        /// <summary>
        ///   Creates a new JavaScript Promise object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="resolveFunction">
        ///     The function called to resolve the created Promise object.
        /// </param>
        /// <param name="rejectFunction">
        ///     The function called to reject the created Promise object.
        /// </param>
        /// <returns>
        ///     The new Promise object.
        /// </returns>
        JavaScriptValueSafeHandle JsCreatePromise(out JavaScriptValueSafeHandle resolveFunction, out JavaScriptValueSafeHandle rejectFunction);

        /// <summary>
        ///   Creates a weak reference to a value.
        /// </summary>
        /// <param name="value">
        ///     The value to be referenced.
        /// </param>
        /// <returns>
        ///     Weak reference to the value.
        /// </returns>
        JavaScriptWeakReferenceSafeHandle JsCreateWeakReference(JavaScriptValueSafeHandle value);

        /// <summary>
        ///   Gets a strong reference to the value referred to by a weak reference.
        /// </summary>
        /// <param name="weakRef">
        ///     A weak reference.
        /// </param>
        /// <returns>
        ///     Reference to the value, or JS_INVALID_REFERENCE if the value is
        ///     no longer available.
        /// </returns>
        JavaScriptValueSafeHandle JsGetWeakReferenceValue(JavaScriptWeakReferenceSafeHandle weakRef);

        /// <summary>
        ///   Creates a Javascript SharedArrayBuffer object with shared content get from JsGetSharedArrayBufferContent.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="sharedContents">
        ///     The storage object of a SharedArrayBuffer which can be shared between multiple thread.
        /// </param>
        /// <returns>
        ///     The new SharedArrayBuffer object.
        /// </returns>
        JavaScriptValueSafeHandle JsCreateSharedArrayBufferWithSharedContent(IntPtr sharedContents);

        /// <summary>
        ///   Get the storage object from a SharedArrayBuffer.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="sharedArrayBuffer">
        ///     The SharedArrayBuffer object.
        /// </param>
        /// <returns>
        ///     The storage object of a SharedArrayBuffer which can be shared between multiple thread.
        ///     User should call JsReleaseSharedArrayBufferContentHandle after finished using it.
        /// </returns>
        IntPtr JsGetSharedArrayBufferContent(JavaScriptValueSafeHandle sharedArrayBuffer);

        /// <summary>
        ///   Decrease the reference count on a SharedArrayBuffer storage object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="sharedContents">
        ///     The storage object of a SharedArrayBuffer which can be shared between multiple thread.
        /// </param>
        void JsReleaseSharedArrayBufferContentHandle(IntPtr sharedContents);

        /// <summary>
        ///   Determines whether an object has a non-inherited property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that may contain the property.
        /// </param>
        /// <param name="propertyId">
        ///     The ID of the property.
        /// </param>
        /// <returns>
        ///     Whether the object has the non-inherited property.
        /// </returns>
        bool JsHasOwnProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId);

        /// <summary>
        ///   Write JS string value into char string buffer without a null terminator
        /// </summary>
        /// <remarks>
        ///     When size of the `buffer` is unknown,
        ///     `buffer` argument can be nullptr.
        ///     In that case, `written` argument will return the length needed.
        ///     When start is out of range or < 0, returns JsErrorInvalidArgument
        ///     and `written` will be equal to 0.
        ///     If calculated length is 0 (It can be due to string length or `start`
        ///     and length combination), then `written` will be equal to 0 and call
        ///     returns JsNoError
        ///     The JS string `value` will be converted one utf16 code point at a time,
        ///     and if it has code points that do not fit in one byte, those values
        ///     will be truncated.
        /// </remarks>
        /// <param name="value">
        ///     JavascriptString value
        /// </param>
        /// <param name="start">
        ///     Start offset of buffer
        /// </param>
        /// <param name="length">
        ///     Number of characters to be written
        /// </param>
        /// <param name="buffer">
        ///     Pointer to buffer
        /// </param>
        /// <returns>
        ///     Total number of characters written
        /// </returns>
        ulong JsCopyStringOneByte(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer);

        /// <summary>
        ///   Obtains frequently used properties of a data view.
        /// </summary>
        /// <param name="dataView">
        ///     The data view instance.
        /// </param>
        /// <param name="byteOffset">
        ///     The offset in bytes from the start of arrayBuffer referenced by the array.
        /// </param>
        /// <param name="byteLength">
        ///     The number of bytes in the array.
        /// </param>
        /// <returns>
        ///     The ArrayBuffer backstore of the view.
        /// </returns>
        JavaScriptValueSafeHandle JsGetDataViewInfo(JavaScriptValueSafeHandle dataView, out uint byteOffset, out uint byteLength);

    }
}
