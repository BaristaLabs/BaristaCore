namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
	using System;
	using System.Runtime.InteropServices;

	///<summary>
	///	   Represents the Unmanaged ChakraCore Library
	///</summary>
	public static class LibChakraCore
	{
		const string DllName = "ChakraCore/ChakraCore";

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsInitializeModuleRecord(IntPtr referencingModule, JavaScriptValueSafeHandle normalizedSpecifier, out IntPtr moduleRecord);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsParseModuleSource(IntPtr requestModule, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags sourceFlag, out JavaScriptValueSafeHandle exceptionValueRef);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsModuleEvaluation(IntPtr requestModule, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Set the host info for the specified module.
		/// </summary>
		/// <param name="requestModule">The request module.</param>
		/// <param name="moduleHostInfo">The type of host info to be set.</param>
		/// <param name="hostInfo">The host info to be set.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, IntPtr hostInfo);

		/// <summary>
		///     Retrieve the host info for the specified module.
		/// </summary>
		/// <param name="requestModule">The request module.</param>
		/// <param name="moduleHostInfo">The type of host info to get.</param>
		/// <param name="hostInfo">The host info to be retrieved.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, out IntPtr hostInfo);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateString(string content, UIntPtr length, out JavaScriptValueSafeHandle value);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateStringUtf8(string content, UIntPtr length, out JavaScriptValueSafeHandle value);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsCreateStringUtf16(string content, UIntPtr length, out JavaScriptValueSafeHandle value);

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
		[DllImport(DllName, CharSet = CharSet.Ansi)]
		public static extern JavaScriptErrorCode JsCopyString(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out UIntPtr written);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCopyStringUtf8(JavaScriptValueSafeHandle value, byte[] buffer, UIntPtr bufferSize, out UIntPtr written);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCopyStringUtf16(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out UIntPtr written);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsParse(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsRun(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreatePropertyIdUtf8(string name, UIntPtr length, out JavaScriptPropertyIdSafeHandle propertyId);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCopyPropertyIdUtf8(JavaScriptPropertyIdSafeHandle propertyId, byte[] buffer, UIntPtr bufferSize, out UIntPtr length);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSerialize(JavaScriptValueSafeHandle script, byte[] buffer, ref ulong bufferSize, JavaScriptParseScriptAttributes parseAttributes);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsParseSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsRunSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Creates a new runtime.
		/// </summary>
		/// <param name="attributes">The attributes of the runtime to be created.</param>
		/// <param name="threadService">The thread service for the runtime. Can be null.</param>
		/// <param name="runtime">The runtime created.</param>
		/// <remarks>In the edge-mode binary, chakra.dll, this function lacks the <c>runtimeVersion</c>
		/// parameter (compare to jsrt9.h).</remarks>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateRuntime(JavaScriptRuntimeAttributes attributes, JavaScriptThreadServiceCallback threadService, out JavaScriptRuntimeSafeHandle runtime);

		/// <summary>
		///     Performs a full garbage collection.
		/// </summary>
		/// <param name="runtime">The runtime in which the garbage collection will be performed.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCollectGarbage(JavaScriptRuntimeSafeHandle handle);

		/// <summary>
		///     Disposes a runtime.
		/// </summary>
		/// <remarks>
		///     Once a runtime has been disposed, all resources owned by it are invalid and cannot be used.
		///     If the runtime is active (i.e. it is set to be current on a particular thread), it cannot
		///     be disposed.
		/// </remarks>
		/// <param name="runtime">The runtime to dispose.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsDisposeRuntime(IntPtr runtime);

		/// <summary>
		///     Gets the current memory usage for a runtime.
		/// </summary>
		/// <remarks>
		///     Memory usage can be always be retrieved, regardless of whether or not the runtime is active
		///     on another thread.
		/// </remarks>
		/// <param name="runtime">The runtime whose memory usage is to be retrieved.</param>
		/// <param name="memoryUsage">The runtime's current memory usage, in bytes.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetRuntimeMemoryUsage(JavaScriptRuntimeSafeHandle runtime, out ulong usage);

		/// <summary>
		///     Gets the current memory limit for a runtime.
		/// </summary>
		/// <remarks>
		///     The memory limit of a runtime can be always be retrieved, regardless of whether or not the
		///     runtime is active on another thread.
		/// </remarks>
		/// <param name="runtime">The runtime whose memory limit is to be retrieved.</param>
		/// <param name="memoryLimit">
		///     The runtime's current memory limit, in bytes, or -1 if no limit has been set.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, out ulong memoryLimit);

		/// <summary>
		///     Sets the current memory limit for a runtime.
		/// </summary>
		/// <remarks>
		///     <para>
		///     A memory limit will cause any operation which exceeds the limit to fail with an "out of
		///     memory" error. Setting a runtime's memory limit to -1 means that the runtime has no memory
		///     limit. New runtimes  default to having no memory limit. If the new memory limit exceeds
		///     current usage, the call will succeed and any future allocations in this runtime will fail
		///     until the runtime's memory usage drops below the limit.
		///     </para>
		///     <para>
		///     A runtime's memory limit can be always be set, regardless of whether or not the runtime is
		///     active on another thread.
		///     </para>
		/// </remarks>
		/// <param name="runtime">The runtime whose memory limit is to be set.</param>
		/// <param name="memoryLimit">
		///     The new runtime memory limit, in bytes, or -1 for no memory limit.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, ulong memoryLimit);

		/// <summary>
		///     Sets a memory allocation callback for specified runtime
		/// </summary>
		/// <remarks>
		///     <para>
		///     Registering a memory allocation callback will cause the runtime to call back to the host
		///     whenever it acquires memory from, or releases memory to, the OS. The callback routine is
		///     called before the runtime memory manager allocates a block of memory. The allocation will
		///     be rejected if the callback returns false. The runtime memory manager will also invoke the
		///     callback routine after freeing a block of memory, as well as after allocation failures.
		///     </para>
		///     <para>
		///     The callback is invoked on the current runtime execution thread, therefore execution is
		///     blocked until the callback completes.
		///     </para>
		///     <para>
		///     The return value of the callback is not stored; previously rejected allocations will not
		///     prevent the runtime from invoking the callback again later for new memory allocations.
		///     </para>
		/// </remarks>
		/// <param name="runtime">The runtime for which to register the allocation callback.</param>
		/// <param name="callbackState">
		///     User provided state that will be passed back to the callback.
		/// </param>
		/// <param name="allocationCallback">
		///     Memory allocation callback to be called for memory allocation events.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetRuntimeMemoryAllocationCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr extraInformation, JavaScriptMemoryAllocationCallback allocationCallback);

		/// <summary>
		///     Sets a callback function that is called by the runtime before garbage collection.
		/// </summary>
		/// <remarks>
		///     <para>
		///     The callback is invoked on the current runtime execution thread, therefore execution is
		///     blocked until the callback completes.
		///     </para>
		///     <para>
		///     The callback can be used by hosts to prepare for garbage collection. For example, by
		///     releasing unnecessary references on Chakra objects.
		///     </para>
		/// </remarks>
		/// <param name="runtime">The runtime for which to register the allocation callback.</param>
		/// <param name="callbackState">
		///     User provided state that will be passed back to the callback.
		/// </param>
		/// <param name="beforeCollectCallback">The callback function being set.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetRuntimeBeforeCollectCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptBeforeCollectCallback beforeCollectCallback);

		/// <summary>
		///     Adds a reference to a garbage collected object.
		/// </summary>
		/// <remarks>
		///     This only needs to be called on <c>JsRef</c> handles that are not going to be stored
		///     somewhere on the stack. Calling <c>JsAddRef</c> ensures that the object the <c>JsRef</c>
		///     refers to will not be freed until <c>JsRelease</c> is called.
		/// </remarks>
		/// <param name="ref">The object to add a reference to.</param>
		/// <param name="count">The object's new reference count (can pass in null).</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsAddRef(IntPtr @ref, out uint count);

		/// <summary>
		///     Releases a reference to a garbage collected object.
		/// </summary>
		/// <remarks>
		///     Removes a reference to a <c>JsRef</c> handle that was created by <c>JsAddRef</c>.
		/// </remarks>
		/// <param name="value">The object to add a reference to.</param>
		/// <param name="count">The object's new reference count (can pass in null).</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName, EntryPoint = "JsRelease" )]
		public static extern JavaScriptErrorCode JsReleaseContext(JavaScriptContextSafeHandle context, out uint count);

		/// <summary>
		///     Releases a reference to a garbage collected object.
		/// </summary>
		/// <remarks>
		///     Removes a reference to a <c>JsRef</c> handle that was created by <c>JsAddRef</c>.
		/// </remarks>
		/// <param name="value">The object to add a reference to.</param>
		/// <param name="count">The object's new reference count (can pass in null).</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName, EntryPoint = "JsRelease" )]
		public static extern JavaScriptErrorCode JsReleasePropertyId(JavaScriptPropertyIdSafeHandle context, out uint count);

		/// <summary>
		///     Releases a reference to a garbage collected object.
		/// </summary>
		/// <remarks>
		///     Removes a reference to a <c>JsRef</c> handle that was created by <c>JsAddRef</c>.
		/// </remarks>
		/// <param name="value">The object to add a reference to.</param>
		/// <param name="count">The object's new reference count (can pass in null).</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName, EntryPoint = "JsRelease" )]
		public static extern JavaScriptErrorCode JsReleaseValue(JavaScriptValueSafeHandle value, out uint count);

		/// <summary>
		///     Releases a reference to a garbage collected object.
		/// </summary>
		/// <remarks>
		///     Removes a reference to a <c>JsRef</c> handle that was created by <c>JsAddRef</c>.
		/// </remarks>
		/// <param name="ref">The object to add a reference to.</param>
		/// <param name="count">The object's new reference count (can pass in null).</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsRelease(IntPtr @ref, out uint count);

		/// <summary>
		///     Sets a callback function that is called by the runtime before garbage collection of
		///     an object.
		/// </summary>
		/// <remarks>
		///     <para>
		///     The callback is invoked on the current runtime execution thread, therefore execution is
		///     blocked until the callback completes.
		///     </para>
		/// </remarks>
		/// <param name="ref">The object for which to register the callback.</param>
		/// <param name="callbackState">
		///     User provided state that will be passed back to the callback.
		/// </param>
		/// <param name="objectBeforeCollectCallback">The callback function being set. Use null to clear
		///     previously registered callback.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetObjectBeforeCollectCallback(JavaScriptValueSafeHandle @ref, IntPtr callbackState, JavaScriptObjectBeforeCollectCallback objectBeforeCollectCallback);

		/// <summary>
		///     Creates a script context for running scripts.
		/// </summary>
		/// <remarks>
		///     Each script context has its own global object that is isolated from all other script
		///     contexts.
		/// </remarks>
		/// <param name="runtime">The runtime the script context is being created in.</param>
		/// <param name="newContext">The created script context.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateContext(JavaScriptRuntimeSafeHandle runtime, out JavaScriptContextSafeHandle context);

		/// <summary>
		///     Gets the current script context on the thread.
		/// </summary>
		/// <param name="currentContext">
		///     The current script context on the thread, null if there is no current script context.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetCurrentContext(out JavaScriptContextSafeHandle context);

		/// <summary>
		///     Sets the current script context on the thread.
		/// </summary>
		/// <param name="context">The script context to make current.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetCurrentContext(JavaScriptContextSafeHandle context);

		/// <summary>
		///     Gets the script context that the object belongs to.
		/// </summary>
		/// <param name="object">The object to get the context from.</param>
		/// <param name="context">The context the object belongs to.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetContextOfObject(JavaScriptValueSafeHandle @object, out JavaScriptContextSafeHandle context);

		/// <summary>
		///     Gets the internal data set on JsrtContext.
		/// </summary>
		/// <param name="context">The context to get the data from.</param>
		/// <param name="data">The pointer to the data where data will be returned.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetContextData(JavaScriptContextSafeHandle context, out IntPtr data);

		/// <summary>
		///     Sets the internal data of JsrtContext.
		/// </summary>
		/// <param name="context">The context to set the data to.</param>
		/// <param name="data">The pointer to the data to be set.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetContextData(JavaScriptContextSafeHandle context, IntPtr data);

		/// <summary>
		///     Gets the runtime that the context belongs to.
		/// </summary>
		/// <param name="context">The context to get the runtime from.</param>
		/// <param name="runtime">The runtime the context belongs to.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetRuntime(JavaScriptContextSafeHandle context, out JavaScriptRuntimeSafeHandle runtime);

		/// <summary>
		///     Tells the runtime to do any idle processing it need to do.
		/// </summary>
		/// <remarks>
		///     <para>
		///     If idle processing has been enabled for the current runtime, calling <c>JsIdle</c> will
		///     inform the current runtime that the host is idle and that the runtime can perform
		///     memory cleanup tasks.
		///     </para>
		///     <para>
		///     <c>JsIdle</c> can also return the number of system ticks until there will be more idle work
		///     for the runtime to do. Calling <c>JsIdle</c> before this number of ticks has passed will do
		///     no work.
		///     </para>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="nextIdleTick">
		///     The next system tick when there will be more idle work to do. Can be null. Returns the
		///     maximum number of ticks if there no upcoming idle work to do.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsIdle(out uint nextIdleTick);

		/// <summary>
		///     Gets the symbol associated with the property ID.
		/// </summary>
		/// <remarks>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="propertyId">The property ID to get the symbol of.</param>
		/// <param name="symbol">The symbol associated with the property ID.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetSymbolFromPropertyId(JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptValueSafeHandle symbol);

		/// <summary>
		///     Gets the type of property
		/// </summary>
		/// <remarks>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="propertyId">The property ID to get the type of.</param>
		/// <param name="propertyIdType">The JsPropertyIdType of the given property ID</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetPropertyIdType(JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptPropertyIdType propertyIdType);

		/// <summary>
		///     Gets the property ID associated with the symbol.
		/// </summary>
		/// <remarks>
		///     <para>
		///     Property IDs are specific to a context and cannot be used across contexts.
		///     </para>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="symbol">
		///     The symbol whose property ID is being retrieved.
		/// </param>
		/// <param name="propertyId">The property ID for the given symbol.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetPropertyIdFromSymbol(JavaScriptValueSafeHandle symbol, out JavaScriptPropertyIdSafeHandle propertyId);

		/// <summary>
		///     Creates a Javascript symbol.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="description">The string description of the symbol. Can be null.</param>
		/// <param name="result">The new symbol.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateSymbol(JavaScriptValueSafeHandle description, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Gets the list of all symbol properties on the object.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object from which to get the property symbols.</param>
		/// <param name="propertySymbols">An array of property symbols.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetOwnPropertySymbols(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle propertySymbols);

		/// <summary>
		///     Gets the value of <c>undefined</c> in the current script context.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="undefinedValue">The <c>undefined</c> value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetUndefinedValue(out JavaScriptValueSafeHandle undefinedValue);

		/// <summary>
		///     Gets the value of <c>null</c> in the current script context.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="nullValue">The <c>null</c> value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetNullValue(out JavaScriptValueSafeHandle nullValue);

		/// <summary>
		///     Gets the value of <c>true</c> in the current script context.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="trueValue">The <c>true</c> value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetTrueValue(out JavaScriptValueSafeHandle trueValue);

		/// <summary>
		///     Gets the value of <c>false</c> in the current script context.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="falseValue">The <c>false</c> value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetFalseValue(out JavaScriptValueSafeHandle falseValue);

		/// <summary>
		///     Creates a Boolean value from a <c>bool</c> value.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="value">The value to be converted.</param>
		/// <param name="booleanValue">The converted value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsBoolToBoolean(bool value, out JavaScriptValueSafeHandle booleanValue);

		/// <summary>
		///     Retrieves the <c>bool</c> value of a Boolean value.
		/// </summary>
		/// <param name="value">The value to be converted.</param>
		/// <param name="boolValue">The converted value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsBooleanToBool(JavaScriptValueSafeHandle value, out bool boolValue);

		/// <summary>
		///     Converts the value to Boolean using standard JavaScript semantics.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="value">The value to be converted.</param>
		/// <param name="booleanValue">The converted value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsConvertValueToBoolean(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle booleanValue);

		/// <summary>
		///     Gets the JavaScript type of a JsValueRef.
		/// </summary>
		/// <param name="value">The value whose type is to be returned.</param>
		/// <param name="type">The type of the value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetValueType(JavaScriptValueSafeHandle value, out JavaScriptValueType type);

		/// <summary>
		///     Creates a number value from a <c>double</c> value.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="doubleValue">The <c>double</c> to convert to a number value.</param>
		/// <param name="value">The new number value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsDoubleToNumber(double doubleValue, out JavaScriptValueSafeHandle value);

		/// <summary>
		///     Creates a number value from an <c>int</c> value.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="intValue">The <c>int</c> to convert to a number value.</param>
		/// <param name="value">The new number value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsIntToNumber(int intValue, out JavaScriptValueSafeHandle value);

		/// <summary>
		///     Retrieves the <c>double</c> value of a number value.
		/// </summary>
		/// <remarks>
		///     This function retrieves the value of a number value. It will fail with
		///     <c>JsErrorInvalidArgument</c> if the type of the value is not number.
		/// </remarks>
		/// <param name="value">The number value to convert to a <c>double</c> value.</param>
		/// <param name="doubleValue">The <c>double</c> value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsNumberToDouble(JavaScriptValueSafeHandle value, out double doubleValue);

		/// <summary>
		///     Retrieves the <c>int</c> value of a number value.
		/// </summary>
		/// <remarks>
		///     This function retrieves the value of a number value and converts to an <c>int</c> value.
		///     It will fail with <c>JsErrorInvalidArgument</c> if the type of the value is not number.
		/// </remarks>
		/// <param name="value">The number value to convert to an <c>int</c> value.</param>
		/// <param name="intValue">The <c>int</c> value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsNumberToInt(JavaScriptValueSafeHandle value, out int intValue);

		/// <summary>
		///     Converts the value to number using standard JavaScript semantics.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="value">The value to be converted.</param>
		/// <param name="numberValue">The converted value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsConvertValueToNumber(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle numberValue);

		/// <summary>
		///     Gets the length of a string value.
		/// </summary>
		/// <param name="stringValue">The string value to get the length of.</param>
		/// <param name="length">The length of the string.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetStringLength(JavaScriptValueSafeHandle stringValue, out int length);

		/// <summary>
		///     Converts the value to string using standard JavaScript semantics.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="value">The value to be converted.</param>
		/// <param name="stringValue">The converted value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsConvertValueToString(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle stringValue);

		/// <summary>
		///     Gets the global object in the current script context.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="globalObject">The global object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetGlobalObject(out JavaScriptValueSafeHandle globalObject);

		/// <summary>
		///     Creates a new object.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The new object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateObject(out JavaScriptValueSafeHandle @object);

		/// <summary>
		///     Creates a new object that stores some external data.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="data">External data that the object will represent. May be null.</param>
		/// <param name="finalizeCallback">
		///     A callback for when the object is finalized. May be null.
		/// </param>
		/// <param name="object">The new object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback, out JavaScriptValueSafeHandle @object);

		/// <summary>
		///     Converts the value to object using standard JavaScript semantics.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="value">The value to be converted.</param>
		/// <param name="object">The converted value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsConvertValueToObject(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle @object);

		/// <summary>
		///     Returns the prototype of an object.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object whose prototype is to be returned.</param>
		/// <param name="prototypeObject">The object's prototype.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetPrototype(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle prototypeObject);

		/// <summary>
		///     Sets the prototype of an object.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object whose prototype is to be changed.</param>
		/// <param name="prototypeObject">The object's new prototype.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetPrototype(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle prototypeObject);

		/// <summary>
		///     Performs JavaScript "instanceof" operator test.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object to test.</param>
		/// <param name="constructor">The constructor function to test against.</param>
		/// <param name="result">Whether "object instanceof constructor" is true.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsInstanceOf(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle constructor, out bool result);

		/// <summary>
		///     Returns a value that indicates whether an object is extensible or not.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object to test.</param>
		/// <param name="value">Whether the object is extensible or not.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetExtensionAllowed(JavaScriptValueSafeHandle @object, out bool value);

		/// <summary>
		///     Makes an object non-extensible.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object to make non-extensible.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsPreventExtension(JavaScriptValueSafeHandle @object);

		/// <summary>
		///     Gets an object's property.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object that contains the property.</param>
		/// <param name="propertyId">The ID of the property.</param>
		/// <param name="value">The value of the property.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptValueSafeHandle value);

		/// <summary>
		///     Gets a property descriptor for an object's own property.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object that has the property.</param>
		/// <param name="propertyId">The ID of the property.</param>
		/// <param name="propertyDescriptor">The property descriptor.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetOwnPropertyDescriptor(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptValueSafeHandle propertyDescriptor);

		/// <summary>
		///     Gets the list of all properties on the object.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object from which to get the property names.</param>
		/// <param name="propertyNames">An array of property names.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetOwnPropertyNames(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle propertyNames);

		/// <summary>
		///     Puts an object's property.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object that contains the property.</param>
		/// <param name="propertyId">The ID of the property.</param>
		/// <param name="value">The new value of the property.</param>
		/// <param name="useStrictRules">The property set should follow strict mode rules.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle value, bool useStrictRules);

		/// <summary>
		///     Determines whether an object has a property.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object that may contain the property.</param>
		/// <param name="propertyId">The ID of the property.</param>
		/// <param name="hasProperty">Whether the object (or a prototype) has the property.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsHasProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, out bool hasProperty);

		/// <summary>
		///     Deletes an object's property.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object that contains the property.</param>
		/// <param name="propertyId">The ID of the property.</param>
		/// <param name="useStrictRules">The property set should follow strict mode rules.</param>
		/// <param name="result">Whether the property was deleted.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsDeleteProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, bool useStrictRules, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Defines a new object's own property from a property descriptor.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object that has the property.</param>
		/// <param name="propertyId">The ID of the property.</param>
		/// <param name="propertyDescriptor">The property descriptor.</param>
		/// <param name="result">Whether the property was defined.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsDefineProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle propertyDescriptor, out bool result);

		/// <summary>
		///     Tests whether an object has a value at the specified index.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object to operate on.</param>
		/// <param name="index">The index to test.</param>
		/// <param name="result">Whether the object has a value at the specified index.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsHasIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, out bool result);

		/// <summary>
		///     Retrieve the value at the specified index of an object.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object to operate on.</param>
		/// <param name="index">The index to retrieve.</param>
		/// <param name="result">The retrieved value.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Set the value at the specified index of an object.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object to operate on.</param>
		/// <param name="index">The index to set.</param>
		/// <param name="value">The value to set.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, JavaScriptValueSafeHandle value);

		/// <summary>
		///     Delete the value at the specified index of an object.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object to operate on.</param>
		/// <param name="index">The index to delete.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsDeleteIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index);

		/// <summary>
		///     Determines whether an object has its indexed properties in external data.
		/// </summary>
		/// <param name="object">The object.</param>
		/// <param name="value">Whether the object has its indexed properties in external data.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsHasIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, out bool value);

		/// <summary>
		///     Retrieves an object's indexed properties external data information.
		/// </summary>
		/// <param name="object">The object.</param>
		/// <param name="data">The external data back store for the object's indexed properties.</param>
		/// <param name="arrayType">The array element type in external data.</param>
		/// <param name="elementLength">The number of array elements in external data.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, IntPtr data, out JavaScriptTypedArrayType arrayType, out uint elementLength);

		/// <summary>
		///     Sets an object's indexed properties to external data. The external data will be used as back
		///     store for the object's indexed properties and accessed like a typed array.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="object">The object to operate on.</param>
		/// <param name="data">The external data to be used as back store for the object's indexed properties.</param>
		/// <param name="arrayType">The array element type in external data.</param>
		/// <param name="elementLength">The number of array elements in external data.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetIndexedPropertiesToExternalData(JavaScriptValueSafeHandle @object, IntPtr Data, JavaScriptTypedArrayType arrayType, uint elementLength);

		/// <summary>
		///     Compare two JavaScript values for equality.
		/// </summary>
		/// <remarks>
		///     <para>
		///     This function is equivalent to the <c>==</c> operator in Javascript.
		///     </para>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="object1">The first object to compare.</param>
		/// <param name="object2">The second object to compare.</param>
		/// <param name="result">Whether the values are equal.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result);

		/// <summary>
		///     Compare two JavaScript values for strict equality.
		/// </summary>
		/// <remarks>
		///     <para>
		///     This function is equivalent to the <c>===</c> operator in Javascript.
		///     </para>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="object1">The first object to compare.</param>
		/// <param name="object2">The second object to compare.</param>
		/// <param name="result">Whether the values are strictly equal.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsStrictEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result);

		/// <summary>
		///     Determines whether an object is an external object.
		/// </summary>
		/// <param name="object">The object.</param>
		/// <param name="value">Whether the object is an external object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsHasExternalData(JavaScriptValueSafeHandle @object, out bool value);

		/// <summary>
		///     Retrieves the data from an external object.
		/// </summary>
		/// <param name="object">The external object.</param>
		/// <param name="externalData">
		///     The external data stored in the object. Can be null if no external data is stored in the
		///     object.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetExternalData(JavaScriptValueSafeHandle @object, out IntPtr externalData);

		/// <summary>
		///     Sets the external data on an external object.
		/// </summary>
		/// <param name="object">The external object.</param>
		/// <param name="externalData">
		///     The external data to be stored in the object. Can be null if no external data is
		///     to be stored in the object.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetExternalData(JavaScriptValueSafeHandle @object, IntPtr externalData);

		/// <summary>
		///     Creates a Javascript array object.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="length">The initial length of the array.</param>
		/// <param name="result">The new array object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateArray(uint length, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Creates a Javascript ArrayBuffer object.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="byteLength">
		///     The number of bytes in the ArrayBuffer.
		/// </param>
		/// <param name="result">The new ArrayBuffer object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateArrayBuffer(uint byteLength, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Creates a Javascript ArrayBuffer object to access external memory.
		/// </summary>
		/// <remarks>Requires an active script context.</remarks>
		/// <param name="data">A pointer to the external memory.</param>
		/// <param name="byteLength">The number of bytes in the external memory.</param>
		/// <param name="finalizeCallback">A callback for when the object is finalized. May be null.</param>
		/// <param name="callbackState">User provided state that will be passed back to finalizeCallback.</param>
		/// <param name="result">The new ArrayBuffer object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateExternalArrayBuffer(IntPtr data, uint byteLength, JavaScriptObjectFinalizeCallback finalizeCallback, IntPtr callbackState, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Creates a Javascript typed array object.
		/// </summary>
		/// <remarks>
		///     <para>
		///     The <c>baseArray</c> can be an <c>ArrayBuffer</c>, another typed array, or a JavaScript
		///     <c>Array</c>. The returned typed array will use the baseArray if it is an ArrayBuffer, or
		///     otherwise create and use a copy of the underlying source array.
		///     </para>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="arrayType">The type of the array to create.</param>
		/// <param name="baseArray">
		///     The base array of the new array. Use <c>JS_INVALID_REFERENCE</c> if no base array.
		/// </param>
		/// <param name="byteOffset">
		///     The offset in bytes from the start of baseArray (ArrayBuffer) for result typed array to reference.
		///     Only applicable when baseArray is an ArrayBuffer object. Must be 0 otherwise.
		/// </param>
		/// <param name="elementLength">
		///     The number of elements in the array. Only applicable when creating a new typed array without
		///     baseArray (baseArray is <c>JS_INVALID_REFERENCE</c>) or when baseArray is an ArrayBuffer object.
		///     Must be 0 otherwise.
		/// </param>
		/// <param name="result">The new typed array object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateTypedArray(JavaScriptTypedArrayType arrayType, JavaScriptValueSafeHandle baseArray, uint byteOffset, uint elementLength, out JavaScriptValueSafeHandle result);

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
		/// <param name="result">The new DataView object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateDataView(JavaScriptValueSafeHandle arrayBuffer, uint byteOffset, uint byteLength, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Obtains frequently used properties of a typed array.
		/// </summary>
		/// <param name="typedArray">The typed array instance.</param>
		/// <param name="arrayType">The type of the array.</param>
		/// <param name="arrayBuffer">The ArrayBuffer backstore of the array.</param>
		/// <param name="byteOffset">The offset in bytes from the start of arrayBuffer referenced by the array.</param>
		/// <param name="byteLength">The number of bytes in the array.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetTypedArrayInfo(JavaScriptValueSafeHandle typedArray, out JavaScriptTypedArrayType arrayType, out JavaScriptValueSafeHandle arrayBuffer, out uint byteOffset, out uint byteLength);

		/// <summary>
		///     Obtains the underlying memory storage used by an <c>ArrayBuffer</c>.
		/// </summary>
		/// <param name="arrayBuffer">The ArrayBuffer instance.</param>
		/// <param name="buffer">
		///     The ArrayBuffer's buffer. The lifetime of the buffer returned is the same as the lifetime of the
		///     the ArrayBuffer. The buffer pointer does not count as a reference to the ArrayBuffer for the purpose
		///     of garbage collection.
		/// </param>
		/// <param name="bufferLength">The number of bytes in the buffer.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetArrayBufferStorage(JavaScriptValueSafeHandle arrayBuffer, out byte[] buffer, out uint bufferLength);

		/// <summary>
		///     Obtains the underlying memory storage used by a typed array.
		/// </summary>
		/// <param name="typedArray">The typed array instance.</param>
		/// <param name="buffer">
		///     The array's buffer. The lifetime of the buffer returned is the same as the lifetime of the
		///     the array. The buffer pointer does not count as a reference to the array for the purpose
		///     of garbage collection.
		/// </param>
		/// <param name="bufferLength">The number of bytes in the buffer.</param>
		/// <param name="arrayType">The type of the array.</param>
		/// <param name="elementSize">
		///     The size of an element of the array.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetTypedArrayStorage(JavaScriptValueSafeHandle typedArray, out byte[] buffer, out uint bufferLength, out JavaScriptTypedArrayType arrayType, out int elementSize);

		/// <summary>
		///     Obtains the underlying memory storage used by a DataView.
		/// </summary>
		/// <param name="dataView">The DataView instance.</param>
		/// <param name="buffer">
		///     The DataView's buffer. The lifetime of the buffer returned is the same as the lifetime of the
		///     the DataView. The buffer pointer does not count as a reference to the DataView for the purpose
		///     of garbage collection.
		/// </param>
		/// <param name="bufferLength">The number of bytes in the buffer.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetDataViewStorage(JavaScriptValueSafeHandle dataView, out byte[] buffer, out uint bufferLength);

		/// <summary>
		///     Invokes a function.
		/// </summary>
		/// <remarks>
		///     Requires thisArg as first argument of arguments.
		///     Requires an active script context.
		/// </remarks>
		/// <param name="function">The function to invoke.</param>
		/// <param name="arguments">The arguments to the call.</param>
		/// <param name="argumentCount">The number of arguments being passed in to the function.</param>
		/// <param name="result">The value returned from the function invocation, if any.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCallFunction(JavaScriptValueSafeHandle @function, IntPtr[] arguments, ushort argumentCount, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Invokes a function as a constructor.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="function">The function to invoke as a constructor.</param>
		/// <param name="arguments">The arguments to the call.</param>
		/// <param name="argumentCount">The number of arguments being passed in to the function.</param>
		/// <param name="result">The value returned from the function invocation.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsConstructObject(JavaScriptValueSafeHandle @function, IntPtr[] arguments, ushort argumentCount, out JavaScriptValueSafeHandle result);

		/// <summary>
		///     Creates a new JavaScript function.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="nativeFunction">The method to call when the function is invoked.</param>
		/// <param name="callbackState">
		///     User provided state that will be passed back to the callback.
		/// </param>
		/// <param name="function">The new function object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateFunction(JavaScriptNativeFunction nativeFunction, IntPtr callbackState, out JavaScriptValueSafeHandle @function);

		/// <summary>
		///     Creates a new JavaScript function with name.
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="name">The name of this function that will be used for diagnostics and stringification purposes.</param>
		/// <param name="nativeFunction">The method to call when the function is invoked.</param>
		/// <param name="callbackState">
		///     User provided state that will be passed back to the callback.
		/// </param>
		/// <param name="function">The new function object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateNamedFunction(JavaScriptValueSafeHandle name, JavaScriptNativeFunction nativeFunction, IntPtr callbackState, out JavaScriptValueSafeHandle @function);

		/// <summary>
		///     Creates a new JavaScript error object
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="message">Message for the error object.</param>
		/// <param name="error">The new error object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

		/// <summary>
		///     Creates a new JavaScript RangeError error object
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="message">Message for the error object.</param>
		/// <param name="error">The new error object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateRangeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

		/// <summary>
		///     Creates a new JavaScript ReferenceError error object
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="message">Message for the error object.</param>
		/// <param name="error">The new error object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateReferenceError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

		/// <summary>
		///     Creates a new JavaScript SyntaxError error object
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="message">Message for the error object.</param>
		/// <param name="error">The new error object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateSyntaxError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

		/// <summary>
		///     Creates a new JavaScript TypeError error object
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="message">Message for the error object.</param>
		/// <param name="error">The new error object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateTypeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

		/// <summary>
		///     Creates a new JavaScript URIError error object
		/// </summary>
		/// <remarks>
		///     Requires an active script context.
		/// </remarks>
		/// <param name="message">Message for the error object.</param>
		/// <param name="error">The new error object.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsCreateURIError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

		/// <summary>
		///     Determines whether the runtime of the current context is in an exception state.
		/// </summary>
		/// <remarks>
		///     <para>
		///     If a call into the runtime results in an exception (either as the result of running a
		///     script or due to something like a conversion failure), the runtime is placed into an
		///     "exception state." All calls into any context created by the runtime (except for the
		///     exception APIs) will fail with <c>JsErrorInExceptionState</c> until the exception is
		///     cleared.
		///     </para>
		///     <para>
		///     If the runtime of the current context is in the exception state when a callback returns
		///     into the engine, the engine will automatically rethrow the exception.
		///     </para>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="hasException">
		///     Whether the runtime of the current context is in the exception state.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsHasException(out bool hasException);

		/// <summary>
		///     Returns the exception that caused the runtime of the current context to be in the
		///     exception state and resets the exception state for that runtime.
		/// </summary>
		/// <remarks>
		///     <para>
		///     If the runtime of the current context is not in an exception state, this API will return
		///     <c>JsErrorInvalidArgument</c>. If the runtime is disabled, this will return an exception
		///     indicating that the script was terminated, but it will not clear the exception (the
		///     exception will be cleared if the runtime is re-enabled using
		///     <c>JsEnableRuntimeExecution</c>).
		///     </para>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="exception">The exception for the runtime of the current context.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsGetAndClearException(out JavaScriptValueSafeHandle exception);

		/// <summary>
		///     Sets the runtime of the current context to an exception state.
		/// </summary>
		/// <remarks>
		///     <para>
		///     If the runtime of the current context is already in an exception state, this API will
		///     return <c>JsErrorInExceptionState</c>.
		///     </para>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="exception">
		///     The JavaScript exception to set for the runtime of the current context.
		/// </param>
		/// <returns>
		///     JsNoError if the engine was set into an exception state, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetException(JavaScriptValueSafeHandle exception);

		/// <summary>
		///     Suspends script execution and terminates any running scripts in a runtime.
		/// </summary>
		/// <remarks>
		///     <para>
		///     Calls to a suspended runtime will fail until <c>JsEnableRuntimeExecution</c> is called.
		///     </para>
		///     <para>
		///     This API does not have to be called on the thread the runtime is active on. Although the
		///     runtime will be set into a suspended state, an executing script may not be suspended
		///     immediately; a running script will be terminated with an uncatchable exception as soon as
		///     possible.
		///     </para>
		///     <para>
		///     Suspending execution in a runtime that is already suspended is a no-op.
		///     </para>
		/// </remarks>
		/// <param name="runtime">The runtime to be suspended.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsDisableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime);

		/// <summary>
		///     Enables script execution in a runtime.
		/// </summary>
		/// <remarks>
		///     Enabling script execution in a runtime that already has script execution enabled is a
		///     no-op.
		/// </remarks>
		/// <param name="runtime">The runtime to be enabled.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsEnableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime);

		/// <summary>
		///     Returns a value that indicates whether script execution is disabled in the runtime.
		/// </summary>
		/// <param name="runtime">Specifies the runtime to check if execution is disabled.</param>
		/// <param name="isDisabled">If execution is disabled, <c>true</c>, <c>false</c> otherwise.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsIsRuntimeExecutionDisabled(JavaScriptRuntimeSafeHandle runtime, out bool isDisabled);

		/// <summary>
		///     Sets a promise continuation callback function that is called by the context when a task
		///     needs to be queued for future execution
		/// </summary>
		/// <remarks>
		///     <para>
		///     Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="promiseContinuationCallback">The callback function being set.</param>
		/// <param name="callbackState">
		///     User provided state that will be passed back to the callback.
		/// </param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsSetPromiseContinuationCallback(JavaScriptPromiseContinuationCallback promiseContinuationCallback, IntPtr callbackState);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsParseScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsParseScriptWithAttributes(string script, JavaScriptSourceContext sourceContext, string sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsRunScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsExperimentalApiRunModule(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsSerializeScript(string script, byte[] buffer, ref ulong bufferSize);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsParseSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsRunSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsParseSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsRunSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsGetPropertyIdFromName(string name, out JavaScriptPropertyIdSafeHandle propertyId);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsGetPropertyNameFromId(JavaScriptPropertyIdSafeHandle propertyId, out string name);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsPointerToString(string stringValue, ulong stringLength, out JavaScriptValueSafeHandle value);

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
		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern JavaScriptErrorCode JsStringToPointer(JavaScriptValueSafeHandle value, out IntPtr stringValue, out ulong stringLength);

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
		[DllImport(DllName)]
		public static extern JavaScriptErrorCode JsDiagStartDebugging(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagDebugEventCallback debugEventCallback, IntPtr callbackState);

	}
}