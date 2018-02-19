namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///	Represents the Unmanaged ChakraCore Library
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCode]
    public static class LibChakraCore
    {
        private const string DllName = "libChakraCore";

        /// <summary>
        ///     Initialize a ModuleRecord from host
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
        /// <param name="moduleRecord">
        ///     The new ModuleRecord created. The host should not try to call this API twice with the same normalizedSpecifier.
        ///     chakra will return an existing ModuleRecord if the specifier was passed in before.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsInitializeModuleRecord", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsInitializeModuleRecord(JavaScriptModuleRecord referencingModule, JavaScriptValueSafeHandle normalizedSpecifier, out JavaScriptModuleRecord moduleRecord);

        /// <summary>
        ///     Parse the module source
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
        /// <param name="exceptionValueRef">
        ///     The error object if there is parse error.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsParseModuleSource", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsParseModuleSource(JavaScriptModuleRecord requestModule, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags sourceFlag, out JavaScriptValueSafeHandle exceptionValueRef);

        /// <summary>
        ///     Execute module code.
        /// </summary>
        /// <remarks>
        ///     This method implements 15.2.1.1.6.5, "ModuleEvaluation" concrete method.
        ///     When this methid is called, the chakra engine should have notified the host that the module and all its dependent are ready to be executed.
        ///     One moduleRecord will be executed only once. Additional execution call on the same moduleRecord will fail.
        /// </remarks>
        /// <param name="requestModule">
        ///     The module to be executed.
        /// </param>
        /// <param name="result">
        ///     The return value of the module.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsModuleEvaluation", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsModuleEvaluation(JavaScriptModuleRecord requestModule, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Set the host info for the specified module.
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
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetModuleHostInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetModuleHostInfo(JavaScriptModuleRecord requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, IntPtr hostInfo);

        /// <summary>
        ///     Retrieve the host info for the specified module.
        /// </summary>
        /// <param name="requestModule">
        ///     The request module.
        /// </param>
        /// <param name="moduleHostInfo">
        ///     The type of host info to get.
        /// </param>
        /// <param name="hostInfo">
        ///     The host info to be retrieved.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetModuleHostInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetModuleHostInfo(JavaScriptModuleRecord requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, out IntPtr hostInfo);

        /// <summary>
        ///     Returns metadata relating to the exception that caused the runtime of the current context to be in the exception state and resets the exception state for that runtime. The metadata includes a reference to the exception itself.
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
        /// <param name="metadata">
        ///     The exception metadata for the runtime of the current context.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetAndClearExceptionWithMetadata", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetAndClearExceptionWithMetadata(out JavaScriptValueSafeHandle metadata);

        /// <summary>
        ///     Create JavascriptString variable from ASCII or Utf8 string
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
        /// <param name="value">
        ///     JsValueRef representing the JavascriptString
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateString", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateString(string content, ulong length, out JavaScriptValueSafeHandle value);

        /// <summary>
        ///     Create JavascriptString variable from Utf16 string
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
        /// <param name="value">
        ///     JsValueRef representing the JavascriptString
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateStringUtf16", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsCreateStringUtf16(string content, ulong length, out JavaScriptValueSafeHandle value);

        /// <summary>
        ///     Write JavascriptString value into C string buffer (Utf8)
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
        /// <param name="length">
        ///     Total number of characters needed or written
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCopyString", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern JsErrorCode JsCopyString(JavaScriptValueSafeHandle value, byte[] buffer, ulong bufferSize, out ulong length);

        /// <summary>
        ///     Write string value into Utf16 string buffer
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
        /// <param name="written">
        ///     Total number of characters written
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCopyStringUtf16", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCopyStringUtf16(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out ulong written);

        /// <summary>
        ///     Parses a script and returns a function representing the script.
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
        /// <param name="result">
        ///     The result of the compiled script.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsParse", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsParse(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Executes a script.
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
        /// <param name="result">
        ///     The result of the script, if any. This parameter can be null.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsRun", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsRun(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Creates the property ID associated with the name.
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
        /// <param name="propertyId">
        ///     The property ID in this runtime for the given name.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreatePropertyId", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreatePropertyId(string name, ulong length, out JavaScriptPropertyIdSafeHandle propertyId);

        /// <summary>
        ///     Copies the name associated with the property ID into a buffer.
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
        /// <param name="length">
        ///     NO DESCRIPTION PROVIDED
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCopyPropertyId", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCopyPropertyId(JavaScriptPropertyIdSafeHandle propertyId, byte[] buffer, ulong bufferSize, out ulong length);

        /// <summary>
        ///     Serializes a parsed script to a buffer than can be reused.
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
        /// <param name="buffer">
        ///     ArrayBuffer
        /// </param>
        /// <param name="parseAttributes">
        ///     Encoding for the script.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSerialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSerialize(JavaScriptValueSafeHandle script, out JavaScriptValueSafeHandle buffer, JavaScriptParseScriptAttributes parseAttributes);

        /// <summary>
        ///     Parses a serialized script and returns a function representing the script. Provides the ability to lazy load the script source only if/when it is needed.
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
        /// <param name="result">
        ///     A function representing the script code.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsParseSerialized", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsParseSerialized(JavaScriptValueSafeHandle buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Runs a serialized script. Provides the ability to lazy load the script source only if/when it is needed.
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
        /// <param name="result">
        ///     The result of running the script, if any. This parameter can be null.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsRunSerialized", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsRunSerialized(JavaScriptValueSafeHandle buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Creates a new JavaScript Promise object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="promise">
        ///     The new Promise object.
        /// </param>
        /// <param name="resolveFunction">
        ///     The function called to resolve the created Promise object.
        /// </param>
        /// <param name="rejectFunction">
        ///     The function called to reject the created Promise object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreatePromise", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreatePromise(out JavaScriptValueSafeHandle promise, out JavaScriptValueSafeHandle resolveFunction, out JavaScriptValueSafeHandle rejectFunction);

        /// <summary>
        ///     Creates a weak reference to a value.
        /// </summary>
        /// <param name="value">
        ///     The value to be referenced.
        /// </param>
        /// <param name="weakRef">
        ///     Weak reference to the value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateWeakReference", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateWeakReference(JavaScriptValueSafeHandle value, out JavaScriptWeakReferenceSafeHandle weakRef);

        /// <summary>
        ///     Gets a strong reference to the value referred to by a weak reference.
        /// </summary>
        /// <param name="weakRef">
        ///     A weak reference.
        /// </param>
        /// <param name="value">
        ///     Reference to the value, or JS_INVALID_REFERENCE if the value is
        ///     no longer available.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetWeakReferenceValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetWeakReferenceValue(JavaScriptWeakReferenceSafeHandle weakRef, out JavaScriptValueSafeHandle value);

        /// <summary>
        ///     Creates a Javascript SharedArrayBuffer object with shared content get from JsGetSharedArrayBufferContent.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="sharedContents">
        ///     The storage object of a SharedArrayBuffer which can be shared between multiple thread.
        /// </param>
        /// <param name="result">
        ///     The new SharedArrayBuffer object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateSharedArrayBufferWithSharedContent", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateSharedArrayBufferWithSharedContent(IntPtr sharedContents, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Get the storage object from a SharedArrayBuffer.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="sharedArrayBuffer">
        ///     The SharedArrayBuffer object.
        /// </param>
        /// <param name="sharedContents">
        ///     The storage object of a SharedArrayBuffer which can be shared between multiple thread.
        ///     User should call JsReleaseSharedArrayBufferContentHandle after finished using it.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetSharedArrayBufferContent", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetSharedArrayBufferContent(JavaScriptValueSafeHandle sharedArrayBuffer, out IntPtr sharedContents);

        /// <summary>
        ///     Decrease the reference count on a SharedArrayBuffer storage object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="sharedContents">
        ///     The storage object of a SharedArrayBuffer which can be shared between multiple thread.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsReleaseSharedArrayBufferContentHandle", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsReleaseSharedArrayBufferContentHandle(IntPtr sharedContents);

        /// <summary>
        ///     Determines whether an object has a non-inherited property.
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
        /// <param name="hasOwnProperty">
        ///     Whether the object has the non-inherited property.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsHasOwnProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsHasOwnProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, out bool hasOwnProperty);

        /// <summary>
        ///     Write JS string value into char string buffer without a null terminator
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
        /// <param name="written">
        ///     Total number of characters written
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCopyStringOneByte", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCopyStringOneByte(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out ulong written);

        /// <summary>
        ///     Obtains frequently used properties of a data view.
        /// </summary>
        /// <param name="dataView">
        ///     The data view instance.
        /// </param>
        /// <param name="arrayBuffer">
        ///     The ArrayBuffer backstore of the view.
        /// </param>
        /// <param name="byteOffset">
        ///     The offset in bytes from the start of arrayBuffer referenced by the array.
        /// </param>
        /// <param name="byteLength">
        ///     The number of bytes in the array.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetDataViewInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetDataViewInfo(JavaScriptValueSafeHandle dataView, out JavaScriptValueSafeHandle arrayBuffer, out uint byteOffset, out uint byteLength);

        /// <summary>
        ///     Determine if one JavaScript value is less than another JavaScript value.
        /// </summary>
        /// <remarks>
        ///     This function is equivalent to the < operator in Javascript.
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="object1">
        ///     The first object to compare.
        /// </param>
        /// <param name="object2">
        ///     The second object to compare.
        /// </param>
        /// <param name="result">
        ///     Whether object1 is less than object2.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsLessThan", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsLessThan(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result);

        /// <summary>
        ///     Determine if one JavaScript value is less than or equal to another JavaScript value.
        /// </summary>
        /// <remarks>
        ///     This function is equivalent to the <= operator in Javascript.
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="object1">
        ///     The first object to compare.
        /// </param>
        /// <param name="object2">
        ///     The second object to compare.
        /// </param>
        /// <param name="result">
        ///     Whether object1 is less than or equal to object2.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsLessThanOrEqual", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsLessThanOrEqual(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result);

        /// <summary>
        ///     Gets an object's property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that contains the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString) to the property.
        /// </param>
        /// <param name="value">
        ///     The value of the property.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsObjectGetProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsObjectGetProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key, out JavaScriptValueSafeHandle value);

        /// <summary>
        ///     Puts an object's property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that contains the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString) to the property.
        /// </param>
        /// <param name="value">
        ///     The new value of the property.
        /// </param>
        /// <param name="useStrictRules">
        ///     The property set should follow strict mode rules.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsObjectSetProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsObjectSetProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key, JavaScriptValueSafeHandle value, bool useStrictRules);

        /// <summary>
        ///     Determines whether an object has a property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that may contain the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString) to the property.
        /// </param>
        /// <param name="hasProperty">
        ///     Whether the object (or a prototype) has the property.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsObjectHasProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsObjectHasProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key, out bool hasProperty);

        /// <summary>
        ///     Defines a new object's own property from a property descriptor.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that has the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString) to the property.
        /// </param>
        /// <param name="propertyDescriptor">
        ///     The property descriptor.
        /// </param>
        /// <param name="result">
        ///     Whether the property was defined.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsObjectDefineProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsObjectDefineProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key, JavaScriptValueSafeHandle propertyDescriptor, out bool result);

        /// <summary>
        ///     Deletes an object's property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that contains the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString) to the property.
        /// </param>
        /// <param name="useStrictRules">
        ///     The property set should follow strict mode rules.
        /// </param>
        /// <param name="result">
        ///     Whether the property was deleted.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsObjectDeleteProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsObjectDeleteProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key, bool useStrictRules, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Gets a property descriptor for an object's own property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that has the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString) to the property.
        /// </param>
        /// <param name="propertyDescriptor">
        ///     The property descriptor.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsObjectGetOwnPropertyDescriptor", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsObjectGetOwnPropertyDescriptor(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key, out JavaScriptValueSafeHandle propertyDescriptor);

        /// <summary>
        ///     Determines whether an object has a non-inherited property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that may contain the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString) to the property.
        /// </param>
        /// <param name="hasOwnProperty">
        ///     Whether the object has the non-inherited property.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsObjectHasOwnProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsObjectHasOwnProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key, out bool hasOwnProperty);

        /// <summary>
        ///     Creates a new runtime.
        /// </summary>
        /// <remarks>
        ///     In the edge-mode binary, chakra.dll, this function lacks the runtimeVersion
        ///     parameter (compare to jsrt9.h).
        /// </remarks>
        /// <param name="attributes">
        ///     The attributes of the runtime to be created.
        /// </param>
        /// <param name="threadService">
        ///     The thread service for the runtime. Can be null.
        /// </param>
        /// <param name="runtime">
        ///     The runtime created.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateRuntime", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateRuntime(JavaScriptRuntimeAttributes attributes, JavaScriptThreadServiceCallback threadService, out JavaScriptRuntimeSafeHandle runtime);

        /// <summary>
        ///     Performs a full garbage collection.
        /// </summary>
        /// <param name="runtime">
        ///     The runtime in which the garbage collection will be performed.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCollectGarbage", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCollectGarbage(JavaScriptRuntimeSafeHandle runtime);

        /// <summary>
        ///     Disposes a runtime.
        /// </summary>
        /// <remarks>
        ///     Once a runtime has been disposed, all resources owned by it are invalid and cannot be used.
        ///     If the runtime is active (i.e. it is set to be current on a particular thread), it cannot
        ///     be disposed.
        /// </remarks>
        /// <param name="runtime">
        ///     The runtime to dispose.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDisposeRuntime", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDisposeRuntime(IntPtr runtime);

        /// <summary>
        ///     Gets the current memory usage for a runtime.
        /// </summary>
        /// <remarks>
        ///     Memory usage can be always be retrieved, regardless of whether or not the runtime is active
        ///     on another thread.
        /// </remarks>
        /// <param name="runtime">
        ///     The runtime whose memory usage is to be retrieved.
        /// </param>
        /// <param name="memoryUsage">
        ///     The runtime's current memory usage, in bytes.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetRuntimeMemoryUsage", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetRuntimeMemoryUsage(JavaScriptRuntimeSafeHandle runtime, out ulong memoryUsage);

        /// <summary>
        ///     Gets the current memory limit for a runtime.
        /// </summary>
        /// <remarks>
        ///     The memory limit of a runtime can be always be retrieved, regardless of whether or not the
        ///     runtime is active on another thread.
        /// </remarks>
        /// <param name="runtime">
        ///     The runtime whose memory limit is to be retrieved.
        /// </param>
        /// <param name="memoryLimit">
        ///     The runtime's current memory limit, in bytes, or -1 if no limit has been set.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetRuntimeMemoryLimit", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, out ulong memoryLimit);

        /// <summary>
        ///     Sets the current memory limit for a runtime.
        /// </summary>
        /// <remarks>
        ///     A memory limit will cause any operation which exceeds the limit to fail with an "out of
        ///     memory" error. Setting a runtime's memory limit to -1 means that the runtime has no memory
        ///     limit. New runtimes  default to having no memory limit. If the new memory limit exceeds
        ///     current usage, the call will succeed and any future allocations in this runtime will fail
        ///     until the runtime's memory usage drops below the limit.
        ///     A runtime's memory limit can be always be set, regardless of whether or not the runtime is
        ///     active on another thread.
        /// </remarks>
        /// <param name="runtime">
        ///     The runtime whose memory limit is to be set.
        /// </param>
        /// <param name="memoryLimit">
        ///     The new runtime memory limit, in bytes, or -1 for no memory limit.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetRuntimeMemoryLimit", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, ulong memoryLimit);

        /// <summary>
        ///     Sets a memory allocation callback for specified runtime
        /// </summary>
        /// <remarks>
        ///     Registering a memory allocation callback will cause the runtime to call back to the host
        ///     whenever it acquires memory from, or releases memory to, the OS. The callback routine is
        ///     called before the runtime memory manager allocates a block of memory. The allocation will
        ///     be rejected if the callback returns false. The runtime memory manager will also invoke the
        ///     callback routine after freeing a block of memory, as well as after allocation failures.
        ///     The callback is invoked on the current runtime execution thread, therefore execution is
        ///     blocked until the callback completes.
        ///     The return value of the callback is not stored; previously rejected allocations will not
        ///     prevent the runtime from invoking the callback again later for new memory allocations.
        /// </remarks>
        /// <param name="runtime">
        ///     The runtime for which to register the allocation callback.
        /// </param>
        /// <param name="callbackState">
        ///     User provided state that will be passed back to the callback.
        /// </param>
        /// <param name="allocationCallback">
        ///     Memory allocation callback to be called for memory allocation events.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetRuntimeMemoryAllocationCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetRuntimeMemoryAllocationCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptMemoryAllocationCallback allocationCallback);

        /// <summary>
        ///     Sets a callback function that is called by the runtime before garbage collection.
        /// </summary>
        /// <remarks>
        ///     The callback is invoked on the current runtime execution thread, therefore execution is
        ///     blocked until the callback completes.
        ///     The callback can be used by hosts to prepare for garbage collection. For example, by
        ///     releasing unnecessary references on Chakra objects.
        /// </remarks>
        /// <param name="runtime">
        ///     The runtime for which to register the allocation callback.
        /// </param>
        /// <param name="callbackState">
        ///     User provided state that will be passed back to the callback.
        /// </param>
        /// <param name="beforeCollectCallback">
        ///     The callback function being set.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetRuntimeBeforeCollectCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetRuntimeBeforeCollectCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptBeforeCollectCallback beforeCollectCallback);

        /// <summary>
        ///     Adds a reference to a garbage collected object.
        /// </summary>
        /// <remarks>
        ///     This only needs to be called on JsRef handles that are not going to be stored
        ///     somewhere on the stack. Calling JsAddRef ensures that the object the JsRef
        ///     refers to will not be freed until JsRelease is called.
        /// </remarks>
        /// <param name="@ref">
        ///     The object to add a reference to.
        /// </param>
        /// <param name="count">
        ///     The object's new reference count (can pass in null).
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsAddRef", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsAddRef(SafeHandle @ref, out uint count);

        /// <summary>
        ///     Releases a reference to a garbage collected object.
        /// </summary>
        /// <remarks>
        ///     Removes a reference to a JsRef handle that was created by JsAddRef.
        /// </remarks>
        /// <param name="@ref">
        ///     The object to add a reference to.
        /// </param>
        /// <param name="count">
        ///     The object's new reference count (can pass in null).
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsRelease", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsRelease(SafeHandle @ref, out uint count);

        /// <summary>
        ///     Sets a callback function that is called by the runtime before garbage collection of an object.
        /// </summary>
        /// <remarks>
        ///     The callback is invoked on the current runtime execution thread, therefore execution is
        ///     blocked until the callback completes.
        /// </remarks>
        /// <param name="@ref">
        ///     The object for which to register the callback.
        /// </param>
        /// <param name="callbackState">
        ///     User provided state that will be passed back to the callback.
        /// </param>
        /// <param name="objectBeforeCollectCallback">
        ///     The callback function being set. Use null to clear
        ///     previously registered callback.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetObjectBeforeCollectCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetObjectBeforeCollectCallback(SafeHandle @ref, IntPtr callbackState, JavaScriptObjectBeforeCollectCallback objectBeforeCollectCallback);

        /// <summary>
        ///     Creates a script context for running scripts.
        /// </summary>
        /// <remarks>
        ///     Each script context has its own global object that is isolated from all other script
        ///     contexts.
        /// </remarks>
        /// <param name="runtime">
        ///     The runtime the script context is being created in.
        /// </param>
        /// <param name="newContext">
        ///     The created script context.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateContext", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateContext(JavaScriptRuntimeSafeHandle runtime, out JavaScriptContextSafeHandle newContext);

        /// <summary>
        ///     Gets the current script context on the thread.
        /// </summary>
        /// <param name="currentContext">
        ///     The current script context on the thread, null if there is no current script context.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetCurrentContext", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetCurrentContext(out JavaScriptContextSafeHandle currentContext);

        /// <summary>
        ///     Sets the current script context on the thread.
        /// </summary>
        /// <param name="context">
        ///     The script context to make current.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetCurrentContext", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetCurrentContext(JavaScriptContextSafeHandle context);

        /// <summary>
        ///     Gets the script context that the object belongs to.
        /// </summary>
        /// <param name="@object">
        ///     The object to get the context from.
        /// </param>
        /// <param name="context">
        ///     The context the object belongs to.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetContextOfObject", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetContextOfObject(JavaScriptValueSafeHandle @object, out JavaScriptContextSafeHandle context);

        /// <summary>
        ///     Gets the internal data set on JsrtContext.
        /// </summary>
        /// <param name="context">
        ///     The context to get the data from.
        /// </param>
        /// <param name="data">
        ///     The pointer to the data where data will be returned.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetContextData", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetContextData(JavaScriptContextSafeHandle context, out IntPtr data);

        /// <summary>
        ///     Sets the internal data of JsrtContext.
        /// </summary>
        /// <param name="context">
        ///     The context to set the data to.
        /// </param>
        /// <param name="data">
        ///     The pointer to the data to be set.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetContextData", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetContextData(JavaScriptContextSafeHandle context, IntPtr data);

        /// <summary>
        ///     Gets the runtime that the context belongs to.
        /// </summary>
        /// <param name="context">
        ///     The context to get the runtime from.
        /// </param>
        /// <param name="runtime">
        ///     The runtime the context belongs to.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetRuntime", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetRuntime(JavaScriptContextSafeHandle context, out JavaScriptRuntimeSafeHandle runtime);

        /// <summary>
        ///     Tells the runtime to do any idle processing it need to do.
        /// </summary>
        /// <remarks>
        ///     If idle processing has been enabled for the current runtime, calling JsIdle will
        ///     inform the current runtime that the host is idle and that the runtime can perform
        ///     memory cleanup tasks.
        ///     JsIdle can also return the number of system ticks until there will be more idle work
        ///     for the runtime to do. Calling JsIdle before this number of ticks has passed will do
        ///     no work.
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="nextIdleTick">
        ///     The next system tick when there will be more idle work to do. Can be null. Returns the
        ///     maximum number of ticks if there no upcoming idle work to do.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsIdle", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsIdle(out uint nextIdleTick);

        /// <summary>
        ///     Gets the symbol associated with the property ID.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="propertyId">
        ///     The property ID to get the symbol of.
        /// </param>
        /// <param name="symbol">
        ///     The symbol associated with the property ID.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetSymbolFromPropertyId", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetSymbolFromPropertyId(JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptValueSafeHandle symbol);

        /// <summary>
        ///     Gets the type of property
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="propertyId">
        ///     The property ID to get the type of.
        /// </param>
        /// <param name="propertyIdType">
        ///     The JsPropertyIdType of the given property ID
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetPropertyIdType", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetPropertyIdType(JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptPropertyIdType propertyIdType);

        /// <summary>
        ///     Gets the property ID associated with the symbol.
        /// </summary>
        /// <remarks>
        ///     Property IDs are specific to a context and cannot be used across contexts.
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="symbol">
        ///     The symbol whose property ID is being retrieved.
        /// </param>
        /// <param name="propertyId">
        ///     The property ID for the given symbol.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetPropertyIdFromSymbol", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetPropertyIdFromSymbol(JavaScriptValueSafeHandle symbol, out JavaScriptPropertyIdSafeHandle propertyId);

        /// <summary>
        ///     Creates a Javascript symbol.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="description">
        ///     The string description of the symbol. Can be null.
        /// </param>
        /// <param name="result">
        ///     The new symbol.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateSymbol", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateSymbol(JavaScriptValueSafeHandle description, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Gets the list of all symbol properties on the object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object from which to get the property symbols.
        /// </param>
        /// <param name="propertySymbols">
        ///     An array of property symbols.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetOwnPropertySymbols", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetOwnPropertySymbols(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle propertySymbols);

        /// <summary>
        ///     Gets the value of undefined in the current script context.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="undefinedValue">
        ///     The undefined value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetUndefinedValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetUndefinedValue(out JavaScriptValueSafeHandle undefinedValue);

        /// <summary>
        ///     Gets the value of null in the current script context.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="nullValue">
        ///     The null value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetNullValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetNullValue(out JavaScriptValueSafeHandle nullValue);

        /// <summary>
        ///     Gets the value of true in the current script context.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="trueValue">
        ///     The true value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetTrueValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetTrueValue(out JavaScriptValueSafeHandle trueValue);

        /// <summary>
        ///     Gets the value of false in the current script context.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="falseValue">
        ///     The false value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetFalseValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetFalseValue(out JavaScriptValueSafeHandle falseValue);

        /// <summary>
        ///     Creates a Boolean value from a bool value.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="value">
        ///     The value to be converted.
        /// </param>
        /// <param name="booleanValue">
        ///     The converted value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsBoolToBoolean", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsBoolToBoolean(bool value, out JavaScriptValueSafeHandle booleanValue);

        /// <summary>
        ///     Retrieves the bool value of a Boolean value.
        /// </summary>
        /// <param name="value">
        ///     The value to be converted.
        /// </param>
        /// <param name="boolValue">
        ///     The converted value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsBooleanToBool", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsBooleanToBool(JavaScriptValueSafeHandle value, out bool boolValue);

        /// <summary>
        ///     Converts the value to Boolean using standard JavaScript semantics.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="value">
        ///     The value to be converted.
        /// </param>
        /// <param name="booleanValue">
        ///     The converted value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsConvertValueToBoolean", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsConvertValueToBoolean(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle booleanValue);

        /// <summary>
        ///     Gets the JavaScript type of a JsValueRef.
        /// </summary>
        /// <param name="value">
        ///     The value whose type is to be returned.
        /// </param>
        /// <param name="type">
        ///     The type of the value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetValueType", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetValueType(JavaScriptValueSafeHandle value, out JsValueType type);

        /// <summary>
        ///     Creates a number value from a double value.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="doubleValue">
        ///     The double to convert to a number value.
        /// </param>
        /// <param name="value">
        ///     The new number value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDoubleToNumber", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDoubleToNumber(double doubleValue, out JavaScriptValueSafeHandle value);

        /// <summary>
        ///     Creates a number value from an int value.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="intValue">
        ///     The int to convert to a number value.
        /// </param>
        /// <param name="value">
        ///     The new number value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsIntToNumber", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsIntToNumber(int intValue, out JavaScriptValueSafeHandle value);

        /// <summary>
        ///     Retrieves the double value of a number value.
        /// </summary>
        /// <remarks>
        ///     This function retrieves the value of a number value. It will fail with
        ///     JsErrorInvalidArgument if the type of the value is not number.
        /// </remarks>
        /// <param name="value">
        ///     The number value to convert to a double value.
        /// </param>
        /// <param name="doubleValue">
        ///     The double value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsNumberToDouble", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsNumberToDouble(JavaScriptValueSafeHandle value, out double doubleValue);

        /// <summary>
        ///     Retrieves the int value of a number value.
        /// </summary>
        /// <remarks>
        ///     This function retrieves the value of a number value and converts to an int value.
        ///     It will fail with JsErrorInvalidArgument if the type of the value is not number.
        /// </remarks>
        /// <param name="value">
        ///     The number value to convert to an int value.
        /// </param>
        /// <param name="intValue">
        ///     The int value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsNumberToInt", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsNumberToInt(JavaScriptValueSafeHandle value, out int intValue);

        /// <summary>
        ///     Converts the value to number using standard JavaScript semantics.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="value">
        ///     The value to be converted.
        /// </param>
        /// <param name="numberValue">
        ///     The converted value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsConvertValueToNumber", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsConvertValueToNumber(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle numberValue);

        /// <summary>
        ///     Gets the length of a string value.
        /// </summary>
        /// <param name="stringValue">
        ///     The string value to get the length of.
        /// </param>
        /// <param name="length">
        ///     The length of the string.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetStringLength", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetStringLength(JavaScriptValueSafeHandle stringValue, out int length);

        /// <summary>
        ///     Converts the value to string using standard JavaScript semantics.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="value">
        ///     The value to be converted.
        /// </param>
        /// <param name="stringValue">
        ///     The converted value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsConvertValueToString", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsConvertValueToString(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle stringValue);

        /// <summary>
        ///     Gets the global object in the current script context.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="globalObject">
        ///     The global object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetGlobalObject", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetGlobalObject(out JavaScriptValueSafeHandle globalObject);

        /// <summary>
        ///     Creates a new object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The new object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateObject", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateObject(out JavaScriptValueSafeHandle @object);

        /// <summary>
        ///     Creates a new object that stores some external data.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="data">
        ///     External data that the object will represent. May be null.
        /// </param>
        /// <param name="finalizeCallback">
        ///     A callback for when the object is finalized. May be null.
        /// </param>
        /// <param name="@object">
        ///     The new object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateExternalObject", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback, out JavaScriptValueSafeHandle @object);

        /// <summary>
        ///     Converts the value to object using standard JavaScript semantics.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="value">
        ///     The value to be converted.
        /// </param>
        /// <param name="@object">
        ///     The converted value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsConvertValueToObject", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsConvertValueToObject(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle @object);

        /// <summary>
        ///     Returns the prototype of an object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object whose prototype is to be returned.
        /// </param>
        /// <param name="prototypeObject">
        ///     The object's prototype.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetPrototype", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetPrototype(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle prototypeObject);

        /// <summary>
        ///     Sets the prototype of an object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object whose prototype is to be changed.
        /// </param>
        /// <param name="prototypeObject">
        ///     The object's new prototype.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetPrototype", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetPrototype(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle prototypeObject);

        /// <summary>
        ///     Performs JavaScript &quot;instanceof&quot; operator test.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object to test.
        /// </param>
        /// <param name="constructor">
        ///     The constructor function to test against.
        /// </param>
        /// <param name="result">
        ///     Whether "object instanceof constructor" is true.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsInstanceOf", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsInstanceOf(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle constructor, out bool result);

        /// <summary>
        ///     Returns a value that indicates whether an object is extensible or not.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object to test.
        /// </param>
        /// <param name="value">
        ///     Whether the object is extensible or not.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetExtensionAllowed", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetExtensionAllowed(JavaScriptValueSafeHandle @object, out bool value);

        /// <summary>
        ///     Makes an object non-extensible.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object to make non-extensible.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsPreventExtension", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsPreventExtension(JavaScriptValueSafeHandle @object);

        /// <summary>
        ///     Gets an object's property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that contains the property.
        /// </param>
        /// <param name="propertyId">
        ///     The ID of the property.
        /// </param>
        /// <param name="value">
        ///     The value of the property.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptValueSafeHandle value);

        /// <summary>
        ///     Gets a property descriptor for an object's own property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that has the property.
        /// </param>
        /// <param name="propertyId">
        ///     The ID of the property.
        /// </param>
        /// <param name="propertyDescriptor">
        ///     The property descriptor.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetOwnPropertyDescriptor", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetOwnPropertyDescriptor(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptValueSafeHandle propertyDescriptor);

        /// <summary>
        ///     Gets the list of all properties on the object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object from which to get the property names.
        /// </param>
        /// <param name="propertyNames">
        ///     An array of property names.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetOwnPropertyNames", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetOwnPropertyNames(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle propertyNames);

        /// <summary>
        ///     Puts an object's property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that contains the property.
        /// </param>
        /// <param name="propertyId">
        ///     The ID of the property.
        /// </param>
        /// <param name="value">
        ///     The new value of the property.
        /// </param>
        /// <param name="useStrictRules">
        ///     The property set should follow strict mode rules.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle value, bool useStrictRules);

        /// <summary>
        ///     Determines whether an object has a property.
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
        /// <param name="hasProperty">
        ///     Whether the object (or a prototype) has the property.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsHasProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsHasProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, out bool hasProperty);

        /// <summary>
        ///     Deletes an object's property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that contains the property.
        /// </param>
        /// <param name="propertyId">
        ///     The ID of the property.
        /// </param>
        /// <param name="useStrictRules">
        ///     The property set should follow strict mode rules.
        /// </param>
        /// <param name="result">
        ///     Whether the property was deleted.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDeleteProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDeleteProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, bool useStrictRules, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Defines a new object's own property from a property descriptor.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that has the property.
        /// </param>
        /// <param name="propertyId">
        ///     The ID of the property.
        /// </param>
        /// <param name="propertyDescriptor">
        ///     The property descriptor.
        /// </param>
        /// <param name="result">
        ///     Whether the property was defined.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDefineProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDefineProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle propertyDescriptor, out bool result);

        /// <summary>
        ///     Tests whether an object has a value at the specified index.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object to operate on.
        /// </param>
        /// <param name="index">
        ///     The index to test.
        /// </param>
        /// <param name="result">
        ///     Whether the object has a value at the specified index.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsHasIndexedProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsHasIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, out bool result);

        /// <summary>
        ///     Retrieve the value at the specified index of an object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object to operate on.
        /// </param>
        /// <param name="index">
        ///     The index to retrieve.
        /// </param>
        /// <param name="result">
        ///     The retrieved value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetIndexedProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Set the value at the specified index of an object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object to operate on.
        /// </param>
        /// <param name="index">
        ///     The index to set.
        /// </param>
        /// <param name="value">
        ///     The value to set.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetIndexedProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, JavaScriptValueSafeHandle value);

        /// <summary>
        ///     Delete the value at the specified index of an object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object to operate on.
        /// </param>
        /// <param name="index">
        ///     The index to delete.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDeleteIndexedProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDeleteIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index);

        /// <summary>
        ///     Determines whether an object has its indexed properties in external data.
        /// </summary>
        /// <param name="@object">
        ///     The object.
        /// </param>
        /// <param name="value">
        ///     Whether the object has its indexed properties in external data.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsHasIndexedPropertiesExternalData", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsHasIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, out bool value);

        /// <summary>
        ///     Retrieves an object's indexed properties external data information.
        /// </summary>
        /// <param name="@object">
        ///     The object.
        /// </param>
        /// <param name="data">
        ///     The external data back store for the object's indexed properties.
        /// </param>
        /// <param name="arrayType">
        ///     The array element type in external data.
        /// </param>
        /// <param name="elementLength">
        ///     The number of array elements in external data.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetIndexedPropertiesExternalData", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, out IntPtr data, out JsTypedArrayType arrayType, out uint elementLength);

        /// <summary>
        ///     Sets an object's indexed properties to external data. The external data will be used as back store for the object's indexed properties and accessed like a typed array.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object to operate on.
        /// </param>
        /// <param name="data">
        ///     The external data to be used as back store for the object's indexed properties.
        /// </param>
        /// <param name="arrayType">
        ///     The array element type in external data.
        /// </param>
        /// <param name="elementLength">
        ///     The number of array elements in external data.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetIndexedPropertiesToExternalData", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetIndexedPropertiesToExternalData(JavaScriptValueSafeHandle @object, IntPtr data, JsTypedArrayType arrayType, uint elementLength);

        /// <summary>
        ///     Compare two JavaScript values for equality.
        /// </summary>
        /// <remarks>
        ///     This function is equivalent to the == operator in Javascript.
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="object1">
        ///     The first object to compare.
        /// </param>
        /// <param name="object2">
        ///     The second object to compare.
        /// </param>
        /// <param name="result">
        ///     Whether the values are equal.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsEquals", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result);

        /// <summary>
        ///     Compare two JavaScript values for strict equality.
        /// </summary>
        /// <remarks>
        ///     This function is equivalent to the === operator in Javascript.
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="object1">
        ///     The first object to compare.
        /// </param>
        /// <param name="object2">
        ///     The second object to compare.
        /// </param>
        /// <param name="result">
        ///     Whether the values are strictly equal.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsStrictEquals", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsStrictEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result);

        /// <summary>
        ///     Determines whether an object is an external object.
        /// </summary>
        /// <param name="@object">
        ///     The object.
        /// </param>
        /// <param name="value">
        ///     Whether the object is an external object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsHasExternalData", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsHasExternalData(JavaScriptValueSafeHandle @object, out bool value);

        /// <summary>
        ///     Retrieves the data from an external object.
        /// </summary>
        /// <param name="@object">
        ///     The external object.
        /// </param>
        /// <param name="externalData">
        ///     The external data stored in the object. Can be null if no external data is stored in the
        ///     object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetExternalData", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetExternalData(JavaScriptValueSafeHandle @object, out IntPtr externalData);

        /// <summary>
        ///     Sets the external data on an external object.
        /// </summary>
        /// <param name="@object">
        ///     The external object.
        /// </param>
        /// <param name="externalData">
        ///     The external data to be stored in the object. Can be null if no external data is
        ///     to be stored in the object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetExternalData", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetExternalData(JavaScriptValueSafeHandle @object, IntPtr externalData);

        /// <summary>
        ///     Creates a Javascript array object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="length">
        ///     The initial length of the array.
        /// </param>
        /// <param name="result">
        ///     The new array object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateArray", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateArray(uint length, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Creates a Javascript ArrayBuffer object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="byteLength">
        ///     The number of bytes in the ArrayBuffer.
        /// </param>
        /// <param name="result">
        ///     The new ArrayBuffer object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateArrayBuffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateArrayBuffer(uint byteLength, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Creates a Javascript ArrayBuffer object to access external memory.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="data">
        ///     A pointer to the external memory.
        /// </param>
        /// <param name="byteLength">
        ///     The number of bytes in the external memory.
        /// </param>
        /// <param name="finalizeCallback">
        ///     A callback for when the object is finalized. May be null.
        /// </param>
        /// <param name="callbackState">
        ///     User provided state that will be passed back to finalizeCallback.
        /// </param>
        /// <param name="result">
        ///     The new ArrayBuffer object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateExternalArrayBuffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateExternalArrayBuffer(IntPtr data, uint byteLength, JavaScriptObjectFinalizeCallback finalizeCallback, IntPtr callbackState, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Creates a Javascript typed array object.
        /// </summary>
        /// <remarks>
        ///     The baseArray can be an ArrayBuffer, another typed array, or a JavaScript
        ///     Array. The returned typed array will use the baseArray if it is an ArrayBuffer, or
        ///     otherwise create and use a copy of the underlying source array.
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="arrayType">
        ///     The type of the array to create.
        /// </param>
        /// <param name="baseArray">
        ///     The base array of the new array. Use JS_INVALID_REFERENCE if no base array.
        /// </param>
        /// <param name="byteOffset">
        ///     The offset in bytes from the start of baseArray (ArrayBuffer) for result typed array to reference.
        ///     Only applicable when baseArray is an ArrayBuffer object. Must be 0 otherwise.
        /// </param>
        /// <param name="elementLength">
        ///     The number of elements in the array. Only applicable when creating a new typed array without
        ///     baseArray (baseArray is JS_INVALID_REFERENCE) or when baseArray is an ArrayBuffer object.
        ///     Must be 0 otherwise.
        /// </param>
        /// <param name="result">
        ///     The new typed array object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateTypedArray", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateTypedArray(JsTypedArrayType arrayType, JavaScriptValueSafeHandle baseArray, uint byteOffset, uint elementLength, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Creates a Javascript DataView object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="arrayBuffer">
        ///     An existing ArrayBuffer object to use as the storage for the result DataView object.
        /// </param>
        /// <param name="byteOffset">
        ///     The offset in bytes from the start of arrayBuffer for result DataView to reference.
        /// </param>
        /// <param name="byteLength">
        ///     The number of bytes in the ArrayBuffer for result DataView to reference.
        /// </param>
        /// <param name="result">
        ///     The new DataView object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateDataView", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateDataView(JavaScriptValueSafeHandle arrayBuffer, uint byteOffset, uint byteLength, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Obtains frequently used properties of a typed array.
        /// </summary>
        /// <param name="typedArray">
        ///     The typed array instance.
        /// </param>
        /// <param name="arrayType">
        ///     The type of the array.
        /// </param>
        /// <param name="arrayBuffer">
        ///     The ArrayBuffer backstore of the array.
        /// </param>
        /// <param name="byteOffset">
        ///     The offset in bytes from the start of arrayBuffer referenced by the array.
        /// </param>
        /// <param name="byteLength">
        ///     The number of bytes in the array.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetTypedArrayInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetTypedArrayInfo(JavaScriptValueSafeHandle typedArray, out JsTypedArrayType arrayType, out JavaScriptValueSafeHandle arrayBuffer, out uint byteOffset, out uint byteLength);

        /// <summary>
        ///     Obtains the underlying memory storage used by an ArrayBuffer.
        /// </summary>
        /// <param name="arrayBuffer">
        ///     The ArrayBuffer instance.
        /// </param>
        /// <param name="buffer">
        ///     The ArrayBuffer's buffer. The lifetime of the buffer returned is the same as the lifetime of the
        ///     the ArrayBuffer. The buffer pointer does not count as a reference to the ArrayBuffer for the purpose
        ///     of garbage collection.
        /// </param>
        /// <param name="bufferLength">
        ///     The number of bytes in the buffer.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetArrayBufferStorage", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetArrayBufferStorage(JavaScriptValueSafeHandle arrayBuffer, out IntPtr buffer, out uint bufferLength);

        /// <summary>
        ///     Obtains the underlying memory storage used by a typed array.
        /// </summary>
        /// <param name="typedArray">
        ///     The typed array instance.
        /// </param>
        /// <param name="buffer">
        ///     The array's buffer. The lifetime of the buffer returned is the same as the lifetime of the
        ///     the array. The buffer pointer does not count as a reference to the array for the purpose
        ///     of garbage collection.
        /// </param>
        /// <param name="bufferLength">
        ///     The number of bytes in the buffer.
        /// </param>
        /// <param name="arrayType">
        ///     The type of the array.
        /// </param>
        /// <param name="elementSize">
        ///     The size of an element of the array.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetTypedArrayStorage", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetTypedArrayStorage(JavaScriptValueSafeHandle typedArray, out IntPtr buffer, out uint bufferLength, out JsTypedArrayType arrayType, out int elementSize);

        /// <summary>
        ///     Obtains the underlying memory storage used by a DataView.
        /// </summary>
        /// <param name="dataView">
        ///     The DataView instance.
        /// </param>
        /// <param name="buffer">
        ///     The DataView's buffer. The lifetime of the buffer returned is the same as the lifetime of the
        ///     the DataView. The buffer pointer does not count as a reference to the DataView for the purpose
        ///     of garbage collection.
        /// </param>
        /// <param name="bufferLength">
        ///     The number of bytes in the buffer.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetDataViewStorage", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetDataViewStorage(JavaScriptValueSafeHandle dataView, out IntPtr buffer, out uint bufferLength);

        /// <summary>
        ///     Invokes a function.
        /// </summary>
        /// <remarks>
        ///     Requires thisArg as first argument of arguments.
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="function">
        ///     The function to invoke.
        /// </param>
        /// <param name="arguments">
        ///     The arguments to the call.
        /// </param>
        /// <param name="argumentCount">
        ///     The number of arguments being passed in to the function.
        /// </param>
        /// <param name="result">
        ///     The value returned from the function invocation, if any.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCallFunction", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCallFunction(JavaScriptValueSafeHandle function, IntPtr[] arguments, ushort argumentCount, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Invokes a function as a constructor.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="function">
        ///     The function to invoke as a constructor.
        /// </param>
        /// <param name="arguments">
        ///     The arguments to the call.
        /// </param>
        /// <param name="argumentCount">
        ///     The number of arguments being passed in to the function.
        /// </param>
        /// <param name="result">
        ///     The value returned from the function invocation.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsConstructObject", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsConstructObject(JavaScriptValueSafeHandle function, IntPtr[] arguments, ushort argumentCount, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Creates a new JavaScript function.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="nativeFunction">
        ///     The method to call when the function is invoked.
        /// </param>
        /// <param name="callbackState">
        ///     User provided state that will be passed back to the callback.
        /// </param>
        /// <param name="function">
        ///     The new function object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateFunction", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateFunction(JavaScriptNativeFunction nativeFunction, IntPtr callbackState, out JavaScriptValueSafeHandle function);

        /// <summary>
        ///     Creates a new JavaScript function with name.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="name">
        ///     The name of this function that will be used for diagnostics and stringification purposes.
        /// </param>
        /// <param name="nativeFunction">
        ///     The method to call when the function is invoked.
        /// </param>
        /// <param name="callbackState">
        ///     User provided state that will be passed back to the callback.
        /// </param>
        /// <param name="function">
        ///     The new function object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateNamedFunction", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateNamedFunction(JavaScriptValueSafeHandle name, JavaScriptNativeFunction nativeFunction, IntPtr callbackState, out JavaScriptValueSafeHandle function);

        /// <summary>
        ///     Creates a new JavaScript error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">
        ///     Message for the error object.
        /// </param>
        /// <param name="error">
        ///     The new error object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateError", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

        /// <summary>
        ///     Creates a new JavaScript RangeError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">
        ///     Message for the error object.
        /// </param>
        /// <param name="error">
        ///     The new error object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateRangeError", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateRangeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

        /// <summary>
        ///     Creates a new JavaScript ReferenceError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">
        ///     Message for the error object.
        /// </param>
        /// <param name="error">
        ///     The new error object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateReferenceError", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateReferenceError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

        /// <summary>
        ///     Creates a new JavaScript SyntaxError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">
        ///     Message for the error object.
        /// </param>
        /// <param name="error">
        ///     The new error object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateSyntaxError", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateSyntaxError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

        /// <summary>
        ///     Creates a new JavaScript TypeError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">
        ///     Message for the error object.
        /// </param>
        /// <param name="error">
        ///     The new error object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateTypeError", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateTypeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

        /// <summary>
        ///     Creates a new JavaScript URIError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">
        ///     Message for the error object.
        /// </param>
        /// <param name="error">
        ///     The new error object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsCreateURIError", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsCreateURIError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

        /// <summary>
        ///     Determines whether the runtime of the current context is in an exception state.
        /// </summary>
        /// <remarks>
        ///     If a call into the runtime results in an exception (either as the result of running a
        ///     script or due to something like a conversion failure), the runtime is placed into an
        ///     "exception state." All calls into any context created by the runtime (except for the
        ///     exception APIs) will fail with JsErrorInExceptionState until the exception is
        ///     cleared.
        ///     If the runtime of the current context is in the exception state when a callback returns
        ///     into the engine, the engine will automatically rethrow the exception.
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="hasException">
        ///     Whether the runtime of the current context is in the exception state.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsHasException", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsHasException(out bool hasException);

        /// <summary>
        ///     Returns the exception that caused the runtime of the current context to be in the exception state and resets the exception state for that runtime.
        /// </summary>
        /// <remarks>
        ///     If the runtime of the current context is not in an exception state, this API will return
        ///     JsErrorInvalidArgument. If the runtime is disabled, this will return an exception
        ///     indicating that the script was terminated, but it will not clear the exception (the
        ///     exception will be cleared if the runtime is re-enabled using
        ///     JsEnableRuntimeExecution).
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="exception">
        ///     The exception for the runtime of the current context.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetAndClearException", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsGetAndClearException(out JavaScriptValueSafeHandle exception);

        /// <summary>
        ///     Sets the runtime of the current context to an exception state.
        /// </summary>
        /// <remarks>
        ///     If the runtime of the current context is already in an exception state, this API will
        ///     return JsErrorInExceptionState.
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="exception">
        ///     The JavaScript exception to set for the runtime of the current context.
        /// </param>
        /// <returns>
        ///     JsNoError if the engine was set into an exception state, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetException", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetException(JavaScriptValueSafeHandle exception);

        /// <summary>
        ///     Suspends script execution and terminates any running scripts in a runtime.
        /// </summary>
        /// <remarks>
        ///     Calls to a suspended runtime will fail until JsEnableRuntimeExecution is called.
        ///     This API does not have to be called on the thread the runtime is active on. Although the
        ///     runtime will be set into a suspended state, an executing script may not be suspended
        ///     immediately; a running script will be terminated with an uncatchable exception as soon as
        ///     possible.
        ///     Suspending execution in a runtime that is already suspended is a no-op.
        /// </remarks>
        /// <param name="runtime">
        ///     The runtime to be suspended.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDisableRuntimeExecution", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDisableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime);

        /// <summary>
        ///     Enables script execution in a runtime.
        /// </summary>
        /// <remarks>
        ///     Enabling script execution in a runtime that already has script execution enabled is a
        ///     no-op.
        /// </remarks>
        /// <param name="runtime">
        ///     The runtime to be enabled.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsEnableRuntimeExecution", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsEnableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime);

        /// <summary>
        ///     Returns a value that indicates whether script execution is disabled in the runtime.
        /// </summary>
        /// <param name="runtime">
        ///     Specifies the runtime to check if execution is disabled.
        /// </param>
        /// <param name="isDisabled">
        ///     If execution is disabled, true, false otherwise.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsIsRuntimeExecutionDisabled", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsIsRuntimeExecutionDisabled(JavaScriptRuntimeSafeHandle runtime, out bool isDisabled);

        /// <summary>
        ///     Sets a promise continuation callback function that is called by the context when a task needs to be queued for future execution
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="promiseContinuationCallback">
        ///     The callback function being set.
        /// </param>
        /// <param name="callbackState">
        ///     User provided state that will be passed back to the callback.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSetPromiseContinuationCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsSetPromiseContinuationCallback(JavaScriptPromiseContinuationCallback promiseContinuationCallback, IntPtr callbackState);

        /// <summary>
        ///     Starts debugging in the given runtime.
        /// </summary>
        /// <remarks>
        ///     The runtime should be active on the current thread and should not be in debug state.
        /// </remarks>
        /// <param name="runtimeHandle">
        ///     Runtime to put into debug mode.
        /// </param>
        /// <param name="debugEventCallback">
        ///     Registers a callback to be called on every JsDiagDebugEvent.
        /// </param>
        /// <param name="callbackState">
        ///     User provided state that will be passed back to the callback.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagStartDebugging", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagStartDebugging(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagDebugEventCallback debugEventCallback, IntPtr callbackState);

        /// <summary>
        ///     Stops debugging in the given runtime.
        /// </summary>
        /// <remarks>
        ///     The runtime should be active on the current thread and in debug state.
        /// </remarks>
        /// <param name="runtimeHandle">
        ///     Runtime to stop debugging.
        /// </param>
        /// <param name="callbackState">
        ///     User provided state that was passed in JsDiagStartDebugging.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagStopDebugging", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagStopDebugging(JavaScriptRuntimeSafeHandle runtimeHandle, out IntPtr callbackState);

        /// <summary>
        ///     Request the runtime to break on next JavaScript statement.
        /// </summary>
        /// <remarks>
        ///     The runtime should be in debug state. This API can be called from another runtime.
        /// </remarks>
        /// <param name="runtimeHandle">
        ///     Runtime to request break.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagRequestAsyncBreak", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagRequestAsyncBreak(JavaScriptRuntimeSafeHandle runtimeHandle);

        /// <summary>
        ///     List all breakpoints in the current runtime.
        /// </summary>
        /// <remarks>
        ///     [{
        ///     "breakpointId" : 1,
        ///     "scriptId" : 1,
        ///     "line" : 0,
        ///     "column" : 62
        ///     }]
        /// </remarks>
        /// <param name="breakpoints">
        ///     Array of breakpoints.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagGetBreakpoints", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagGetBreakpoints(out JavaScriptValueSafeHandle breakpoints);

        /// <summary>
        ///     Sets breakpoint in the specified script at give location.
        /// </summary>
        /// <remarks>
        ///     {
        ///     "breakpointId" : 1,
        ///     "line" : 2,
        ///     "column" : 4
        ///     }
        /// </remarks>
        /// <param name="scriptId">
        ///     Id of script from JsDiagGetScripts or JsDiagGetSource to put breakpoint.
        /// </param>
        /// <param name="lineNumber">
        ///     0 based line number to put breakpoint.
        /// </param>
        /// <param name="columnNumber">
        ///     0 based column number to put breakpoint.
        /// </param>
        /// <param name="breakpoint">
        ///     Breakpoint object with id, line and column if success.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagSetBreakpoint", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagSetBreakpoint(uint scriptId, uint lineNumber, uint columnNumber, out JavaScriptValueSafeHandle breakpoint);

        /// <summary>
        ///     Remove a breakpoint.
        /// </summary>
        /// <remarks>
        ///     The current runtime should be in debug state. This API can be called when runtime is at a break or running.
        /// </remarks>
        /// <param name="breakpointId">
        ///     Breakpoint id returned from JsDiagSetBreakpoint.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagRemoveBreakpoint", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagRemoveBreakpoint(uint breakpointId);

        /// <summary>
        ///     Sets break on exception handling.
        /// </summary>
        /// <remarks>
        ///     If this API is not called the default value is set to JsDiagBreakOnExceptionAttributeUncaught in the runtime.
        /// </remarks>
        /// <param name="runtimeHandle">
        ///     Runtime to set break on exception attributes.
        /// </param>
        /// <param name="exceptionAttributes">
        ///     Mask of JsDiagBreakOnExceptionAttributes to set.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagSetBreakOnException", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagSetBreakOnException(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagBreakOnExceptionAttributes exceptionAttributes);

        /// <summary>
        ///     Gets break on exception setting.
        /// </summary>
        /// <remarks>
        ///     The runtime should be in debug state. This API can be called from another runtime.
        /// </remarks>
        /// <param name="runtimeHandle">
        ///     Runtime from which to get break on exception attributes, should be in debug mode.
        /// </param>
        /// <param name="exceptionAttributes">
        ///     Mask of JsDiagBreakOnExceptionAttributes.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagGetBreakOnException", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagGetBreakOnException(JavaScriptRuntimeSafeHandle runtimeHandle, out JavaScriptDiagBreakOnExceptionAttributes exceptionAttributes);

        /// <summary>
        ///     Sets the step type in the runtime after a debug break.
        /// </summary>
        /// <remarks>
        ///     Requires to be at a debug break.
        /// </remarks>
        /// <param name="stepType">
        ///     NO DESCRIPTION PROVIDED
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagSetStepType", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagSetStepType(JavaScriptDiagStepType stepType);

        /// <summary>
        ///     Gets list of scripts.
        /// </summary>
        /// <remarks>
        ///     [{
        ///     "scriptId" : 2,
        ///     "fileName" : "c:\\Test\\Test.js",
        ///     "lineCount" : 4,
        ///     "sourceLength" : 111
        ///     }, {
        ///     "scriptId" : 3,
        ///     "parentScriptId" : 2,
        ///     "scriptType" : "eval code",
        ///     "lineCount" : 1,
        ///     "sourceLength" : 12
        ///     }]
        /// </remarks>
        /// <param name="scriptsArray">
        ///     Array of script objects.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagGetScripts", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagGetScripts(out JavaScriptValueSafeHandle scriptsArray);

        /// <summary>
        ///     Gets source for a specific script identified by scriptId from JsDiagGetScripts.
        /// </summary>
        /// <remarks>
        ///     {
        ///     "scriptId" : 1,
        ///     "fileName" : "c:\\Test\\Test.js",
        ///     "lineCount" : 12,
        ///     "sourceLength" : 15154,
        ///     "source" : "var x = 1;"
        ///     }
        /// </remarks>
        /// <param name="scriptId">
        ///     Id of the script.
        /// </param>
        /// <param name="source">
        ///     Source object.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagGetSource", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagGetSource(uint scriptId, out JavaScriptValueSafeHandle source);

        /// <summary>
        ///     Gets the source information for a function object.
        /// </summary>
        /// <remarks>
        ///     {
        ///     "scriptId" : 1,
        ///     "fileName" : "c:\\Test\\Test.js",
        ///     "line" : 1,
        ///     "column" : 2,
        ///     "firstStatementLine" : 6,
        ///     "firstStatementColumn" : 0
        ///     }
        /// </remarks>
        /// <param name="function">
        ///     JavaScript function.
        /// </param>
        /// <param name="functionPosition">
        ///     Function position - scriptId, start line, start column, line number of first statement, column number of first statement.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagGetFunctionPosition", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagGetFunctionPosition(JavaScriptValueSafeHandle function, out JavaScriptValueSafeHandle functionPosition);

        /// <summary>
        ///     Gets the stack trace information.
        /// </summary>
        /// <remarks>
        ///     [{
        ///     "index" : 0,
        ///     "scriptId" : 2,
        ///     "line" : 3,
        ///     "column" : 0,
        ///     "sourceLength" : 9,
        ///     "sourceText" : "var x = 1",
        ///     "functionHandle" : 1
        ///     }]
        /// </remarks>
        /// <param name="stackTrace">
        ///     Stack trace information.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagGetStackTrace", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagGetStackTrace(out JavaScriptValueSafeHandle stackTrace);

        /// <summary>
        ///     Gets the list of properties corresponding to the frame.
        /// </summary>
        /// <remarks>
        ///     propertyAttributes is a bit mask of
        ///     NONE = 0x1,
        ///     HAVE_CHILDRENS = 0x2,
        ///     READ_ONLY_VALUE = 0x4,
        ///     IN_TDZ = 0x8,
        ///     {
        ///     "thisObject": {
        ///     "name": "this",
        ///     "type" : "object",
        ///     "className" : "Object",
        ///     "display" : "{...}",
        ///     "propertyAttributes" : 1,
        ///     "handle" : 306
        ///     },
        ///     "exception" : {
        ///     "name" : "{exception}",
        ///     "type" : "object",
        ///     "display" : "'a' is undefined",
        ///     "className" : "Error",
        ///     "propertyAttributes" : 1,
        ///     "handle" : 307
        ///     }
        ///     "arguments" : {
        ///     "name" : "arguments",
        ///     "type" : "object",
        ///     "display" : "{...}",
        ///     "className" : "Object",
        ///     "propertyAttributes" : 1,
        ///     "handle" : 190
        ///     },
        ///     "returnValue" : {
        ///     "name" : "[Return value]",
        ///     "type" : "undefined",
        ///     "propertyAttributes" : 0,
        ///     "handle" : 192
        ///     },
        ///     "functionCallsReturn" : [{
        ///     "name" : "[foo1 returned]",
        ///     "type" : "number",
        ///     "value" : 1,
        ///     "propertyAttributes" : 2,
        ///     "handle" : 191
        ///     }
        ///     ],
        ///     "locals" : [],
        ///     "scopes" : [{
        ///     "index" : 0,
        ///     "handle" : 193
        ///     }
        ///     ],
        ///     "globals" : {
        ///     "handle" : 194
        ///     }
        ///     }
        /// </remarks>
        /// <param name="stackFrameIndex">
        ///     Index of stack frame from JsDiagGetStackTrace.
        /// </param>
        /// <param name="properties">
        ///     Object of properties array (properties, scopes and globals).
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagGetStackProperties", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagGetStackProperties(uint stackFrameIndex, out JavaScriptValueSafeHandle properties);

        /// <summary>
        ///     Gets the list of children of a handle.
        /// </summary>
        /// <remarks>
        ///     Handle should be from objects returned from call to JsDiagGetStackProperties.
        /// </remarks>
        /// <param name="objectHandle">
        ///     Handle of object.
        /// </param>
        /// <param name="fromCount">
        ///     0-based from count of properties, usually 0.
        /// </param>
        /// <param name="totalCount">
        ///     Number of properties to return.
        /// </param>
        /// <param name="propertiesObject">
        ///     Array of properties.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagGetProperties", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagGetProperties(uint objectHandle, uint fromCount, uint totalCount, out JavaScriptValueSafeHandle propertiesObject);

        /// <summary>
        ///     Gets the object corresponding to handle.
        /// </summary>
        /// <remarks>
        ///     {
        ///     "scriptId" : 24,
        ///     "line" : 1,
        ///     "column" : 63,
        ///     "name" : "foo",
        ///     "type" : "function",
        ///     "handle" : 2
        ///     }
        /// </remarks>
        /// <param name="objectHandle">
        ///     Handle of object.
        /// </param>
        /// <param name="handleObject">
        ///     Object corresponding to the handle.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagGetObjectFromHandle", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagGetObjectFromHandle(uint objectHandle, out JavaScriptValueSafeHandle handleObject);

        /// <summary>
        ///     Evaluates an expression on given frame.
        /// </summary>
        /// <remarks>
        ///     evalResult when evaluating 'this' and return is JsNoError
        ///     {
        ///     "name" : "this",
        ///     "type" : "object",
        ///     "className" : "Object",
        ///     "display" : "{...}",
        ///     "propertyAttributes" : 1,
        ///     "handle" : 18
        ///     }
        ///     evalResult when evaluating a script which throws JavaScript error and return is JsErrorScriptException
        ///     {
        ///     "name" : "a.b.c",
        ///     "type" : "object",
        ///     "className" : "Error",
        ///     "display" : "'a' is undefined",
        ///     "propertyAttributes" : 1,
        ///     "handle" : 18
        ///     }
        /// </remarks>
        /// <param name="expression">
        ///     Javascript String or ArrayBuffer (incl. ExternalArrayBuffer).
        /// </param>
        /// <param name="stackFrameIndex">
        ///     Index of stack frame on which to evaluate the expression.
        /// </param>
        /// <param name="parseAttributes">
        ///     Defines how `expression` (JsValueRef) should be parsed.
        ///     - `JsParseScriptAttributeNone` when `expression` is a Utf8 encoded ArrayBuffer and/or a Javascript String (encoding independent)
        ///     - `JsParseScriptAttributeArrayBufferIsUtf16Encoded` when `expression` is Utf16 Encoded ArrayBuffer
        ///     - `JsParseScriptAttributeLibraryCode` has no use for this function and has similar effect with `JsParseScriptAttributeNone`
        /// </param>
        /// <param name="forceSetValueProp">
        ///     Forces the result to contain the raw value of the expression result.
        /// </param>
        /// <param name="evalResult">
        ///     Result of evaluation.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, evalResult will contain the result
        ///     The code JsErrorScriptException if evaluate generated a JavaScript exception, evalResult will contain the error details
        ///     Other error code for invalid parameters or API was not called at break
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsDiagEvaluate", CallingConvention = CallingConvention.Cdecl)]
        public static extern JsErrorCode JsDiagEvaluate(JavaScriptValueSafeHandle expression, uint stackFrameIndex, JavaScriptParseScriptAttributes parseAttributes, bool forceSetValueProp, out JavaScriptValueSafeHandle evalResult);

        /// <summary>
        ///     Parses a script and returns a function representing the script.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="script">
        ///     The script to parse.
        /// </param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by debuggable script contexts.
        /// </param>
        /// <param name="sourceUrl">
        ///     The location the script came from.
        /// </param>
        /// <param name="result">
        ///     A function representing the script code.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsParseScript", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsParseScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Parses a script and returns a function representing the script.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="script">
        ///     The script to parse.
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
        /// <param name="result">
        ///     A function representing the script code.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsParseScriptWithAttributes", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsParseScriptWithAttributes(string script, JavaScriptSourceContext sourceContext, string sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Executes a script.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
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
        /// <param name="result">
        ///     The result of the script, if any. This parameter can be null.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsRunScript", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsRunScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Executes a module.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="script">
        ///     The module script to parse and execute.
        /// </param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by debuggable script contexts.
        /// </param>
        /// <param name="sourceUrl">
        ///     The location the module script came from.
        /// </param>
        /// <param name="result">
        ///     The result of executing the module script, if any. This parameter can be null.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsExperimentalApiRunModule", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsExperimentalApiRunModule(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Serializes a parsed script to a buffer than can be reused.
        /// </summary>
        /// <remarks>
        ///     JsSerializeScript parses a script and then stores the parsed form of the script in a
        ///     runtime-independent format. The serialized script then can be deserialized in any
        ///     runtime without requiring the script to be re-parsed.
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="script">
        ///     The script to serialize.
        /// </param>
        /// <param name="buffer">
        ///     The buffer to put the serialized script into. Can be null.
        /// </param>
        /// <param name="bufferSize">
        ///     On entry, the size of the buffer, in bytes; on exit, the size of the buffer, in bytes,
        ///     required to hold the serialized script.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsSerializeScript", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsSerializeScript(string script, byte[] buffer, ref uint bufferSize);

        /// <summary>
        ///     Parses a serialized script and returns a function representing the script. Provides the ability to lazy load the script source only if/when it is needed.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        ///     The runtime will hold on to the buffer until all instances of any functions created from
        ///     the buffer are garbage collected.  It will then call scriptUnloadCallback to inform the
        ///     caller it is safe to release.
        /// </remarks>
        /// <param name="scriptLoadCallback">
        ///     Callback called when the source code of the script needs to be loaded.
        /// </param>
        /// <param name="scriptUnloadCallback">
        ///     Callback called when the serialized script and source code are no longer needed.
        /// </param>
        /// <param name="buffer">
        ///     The serialized script.
        /// </param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by debuggable script contexts.
        ///     This context will passed into scriptLoadCallback and scriptUnloadCallback.
        /// </param>
        /// <param name="sourceUrl">
        ///     The location the script came from.
        /// </param>
        /// <param name="result">
        ///     A function representing the script code.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsParseSerializedScriptWithCallback", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsParseSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Runs a serialized script. Provides the ability to lazy load the script source only if/when it is needed.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        ///     The runtime will hold on to the buffer until all instances of any functions created from
        ///     the buffer are garbage collected.  It will then call scriptUnloadCallback to inform the
        ///     caller it is safe to release.
        /// </remarks>
        /// <param name="scriptLoadCallback">
        ///     Callback called when the source code of the script needs to be loaded.
        /// </param>
        /// <param name="scriptUnloadCallback">
        ///     Callback called when the serialized script and source code are no longer needed.
        /// </param>
        /// <param name="buffer">
        ///     The serialized script.
        /// </param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by debuggable script contexts.
        ///     This context will passed into scriptLoadCallback and scriptUnloadCallback.
        /// </param>
        /// <param name="sourceUrl">
        ///     The location the script came from.
        /// </param>
        /// <param name="result">
        ///     The result of running the script, if any. This parameter can be null.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsRunSerializedScriptWithCallback", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsRunSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Parses a serialized script and returns a function representing the script.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        ///     The runtime will hold on to the buffer until all instances of any functions created from
        ///     the buffer are garbage collected.
        /// </remarks>
        /// <param name="script">
        ///     The script to parse.
        /// </param>
        /// <param name="buffer">
        ///     The serialized script.
        /// </param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by debuggable script contexts.
        /// </param>
        /// <param name="sourceUrl">
        ///     The location the script came from.
        /// </param>
        /// <param name="result">
        ///     A function representing the script code.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsParseSerializedScript", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsParseSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Runs a serialized script.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        ///     The runtime will hold on to the buffer until all instances of any functions created from
        ///     the buffer are garbage collected.
        /// </remarks>
        /// <param name="script">
        ///     The source code of the serialized script.
        /// </param>
        /// <param name="buffer">
        ///     The serialized script.
        /// </param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by debuggable script contexts.
        /// </param>
        /// <param name="sourceUrl">
        ///     The location the script came from.
        /// </param>
        /// <param name="result">
        ///     The result of running the script, if any. This parameter can be null.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsRunSerializedScript", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsRunSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

        /// <summary>
        ///     Gets the property ID associated with the name.
        /// </summary>
        /// <remarks>
        ///     Property IDs are specific to a context and cannot be used across contexts.
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="name">
        ///     The name of the property ID to get or create. The name may consist of only digits.
        /// </param>
        /// <param name="propertyId">
        ///     The property ID in this runtime for the given name.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetPropertyIdFromName", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsGetPropertyIdFromName(string name, out JavaScriptPropertyIdSafeHandle propertyId);

        /// <summary>
        ///     Gets the name associated with the property ID.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        ///     The returned buffer is valid as long as the runtime is alive and cannot be used
        ///     once the runtime has been disposed.
        /// </remarks>
        /// <param name="propertyId">
        ///     The property ID to get the name of.
        /// </param>
        /// <param name="name">
        ///     The name associated with the property ID.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsGetPropertyNameFromId", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsGetPropertyNameFromId(JavaScriptPropertyIdSafeHandle propertyId, out IntPtr name);

        /// <summary>
        ///     Creates a string value from a string pointer.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="stringValue">
        ///     The string pointer to convert to a string value.
        /// </param>
        /// <param name="stringLength">
        ///     The length of the string to convert.
        /// </param>
        /// <param name="value">
        ///     The new string value.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsPointerToString", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsPointerToString(string stringValue, ulong stringLength, out JavaScriptValueSafeHandle value);

        /// <summary>
        ///     Retrieves the string pointer of a string value.
        /// </summary>
        /// <remarks>
        ///     This function retrieves the string pointer of a string value. It will fail with
        ///     JsErrorInvalidArgument if the type of the value is not string. The lifetime
        ///     of the string returned will be the same as the lifetime of the value it came from, however
        ///     the string pointer is not considered a reference to the value (and so will not keep it
        ///     from being collected).
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="value">
        ///     The string value to convert to a string pointer.
        /// </param>
        /// <param name="stringValue">
        ///     The string pointer.
        /// </param>
        /// <param name="stringLength">
        ///     The length of the string.
        /// </param>
        /// <returns>
        ///     The code JsNoError if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName, EntryPoint = "JsStringToPointer", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsStringToPointer(JavaScriptValueSafeHandle value, out IntPtr stringValue, out ulong stringLength);

    }
}
