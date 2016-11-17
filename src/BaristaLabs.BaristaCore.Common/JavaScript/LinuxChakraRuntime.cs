namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

	using System;
	using System.Runtime.InteropServices;

	public sealed class LinuxChakraRuntime : IJavaScriptRuntime
	{
			public JavaScriptErrorCode JsInitializeModuleRecord(IntPtr referencingModule, JavaScriptValueSafeHandle normalizedSpecifier, out IntPtr moduleRecord)
			{
				return LibChakraCore.JsInitializeModuleRecord(referencingModule, normalizedSpecifier, out moduleRecord);
			}

			public JavaScriptErrorCode JsParseModuleSource(IntPtr requestModule, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags sourceFlag, out JavaScriptValueSafeHandle exceptionValueRef)
			{
				return LibChakraCore.JsParseModuleSource(requestModule, sourceContext, script, scriptLength, sourceFlag, out exceptionValueRef);
			}

			public JavaScriptErrorCode JsModuleEvaluation(IntPtr requestModule, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsModuleEvaluation(requestModule, out result);
			}

			public JavaScriptErrorCode JsSetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, IntPtr hostInfo)
			{
				return LibChakraCore.JsSetModuleHostInfo(requestModule, moduleHostInfo, hostInfo);
			}

			public JavaScriptErrorCode JsGetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, out IntPtr hostInfo)
			{
				return LibChakraCore.JsGetModuleHostInfo(requestModule, moduleHostInfo, out hostInfo);
			}

			public JavaScriptErrorCode JsCreateString(string content, UIntPtr length, out JavaScriptValueSafeHandle value)
			{
				return LibChakraCore.JsCreateString(content, length, out value);
			}

			public JavaScriptErrorCode JsCreateStringUtf8(string content, UIntPtr length, out JavaScriptValueSafeHandle value)
			{
				return LibChakraCore.JsCreateStringUtf8(content, length, out value);
			}

			public JavaScriptErrorCode JsCreateStringUtf16(string content, UIntPtr length, out JavaScriptValueSafeHandle value)
			{
				return LibChakraCore.JsCreateStringUtf16(content, length, out value);
			}

			public JavaScriptErrorCode JsCopyString(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out UIntPtr written)
			{
				return LibChakraCore.JsCopyString(value, start, length, buffer, out written);
			}

			public JavaScriptErrorCode JsCopyStringUtf8(JavaScriptValueSafeHandle value, byte[] buffer, UIntPtr bufferSize, out UIntPtr written)
			{
				return LibChakraCore.JsCopyStringUtf8(value, buffer, bufferSize, out written);
			}

			public JavaScriptErrorCode JsCopyStringUtf16(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out UIntPtr written)
			{
				return LibChakraCore.JsCopyStringUtf16(value, start, length, buffer, out written);
			}

			public JavaScriptErrorCode JsParse(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsParse(script, sourceContext, sourceUrl, parseAttributes, out result);
			}

			public JavaScriptErrorCode JsRun(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsRun(script, sourceContext, sourceUrl, parseAttributes, out result);
			}

			public JavaScriptErrorCode JsCreatePropertyIdUtf8(string name, UIntPtr length, out JavaScriptPropertyIdSafeHandle propertyId)
			{
				return LibChakraCore.JsCreatePropertyIdUtf8(name, length, out propertyId);
			}

			public JavaScriptErrorCode JsCopyPropertyIdUtf8(JavaScriptPropertyIdSafeHandle propertyId, byte[] buffer, UIntPtr bufferSize, out UIntPtr length)
			{
				return LibChakraCore.JsCopyPropertyIdUtf8(propertyId, buffer, bufferSize, out length);
			}

			public JavaScriptErrorCode JsSerialize(JavaScriptValueSafeHandle script, byte[] buffer, ref ulong bufferSize, JavaScriptParseScriptAttributes parseAttributes)
			{
				return LibChakraCore.JsSerialize(script, buffer, ref bufferSize, parseAttributes);
			}

			public JavaScriptErrorCode JsParseSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsParseSerialized(buffer, scriptLoadCallback, sourceContext, sourceUrl, out result);
			}

			public JavaScriptErrorCode JsRunSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsRunSerialized(buffer, scriptLoadCallback, sourceContext, sourceUrl, out result);
			}

			public JavaScriptErrorCode JsCreateRuntime(JavaScriptRuntimeAttributes attributes, JavaScriptThreadServiceCallback threadService, out JavaScriptRuntimeSafeHandle runtime)
			{
				return LibChakraCore.JsCreateRuntime(attributes, threadService, out runtime);
			}

			public JavaScriptErrorCode JsCollectGarbage(JavaScriptRuntimeSafeHandle handle)
			{
				return LibChakraCore.JsCollectGarbage(handle);
			}

			public JavaScriptErrorCode JsDisposeRuntime(IntPtr runtime)
			{
				return LibChakraCore.JsDisposeRuntime(runtime);
			}

			public JavaScriptErrorCode JsGetRuntimeMemoryUsage(JavaScriptRuntimeSafeHandle runtime, out ulong usage)
			{
				return LibChakraCore.JsGetRuntimeMemoryUsage(runtime, out usage);
			}

			public JavaScriptErrorCode JsGetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, out ulong memoryLimit)
			{
				return LibChakraCore.JsGetRuntimeMemoryLimit(runtime, out memoryLimit);
			}

			public JavaScriptErrorCode JsSetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, ulong memoryLimit)
			{
				return LibChakraCore.JsSetRuntimeMemoryLimit(runtime, memoryLimit);
			}

			public JavaScriptErrorCode JsSetRuntimeMemoryAllocationCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr extraInformation, JavaScriptMemoryAllocationCallback allocationCallback)
			{
				return LibChakraCore.JsSetRuntimeMemoryAllocationCallback(runtime, extraInformation, allocationCallback);
			}

			public JavaScriptErrorCode JsSetRuntimeBeforeCollectCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptBeforeCollectCallback beforeCollectCallback)
			{
				return LibChakraCore.JsSetRuntimeBeforeCollectCallback(runtime, callbackState, beforeCollectCallback);
			}

			public JavaScriptErrorCode JsAddRef(IntPtr @ref, out uint count)
			{
				return LibChakraCore.JsAddRef(@ref, out count);
			}

			public JavaScriptErrorCode JsReleaseContext(JavaScriptContextSafeHandle context, out uint count)
			{
				return LibChakraCore.JsReleaseContext(context, out count);
			}

			public JavaScriptErrorCode JsReleasePropertyId(JavaScriptPropertyIdSafeHandle context, out uint count)
			{
				return LibChakraCore.JsReleasePropertyId(context, out count);
			}

			public JavaScriptErrorCode JsReleaseValue(JavaScriptValueSafeHandle value, out uint count)
			{
				return LibChakraCore.JsReleaseValue(value, out count);
			}

			public JavaScriptErrorCode JsRelease(IntPtr @ref, out uint count)
			{
				return LibChakraCore.JsRelease(@ref, out count);
			}

			public JavaScriptErrorCode JsSetObjectBeforeCollectCallback(JavaScriptValueSafeHandle @ref, IntPtr callbackState, JavaScriptObjectBeforeCollectCallback objectBeforeCollectCallback)
			{
				return LibChakraCore.JsSetObjectBeforeCollectCallback(@ref, callbackState, objectBeforeCollectCallback);
			}

			public JavaScriptErrorCode JsCreateContext(JavaScriptRuntimeSafeHandle runtime, out JavaScriptContextSafeHandle context)
			{
				return LibChakraCore.JsCreateContext(runtime, out context);
			}

			public JavaScriptErrorCode JsGetCurrentContext(out JavaScriptContextSafeHandle context)
			{
				return LibChakraCore.JsGetCurrentContext(out context);
			}

			public JavaScriptErrorCode JsSetCurrentContext(JavaScriptContextSafeHandle context)
			{
				return LibChakraCore.JsSetCurrentContext(context);
			}

			public JavaScriptErrorCode JsGetContextOfObject(JavaScriptValueSafeHandle @object, out JavaScriptContextSafeHandle context)
			{
				return LibChakraCore.JsGetContextOfObject(@object, out context);
			}

			public JavaScriptErrorCode JsGetContextData(JavaScriptContextSafeHandle context, out IntPtr data)
			{
				return LibChakraCore.JsGetContextData(context, out data);
			}

			public JavaScriptErrorCode JsSetContextData(JavaScriptContextSafeHandle context, IntPtr data)
			{
				return LibChakraCore.JsSetContextData(context, data);
			}

			public JavaScriptErrorCode JsGetRuntime(JavaScriptContextSafeHandle context, out JavaScriptRuntimeSafeHandle runtime)
			{
				return LibChakraCore.JsGetRuntime(context, out runtime);
			}

			public JavaScriptErrorCode JsIdle(out uint nextIdleTick)
			{
				return LibChakraCore.JsIdle(out nextIdleTick);
			}

			public JavaScriptErrorCode JsGetSymbolFromPropertyId(JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptValueSafeHandle symbol)
			{
				return LibChakraCore.JsGetSymbolFromPropertyId(propertyId, out symbol);
			}

			public JavaScriptErrorCode JsGetPropertyIdType(JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptPropertyIdType propertyIdType)
			{
				return LibChakraCore.JsGetPropertyIdType(propertyId, out propertyIdType);
			}

			public JavaScriptErrorCode JsGetPropertyIdFromSymbol(JavaScriptValueSafeHandle symbol, out JavaScriptPropertyIdSafeHandle propertyId)
			{
				return LibChakraCore.JsGetPropertyIdFromSymbol(symbol, out propertyId);
			}

			public JavaScriptErrorCode JsCreateSymbol(JavaScriptValueSafeHandle description, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsCreateSymbol(description, out result);
			}

			public JavaScriptErrorCode JsGetOwnPropertySymbols(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle propertySymbols)
			{
				return LibChakraCore.JsGetOwnPropertySymbols(@object, out propertySymbols);
			}

			public JavaScriptErrorCode JsGetUndefinedValue(out JavaScriptValueSafeHandle undefinedValue)
			{
				return LibChakraCore.JsGetUndefinedValue(out undefinedValue);
			}

			public JavaScriptErrorCode JsGetNullValue(out JavaScriptValueSafeHandle nullValue)
			{
				return LibChakraCore.JsGetNullValue(out nullValue);
			}

			public JavaScriptErrorCode JsGetTrueValue(out JavaScriptValueSafeHandle trueValue)
			{
				return LibChakraCore.JsGetTrueValue(out trueValue);
			}

			public JavaScriptErrorCode JsGetFalseValue(out JavaScriptValueSafeHandle falseValue)
			{
				return LibChakraCore.JsGetFalseValue(out falseValue);
			}

			public JavaScriptErrorCode JsBoolToBoolean(bool value, out JavaScriptValueSafeHandle booleanValue)
			{
				return LibChakraCore.JsBoolToBoolean(value, out booleanValue);
			}

			public JavaScriptErrorCode JsBooleanToBool(JavaScriptValueSafeHandle value, out bool boolValue)
			{
				return LibChakraCore.JsBooleanToBool(value, out boolValue);
			}

			public JavaScriptErrorCode JsConvertValueToBoolean(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle booleanValue)
			{
				return LibChakraCore.JsConvertValueToBoolean(value, out booleanValue);
			}

			public JavaScriptErrorCode JsGetValueType(JavaScriptValueSafeHandle value, out JavaScriptValueType type)
			{
				return LibChakraCore.JsGetValueType(value, out type);
			}

			public JavaScriptErrorCode JsDoubleToNumber(double doubleValue, out JavaScriptValueSafeHandle value)
			{
				return LibChakraCore.JsDoubleToNumber(doubleValue, out value);
			}

			public JavaScriptErrorCode JsIntToNumber(int intValue, out JavaScriptValueSafeHandle value)
			{
				return LibChakraCore.JsIntToNumber(intValue, out value);
			}

			public JavaScriptErrorCode JsNumberToDouble(JavaScriptValueSafeHandle value, out double doubleValue)
			{
				return LibChakraCore.JsNumberToDouble(value, out doubleValue);
			}

			public JavaScriptErrorCode JsNumberToInt(JavaScriptValueSafeHandle value, out int intValue)
			{
				return LibChakraCore.JsNumberToInt(value, out intValue);
			}

			public JavaScriptErrorCode JsConvertValueToNumber(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle numberValue)
			{
				return LibChakraCore.JsConvertValueToNumber(value, out numberValue);
			}

			public JavaScriptErrorCode JsGetStringLength(JavaScriptValueSafeHandle stringValue, out int length)
			{
				return LibChakraCore.JsGetStringLength(stringValue, out length);
			}

			public JavaScriptErrorCode JsConvertValueToString(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle stringValue)
			{
				return LibChakraCore.JsConvertValueToString(value, out stringValue);
			}

			public JavaScriptErrorCode JsGetGlobalObject(out JavaScriptValueSafeHandle globalObject)
			{
				return LibChakraCore.JsGetGlobalObject(out globalObject);
			}

			public JavaScriptErrorCode JsCreateObject(out JavaScriptValueSafeHandle @object)
			{
				return LibChakraCore.JsCreateObject(out @object);
			}

			public JavaScriptErrorCode JsCreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback, out JavaScriptValueSafeHandle @object)
			{
				return LibChakraCore.JsCreateExternalObject(data, finalizeCallback, out @object);
			}

			public JavaScriptErrorCode JsConvertValueToObject(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle @object)
			{
				return LibChakraCore.JsConvertValueToObject(value, out @object);
			}

			public JavaScriptErrorCode JsGetPrototype(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle prototypeObject)
			{
				return LibChakraCore.JsGetPrototype(@object, out prototypeObject);
			}

			public JavaScriptErrorCode JsSetPrototype(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle prototypeObject)
			{
				return LibChakraCore.JsSetPrototype(@object, prototypeObject);
			}

			public JavaScriptErrorCode JsInstanceOf(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle constructor, out bool result)
			{
				return LibChakraCore.JsInstanceOf(@object, constructor, out result);
			}

			public JavaScriptErrorCode JsGetExtensionAllowed(JavaScriptValueSafeHandle @object, out bool value)
			{
				return LibChakraCore.JsGetExtensionAllowed(@object, out value);
			}

			public JavaScriptErrorCode JsPreventExtension(JavaScriptValueSafeHandle @object)
			{
				return LibChakraCore.JsPreventExtension(@object);
			}

			public JavaScriptErrorCode JsGetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptValueSafeHandle value)
			{
				return LibChakraCore.JsGetProperty(@object, propertyId, out value);
			}

			public JavaScriptErrorCode JsGetOwnPropertyDescriptor(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptValueSafeHandle propertyDescriptor)
			{
				return LibChakraCore.JsGetOwnPropertyDescriptor(@object, propertyId, out propertyDescriptor);
			}

			public JavaScriptErrorCode JsGetOwnPropertyNames(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle propertyNames)
			{
				return LibChakraCore.JsGetOwnPropertyNames(@object, out propertyNames);
			}

			public JavaScriptErrorCode JsSetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle value, bool useStrictRules)
			{
				return LibChakraCore.JsSetProperty(@object, propertyId, value, useStrictRules);
			}

			public JavaScriptErrorCode JsHasProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, out bool hasProperty)
			{
				return LibChakraCore.JsHasProperty(@object, propertyId, out hasProperty);
			}

			public JavaScriptErrorCode JsDeleteProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, bool useStrictRules, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsDeleteProperty(@object, propertyId, useStrictRules, out result);
			}

			public JavaScriptErrorCode JsDefineProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle propertyDescriptor, out bool result)
			{
				return LibChakraCore.JsDefineProperty(@object, propertyId, propertyDescriptor, out result);
			}

			public JavaScriptErrorCode JsHasIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, out bool result)
			{
				return LibChakraCore.JsHasIndexedProperty(@object, index, out result);
			}

			public JavaScriptErrorCode JsGetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsGetIndexedProperty(@object, index, out result);
			}

			public JavaScriptErrorCode JsSetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, JavaScriptValueSafeHandle value)
			{
				return LibChakraCore.JsSetIndexedProperty(@object, index, value);
			}

			public JavaScriptErrorCode JsDeleteIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index)
			{
				return LibChakraCore.JsDeleteIndexedProperty(@object, index);
			}

			public JavaScriptErrorCode JsHasIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, out bool value)
			{
				return LibChakraCore.JsHasIndexedPropertiesExternalData(@object, out value);
			}

			public JavaScriptErrorCode JsGetIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, IntPtr data, out JavaScriptTypedArrayType arrayType, out uint elementLength)
			{
				return LibChakraCore.JsGetIndexedPropertiesExternalData(@object, data, out arrayType, out elementLength);
			}

			public JavaScriptErrorCode JsSetIndexedPropertiesToExternalData(JavaScriptValueSafeHandle @object, IntPtr Data, JavaScriptTypedArrayType arrayType, uint elementLength)
			{
				return LibChakraCore.JsSetIndexedPropertiesToExternalData(@object, Data, arrayType, elementLength);
			}

			public JavaScriptErrorCode JsEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result)
			{
				return LibChakraCore.JsEquals(object1, object2, out result);
			}

			public JavaScriptErrorCode JsStrictEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result)
			{
				return LibChakraCore.JsStrictEquals(object1, object2, out result);
			}

			public JavaScriptErrorCode JsHasExternalData(JavaScriptValueSafeHandle @object, out bool value)
			{
				return LibChakraCore.JsHasExternalData(@object, out value);
			}

			public JavaScriptErrorCode JsGetExternalData(JavaScriptValueSafeHandle @object, out IntPtr externalData)
			{
				return LibChakraCore.JsGetExternalData(@object, out externalData);
			}

			public JavaScriptErrorCode JsSetExternalData(JavaScriptValueSafeHandle @object, IntPtr externalData)
			{
				return LibChakraCore.JsSetExternalData(@object, externalData);
			}

			public JavaScriptErrorCode JsCreateArray(uint length, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsCreateArray(length, out result);
			}

			public JavaScriptErrorCode JsCreateArrayBuffer(uint byteLength, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsCreateArrayBuffer(byteLength, out result);
			}

			public JavaScriptErrorCode JsCreateExternalArrayBuffer(IntPtr data, uint byteLength, JavaScriptObjectFinalizeCallback finalizeCallback, IntPtr callbackState, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsCreateExternalArrayBuffer(data, byteLength, finalizeCallback, callbackState, out result);
			}

			public JavaScriptErrorCode JsCreateTypedArray(JavaScriptTypedArrayType arrayType, JavaScriptValueSafeHandle baseArray, uint byteOffset, uint elementLength, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsCreateTypedArray(arrayType, baseArray, byteOffset, elementLength, out result);
			}

			public JavaScriptErrorCode JsCreateDataView(JavaScriptValueSafeHandle arrayBuffer, uint byteOffset, uint byteLength, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsCreateDataView(arrayBuffer, byteOffset, byteLength, out result);
			}

			public JavaScriptErrorCode JsGetTypedArrayInfo(JavaScriptValueSafeHandle typedArray, out JavaScriptTypedArrayType arrayType, out JavaScriptValueSafeHandle arrayBuffer, out uint byteOffset, out uint byteLength)
			{
				return LibChakraCore.JsGetTypedArrayInfo(typedArray, out arrayType, out arrayBuffer, out byteOffset, out byteLength);
			}

			public JavaScriptErrorCode JsGetArrayBufferStorage(JavaScriptValueSafeHandle arrayBuffer, out byte[] buffer, out uint bufferLength)
			{
				return LibChakraCore.JsGetArrayBufferStorage(arrayBuffer, out buffer, out bufferLength);
			}

			public JavaScriptErrorCode JsGetTypedArrayStorage(JavaScriptValueSafeHandle typedArray, out byte[] buffer, out uint bufferLength, out JavaScriptTypedArrayType arrayType, out int elementSize)
			{
				return LibChakraCore.JsGetTypedArrayStorage(typedArray, out buffer, out bufferLength, out arrayType, out elementSize);
			}

			public JavaScriptErrorCode JsGetDataViewStorage(JavaScriptValueSafeHandle dataView, out byte[] buffer, out uint bufferLength)
			{
				return LibChakraCore.JsGetDataViewStorage(dataView, out buffer, out bufferLength);
			}

			public JavaScriptErrorCode JsCallFunction(JavaScriptValueSafeHandle @function, IntPtr[] arguments, ushort argumentCount, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsCallFunction(@function, arguments, argumentCount, out result);
			}

			public JavaScriptErrorCode JsConstructObject(JavaScriptValueSafeHandle @function, IntPtr[] arguments, ushort argumentCount, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsConstructObject(@function, arguments, argumentCount, out result);
			}

			public JavaScriptErrorCode JsCreateFunction(JavaScriptNativeFunction nativeFunction, IntPtr callbackState, out JavaScriptValueSafeHandle @function)
			{
				return LibChakraCore.JsCreateFunction(nativeFunction, callbackState, out @function);
			}

			public JavaScriptErrorCode JsCreateNamedFunction(JavaScriptValueSafeHandle name, JavaScriptNativeFunction nativeFunction, IntPtr callbackState, out JavaScriptValueSafeHandle @function)
			{
				return LibChakraCore.JsCreateNamedFunction(name, nativeFunction, callbackState, out @function);
			}

			public JavaScriptErrorCode JsCreateError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				return LibChakraCore.JsCreateError(message, out error);
			}

			public JavaScriptErrorCode JsCreateRangeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				return LibChakraCore.JsCreateRangeError(message, out error);
			}

			public JavaScriptErrorCode JsCreateReferenceError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				return LibChakraCore.JsCreateReferenceError(message, out error);
			}

			public JavaScriptErrorCode JsCreateSyntaxError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				return LibChakraCore.JsCreateSyntaxError(message, out error);
			}

			public JavaScriptErrorCode JsCreateTypeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				return LibChakraCore.JsCreateTypeError(message, out error);
			}

			public JavaScriptErrorCode JsCreateURIError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				return LibChakraCore.JsCreateURIError(message, out error);
			}

			public JavaScriptErrorCode JsHasException(out bool hasException)
			{
				return LibChakraCore.JsHasException(out hasException);
			}

			public JavaScriptErrorCode JsGetAndClearException(out JavaScriptValueSafeHandle exception)
			{
				return LibChakraCore.JsGetAndClearException(out exception);
			}

			public JavaScriptErrorCode JsSetException(JavaScriptValueSafeHandle exception)
			{
				return LibChakraCore.JsSetException(exception);
			}

			public JavaScriptErrorCode JsDisableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime)
			{
				return LibChakraCore.JsDisableRuntimeExecution(runtime);
			}

			public JavaScriptErrorCode JsEnableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime)
			{
				return LibChakraCore.JsEnableRuntimeExecution(runtime);
			}

			public JavaScriptErrorCode JsIsRuntimeExecutionDisabled(JavaScriptRuntimeSafeHandle runtime, out bool isDisabled)
			{
				return LibChakraCore.JsIsRuntimeExecutionDisabled(runtime, out isDisabled);
			}

			public JavaScriptErrorCode JsSetPromiseContinuationCallback(JavaScriptPromiseContinuationCallback promiseContinuationCallback, IntPtr callbackState)
			{
				return LibChakraCore.JsSetPromiseContinuationCallback(promiseContinuationCallback, callbackState);
			}

			public JavaScriptErrorCode JsParseScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsParseScript(script, sourceContext, sourceUrl, out result);
			}

			public JavaScriptErrorCode JsParseScriptWithAttributes(string script, JavaScriptSourceContext sourceContext, string sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsParseScriptWithAttributes(script, sourceContext, sourceUrl, parseAttributes, out result);
			}

			public JavaScriptErrorCode JsRunScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsRunScript(script, sourceContext, sourceUrl, out result);
			}

			public JavaScriptErrorCode JsExperimentalApiRunModule(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsExperimentalApiRunModule(script, sourceContext, sourceUrl, out result);
			}

			public JavaScriptErrorCode JsSerializeScript(string script, byte[] buffer, ref ulong bufferSize)
			{
				return LibChakraCore.JsSerializeScript(script, buffer, ref bufferSize);
			}

			public JavaScriptErrorCode JsParseSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsParseSerializedScriptWithCallback(scriptLoadCallback, scriptUnloadCallback, buffer, sourceContext, sourceUrl, out result);
			}

			public JavaScriptErrorCode JsRunSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsRunSerializedScriptWithCallback(scriptLoadCallback, scriptUnloadCallback, buffer, sourceContext, sourceUrl, out result);
			}

			public JavaScriptErrorCode JsParseSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsParseSerializedScript(script, buffer, sourceContext, sourceUrl, out result);
			}

			public JavaScriptErrorCode JsRunSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return LibChakraCore.JsRunSerializedScript(script, buffer, sourceContext, sourceUrl, out result);
			}

			public JavaScriptErrorCode JsGetPropertyIdFromName(string name, out JavaScriptPropertyIdSafeHandle propertyId)
			{
				return LibChakraCore.JsGetPropertyIdFromName(name, out propertyId);
			}

			public JavaScriptErrorCode JsGetPropertyNameFromId(JavaScriptPropertyIdSafeHandle propertyId, out string name)
			{
				return LibChakraCore.JsGetPropertyNameFromId(propertyId, out name);
			}

			public JavaScriptErrorCode JsPointerToString(string stringValue, ulong stringLength, out JavaScriptValueSafeHandle value)
			{
				return LibChakraCore.JsPointerToString(stringValue, stringLength, out value);
			}

			public JavaScriptErrorCode JsStringToPointer(JavaScriptValueSafeHandle value, out IntPtr stringValue, out ulong stringLength)
			{
				return LibChakraCore.JsStringToPointer(value, out stringValue, out stringLength);
			}

			public JavaScriptErrorCode JsDiagStartDebugging(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagDebugEventCallback debugEventCallback, IntPtr callbackState)
			{
				return LibChakraCore.JsDiagStartDebugging(runtimeHandle, debugEventCallback, callbackState);
			}

	}
}