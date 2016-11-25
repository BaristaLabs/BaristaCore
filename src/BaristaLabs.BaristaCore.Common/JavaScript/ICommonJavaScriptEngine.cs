namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

    using System;
	using System.Runtime.InteropServices;

    /// <summary>
    /// ChakraCommon.h interface
    /// </summary>
    public interface ICommonJavaScriptEngine
	{
		/// <summary>
		///		Creates a new runtime.
		/// </summary>
		/// <remarks>
		///		In the edge-mode binary, chakra.dll, this function lacks the runtimeVersion
		///		parameter (compare to jsrt9.h).
		/// </remarks>
		/// <param name="attributes">
		///		The attributes of the runtime to be created.
		/// </param>
		/// <param name="threadService">
		///		The thread service for the runtime. Can be null.
		/// </param>
		/// <returns>
		///		The runtime created.
		/// </returns>
		JavaScriptRuntimeSafeHandle JsCreateRuntime(JavaScriptRuntimeAttributes attributes, JavaScriptThreadServiceCallback threadService);

		/// <summary>
		///		Performs a full garbage collection.
		/// </summary>
		void JsCollectGarbage(JavaScriptRuntimeSafeHandle handle);

		/// <summary>
		///		Disposes a runtime.
		/// </summary>
		/// <remarks>
		///		Once a runtime has been disposed, all resources owned by it are invalid and cannot be used.
		///		If the runtime is active (i.e. it is set to be current on a particular thread), it cannot
		///		be disposed.
		/// </remarks>
		/// <param name="runtime">
		///		The runtime to dispose.
		/// </param>
		void JsDisposeRuntime(IntPtr runtime);

		/// <summary>
		///		Gets the current memory usage for a runtime.
		/// </summary>
		/// <remarks>
		///		Memory usage can be always be retrieved, regardless of whether or not the runtime is active
		///		on another thread.
		/// </remarks>
		/// <param name="runtime">
		///		The runtime whose memory usage is to be retrieved.
		/// </param>
		/// <returns>
		///		The runtime's current memory usage, in bytes.
		/// </returns>
		ulong JsGetRuntimeMemoryUsage(JavaScriptRuntimeSafeHandle runtime);

		/// <summary>
		///		Gets the current memory limit for a runtime.
		/// </summary>
		/// <remarks>
		///		The memory limit of a runtime can be always be retrieved, regardless of whether or not the
		///		runtime is active on another thread.
		/// </remarks>
		/// <param name="runtime">
		///		The runtime whose memory limit is to be retrieved.
		/// </param>
		/// <returns>
		///		The runtime's current memory limit, in bytes, or -1 if no limit has been set.
		/// </returns>
		ulong JsGetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime);

		/// <summary>
		///		Sets the current memory limit for a runtime.
		/// </summary>
		/// <remarks>
		///		A memory limit will cause any operation which exceeds the limit to fail with an "out of
		///		memory" error. Setting a runtime's memory limit to -1 means that the runtime has no memory
		///		limit. New runtimes  default to having no memory limit. If the new memory limit exceeds
		///		current usage, the call will succeed and any future allocations in this runtime will fail
		///		until the runtime's memory usage drops below the limit.
		///		A runtime's memory limit can be always be set, regardless of whether or not the runtime is
		///		active on another thread.
		/// </remarks>
		/// <param name="runtime">
		///		The runtime whose memory limit is to be set.
		/// </param>
		/// <param name="memoryLimit">
		///		The new runtime memory limit, in bytes, or -1 for no memory limit.
		/// </param>
		void JsSetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, ulong memoryLimit);

		/// <summary>
		///		Sets a memory allocation callback for specified runtime
		/// </summary>
		/// <remarks>
		///		Registering a memory allocation callback will cause the runtime to call back to the host
		///		whenever it acquires memory from, or releases memory to, the OS. The callback routine is
		///		called before the runtime memory manager allocates a block of memory. The allocation will
		///		be rejected if the callback returns false. The runtime memory manager will also invoke the
		///		callback routine after freeing a block of memory, as well as after allocation failures.
		///		The callback is invoked on the current runtime execution thread, therefore execution is
		///		blocked until the callback completes.
		///		The return value of the callback is not stored; previously rejected allocations will not
		///		prevent the runtime from invoking the callback again later for new memory allocations.
		/// </remarks>
		/// <param name="runtime">
		///		The runtime for which to register the allocation callback.
		/// </param>
		/// <param name="callbackState">
		///		User provided state that will be passed back to the callback.
		/// </param>
		/// <param name="allocationCallback">
		///		Memory allocation callback to be called for memory allocation events.
		/// </param>
		void JsSetRuntimeMemoryAllocationCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptMemoryAllocationCallback allocationCallback);

		/// <summary>
		///		Sets a callback function that is called by the runtime before garbage collection.
		/// </summary>
		/// <remarks>
		///		The callback is invoked on the current runtime execution thread, therefore execution is
		///		blocked until the callback completes.
		///		The callback can be used by hosts to prepare for garbage collection. For example, by
		///		releasing unnecessary references on Chakra objects.
		/// </remarks>
		/// <param name="runtime">
		///		The runtime for which to register the allocation callback.
		/// </param>
		/// <param name="callbackState">
		///		User provided state that will be passed back to the callback.
		/// </param>
		/// <param name="beforeCollectCallback">
		///		The callback function being set.
		/// </param>
		void JsSetRuntimeBeforeCollectCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptBeforeCollectCallback beforeCollectCallback);

		/// <summary>
		///		Adds a reference to a garbage collected object.
		/// </summary>
		/// <remarks>
		///		This only needs to be called on JsRef handles that are not going to be stored
		///		somewhere on the stack. Calling JsAddRef ensures that the object the JsRef
		///		refers to will not be freed until JsRelease is called.
		/// </remarks>
		/// <param name="@ref">
		///		The object to add a reference to.
		/// </param>
		/// <returns>
		///		The object's new reference count (can pass in null).
		/// </returns>
		uint JsAddRef(SafeHandle @ref);

		/// <summary>
		///		Releases a reference to a garbage collected object.
		/// </summary>
		/// <remarks>
		///		Removes a reference to a JsRef handle that was created by JsAddRef.
		/// </remarks>
		/// <param name="@ref">
		///		The object to add a reference to.
		/// </param>
		/// <returns>
		///		The object's new reference count (can pass in null).
		/// </returns>
		uint JsRelease(SafeHandle @ref);

		/// <summary>
		///		Sets a callback function that is called by the runtime before garbage collection of an object.
		/// </summary>
		/// <remarks>
		///		The callback is invoked on the current runtime execution thread, therefore execution is
		///		blocked until the callback completes.
		/// </remarks>
		/// <param name="@ref">
		///		The object for which to register the callback.
		/// </param>
		/// <param name="callbackState">
		///		User provided state that will be passed back to the callback.
		/// </param>
		/// <param name="objectBeforeCollectCallback">
		///		The callback function being set. Use null to clear
		///		previously registered callback.
		/// </param>
		void JsSetObjectBeforeCollectCallback(SafeHandle @ref, IntPtr callbackState, JavaScriptObjectBeforeCollectCallback objectBeforeCollectCallback);

		/// <summary>
		///		Creates a script context for running scripts.
		/// </summary>
		/// <remarks>
		///		Each script context has its own global object that is isolated from all other script
		///		contexts.
		/// </remarks>
		/// <param name="runtime">
		///		The runtime the script context is being created in.
		/// </param>
		/// <returns>
		///		The created script context.
		/// </returns>
		JavaScriptContextSafeHandle JsCreateContext(JavaScriptRuntimeSafeHandle runtime);

		/// <summary>
		///		Gets the current script context on the thread.
		/// </summary>
		/// <returns>
		///		The current script context on the thread, null if there is no current script context.
		/// </returns>
		JavaScriptContextSafeHandle JsGetCurrentContext();

		/// <summary>
		///		Sets the current script context on the thread.
		/// </summary>
		void JsSetCurrentContext(JavaScriptContextSafeHandle context);

		/// <summary>
		///		Gets the script context that the object belongs to.
		/// </summary>
		/// <returns>
		///		The context the object belongs to.
		/// </returns>
		JavaScriptContextSafeHandle JsGetContextOfObject(JavaScriptValueSafeHandle @object);

		/// <summary>
		///		Gets the internal data set on JsrtContext.
		/// </summary>
		/// <returns>
		///		The pointer to the data where data will be returned.
		/// </returns>
		IntPtr JsGetContextData(JavaScriptContextSafeHandle context);

		/// <summary>
		///		Sets the internal data of JsrtContext.
		/// </summary>
		void JsSetContextData(JavaScriptContextSafeHandle context, IntPtr data);

		/// <summary>
		///		Gets the runtime that the context belongs to.
		/// </summary>
		/// <returns>
		///		The runtime the context belongs to.
		/// </returns>
		JavaScriptRuntimeSafeHandle JsGetRuntime(JavaScriptContextSafeHandle context);

		/// <summary>
		///		Tells the runtime to do any idle processing it need to do.
		/// </summary>
		/// <remarks>
		///		If idle processing has been enabled for the current runtime, calling JsIdle will
		///		inform the current runtime that the host is idle and that the runtime can perform
		///		memory cleanup tasks.
		///		JsIdle can also return the number of system ticks until there will be more idle work
		///		for the runtime to do. Calling JsIdle before this number of ticks has passed will do
		///		no work.
		///		Requires an active script context.
		/// </remarks>
		/// <returns>
		///		The next system tick when there will be more idle work to do. Can be null. Returns the
		///		maximum number of ticks if there no upcoming idle work to do.
		/// </returns>
		uint JsIdle();

		/// <summary>
		///		Gets the symbol associated with the property ID.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="propertyId">
		///		The property ID to get the symbol of.
		/// </param>
		/// <returns>
		///		The symbol associated with the property ID.
		/// </returns>
		JavaScriptValueSafeHandle JsGetSymbolFromPropertyId(JavaScriptPropertyIdSafeHandle propertyId);

		/// <summary>
		///		Gets the type of property
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="propertyId">
		///		The property ID to get the type of.
		/// </param>
		/// <returns>
		///		The JsPropertyIdType of the given property ID
		/// </returns>
		JavaScriptPropertyIdType JsGetPropertyIdType(JavaScriptPropertyIdSafeHandle propertyId);

		/// <summary>
		///		Gets the property ID associated with the symbol.
		/// </summary>
		/// <remarks>
		///		Property IDs are specific to a context and cannot be used across contexts.
		///		Requires an active script context.
		/// </remarks>
		/// <param name="symbol">
		///		The symbol whose property ID is being retrieved.
		/// </param>
		/// <returns>
		///		The property ID for the given symbol.
		/// </returns>
		JavaScriptPropertyIdSafeHandle JsGetPropertyIdFromSymbol(JavaScriptValueSafeHandle symbol);

		/// <summary>
		///		Creates a Javascript symbol.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="description">
		///		The string description of the symbol. Can be null.
		/// </param>
		/// <returns>
		///		The new symbol.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateSymbol(JavaScriptValueSafeHandle description);

		/// <summary>
		///		Gets the list of all symbol properties on the object.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object from which to get the property symbols.
		/// </param>
		/// <returns>
		///		An array of property symbols.
		/// </returns>
		JavaScriptValueSafeHandle JsGetOwnPropertySymbols(JavaScriptValueSafeHandle @object);

		/// <summary>
		///		Gets the value of undefined in the current script context.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <returns>
		///		The undefined value.
		/// </returns>
		JavaScriptValueSafeHandle JsGetUndefinedValue();

		/// <summary>
		///		Gets the value of null in the current script context.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <returns>
		///		The null value.
		/// </returns>
		JavaScriptValueSafeHandle JsGetNullValue();

		/// <summary>
		///		Gets the value of true in the current script context.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <returns>
		///		The true value.
		/// </returns>
		JavaScriptValueSafeHandle JsGetTrueValue();

		/// <summary>
		///		Gets the value of false in the current script context.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <returns>
		///		The false value.
		/// </returns>
		JavaScriptValueSafeHandle JsGetFalseValue();

		/// <summary>
		///		Creates a Boolean value from a bool value.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="value">
		///		The value to be converted.
		/// </param>
		/// <returns>
		///		The converted value.
		/// </returns>
		JavaScriptValueSafeHandle JsBoolToBoolean(bool value);

		/// <summary>
		///		Retrieves the bool value of a Boolean value.
		/// </summary>
		/// <returns>
		///		The converted value.
		/// </returns>
		bool JsBooleanToBool(JavaScriptValueSafeHandle value);

		/// <summary>
		///		Converts the value to Boolean using standard JavaScript semantics.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="value">
		///		The value to be converted.
		/// </param>
		/// <returns>
		///		The converted value.
		/// </returns>
		JavaScriptValueSafeHandle JsConvertValueToBoolean(JavaScriptValueSafeHandle value);

		/// <summary>
		///		Gets the JavaScript type of a JsValueRef.
		/// </summary>
		/// <returns>
		///		The type of the value.
		/// </returns>
		JavaScriptValueType JsGetValueType(JavaScriptValueSafeHandle value);

		/// <summary>
		///		Creates a number value from a double value.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="doubleValue">
		///		The double to convert to a number value.
		/// </param>
		/// <returns>
		///		The new number value.
		/// </returns>
		JavaScriptValueSafeHandle JsDoubleToNumber(double doubleValue);

		/// <summary>
		///		Creates a number value from an int value.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="intValue">
		///		The int to convert to a number value.
		/// </param>
		/// <returns>
		///		The new number value.
		/// </returns>
		JavaScriptValueSafeHandle JsIntToNumber(int intValue);

		/// <summary>
		///		Retrieves the double value of a number value.
		/// </summary>
		/// <remarks>
		///		This function retrieves the value of a number value. It will fail with
		///		JsErrorInvalidArgument if the type of the value is not number.
		/// </remarks>
		/// <param name="value">
		///		The number value to convert to a double value.
		/// </param>
		/// <returns>
		///		The double value.
		/// </returns>
		double JsNumberToDouble(JavaScriptValueSafeHandle value);

		/// <summary>
		///		Retrieves the int value of a number value.
		/// </summary>
		/// <remarks>
		///		This function retrieves the value of a number value and converts to an int value.
		///		It will fail with JsErrorInvalidArgument if the type of the value is not number.
		/// </remarks>
		/// <param name="value">
		///		The number value to convert to an int value.
		/// </param>
		/// <returns>
		///		The int value.
		/// </returns>
		int JsNumberToInt(JavaScriptValueSafeHandle value);

		/// <summary>
		///		Converts the value to number using standard JavaScript semantics.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="value">
		///		The value to be converted.
		/// </param>
		/// <returns>
		///		The converted value.
		/// </returns>
		JavaScriptValueSafeHandle JsConvertValueToNumber(JavaScriptValueSafeHandle value);

		/// <summary>
		///		Gets the length of a string value.
		/// </summary>
		/// <returns>
		///		The length of the string.
		/// </returns>
		int JsGetStringLength(JavaScriptValueSafeHandle stringValue);

		/// <summary>
		///		Converts the value to string using standard JavaScript semantics.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="value">
		///		The value to be converted.
		/// </param>
		/// <returns>
		///		The converted value.
		/// </returns>
		JavaScriptValueSafeHandle JsConvertValueToString(JavaScriptValueSafeHandle value);

		/// <summary>
		///		Gets the global object in the current script context.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <returns>
		///		The global object.
		/// </returns>
		JavaScriptValueSafeHandle JsGetGlobalObject();

		/// <summary>
		///		Creates a new object.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <returns>
		///		The new object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateObject();

		/// <summary>
		///		Creates a new object that stores some external data.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="data">
		///		External data that the object will represent. May be null.
		/// </param>
		/// <param name="finalizeCallback">
		///		A callback for when the object is finalized. May be null.
		/// </param>
		/// <returns>
		///		The new object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback);

		/// <summary>
		///		Converts the value to object using standard JavaScript semantics.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="value">
		///		The value to be converted.
		/// </param>
		/// <returns>
		///		The converted value.
		/// </returns>
		JavaScriptValueSafeHandle JsConvertValueToObject(JavaScriptValueSafeHandle value);

		/// <summary>
		///		Returns the prototype of an object.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object whose prototype is to be returned.
		/// </param>
		/// <returns>
		///		The object's prototype.
		/// </returns>
		JavaScriptValueSafeHandle JsGetPrototype(JavaScriptValueSafeHandle @object);

		/// <summary>
		///		Sets the prototype of an object.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object whose prototype is to be changed.
		/// </param>
		/// <param name="prototypeObject">
		///		The object's new prototype.
		/// </param>
		void JsSetPrototype(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle prototypeObject);

		/// <summary>
		///		Performs JavaScript "instanceof" operator test.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object to test.
		/// </param>
		/// <param name="constructor">
		///		The constructor function to test against.
		/// </param>
		/// <returns>
		///		Whether "object instanceof constructor" is true.
		/// </returns>
		bool JsInstanceOf(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle constructor);

		/// <summary>
		///		Returns a value that indicates whether an object is extensible or not.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object to test.
		/// </param>
		/// <returns>
		///		Whether the object is extensible or not.
		/// </returns>
		bool JsGetExtensionAllowed(JavaScriptValueSafeHandle @object);

		/// <summary>
		///		Makes an object non-extensible.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object to make non-extensible.
		/// </param>
		void JsPreventExtension(JavaScriptValueSafeHandle @object);

		/// <summary>
		///		Gets an object's property.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object that contains the property.
		/// </param>
		/// <param name="propertyId">
		///		The ID of the property.
		/// </param>
		/// <returns>
		///		The value of the property.
		/// </returns>
		JavaScriptValueSafeHandle JsGetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId);

		/// <summary>
		///		Gets a property descriptor for an object's own property.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object that has the property.
		/// </param>
		/// <param name="propertyId">
		///		The ID of the property.
		/// </param>
		/// <returns>
		///		The property descriptor.
		/// </returns>
		JavaScriptValueSafeHandle JsGetOwnPropertyDescriptor(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId);

		/// <summary>
		///		Gets the list of all properties on the object.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object from which to get the property names.
		/// </param>
		/// <returns>
		///		An array of property names.
		/// </returns>
		JavaScriptValueSafeHandle JsGetOwnPropertyNames(JavaScriptValueSafeHandle @object);

		/// <summary>
		///		Puts an object's property.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object that contains the property.
		/// </param>
		/// <param name="propertyId">
		///		The ID of the property.
		/// </param>
		/// <param name="value">
		///		The new value of the property.
		/// </param>
		/// <param name="useStrictRules">
		///		The property set should follow strict mode rules.
		/// </param>
		void JsSetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle value, bool useStrictRules);

		/// <summary>
		///		Determines whether an object has a property.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object that may contain the property.
		/// </param>
		/// <param name="propertyId">
		///		The ID of the property.
		/// </param>
		/// <returns>
		///		Whether the object (or a prototype) has the property.
		/// </returns>
		bool JsHasProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId);

		/// <summary>
		///		Deletes an object's property.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object that contains the property.
		/// </param>
		/// <param name="propertyId">
		///		The ID of the property.
		/// </param>
		/// <param name="useStrictRules">
		///		The property set should follow strict mode rules.
		/// </param>
		/// <returns>
		///		Whether the property was deleted.
		/// </returns>
		JavaScriptValueSafeHandle JsDeleteProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, bool useStrictRules);

		/// <summary>
		///		Defines a new object's own property from a property descriptor.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object that has the property.
		/// </param>
		/// <param name="propertyId">
		///		The ID of the property.
		/// </param>
		/// <param name="propertyDescriptor">
		///		The property descriptor.
		/// </param>
		/// <returns>
		///		Whether the property was defined.
		/// </returns>
		bool JsDefineProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle propertyDescriptor);

		/// <summary>
		///		Tests whether an object has a value at the specified index.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object to operate on.
		/// </param>
		/// <param name="index">
		///		The index to test.
		/// </param>
		/// <returns>
		///		Whether the object has a value at the specified index.
		/// </returns>
		bool JsHasIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index);

		/// <summary>
		///		Retrieve the value at the specified index of an object.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object to operate on.
		/// </param>
		/// <param name="index">
		///		The index to retrieve.
		/// </param>
		/// <returns>
		///		The retrieved value.
		/// </returns>
		JavaScriptValueSafeHandle JsGetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index);

		/// <summary>
		///		Set the value at the specified index of an object.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object to operate on.
		/// </param>
		/// <param name="index">
		///		The index to set.
		/// </param>
		/// <param name="value">
		///		The value to set.
		/// </param>
		void JsSetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, JavaScriptValueSafeHandle value);

		/// <summary>
		///		Delete the value at the specified index of an object.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object to operate on.
		/// </param>
		/// <param name="index">
		///		The index to delete.
		/// </param>
		void JsDeleteIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index);

		/// <summary>
		///		Determines whether an object has its indexed properties in external data.
		/// </summary>
		/// <returns>
		///		Whether the object has its indexed properties in external data.
		/// </returns>
		bool JsHasIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object);

		/// <summary>
		///		Retrieves an object's indexed properties external data information.
		/// </summary>
		/// <returns>
		///		The array element type in external data.
		/// </returns>
		JavaScriptTypedArrayType JsGetIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, IntPtr data, out uint elementLength);

		/// <summary>
		///		Sets an object's indexed properties to external data. The external data will be used as back store for the object's indexed properties and accessed like a typed array.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="@object">
		///		The object to operate on.
		/// </param>
		/// <param name="data">
		///		The external data to be used as back store for the object's indexed properties.
		/// </param>
		/// <param name="arrayType">
		///		The array element type in external data.
		/// </param>
		/// <param name="elementLength">
		///		The number of array elements in external data.
		/// </param>
		void JsSetIndexedPropertiesToExternalData(JavaScriptValueSafeHandle @object, IntPtr data, JavaScriptTypedArrayType arrayType, uint elementLength);

		/// <summary>
		///		Compare two JavaScript values for equality.
		/// </summary>
		/// <remarks>
		///		This function is equivalent to the == operator in Javascript.
		///		Requires an active script context.
		/// </remarks>
		/// <param name="object1">
		///		The first object to compare.
		/// </param>
		/// <param name="object2">
		///		The second object to compare.
		/// </param>
		/// <returns>
		///		Whether the values are equal.
		/// </returns>
		bool JsEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2);

		/// <summary>
		///		Compare two JavaScript values for strict equality.
		/// </summary>
		/// <remarks>
		///		This function is equivalent to the === operator in Javascript.
		///		Requires an active script context.
		/// </remarks>
		/// <param name="object1">
		///		The first object to compare.
		/// </param>
		/// <param name="object2">
		///		The second object to compare.
		/// </param>
		/// <returns>
		///		Whether the values are strictly equal.
		/// </returns>
		bool JsStrictEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2);

		/// <summary>
		///		Determines whether an object is an external object.
		/// </summary>
		/// <returns>
		///		Whether the object is an external object.
		/// </returns>
		bool JsHasExternalData(JavaScriptValueSafeHandle @object);

		/// <summary>
		///		Retrieves the data from an external object.
		/// </summary>
		/// <returns>
		///		The external data stored in the object. Can be null if no external data is stored in the
		///		object.
		/// </returns>
		IntPtr JsGetExternalData(JavaScriptValueSafeHandle @object);

		/// <summary>
		///		Sets the external data on an external object.
		/// </summary>
		void JsSetExternalData(JavaScriptValueSafeHandle @object, IntPtr externalData);

		/// <summary>
		///		Creates a Javascript array object.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="length">
		///		The initial length of the array.
		/// </param>
		/// <returns>
		///		The new array object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateArray(uint length);

		/// <summary>
		///		Creates a Javascript ArrayBuffer object.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="byteLength">
		///		The number of bytes in the ArrayBuffer.
		/// </param>
		/// <returns>
		///		The new ArrayBuffer object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateArrayBuffer(uint byteLength);

		/// <summary>
		///		Creates a Javascript ArrayBuffer object to access external memory.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="data">
		///		A pointer to the external memory.
		/// </param>
		/// <param name="byteLength">
		///		The number of bytes in the external memory.
		/// </param>
		/// <param name="finalizeCallback">
		///		A callback for when the object is finalized. May be null.
		/// </param>
		/// <param name="callbackState">
		///		User provided state that will be passed back to finalizeCallback.
		/// </param>
		/// <returns>
		///		The new ArrayBuffer object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateExternalArrayBuffer(IntPtr data, uint byteLength, JavaScriptObjectFinalizeCallback finalizeCallback, IntPtr callbackState);

		/// <summary>
		///		Creates a Javascript typed array object.
		/// </summary>
		/// <remarks>
		///		The baseArray can be an ArrayBuffer, another typed array, or a JavaScript
		///		Array. The returned typed array will use the baseArray if it is an ArrayBuffer, or
		///		otherwise create and use a copy of the underlying source array.
		///		Requires an active script context.
		/// </remarks>
		/// <param name="arrayType">
		///		The type of the array to create.
		/// </param>
		/// <param name="baseArray">
		///		The base array of the new array. Use JS_INVALID_REFERENCE if no base array.
		/// </param>
		/// <param name="byteOffset">
		///		The offset in bytes from the start of baseArray (ArrayBuffer) for result typed array to reference.
		///		Only applicable when baseArray is an ArrayBuffer object. Must be 0 otherwise.
		/// </param>
		/// <param name="elementLength">
		///		The number of elements in the array. Only applicable when creating a new typed array without
		///		baseArray (baseArray is JS_INVALID_REFERENCE) or when baseArray is an ArrayBuffer object.
		///		Must be 0 otherwise.
		/// </param>
		/// <returns>
		///		The new typed array object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateTypedArray(JavaScriptTypedArrayType arrayType, JavaScriptValueSafeHandle baseArray, uint byteOffset, uint elementLength);

		/// <summary>
		///		Creates a Javascript DataView object.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="arrayBuffer">
		///		An existing ArrayBuffer object to use as the storage for the result DataView object.
		/// </param>
		/// <param name="byteOffset">
		///		The offset in bytes from the start of arrayBuffer for result DataView to reference.
		/// </param>
		/// <param name="byteLength">
		///		The number of bytes in the ArrayBuffer for result DataView to reference.
		/// </param>
		/// <returns>
		///		The new DataView object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateDataView(JavaScriptValueSafeHandle arrayBuffer, uint byteOffset, uint byteLength);

		/// <summary>
		///		Obtains frequently used properties of a typed array.
		/// </summary>
		/// <returns>
		///		The type of the array.
		/// </returns>
		JavaScriptTypedArrayType JsGetTypedArrayInfo(JavaScriptValueSafeHandle typedArray, out JavaScriptValueSafeHandle arrayBuffer, out uint byteOffset, out uint byteLength);

		/// <summary>
		///		Obtains the underlying memory storage used by an ArrayBuffer.
		/// </summary>
		/// <returns>
		///		The ArrayBuffer's buffer. The lifetime of the buffer returned is the same as the lifetime of the
		///		the ArrayBuffer. The buffer pointer does not count as a reference to the ArrayBuffer for the purpose
		///		of garbage collection.
		/// </returns>
		IntPtr JsGetArrayBufferStorage(JavaScriptValueSafeHandle arrayBuffer, out uint bufferLength);

		/// <summary>
		///		Obtains the underlying memory storage used by a typed array.
		/// </summary>
		/// <returns>
		///		The array's buffer. The lifetime of the buffer returned is the same as the lifetime of the
		///		the array. The buffer pointer does not count as a reference to the array for the purpose
		///		of garbage collection.
		/// </returns>
		IntPtr JsGetTypedArrayStorage(JavaScriptValueSafeHandle typedArray, out uint bufferLength, out JavaScriptTypedArrayType arrayType, out int elementSize);

		/// <summary>
		///		Obtains the underlying memory storage used by a DataView.
		/// </summary>
		/// <returns>
		///		The DataView's buffer. The lifetime of the buffer returned is the same as the lifetime of the
		///		the DataView. The buffer pointer does not count as a reference to the DataView for the purpose
		///		of garbage collection.
		/// </returns>
		IntPtr JsGetDataViewStorage(JavaScriptValueSafeHandle dataView, out uint bufferLength);

		/// <summary>
		///		Invokes a function.
		/// </summary>
		/// <remarks>
		///		Requires thisArg as first argument of arguments.
		///		Requires an active script context.
		/// </remarks>
		/// <param name="function">
		///		The function to invoke.
		/// </param>
		/// <param name="arguments">
		///		The arguments to the call.
		/// </param>
		/// <param name="argumentCount">
		///		The number of arguments being passed in to the function.
		/// </param>
		/// <returns>
		///		The value returned from the function invocation, if any.
		/// </returns>
		JavaScriptValueSafeHandle JsCallFunction(JavaScriptValueSafeHandle function, IntPtr[] arguments, ushort argumentCount);

		/// <summary>
		///		Invokes a function as a constructor.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="function">
		///		The function to invoke as a constructor.
		/// </param>
		/// <param name="arguments">
		///		The arguments to the call.
		/// </param>
		/// <param name="argumentCount">
		///		The number of arguments being passed in to the function.
		/// </param>
		/// <returns>
		///		The value returned from the function invocation.
		/// </returns>
		JavaScriptValueSafeHandle JsConstructObject(JavaScriptValueSafeHandle function, IntPtr[] arguments, ushort argumentCount);

		/// <summary>
		///		Creates a new JavaScript function.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="nativeFunction">
		///		The method to call when the function is invoked.
		/// </param>
		/// <param name="callbackState">
		///		User provided state that will be passed back to the callback.
		/// </param>
		/// <returns>
		///		The new function object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateFunction(JavaScriptNativeFunction nativeFunction, IntPtr callbackState);

		/// <summary>
		///		Creates a new JavaScript function with name.
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="name">
		///		The name of this function that will be used for diagnostics and stringification purposes.
		/// </param>
		/// <param name="nativeFunction">
		///		The method to call when the function is invoked.
		/// </param>
		/// <param name="callbackState">
		///		User provided state that will be passed back to the callback.
		/// </param>
		/// <returns>
		///		The new function object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateNamedFunction(JavaScriptValueSafeHandle name, JavaScriptNativeFunction nativeFunction, IntPtr callbackState);

		/// <summary>
		///		Creates a new JavaScript error object
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="message">
		///		Message for the error object.
		/// </param>
		/// <returns>
		///		The new error object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateError(JavaScriptValueSafeHandle message);

		/// <summary>
		///		Creates a new JavaScript RangeError error object
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="message">
		///		Message for the error object.
		/// </param>
		/// <returns>
		///		The new error object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateRangeError(JavaScriptValueSafeHandle message);

		/// <summary>
		///		Creates a new JavaScript ReferenceError error object
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="message">
		///		Message for the error object.
		/// </param>
		/// <returns>
		///		The new error object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateReferenceError(JavaScriptValueSafeHandle message);

		/// <summary>
		///		Creates a new JavaScript SyntaxError error object
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="message">
		///		Message for the error object.
		/// </param>
		/// <returns>
		///		The new error object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateSyntaxError(JavaScriptValueSafeHandle message);

		/// <summary>
		///		Creates a new JavaScript TypeError error object
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="message">
		///		Message for the error object.
		/// </param>
		/// <returns>
		///		The new error object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateTypeError(JavaScriptValueSafeHandle message);

		/// <summary>
		///		Creates a new JavaScript URIError error object
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="message">
		///		Message for the error object.
		/// </param>
		/// <returns>
		///		The new error object.
		/// </returns>
		JavaScriptValueSafeHandle JsCreateURIError(JavaScriptValueSafeHandle message);

		/// <summary>
		///		Determines whether the runtime of the current context is in an exception state.
		/// </summary>
		/// <remarks>
		///		If a call into the runtime results in an exception (either as the result of running a
		///		script or due to something like a conversion failure), the runtime is placed into an
		///		"exception state." All calls into any context created by the runtime (except for the
		///		exception APIs) will fail with JsErrorInExceptionState until the exception is
		///		cleared.
		///		If the runtime of the current context is in the exception state when a callback returns
		///		into the engine, the engine will automatically rethrow the exception.
		///		Requires an active script context.
		/// </remarks>
		/// <returns>
		///		Whether the runtime of the current context is in the exception state.
		/// </returns>
		bool JsHasException();

		/// <summary>
		///		Returns the exception that caused the runtime of the current context to be in the exception state and resets the exception state for that runtime.
		/// </summary>
		/// <remarks>
		///		If the runtime of the current context is not in an exception state, this API will return
		///		JsErrorInvalidArgument. If the runtime is disabled, this will return an exception
		///		indicating that the script was terminated, but it will not clear the exception (the
		///		exception will be cleared if the runtime is re-enabled using
		///		JsEnableRuntimeExecution).
		///		Requires an active script context.
		/// </remarks>
		/// <returns>
		///		The exception for the runtime of the current context.
		/// </returns>
		JavaScriptValueSafeHandle JsGetAndClearException();

		/// <summary>
		///		Sets the runtime of the current context to an exception state.
		/// </summary>
		/// <remarks>
		///		If the runtime of the current context is already in an exception state, this API will
		///		return JsErrorInExceptionState.
		///		Requires an active script context.
		/// </remarks>
		/// <param name="exception">
		///		The JavaScript exception to set for the runtime of the current context.
		/// </param>
		void JsSetException(JavaScriptValueSafeHandle exception);

		/// <summary>
		///		Suspends script execution and terminates any running scripts in a runtime.
		/// </summary>
		/// <remarks>
		///		Calls to a suspended runtime will fail until JsEnableRuntimeExecution is called.
		///		This API does not have to be called on the thread the runtime is active on. Although the
		///		runtime will be set into a suspended state, an executing script may not be suspended
		///		immediately; a running script will be terminated with an uncatchable exception as soon as
		///		possible.
		///		Suspending execution in a runtime that is already suspended is a no-op.
		/// </remarks>
		/// <param name="runtime">
		///		The runtime to be suspended.
		/// </param>
		void JsDisableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime);

		/// <summary>
		///		Enables script execution in a runtime.
		/// </summary>
		/// <remarks>
		///		Enabling script execution in a runtime that already has script execution enabled is a
		///		no-op.
		/// </remarks>
		/// <param name="runtime">
		///		The runtime to be enabled.
		/// </param>
		void JsEnableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime);

		/// <summary>
		///		Returns a value that indicates whether script execution is disabled in the runtime.
		/// </summary>
		/// <returns>
		///		If execution is disabled, true, false otherwise.
		/// </returns>
		bool JsIsRuntimeExecutionDisabled(JavaScriptRuntimeSafeHandle runtime);

		/// <summary>
		///		Sets a promise continuation callback function that is called by the context when a task needs to be queued for future execution
		/// </summary>
		/// <remarks>
		///		Requires an active script context.
		/// </remarks>
		/// <param name="promiseContinuationCallback">
		///		The callback function being set.
		/// </param>
		/// <param name="callbackState">
		///		User provided state that will be passed back to the callback.
		/// </param>
		void JsSetPromiseContinuationCallback(JavaScriptPromiseContinuationCallback promiseContinuationCallback, IntPtr callbackState);

	}
}