namespace BaristaLabs.BaristaCore.JavaScript
{
	using Callbacks;
    using Interfaces;
	using SafeHandles;

	using System;
	using System.Runtime.InteropServices;
	using System.Text;

	internal sealed partial class ChakraApi {
		
		private class ChakraApi_Windows : IChakraApi, IChakraCommonWindows
		{
			public JsErrorCode JsInitializeModuleRecord(IntPtr referencingModule, JavaScriptValueSafeHandle normalizedSpecifier, out IntPtr moduleRecord)
			{
				return NativeMethods.JsInitializeModuleRecord(referencingModule, normalizedSpecifier, out moduleRecord);
			}
			public JsErrorCode JsParseModuleSource(IntPtr requestModule, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags sourceFlag, out JavaScriptValueSafeHandle exceptionValueRef)
			{
				return NativeMethods.JsParseModuleSource(requestModule, sourceContext, script, scriptLength, sourceFlag, out exceptionValueRef);
			}
			public JsErrorCode JsModuleEvaluation(IntPtr requestModule, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsModuleEvaluation(requestModule, out result);
			}
			public JsErrorCode JsSetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, IntPtr hostInfo)
			{
				return NativeMethods.JsSetModuleHostInfo(requestModule, moduleHostInfo, hostInfo);
			}
			public JsErrorCode JsGetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, out IntPtr hostInfo)
			{
				return NativeMethods.JsGetModuleHostInfo(requestModule, moduleHostInfo, out hostInfo);
			}
			public JsErrorCode JsCreateString(string content, UIntPtr length, out JavaScriptValueSafeHandle value)
			{
				return NativeMethods.JsCreateString(content, length, out value);
			}
			public JsErrorCode JsCreateStringUtf8(string content, UIntPtr length, out JavaScriptValueSafeHandle value)
			{
				return NativeMethods.JsCreateStringUtf8(content, length, out value);
			}
			public JsErrorCode JsCreateStringUtf16(string content, UIntPtr length, out JavaScriptValueSafeHandle value)
			{
				return NativeMethods.JsCreateStringUtf16(content, length, out value);
			}
			public JsErrorCode JsCopyString(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out UIntPtr written)
			{
				return NativeMethods.JsCopyString(value, start, length, buffer, out written);
			}
			public JsErrorCode JsCopyStringUtf8(JavaScriptValueSafeHandle value, byte[] buffer, UIntPtr bufferSize, out UIntPtr written)
			{
				return NativeMethods.JsCopyStringUtf8(value, buffer, bufferSize, out written);
			}
			public JsErrorCode JsCopyStringUtf16(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out UIntPtr written)
			{
				return NativeMethods.JsCopyStringUtf16(value, start, length, buffer, out written);
			}
			public JsErrorCode JsParse(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JsParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsParse(script, sourceContext, sourceUrl, parseAttributes, out result);
			}
			public JsErrorCode JsRun(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JsParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsRun(script, sourceContext, sourceUrl, parseAttributes, out result);
			}
			public JsErrorCode JsCreatePropertyIdUtf8(byte[] name, UIntPtr length, out JavaScriptPropertyId propertyId)
			{
				return NativeMethods.JsCreatePropertyIdUtf8(name, length, out propertyId);
			}
			public JsErrorCode JsCopyPropertyIdUtf8(JavaScriptPropertyId propertyId, out byte[] buffer, UIntPtr bufferSize, out UIntPtr length)
			{
				return NativeMethods.JsCopyPropertyIdUtf8(propertyId, out buffer, bufferSize, out length);
			}
			public JsErrorCode JsSerialize(JavaScriptValueSafeHandle script, out byte[] buffer, ref uint bufferSize, JsParseScriptAttributes Name)
			{
				return NativeMethods.JsSerialize(script, out buffer, ref bufferSize, Name);
			}
			public JsErrorCode JsParseSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsParseSerialized(buffer, scriptLoadCallback, sourceContext, sourceUrl, out result);
			}
			public JsErrorCode JsRunSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsRunSerialized(buffer, scriptLoadCallback, sourceContext, sourceUrl, out result);
			}
			public JsErrorCode JsCreateRuntime(JsRuntimeAttributes attributes, JavaScriptThreadServiceCallback threadService, out JavaScriptRuntimeSafeHandle runtime)
			{
				return NativeMethods.JsCreateRuntime(attributes, threadService, out runtime);
			}
			public JsErrorCode JsCollectGarbage(JavaScriptRuntimeSafeHandle handle)
			{
				return NativeMethods.JsCollectGarbage(handle);
			}
			public JsErrorCode JsDisposeRuntime(IntPtr runtime)
			{
				return NativeMethods.JsDisposeRuntime(runtime);
			}
			public JsErrorCode JsGetRuntimeMemoryUsage(JavaScriptRuntimeSafeHandle runtime, out UIntPtr usage)
			{
				return NativeMethods.JsGetRuntimeMemoryUsage(runtime, out usage);
			}
			public JsErrorCode JsGetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, out UIntPtr memoryLimit)
			{
				return NativeMethods.JsGetRuntimeMemoryLimit(runtime, out memoryLimit);
			}
			public JsErrorCode JsSetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, UIntPtr memoryLimit)
			{
				return NativeMethods.JsSetRuntimeMemoryLimit(runtime, memoryLimit);
			}
			public JsErrorCode JsSetRuntimeMemoryAllocationCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr extraInformation, JavaScriptMemoryAllocationCallback allocationCallback)
			{
				return NativeMethods.JsSetRuntimeMemoryAllocationCallback(runtime, extraInformation, allocationCallback);
			}
			public JsErrorCode JsSetRuntimeBeforeCollectCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptBeforeCollectCallback beforeCollectCallback)
			{
				return NativeMethods.JsSetRuntimeBeforeCollectCallback(runtime, callbackState, beforeCollectCallback);
			}
			public JsErrorCode JsAddRef(IntPtr @ref, out uint count)
			{
				return NativeMethods.JsAddRef(@ref, out count);
			}
			public JsErrorCode JsReleaseContext(JavaScriptContextSafeHandle context, out uint count)
			{
				return NativeMethods.JsReleaseContext(context, out count);
			}
			public JsErrorCode JsReleaseValue(JavaScriptValueSafeHandle value, out uint count)
			{
				return NativeMethods.JsReleaseValue(value, out count);
			}
			public JsErrorCode JsRelease(IntPtr @ref, out uint count)
			{
				return NativeMethods.JsRelease(@ref, out count);
			}
			public JsErrorCode JsSetObjectBeforeCollectCallback(JavaScriptValueSafeHandle @ref, IntPtr callbackState, JavaScriptObjectBeforeCollectCallback objectBeforeCollectCallback)
			{
				return NativeMethods.JsSetObjectBeforeCollectCallback(@ref, callbackState, objectBeforeCollectCallback);
			}
			public JsErrorCode JsCreateContext(JavaScriptRuntimeSafeHandle runtime, out JavaScriptContextSafeHandle context)
			{
				return NativeMethods.JsCreateContext(runtime, out context);
			}
			public JsErrorCode JsGetCurrentContext(out JavaScriptContextSafeHandle context)
			{
				return NativeMethods.JsGetCurrentContext(out context);
			}
			public JsErrorCode JsSetCurrentContext(JavaScriptContextSafeHandle context)
			{
				return NativeMethods.JsSetCurrentContext(context);
			}
			public JsErrorCode JsGetContextOfObject(JavaScriptValueSafeHandle @object, out JavaScriptContextSafeHandle Nacontextme)
			{
				return NativeMethods.JsGetContextOfObject(@object, out Nacontextme);
			}
			public JsErrorCode JsGetContextData(JavaScriptContextSafeHandle context, out IntPtr data)
			{
				return NativeMethods.JsGetContextData(context, out data);
			}
			public JsErrorCode JsSetContextData(JavaScriptContextSafeHandle context, IntPtr data)
			{
				return NativeMethods.JsSetContextData(context, data);
			}
			public JsErrorCode JsGetRuntime(JavaScriptContextSafeHandle context, out JavaScriptRuntimeSafeHandle runtime)
			{
				return NativeMethods.JsGetRuntime(context, out runtime);
			}
			public JsErrorCode JsIdle(out uint nextIdleTick)
			{
				return NativeMethods.JsIdle(out nextIdleTick);
			}
			public JsErrorCode JsGetSymbolFromPropertyId(JavaScriptPropertyId propertyId, out JavaScriptValueSafeHandle symbol)
			{
				return NativeMethods.JsGetSymbolFromPropertyId(propertyId, out symbol);
			}
			public JsErrorCode JsGetPropertyIdType(JavaScriptPropertyId propertyId, out JavaSciptPropertyIdType propertyIdType)
			{
				return NativeMethods.JsGetPropertyIdType(propertyId, out propertyIdType);
			}
			public JsErrorCode JsGetPropertyIdFromSymbol(JavaScriptValueSafeHandle symbol, out JavaScriptPropertyId propertyId)
			{
				return NativeMethods.JsGetPropertyIdFromSymbol(symbol, out propertyId);
			}
			public JsErrorCode JsCreateSymbol(JavaScriptValueSafeHandle description, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsCreateSymbol(description, out result);
			}
			public JsErrorCode JsGetOwnPropertySymbols(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle propertySymbols)
			{
				return NativeMethods.JsGetOwnPropertySymbols(@object, out propertySymbols);
			}
			public JsErrorCode JsGetUndefinedValue(out JavaScriptValueSafeHandle undefinedValue)
			{
				return NativeMethods.JsGetUndefinedValue(out undefinedValue);
			}
			public JsErrorCode JsGetNullValue(out JavaScriptValueSafeHandle nullValue)
			{
				return NativeMethods.JsGetNullValue(out nullValue);
			}
			public JsErrorCode JsGetTrueValue(out JavaScriptValueSafeHandle trueValue)
			{
				return NativeMethods.JsGetTrueValue(out trueValue);
			}
			public JsErrorCode JsGetFalseValue(out JavaScriptValueSafeHandle falseValue)
			{
				return NativeMethods.JsGetFalseValue(out falseValue);
			}
			public JsErrorCode JsBoolToBoolean(bool value, out JavaScriptValueSafeHandle booleanValue)
			{
				return NativeMethods.JsBoolToBoolean(value, out booleanValue);
			}
			public JsErrorCode JsBooleanToBool(JavaScriptValueSafeHandle value, out bool boolValue)
			{
				return NativeMethods.JsBooleanToBool(value, out boolValue);
			}
			public JsErrorCode JsConvertValueToBoolean(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle booleanValue)
			{
				return NativeMethods.JsConvertValueToBoolean(value, out booleanValue);
			}
			public JsErrorCode JsGetValueType(JavaScriptValueSafeHandle value, out JsValueType type)
			{
				return NativeMethods.JsGetValueType(value, out type);
			}
			public JsErrorCode JsDoubleToNumber(double doubleValue, out JavaScriptValueSafeHandle value)
			{
				return NativeMethods.JsDoubleToNumber(doubleValue, out value);
			}
			public JsErrorCode JsIntToNumber(int intValue, out JavaScriptValueSafeHandle value)
			{
				return NativeMethods.JsIntToNumber(intValue, out value);
			}
			public JsErrorCode JsNumberToDouble(JavaScriptValueSafeHandle value, out double doubleValue)
			{
				return NativeMethods.JsNumberToDouble(value, out doubleValue);
			}
			public JsErrorCode JsNumberToInt(JavaScriptValueSafeHandle value, out int intValue)
			{
				return NativeMethods.JsNumberToInt(value, out intValue);
			}
			public JsErrorCode JsConvertValueToNumber(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle numberValue)
			{
				return NativeMethods.JsConvertValueToNumber(value, out numberValue);
			}
			public JsErrorCode JsGetStringLength(JavaScriptValueSafeHandle stringValue, out int length)
			{
				return NativeMethods.JsGetStringLength(stringValue, out length);
			}
			public JsErrorCode JsConvertValueToString(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle stringValue)
			{
				return NativeMethods.JsConvertValueToString(value, out stringValue);
			}
			public JsErrorCode JsGetGlobalObject(out JavaScriptValueSafeHandle globalObject)
			{
				return NativeMethods.JsGetGlobalObject(out globalObject);
			}
			public JsErrorCode JsCreateObject(out JavaScriptValueSafeHandle @object)
			{
				return NativeMethods.JsCreateObject(out @object);
			}
			public JsErrorCode JsCreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback, out JavaScriptValueSafeHandle @object)
			{
				return NativeMethods.JsCreateExternalObject(data, finalizeCallback, out @object);
			}
			public JsErrorCode JsConvertValueToObject(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle @object)
			{
				return NativeMethods.JsConvertValueToObject(value, out @object);
			}
			public JsErrorCode JsGetPrototype(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle prototypeObject)
			{
				return NativeMethods.JsGetPrototype(@object, out prototypeObject);
			}
			public JsErrorCode JsSetPrototype(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle prototypeObject)
			{
				return NativeMethods.JsSetPrototype(@object, prototypeObject);
			}
			public JsErrorCode JsInstanceOf(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle constructor, out bool result)
			{
				return NativeMethods.JsInstanceOf(@object, constructor, out result);
			}
			public JsErrorCode JsGetExtensionAllowed(JavaScriptValueSafeHandle @object, out bool value)
			{
				return NativeMethods.JsGetExtensionAllowed(@object, out value);
			}
			public JsErrorCode JsPreventExtension(JavaScriptValueSafeHandle @object)
			{
				return NativeMethods.JsPreventExtension(@object);
			}
			public JsErrorCode JsGetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyId propertyId, out JavaScriptValueSafeHandle value)
			{
				return NativeMethods.JsGetProperty(@object, propertyId, out value);
			}
			public JsErrorCode JsGetOwnPropertyDescriptor(JavaScriptValueSafeHandle @object, JavaScriptPropertyId propertyId, out JavaScriptValueSafeHandle propertyDescriptor)
			{
				return NativeMethods.JsGetOwnPropertyDescriptor(@object, propertyId, out propertyDescriptor);
			}
			public JsErrorCode JsGetOwnPropertyNames(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle propertyNames)
			{
				return NativeMethods.JsGetOwnPropertyNames(@object, out propertyNames);
			}
			public JsErrorCode JsSetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyId propertyId, JavaScriptValueSafeHandle value, bool useStrictRules)
			{
				return NativeMethods.JsSetProperty(@object, propertyId, value, useStrictRules);
			}
			public JsErrorCode JsHasProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyId propertyId, out bool hasProperty)
			{
				return NativeMethods.JsHasProperty(@object, propertyId, out hasProperty);
			}
			public JsErrorCode JsDeleteProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyId propertyId, bool useStrictRules, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsDeleteProperty(@object, propertyId, useStrictRules, out result);
			}
			public JsErrorCode JsDefineProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyId propertyId, JavaScriptValueSafeHandle propertyDescriptor, out bool result)
			{
				return NativeMethods.JsDefineProperty(@object, propertyId, propertyDescriptor, out result);
			}
			public JsErrorCode JsHasIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, out bool result)
			{
				return NativeMethods.JsHasIndexedProperty(@object, index, out result);
			}
			public JsErrorCode JsGetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsGetIndexedProperty(@object, index, out result);
			}
			public JsErrorCode JsSetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, JavaScriptValueSafeHandle value)
			{
				return NativeMethods.JsSetIndexedProperty(@object, index, value);
			}
			public JsErrorCode JsDeleteIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index)
			{
				return NativeMethods.JsDeleteIndexedProperty(@object, index);
			}
			public JsErrorCode JsHasIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, out bool value)
			{
				return NativeMethods.JsHasIndexedPropertiesExternalData(@object, out value);
			}
			public JsErrorCode JsGetIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, IntPtr data, out JavaScriptTypedArrayType arrayType, out uint elementLength)
			{
				return NativeMethods.JsGetIndexedPropertiesExternalData(@object, data, out arrayType, out elementLength);
			}
			public JsErrorCode JsSetIndexedPropertiesToExternalData(JavaScriptValueSafeHandle @object, IntPtr Data, JavaScriptTypedArrayType arrayType, uint elementLength)
			{
				return NativeMethods.JsSetIndexedPropertiesToExternalData(@object, Data, arrayType, elementLength);
			}
			public JsErrorCode JsEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result)
			{
				return NativeMethods.JsEquals(object1, object2, out result);
			}
			public JsErrorCode JsStrictEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result)
			{
				return NativeMethods.JsStrictEquals(object1, object2, out result);
			}
			public JsErrorCode JsHasExternalData(JavaScriptValueSafeHandle @object, out bool value)
			{
				return NativeMethods.JsHasExternalData(@object, out value);
			}
			public JsErrorCode JsGetExternalData(JavaScriptValueSafeHandle @object, out IntPtr externalData)
			{
				return NativeMethods.JsGetExternalData(@object, out externalData);
			}
			public JsErrorCode JsSetExternalData(JavaScriptValueSafeHandle @object, IntPtr externalData)
			{
				return NativeMethods.JsSetExternalData(@object, externalData);
			}
			public JsErrorCode JsCreateArray(uint length, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsCreateArray(length, out result);
			}
			public JsErrorCode JsCreateArrayBuffer(uint byteLength, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsCreateArrayBuffer(byteLength, out result);
			}
			public JsErrorCode JsCreateExternalArrayBuffer(IntPtr data, uint byteLength, JavaScriptObjectFinalizeCallback finalizeCallback, IntPtr callbackState, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsCreateExternalArrayBuffer(data, byteLength, finalizeCallback, callbackState, out result);
			}
			public JsErrorCode JsCreateTypedArray(JavaScriptTypedArrayType arrayType, JavaScriptValueSafeHandle baseArray, uint byteOffset, uint elementLength, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsCreateTypedArray(arrayType, baseArray, byteOffset, elementLength, out result);
			}
			public JsErrorCode JsCreateDataView(JavaScriptValueSafeHandle arrayBuffer, uint byteOffset, uint byteLength, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsCreateDataView(arrayBuffer, byteOffset, byteLength, out result);
			}
			public JsErrorCode JsGetTypedArrayInfo(JavaScriptValueSafeHandle typedArray, out JavaScriptTypedArrayType arrayType, out JavaScriptValueSafeHandle arrayBuffer, out uint byteOffset, out uint byteLength)
			{
				return NativeMethods.JsGetTypedArrayInfo(typedArray, out arrayType, out arrayBuffer, out byteOffset, out byteLength);
			}
			public JsErrorCode JsGetArrayBufferStorage(JavaScriptValueSafeHandle arrayBuffer, out byte[] buffer, out uint bufferLength)
			{
				return NativeMethods.JsGetArrayBufferStorage(arrayBuffer, out buffer, out bufferLength);
			}
			public JsErrorCode JsGetTypedArrayStorage(JavaScriptValueSafeHandle typedArray, out byte[] buffer, out uint bufferLength, out JavaScriptTypedArrayType arrayType, out int elementSize)
			{
				return NativeMethods.JsGetTypedArrayStorage(typedArray, out buffer, out bufferLength, out arrayType, out elementSize);
			}
			public JsErrorCode JsGetDataViewStorage(JavaScriptValueSafeHandle dataView, out byte[] buffer, out uint bufferLength)
			{
				return NativeMethods.JsGetDataViewStorage(dataView, out buffer, out bufferLength);
			}
			public JsErrorCode JsCallFunction(JavaScriptValueSafeHandle @function, IntPtr[] arguments, ushort argumentCount, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsCallFunction(@function, arguments, argumentCount, out result);
			}
			public JsErrorCode JsConstructObject(JavaScriptValueSafeHandle @function, IntPtr[] arguments, ushort argumentCount, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsConstructObject(@function, arguments, argumentCount, out result);
			}
			public JsErrorCode JsCreateFunction(JavaScriptNativeFunction nativeFunction, IntPtr callbackState, out JavaScriptValueSafeHandle @function)
			{
				return NativeMethods.JsCreateFunction(nativeFunction, callbackState, out @function);
			}
			public JsErrorCode JsCreateNamedFunction(JavaScriptValueSafeHandle name, JavaScriptNativeFunction nativeFunction, IntPtr callbackState, out JavaScriptValueSafeHandle @function)
			{
				return NativeMethods.JsCreateNamedFunction(name, nativeFunction, callbackState, out @function);
			}
			public JsErrorCode JsCreateError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				return NativeMethods.JsCreateError(message, out error);
			}
			public JsErrorCode JsCreateRangeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				return NativeMethods.JsCreateRangeError(message, out error);
			}
			public JsErrorCode JsCreateReferenceError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				return NativeMethods.JsCreateReferenceError(message, out error);
			}
			public JsErrorCode JsCreateSyntaxError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				return NativeMethods.JsCreateSyntaxError(message, out error);
			}
			public JsErrorCode JsCreateTypeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				return NativeMethods.JsCreateTypeError(message, out error);
			}
			public JsErrorCode JsCreateURIError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				return NativeMethods.JsCreateURIError(message, out error);
			}
			public JsErrorCode JsHasException(out bool hasException)
			{
				return NativeMethods.JsHasException(out hasException);
			}
			public JsErrorCode JsGetAndClearException(out JavaScriptValueSafeHandle exception)
			{
				return NativeMethods.JsGetAndClearException(out exception);
			}
			public JsErrorCode JsSetException(JavaScriptValueSafeHandle exception)
			{
				return NativeMethods.JsSetException(exception);
			}
			public JsErrorCode JsDisableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime)
			{
				return NativeMethods.JsDisableRuntimeExecution(runtime);
			}
			public JsErrorCode JsEnableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime)
			{
				return NativeMethods.JsEnableRuntimeExecution(runtime);
			}
			public JsErrorCode JsIsRuntimeExecutionDisabled(JavaScriptRuntimeSafeHandle runtime, out bool isDisabled)
			{
				return NativeMethods.JsIsRuntimeExecutionDisabled(runtime, out isDisabled);
			}
			public JsErrorCode JsSetPromiseContinuationCallback(JavaScriptPromiseContinuationCallback promiseContinuationCallback, IntPtr callbackState)
			{
				return NativeMethods.JsSetPromiseContinuationCallback(promiseContinuationCallback, callbackState);
			}
			public JsErrorCode JsParseScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsParseScript(script, sourceContext, sourceUrl, out result);
			}
			public JsErrorCode JsParseScriptWithAttributes(string script, JavaScriptSourceContext sourceContext, string sourceUrl, JsParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsParseScriptWithAttributes(script, sourceContext, sourceUrl, parseAttributes, out result);
			}
			public JsErrorCode JsRunScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsRunScript(script, sourceContext, sourceUrl, out result);
			}
			public JsErrorCode JsExperimentalApiRunModule(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsExperimentalApiRunModule(script, sourceContext, sourceUrl, out result);
			}
			public JsErrorCode JsSerializeScript(string script, byte[] buffer, ref ulong bufferSize)
			{
				return NativeMethods.JsSerializeScript(script, buffer, ref bufferSize);
			}
			public JsErrorCode JsParseSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsParseSerializedScriptWithCallback(scriptLoadCallback, scriptUnloadCallback, buffer, sourceContext, sourceUrl, out result);
			}
			public JsErrorCode JsRunSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsRunSerializedScriptWithCallback(scriptLoadCallback, scriptUnloadCallback, buffer, sourceContext, sourceUrl, out result);
			}
			public JsErrorCode JsParseSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsParseSerializedScript(script, buffer, sourceContext, sourceUrl, out result);
			}
			public JsErrorCode JsRunSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				return NativeMethods.JsRunSerializedScript(script, buffer, sourceContext, sourceUrl, out result);
			}
			public JsErrorCode JsGetPropertyIdFromName(string name, out JavaScriptPropertyId propertyId)
			{
				return NativeMethods.JsGetPropertyIdFromName(name, out propertyId);
			}
			public JsErrorCode JsGetPropertyNameFromId(JavaScriptPropertyId propertyId, out string name)
			{
				return NativeMethods.JsGetPropertyNameFromId(propertyId, out name);
			}
			public JsErrorCode JsPointerToString(string stringValue, UIntPtr stringLength, out JavaScriptValueSafeHandle value)
			{
				return NativeMethods.JsPointerToString(stringValue, stringLength, out value);
			}
			public JsErrorCode JsStringToPointer(JavaScriptValueSafeHandle value, out IntPtr stringValue, out UIntPtr stringLength)
			{
				return NativeMethods.JsStringToPointer(value, out stringValue, out stringLength);
			}
			public JsErrorCode JsDiagStartDebugging(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagDebugEventCallback debugEventCallback, IntPtr callbackState)
			{
				return NativeMethods.JsDiagStartDebugging(runtimeHandle, debugEventCallback, callbackState);
			}

			private static class NativeMethods {

				const string DllName = "win_x64_rel_chakracore.dll";

				[DllImport(DllName)]
				internal static extern JsErrorCode JsInitializeModuleRecord(IntPtr referencingModule, JavaScriptValueSafeHandle normalizedSpecifier, out IntPtr moduleRecord);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsParseModuleSource(IntPtr requestModule, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags sourceFlag, out JavaScriptValueSafeHandle exceptionValueRef);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsModuleEvaluation(IntPtr requestModule, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, IntPtr hostInfo);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, out IntPtr hostInfo);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateString(string content, UIntPtr length, out JavaScriptValueSafeHandle value);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateStringUtf8(string content, UIntPtr length, out JavaScriptValueSafeHandle value);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsCreateStringUtf16(string content, UIntPtr length, out JavaScriptValueSafeHandle value);

				[DllImport(DllName, CharSet = CharSet.Ansi)]
				internal static extern JsErrorCode JsCopyString(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out UIntPtr written);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCopyStringUtf8(JavaScriptValueSafeHandle value, byte[] buffer, UIntPtr bufferSize, out UIntPtr written);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCopyStringUtf16(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out UIntPtr written);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsParse(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JsParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsRun(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JsParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreatePropertyIdUtf8(byte[] name, UIntPtr length, out JavaScriptPropertyId propertyId);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCopyPropertyIdUtf8(JavaScriptPropertyId propertyId, out byte[] buffer, UIntPtr bufferSize, out UIntPtr length);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSerialize(JavaScriptValueSafeHandle script, out byte[] buffer, ref uint bufferSize, JsParseScriptAttributes Name);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsParseSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsRunSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateRuntime(JsRuntimeAttributes attributes, JavaScriptThreadServiceCallback threadService, out JavaScriptRuntimeSafeHandle runtime);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCollectGarbage(JavaScriptRuntimeSafeHandle handle);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsDisposeRuntime(IntPtr runtime);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetRuntimeMemoryUsage(JavaScriptRuntimeSafeHandle runtime, out UIntPtr usage);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, out UIntPtr memoryLimit);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, UIntPtr memoryLimit);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetRuntimeMemoryAllocationCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr extraInformation, JavaScriptMemoryAllocationCallback allocationCallback);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetRuntimeBeforeCollectCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptBeforeCollectCallback beforeCollectCallback);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsAddRef(IntPtr @ref, out uint count);

				[DllImport(DllName, EntryPoint = "JsRelease" )]
				internal static extern JsErrorCode JsReleaseContext(JavaScriptContextSafeHandle context, out uint count);

				[DllImport(DllName, EntryPoint = "JsRelease" )]
				internal static extern JsErrorCode JsReleaseValue(JavaScriptValueSafeHandle value, out uint count);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsRelease(IntPtr @ref, out uint count);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetObjectBeforeCollectCallback(JavaScriptValueSafeHandle @ref, IntPtr callbackState, JavaScriptObjectBeforeCollectCallback objectBeforeCollectCallback);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateContext(JavaScriptRuntimeSafeHandle runtime, out JavaScriptContextSafeHandle context);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetCurrentContext(out JavaScriptContextSafeHandle context);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetCurrentContext(JavaScriptContextSafeHandle context);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetContextOfObject(JavaScriptValueSafeHandle @object, out JavaScriptContextSafeHandle Nacontextme);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetContextData(JavaScriptContextSafeHandle context, out IntPtr data);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetContextData(JavaScriptContextSafeHandle context, IntPtr data);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetRuntime(JavaScriptContextSafeHandle context, out JavaScriptRuntimeSafeHandle runtime);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsIdle(out uint nextIdleTick);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetSymbolFromPropertyId(JavaScriptPropertyId propertyId, out JavaScriptValueSafeHandle symbol);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetPropertyIdType(JavaScriptPropertyId propertyId, out JavaSciptPropertyIdType propertyIdType);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetPropertyIdFromSymbol(JavaScriptValueSafeHandle symbol, out JavaScriptPropertyId propertyId);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateSymbol(JavaScriptValueSafeHandle description, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetOwnPropertySymbols(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle propertySymbols);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetUndefinedValue(out JavaScriptValueSafeHandle undefinedValue);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetNullValue(out JavaScriptValueSafeHandle nullValue);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetTrueValue(out JavaScriptValueSafeHandle trueValue);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetFalseValue(out JavaScriptValueSafeHandle falseValue);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsBoolToBoolean(bool value, out JavaScriptValueSafeHandle booleanValue);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsBooleanToBool(JavaScriptValueSafeHandle value, out bool boolValue);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsConvertValueToBoolean(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle booleanValue);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetValueType(JavaScriptValueSafeHandle value, out JsValueType type);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsDoubleToNumber(double doubleValue, out JavaScriptValueSafeHandle value);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsIntToNumber(int intValue, out JavaScriptValueSafeHandle value);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsNumberToDouble(JavaScriptValueSafeHandle value, out double doubleValue);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsNumberToInt(JavaScriptValueSafeHandle value, out int intValue);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsConvertValueToNumber(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle numberValue);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetStringLength(JavaScriptValueSafeHandle stringValue, out int length);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsConvertValueToString(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle stringValue);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetGlobalObject(out JavaScriptValueSafeHandle globalObject);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateObject(out JavaScriptValueSafeHandle @object);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback, out JavaScriptValueSafeHandle @object);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsConvertValueToObject(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle @object);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetPrototype(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle prototypeObject);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetPrototype(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle prototypeObject);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsInstanceOf(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle constructor, out bool result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetExtensionAllowed(JavaScriptValueSafeHandle @object, out bool value);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsPreventExtension(JavaScriptValueSafeHandle @object);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyId propertyId, out JavaScriptValueSafeHandle value);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetOwnPropertyDescriptor(JavaScriptValueSafeHandle @object, JavaScriptPropertyId propertyId, out JavaScriptValueSafeHandle propertyDescriptor);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetOwnPropertyNames(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle propertyNames);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyId propertyId, JavaScriptValueSafeHandle value, bool useStrictRules);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsHasProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyId propertyId, out bool hasProperty);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsDeleteProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyId propertyId, bool useStrictRules, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsDefineProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyId propertyId, JavaScriptValueSafeHandle propertyDescriptor, out bool result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsHasIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, out bool result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, JavaScriptValueSafeHandle value);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsDeleteIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsHasIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, out bool value);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, IntPtr data, out JavaScriptTypedArrayType arrayType, out uint elementLength);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetIndexedPropertiesToExternalData(JavaScriptValueSafeHandle @object, IntPtr Data, JavaScriptTypedArrayType arrayType, uint elementLength);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsStrictEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsHasExternalData(JavaScriptValueSafeHandle @object, out bool value);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetExternalData(JavaScriptValueSafeHandle @object, out IntPtr externalData);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetExternalData(JavaScriptValueSafeHandle @object, IntPtr externalData);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateArray(uint length, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateArrayBuffer(uint byteLength, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateExternalArrayBuffer(IntPtr data, uint byteLength, JavaScriptObjectFinalizeCallback finalizeCallback, IntPtr callbackState, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateTypedArray(JavaScriptTypedArrayType arrayType, JavaScriptValueSafeHandle baseArray, uint byteOffset, uint elementLength, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateDataView(JavaScriptValueSafeHandle arrayBuffer, uint byteOffset, uint byteLength, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetTypedArrayInfo(JavaScriptValueSafeHandle typedArray, out JavaScriptTypedArrayType arrayType, out JavaScriptValueSafeHandle arrayBuffer, out uint byteOffset, out uint byteLength);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetArrayBufferStorage(JavaScriptValueSafeHandle arrayBuffer, out byte[] buffer, out uint bufferLength);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetTypedArrayStorage(JavaScriptValueSafeHandle typedArray, out byte[] buffer, out uint bufferLength, out JavaScriptTypedArrayType arrayType, out int elementSize);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetDataViewStorage(JavaScriptValueSafeHandle dataView, out byte[] buffer, out uint bufferLength);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCallFunction(JavaScriptValueSafeHandle @function, IntPtr[] arguments, ushort argumentCount, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsConstructObject(JavaScriptValueSafeHandle @function, IntPtr[] arguments, ushort argumentCount, out JavaScriptValueSafeHandle result);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateFunction(JavaScriptNativeFunction nativeFunction, IntPtr callbackState, out JavaScriptValueSafeHandle @function);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateNamedFunction(JavaScriptValueSafeHandle name, JavaScriptNativeFunction nativeFunction, IntPtr callbackState, out JavaScriptValueSafeHandle @function);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateRangeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateReferenceError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateSyntaxError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateTypeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsCreateURIError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsHasException(out bool hasException);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsGetAndClearException(out JavaScriptValueSafeHandle exception);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetException(JavaScriptValueSafeHandle exception);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsDisableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsEnableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsIsRuntimeExecutionDisabled(JavaScriptRuntimeSafeHandle runtime, out bool isDisabled);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsSetPromiseContinuationCallback(JavaScriptPromiseContinuationCallback promiseContinuationCallback, IntPtr callbackState);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsParseScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsParseScriptWithAttributes(string script, JavaScriptSourceContext sourceContext, string sourceUrl, JsParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsRunScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsExperimentalApiRunModule(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsSerializeScript(string script, byte[] buffer, ref ulong bufferSize);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsParseSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsRunSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsParseSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsRunSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsGetPropertyIdFromName(string name, out JavaScriptPropertyId propertyId);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsGetPropertyNameFromId(JavaScriptPropertyId propertyId, out string name);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsPointerToString(string stringValue, UIntPtr stringLength, out JavaScriptValueSafeHandle value);

				[DllImport(DllName, CharSet = CharSet.Unicode)]
				internal static extern JsErrorCode JsStringToPointer(JavaScriptValueSafeHandle value, out IntPtr stringValue, out UIntPtr stringLength);

				[DllImport(DllName)]
				internal static extern JsErrorCode JsDiagStartDebugging(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagDebugEventCallback debugEventCallback, IntPtr callbackState);

			}
		}
	}
}
