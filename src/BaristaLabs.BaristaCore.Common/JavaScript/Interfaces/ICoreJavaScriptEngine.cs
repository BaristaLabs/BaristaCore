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
        ///   Creates a new enhanced JavaScript function.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="nativeFunction">
        ///     The method to call when the function is invoked.
        /// </param>
        /// <param name="metadata">
        ///     If this is not JS_INVALID_REFERENCE, it is converted to a string and used as the name of the function.
        /// </param>
        /// <param name="callbackState">
        ///     User provided state that will be passed back to the callback.
        /// </param>
        /// <returns>
        ///     The new function object.
        /// </returns>
        JavaScriptValueSafeHandle JsCreateEnhancedFunction(JavaScriptEnhancedNativeFunction nativeFunction, JavaScriptValueSafeHandle metadata, IntPtr callbackState);

        /// <summary>
        ///   Initialize a ModuleRecord from host
        /// </summary>
        /// <remarks>
        ///     Bootstrap the module loading process by creating a new module record.
        /// </remarks>
        /// <param name="referencingModule">
        ///     The parent module of the new module - nullptr for a root module.
        /// </param>
        /// <param name="normalizedSpecifier">
        ///     The normalized specifier for the module.
        /// </param>
        /// <returns>
        ///     The new module record. The host should not try to call this API twice
        ///     with the same normalizedSpecifier.
        /// </returns>
        JavaScriptModuleRecord JsInitializeModuleRecord(JavaScriptModuleRecord referencingModule, JavaScriptValueSafeHandle normalizedSpecifier);

        /// <summary>
        ///   Parse the source for an ES module
        /// </summary>
        /// <remarks>
        ///     This is basically ParseModule operation in ES6 spec. It is slightly different in that:
        ///     a) The ModuleRecord was initialized earlier, and passed in as an argument.
        ///     b) This includes a check to see if the module being Parsed is the last module in the
        ///     dependency tree. If it is it automatically triggers Module Instantiation.
        /// </remarks>
        /// <param name="requestModule">
        ///     The ModuleRecord being parsed.
        /// </param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by debuggable script contexts.
        /// </param>
        /// <param name="script">
        ///     The source script to be parsed, but not executed in this code.
        /// </param>
        /// <param name="scriptLength">
        ///     The length of sourceText in bytes. As the input might contain a embedded null.
        /// </param>
        /// <param name="sourceFlag">
        ///     The type of the source code passed in. It could be utf16 or utf8 at this time.
        /// </param>
        /// <returns>
        ///     The error object if there is parse error.
        /// </returns>
        JavaScriptValueSafeHandle JsParseModuleSource(JavaScriptModuleRecord requestModule, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags sourceFlag);

        /// <summary>
        ///   Execute module code.
        /// </summary>
        /// <remarks>
        ///     This method implements 15.2.1.1.6.5, "ModuleEvaluation" concrete method.
        ///     This method should be called after the engine notifies the host that the module is ready.
        ///     This method only needs to be called on root modules - it will execute all of the dependent modules.
        ///     One moduleRecord will be executed only once. Additional execution call on the same moduleRecord will fail.
        /// </remarks>
        /// <param name="requestModule">
        ///     The ModuleRecord being executed.
        /// </param>
        /// <returns>
        ///     The return value of the module.
        /// </returns>
        JavaScriptValueSafeHandle JsModuleEvaluation(JavaScriptModuleRecord requestModule);

        /// <summary>
        ///   Set host info for the specified module.
        /// </summary>
        /// <remarks>
        ///     This is used for four things:
        ///     1. Setting up the callbacks for module loading - note these are actually
        ///     set on the current Context not the module so only have to be set for
        ///     the first root module in any given context.
        ///     2. Setting host defined info on a module record - can be anything that
        ///     you wish to associate with your modules.
        ///     3. Setting a URL for a module to be used for stack traces/debugging -
        ///     note this must be set before calling JsParseModuleSource on the module
        ///     or it will be ignored.
        ///     4. Setting an exception on the module object - only relevant prior to it being Parsed.
        /// </remarks>
        /// <param name="requestModule">
        ///     The request module.
        /// </param>
        /// <param name="moduleHostInfo">
        ///     The type of host info to be set.
        /// </param>
        /// <param name="hostInfo">
        ///     The host info to be set.
        /// </param>
        void JsSetModuleHostInfo(JavaScriptModuleRecord requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, IntPtr hostInfo);

        /// <summary>
        ///   Retrieve the host info for the specified module.
        /// </summary>
        /// <remarks>
        ///     This can used to retrieve info previously set with JsSetModuleHostInfo.
        /// </remarks>
        /// <param name="requestModule">
        ///     The request module.
        /// </param>
        /// <param name="moduleHostInfo">
        ///     The type of host info to be retrieved.
        /// </param>
        /// <returns>
        ///     The retrieved host info for the module.
        /// </returns>
        IntPtr JsGetModuleHostInfo(JavaScriptModuleRecord requestModule, JavaScriptModuleHostInfoKind moduleHostInfo);

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
        ///   Gets the state of a given Promise object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="promise">
        ///     The Promise object.
        /// </param>
        /// <returns>
        ///     The current state of the Promise.
        /// </returns>
        JavaScriptPromiseState JsGetPromiseState(JavaScriptValueSafeHandle promise);

        /// <summary>
        ///   Gets the result of a given Promise object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="promise">
        ///     The Promise object.
        /// </param>
        /// <returns>
        ///     The result of the Promise.
        /// </returns>
        JavaScriptValueSafeHandle JsGetPromiseResult(JavaScriptValueSafeHandle promise);

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

        /// <summary>
        ///   Determine if one JavaScript value is less than another JavaScript value.
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
        /// <returns>
        ///     Whether object1 is less than object2.
        /// </returns>
        bool JsLessThan(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2);

        /// <summary>
        ///   Determine if one JavaScript value is less than or equal to another JavaScript value.
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
        /// <returns>
        ///     Whether object1 is less than or equal to object2.
        /// </returns>
        bool JsLessThanOrEqual(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2);

        /// <summary>
        ///   Creates a new object (with prototype) that stores some external data.
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
        /// <param name="prototype">
        ///     Prototype object or nullptr.
        /// </param>
        /// <returns>
        ///     The new object.
        /// </returns>
        JavaScriptValueSafeHandle JsCreateExternalObjectWithPrototype(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback, JavaScriptValueSafeHandle prototype);

        /// <summary>
        ///   Gets an object's property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that contains the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString or JavascriptSymbol) to the property.
        /// </param>
        /// <returns>
        ///     The value of the property.
        /// </returns>
        JavaScriptValueSafeHandle JsObjectGetProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key);

        /// <summary>
        ///   Puts an object's property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that contains the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString or JavascriptSymbol) to the property.
        /// </param>
        /// <param name="value">
        ///     The new value of the property.
        /// </param>
        /// <param name="useStrictRules">
        ///     The property set should follow strict mode rules.
        /// </param>
        void JsObjectSetProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key, JavaScriptValueSafeHandle value, bool useStrictRules);

        /// <summary>
        ///   Determines whether an object has a property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that may contain the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString or JavascriptSymbol) to the property.
        /// </param>
        /// <returns>
        ///     Whether the object (or a prototype) has the property.
        /// </returns>
        bool JsObjectHasProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key);

        /// <summary>
        ///   Defines a new object's own property from a property descriptor.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that has the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString or JavascriptSymbol) to the property.
        /// </param>
        /// <param name="propertyDescriptor">
        ///     The property descriptor.
        /// </param>
        /// <returns>
        ///     Whether the property was defined.
        /// </returns>
        bool JsObjectDefineProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key, JavaScriptValueSafeHandle propertyDescriptor);

        /// <summary>
        ///   Deletes an object's property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that contains the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString or JavascriptSymbol) to the property.
        /// </param>
        /// <param name="useStrictRules">
        ///     The property set should follow strict mode rules.
        /// </param>
        /// <returns>
        ///     Whether the property was deleted.
        /// </returns>
        JavaScriptValueSafeHandle JsObjectDeleteProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key, bool useStrictRules);

        /// <summary>
        ///   Gets a property descriptor for an object's own property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that has the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString or JavascriptSymbol) to the property.
        /// </param>
        /// <returns>
        ///     The property descriptor.
        /// </returns>
        JavaScriptValueSafeHandle JsObjectGetOwnPropertyDescriptor(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key);

        /// <summary>
        ///   Determines whether an object has a non-inherited property.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="@object">
        ///     The object that may contain the property.
        /// </param>
        /// <param name="key">
        ///     The key (JavascriptString or JavascriptSymbol) to the property.
        /// </param>
        /// <returns>
        ///     Whether the object has the non-inherited property.
        /// </returns>
        bool JsObjectHasOwnProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle key);

        /// <summary>
        ///   Sets whether any action should be taken when a promise is rejected with no reactions or a reaction is added to a promise that was rejected before it had reactions. By default in either of these cases nothing occurs. This function allows you to specify if something should occur and provide a callback to implement whatever should occur.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="promiseRejectionTrackerCallback">
        ///     The callback function being set.
        /// </param>
        /// <param name="callbackState">
        ///     User provided state that will be passed back to the callback.
        /// </param>
        void JsSetHostPromiseRejectionTracker(JavaScriptPromiseRejectionTrackerCallback promiseRejectionTrackerCallback, IntPtr callbackState);

        /// <summary>
        ///   Retrieve the namespace object for a module.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context and that the module has already been evaluated.
        /// </remarks>
        /// <param name="requestModule">
        ///     The JsModuleRecord for which the namespace is being requested.
        /// </param>
        /// <returns>
        ///     A JsValueRef - the requested namespace object.
        /// </returns>
        JavaScriptValueSafeHandle JsGetModuleNamespace(JavaScriptModuleRecord requestModule);

        /// <summary>
        ///   Determines if a provided object is a JavscriptProxy Object and provides references to a Proxy's target and handler.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        ///     If object is not a Proxy object the target and handler parameters are not touched.
        ///     If nullptr is supplied for target or handler the function returns after
        ///     setting the isProxy value.
        ///     If the object is a revoked Proxy target and handler are set to JS_INVALID_REFERENCE.
        ///     If it is a Proxy object that has not been revoked target and handler are set to the
        ///     the object's target and handler.
        /// </remarks>
        /// <param name="@object">
        ///     The object that may be a Proxy.
        /// </param>
        /// <param name="target">
        ///     Pointer to a JsValueRef - the object's target.
        /// </param>
        /// <param name="handler">
        ///     Pointer to a JsValueRef - the object's handler.
        /// </param>
        /// <returns>
        ///     Pointer to a Boolean - is the object a proxy?
        /// </returns>
        bool JsGetProxyProperties(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle target, out JavaScriptValueSafeHandle handler);

        /// <summary>
        ///   Parses a script and stores the generated parser state cache into a buffer which can be reused.
        /// </summary>
        /// <remarks>
        ///     JsSerializeParserState parses a script and then stores a cache of the parser state
        ///     in a runtime-independent format. The parser state may be deserialized in any runtime along
        ///     with the same script to skip the initial parse phase.
        ///     Requires an active script context.
        ///     Script source can be either JavascriptString or JavascriptExternalArrayBuffer.
        ///     In case it is an ExternalArrayBuffer, and the encoding of the buffer is Utf16,
        ///     JsParseScriptAttributeArrayBufferIsUtf16Encoded is expected on parseAttributes.
        ///     Use JavascriptExternalArrayBuffer with Utf8/ASCII script source
        ///     for better performance and smaller memory footprint.
        /// </remarks>
        /// <param name="scriptVal">
        ///     The script to parse.
        /// </param>
        /// <param name="parseAttributes">
        ///     Encoding for the script.
        /// </param>
        /// <returns>
        ///     The buffer to put the serialized parser state cache into.
        /// </returns>
        JavaScriptValueSafeHandle JsSerializeParserState(JavaScriptValueSafeHandle scriptVal, JavaScriptParseScriptAttributes parseAttributes);

        /// <summary>
        ///   Deserializes the cache of initial parser state and (along with the same script source) executes the script and returns the result.
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
        /// <param name="parserState">
        ///     A buffer containing a cache of the parser state generated by JsSerializeParserState.
        /// </param>
        /// <returns>
        ///     The result of the script, if any. This parameter can be null.
        /// </returns>
        JavaScriptValueSafeHandle JsRunScriptWithParserState(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes, JavaScriptValueSafeHandle parserState);

    }
}
