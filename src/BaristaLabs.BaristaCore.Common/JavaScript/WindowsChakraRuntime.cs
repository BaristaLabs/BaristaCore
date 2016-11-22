namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

	using System;
	using System.Runtime.InteropServices;

	public sealed class WindowsChakraRuntime : IJavaScriptRuntime, ICommonWindowsJavaScriptRuntime
	{
			public JavaScriptErrorCode JsInitializeModuleRecord(IntPtr referencingModule, JavaScriptValueSafeHandle normalizedSpecifier, out IntPtr moduleRecord)
			{
				var errCode = LibChakraCore.JsInitializeModuleRecord(referencingModule, normalizedSpecifier, out moduleRecord);
				return errCode;
			}

			public JavaScriptErrorCode JsParseModuleSource(IntPtr requestModule, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags sourceFlag, out JavaScriptValueSafeHandle exceptionValueRef)
			{
				var errCode = LibChakraCore.JsParseModuleSource(requestModule, sourceContext, script, scriptLength, sourceFlag, out exceptionValueRef);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(exceptionValueRef, out valueRefCount);
				exceptionValueRef.NativeFunctionSource = nameof(LibChakraCore.JsParseModuleSource);
				return errCode;
			}

			public JavaScriptErrorCode JsModuleEvaluation(IntPtr requestModule, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsModuleEvaluation(requestModule, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsModuleEvaluation);
				return errCode;
			}

			public JavaScriptErrorCode JsSetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, IntPtr hostInfo)
			{
				var errCode = LibChakraCore.JsSetModuleHostInfo(requestModule, moduleHostInfo, hostInfo);
				return errCode;
			}

			public JavaScriptErrorCode JsGetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, out IntPtr hostInfo)
			{
				var errCode = LibChakraCore.JsGetModuleHostInfo(requestModule, moduleHostInfo, out hostInfo);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateString(string content, UIntPtr length, out JavaScriptValueSafeHandle value)
			{
				var errCode = LibChakraCore.JsCreateString(content, length, out value);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsCreateString);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateStringUtf8(string content, UIntPtr length, out JavaScriptValueSafeHandle value)
			{
				var errCode = LibChakraCore.JsCreateStringUtf8(content, length, out value);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsCreateStringUtf8);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateStringUtf16(string content, UIntPtr length, out JavaScriptValueSafeHandle value)
			{
				var errCode = LibChakraCore.JsCreateStringUtf16(content, length, out value);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsCreateStringUtf16);
				return errCode;
			}

			public JavaScriptErrorCode JsCopyString(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out UIntPtr written)
			{
				var errCode = LibChakraCore.JsCopyString(value, start, length, buffer, out written);
				return errCode;
			}

			public JavaScriptErrorCode JsCopyStringUtf8(JavaScriptValueSafeHandle value, byte[] buffer, UIntPtr bufferSize, out UIntPtr written)
			{
				var errCode = LibChakraCore.JsCopyStringUtf8(value, buffer, bufferSize, out written);
				return errCode;
			}

			public JavaScriptErrorCode JsCopyStringUtf16(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer, out UIntPtr written)
			{
				var errCode = LibChakraCore.JsCopyStringUtf16(value, start, length, buffer, out written);
				return errCode;
			}

			public JavaScriptErrorCode JsParse(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsParse(script, sourceContext, sourceUrl, parseAttributes, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsParse);
				return errCode;
			}

			public JavaScriptErrorCode JsRun(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsRun(script, sourceContext, sourceUrl, parseAttributes, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsRun);
				return errCode;
			}

			public JavaScriptErrorCode JsCreatePropertyIdUtf8(string name, UIntPtr length, out JavaScriptPropertyIdSafeHandle propertyId)
			{
				var errCode = LibChakraCore.JsCreatePropertyIdUtf8(name, length, out propertyId);
				return errCode;
			}

			public JavaScriptErrorCode JsCopyPropertyIdUtf8(JavaScriptPropertyIdSafeHandle propertyId, byte[] buffer, UIntPtr bufferSize, out UIntPtr written)
			{
				var errCode = LibChakraCore.JsCopyPropertyIdUtf8(propertyId, buffer, bufferSize, out written);
				return errCode;
			}

			public JavaScriptErrorCode JsSerialize(JavaScriptValueSafeHandle script, byte[] buffer, ref ulong bufferSize, JavaScriptParseScriptAttributes parseAttributes)
			{
				var errCode = LibChakraCore.JsSerialize(script, buffer, ref bufferSize, parseAttributes);
				return errCode;
			}

			public JavaScriptErrorCode JsParseSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsParseSerialized(buffer, scriptLoadCallback, sourceContext, sourceUrl, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerialized);
				return errCode;
			}

			public JavaScriptErrorCode JsRunSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsRunSerialized(buffer, scriptLoadCallback, sourceContext, sourceUrl, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerialized);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateRuntime(JavaScriptRuntimeAttributes attributes, JavaScriptThreadServiceCallback threadService, out JavaScriptRuntimeSafeHandle runtime)
			{
				var errCode = LibChakraCore.JsCreateRuntime(attributes, threadService, out runtime);
				return errCode;
			}

			public JavaScriptErrorCode JsCollectGarbage(JavaScriptRuntimeSafeHandle handle)
			{
				var errCode = LibChakraCore.JsCollectGarbage(handle);
				return errCode;
			}

			public JavaScriptErrorCode JsDisposeRuntime(IntPtr runtime)
			{
				var errCode = LibChakraCore.JsDisposeRuntime(runtime);
				return errCode;
			}

			public JavaScriptErrorCode JsGetRuntimeMemoryUsage(JavaScriptRuntimeSafeHandle runtime, out ulong memoryUsage)
			{
				var errCode = LibChakraCore.JsGetRuntimeMemoryUsage(runtime, out memoryUsage);
				return errCode;
			}

			public JavaScriptErrorCode JsGetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, out ulong memoryLimit)
			{
				var errCode = LibChakraCore.JsGetRuntimeMemoryLimit(runtime, out memoryLimit);
				return errCode;
			}

			public JavaScriptErrorCode JsSetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, ulong memoryLimit)
			{
				var errCode = LibChakraCore.JsSetRuntimeMemoryLimit(runtime, memoryLimit);
				return errCode;
			}

			public JavaScriptErrorCode JsSetRuntimeMemoryAllocationCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptMemoryAllocationCallback allocationCallback)
			{
				var errCode = LibChakraCore.JsSetRuntimeMemoryAllocationCallback(runtime, callbackState, allocationCallback);
				return errCode;
			}

			public JavaScriptErrorCode JsSetRuntimeBeforeCollectCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptBeforeCollectCallback beforeCollectCallback)
			{
				var errCode = LibChakraCore.JsSetRuntimeBeforeCollectCallback(runtime, callbackState, beforeCollectCallback);
				return errCode;
			}

			public JavaScriptErrorCode JsAddValueRef(JavaScriptValueSafeHandle @ref, out uint count)
			{
				var errCode = LibChakraCore.JsAddValueRef(@ref, out count);
				return errCode;
			}

			public JavaScriptErrorCode JsAddRef(IntPtr @ref, out uint count)
			{
				var errCode = LibChakraCore.JsAddRef(@ref, out count);
				return errCode;
			}

			public JavaScriptErrorCode JsReleaseContext(JavaScriptContextSafeHandle context, out uint count)
			{
				var errCode = LibChakraCore.JsReleaseContext(context, out count);
				return errCode;
			}

			public JavaScriptErrorCode JsReleasePropertyId(JavaScriptPropertyIdSafeHandle propertyId, out uint count)
			{
				var errCode = LibChakraCore.JsReleasePropertyId(propertyId, out count);
				return errCode;
			}

			public JavaScriptErrorCode JsReleaseValue(JavaScriptValueSafeHandle value, out uint count)
			{
				var errCode = LibChakraCore.JsReleaseValue(value, out count);
				return errCode;
			}

			public JavaScriptErrorCode JsRelease(IntPtr @ref, out uint count)
			{
				var errCode = LibChakraCore.JsRelease(@ref, out count);
				return errCode;
			}

			public JavaScriptErrorCode JsSetObjectBeforeCollectCallback(JavaScriptValueSafeHandle @ref, IntPtr callbackState, JavaScriptObjectBeforeCollectCallback objectBeforeCollectCallback)
			{
				var errCode = LibChakraCore.JsSetObjectBeforeCollectCallback(@ref, callbackState, objectBeforeCollectCallback);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateContext(JavaScriptRuntimeSafeHandle runtime, out JavaScriptContextSafeHandle newContext)
			{
				var errCode = LibChakraCore.JsCreateContext(runtime, out newContext);
				uint contextRefCount;
				LibChakraCore.JsAddContextRef(newContext, out contextRefCount);
				newContext.NativeFunctionSource = nameof(LibChakraCore.JsCreateContext);
				return errCode;
			}

			public JavaScriptErrorCode JsGetCurrentContext(out JavaScriptContextSafeHandle currentContext)
			{
				var errCode = LibChakraCore.JsGetCurrentContext(out currentContext);
				uint contextRefCount;
				LibChakraCore.JsAddContextRef(currentContext, out contextRefCount);
				currentContext.NativeFunctionSource = nameof(LibChakraCore.JsGetCurrentContext);
				return errCode;
			}

			public JavaScriptErrorCode JsSetCurrentContext(JavaScriptContextSafeHandle context)
			{
				var errCode = LibChakraCore.JsSetCurrentContext(context);
				return errCode;
			}

			public JavaScriptErrorCode JsGetContextOfObject(JavaScriptValueSafeHandle @object, out JavaScriptContextSafeHandle context)
			{
				var errCode = LibChakraCore.JsGetContextOfObject(@object, out context);
				uint contextRefCount;
				LibChakraCore.JsAddContextRef(context, out contextRefCount);
				context.NativeFunctionSource = nameof(LibChakraCore.JsGetContextOfObject);
				return errCode;
			}

			public JavaScriptErrorCode JsGetContextData(JavaScriptContextSafeHandle context, out IntPtr data)
			{
				var errCode = LibChakraCore.JsGetContextData(context, out data);
				return errCode;
			}

			public JavaScriptErrorCode JsSetContextData(JavaScriptContextSafeHandle context, IntPtr data)
			{
				var errCode = LibChakraCore.JsSetContextData(context, data);
				return errCode;
			}

			public JavaScriptErrorCode JsGetRuntime(JavaScriptContextSafeHandle context, out JavaScriptRuntimeSafeHandle runtime)
			{
				var errCode = LibChakraCore.JsGetRuntime(context, out runtime);
				return errCode;
			}

			public JavaScriptErrorCode JsIdle(out uint nextIdleTick)
			{
				var errCode = LibChakraCore.JsIdle(out nextIdleTick);
				return errCode;
			}

			public JavaScriptErrorCode JsGetSymbolFromPropertyId(JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptValueSafeHandle symbol)
			{
				var errCode = LibChakraCore.JsGetSymbolFromPropertyId(propertyId, out symbol);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(symbol, out valueRefCount);
				symbol.NativeFunctionSource = nameof(LibChakraCore.JsGetSymbolFromPropertyId);
				return errCode;
			}

			public JavaScriptErrorCode JsGetPropertyIdType(JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptPropertyIdType propertyIdType)
			{
				var errCode = LibChakraCore.JsGetPropertyIdType(propertyId, out propertyIdType);
				return errCode;
			}

			public JavaScriptErrorCode JsGetPropertyIdFromSymbol(JavaScriptValueSafeHandle symbol, out JavaScriptPropertyIdSafeHandle propertyId)
			{
				var errCode = LibChakraCore.JsGetPropertyIdFromSymbol(symbol, out propertyId);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateSymbol(JavaScriptValueSafeHandle description, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsCreateSymbol(description, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCreateSymbol);
				return errCode;
			}

			public JavaScriptErrorCode JsGetOwnPropertySymbols(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle propertySymbols)
			{
				var errCode = LibChakraCore.JsGetOwnPropertySymbols(@object, out propertySymbols);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(propertySymbols, out valueRefCount);
				propertySymbols.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertySymbols);
				return errCode;
			}

			public JavaScriptErrorCode JsGetUndefinedValue(out JavaScriptValueSafeHandle undefinedValue)
			{
				var errCode = LibChakraCore.JsGetUndefinedValue(out undefinedValue);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(undefinedValue, out valueRefCount);
				undefinedValue.NativeFunctionSource = nameof(LibChakraCore.JsGetUndefinedValue);
				return errCode;
			}

			public JavaScriptErrorCode JsGetNullValue(out JavaScriptValueSafeHandle nullValue)
			{
				var errCode = LibChakraCore.JsGetNullValue(out nullValue);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(nullValue, out valueRefCount);
				nullValue.NativeFunctionSource = nameof(LibChakraCore.JsGetNullValue);
				return errCode;
			}

			public JavaScriptErrorCode JsGetTrueValue(out JavaScriptValueSafeHandle trueValue)
			{
				var errCode = LibChakraCore.JsGetTrueValue(out trueValue);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(trueValue, out valueRefCount);
				trueValue.NativeFunctionSource = nameof(LibChakraCore.JsGetTrueValue);
				return errCode;
			}

			public JavaScriptErrorCode JsGetFalseValue(out JavaScriptValueSafeHandle falseValue)
			{
				var errCode = LibChakraCore.JsGetFalseValue(out falseValue);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(falseValue, out valueRefCount);
				falseValue.NativeFunctionSource = nameof(LibChakraCore.JsGetFalseValue);
				return errCode;
			}

			public JavaScriptErrorCode JsBoolToBoolean(bool value, out JavaScriptValueSafeHandle booleanValue)
			{
				var errCode = LibChakraCore.JsBoolToBoolean(value, out booleanValue);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(booleanValue, out valueRefCount);
				booleanValue.NativeFunctionSource = nameof(LibChakraCore.JsBoolToBoolean);
				return errCode;
			}

			public JavaScriptErrorCode JsBooleanToBool(JavaScriptValueSafeHandle value, out bool boolValue)
			{
				var errCode = LibChakraCore.JsBooleanToBool(value, out boolValue);
				return errCode;
			}

			public JavaScriptErrorCode JsConvertValueToBoolean(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle booleanValue)
			{
				var errCode = LibChakraCore.JsConvertValueToBoolean(value, out booleanValue);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(booleanValue, out valueRefCount);
				booleanValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToBoolean);
				return errCode;
			}

			public JavaScriptErrorCode JsGetValueType(JavaScriptValueSafeHandle value, out JavaScriptValueType type)
			{
				var errCode = LibChakraCore.JsGetValueType(value, out type);
				return errCode;
			}

			public JavaScriptErrorCode JsDoubleToNumber(double doubleValue, out JavaScriptValueSafeHandle value)
			{
				var errCode = LibChakraCore.JsDoubleToNumber(doubleValue, out value);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsDoubleToNumber);
				return errCode;
			}

			public JavaScriptErrorCode JsIntToNumber(int intValue, out JavaScriptValueSafeHandle value)
			{
				var errCode = LibChakraCore.JsIntToNumber(intValue, out value);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsIntToNumber);
				return errCode;
			}

			public JavaScriptErrorCode JsNumberToDouble(JavaScriptValueSafeHandle value, out double doubleValue)
			{
				var errCode = LibChakraCore.JsNumberToDouble(value, out doubleValue);
				return errCode;
			}

			public JavaScriptErrorCode JsNumberToInt(JavaScriptValueSafeHandle value, out int intValue)
			{
				var errCode = LibChakraCore.JsNumberToInt(value, out intValue);
				return errCode;
			}

			public JavaScriptErrorCode JsConvertValueToNumber(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle numberValue)
			{
				var errCode = LibChakraCore.JsConvertValueToNumber(value, out numberValue);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(numberValue, out valueRefCount);
				numberValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToNumber);
				return errCode;
			}

			public JavaScriptErrorCode JsGetStringLength(JavaScriptValueSafeHandle stringValue, out int length)
			{
				var errCode = LibChakraCore.JsGetStringLength(stringValue, out length);
				return errCode;
			}

			public JavaScriptErrorCode JsConvertValueToString(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle stringValue)
			{
				var errCode = LibChakraCore.JsConvertValueToString(value, out stringValue);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(stringValue, out valueRefCount);
				stringValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToString);
				return errCode;
			}

			public JavaScriptErrorCode JsGetGlobalObject(out JavaScriptValueSafeHandle globalObject)
			{
				var errCode = LibChakraCore.JsGetGlobalObject(out globalObject);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(globalObject, out valueRefCount);
				globalObject.NativeFunctionSource = nameof(LibChakraCore.JsGetGlobalObject);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateObject(out JavaScriptValueSafeHandle @object)
			{
				var errCode = LibChakraCore.JsCreateObject(out @object);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(@object, out valueRefCount);
				@object.NativeFunctionSource = nameof(LibChakraCore.JsCreateObject);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback, out JavaScriptValueSafeHandle @object)
			{
				var errCode = LibChakraCore.JsCreateExternalObject(data, finalizeCallback, out @object);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(@object, out valueRefCount);
				@object.NativeFunctionSource = nameof(LibChakraCore.JsCreateExternalObject);
				return errCode;
			}

			public JavaScriptErrorCode JsConvertValueToObject(JavaScriptValueSafeHandle value, out JavaScriptValueSafeHandle @object)
			{
				var errCode = LibChakraCore.JsConvertValueToObject(value, out @object);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(@object, out valueRefCount);
				@object.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToObject);
				return errCode;
			}

			public JavaScriptErrorCode JsGetPrototype(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle prototypeObject)
			{
				var errCode = LibChakraCore.JsGetPrototype(@object, out prototypeObject);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(prototypeObject, out valueRefCount);
				prototypeObject.NativeFunctionSource = nameof(LibChakraCore.JsGetPrototype);
				return errCode;
			}

			public JavaScriptErrorCode JsSetPrototype(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle prototypeObject)
			{
				var errCode = LibChakraCore.JsSetPrototype(@object, prototypeObject);
				return errCode;
			}

			public JavaScriptErrorCode JsInstanceOf(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle constructor, out bool result)
			{
				var errCode = LibChakraCore.JsInstanceOf(@object, constructor, out result);
				return errCode;
			}

			public JavaScriptErrorCode JsGetExtensionAllowed(JavaScriptValueSafeHandle @object, out bool value)
			{
				var errCode = LibChakraCore.JsGetExtensionAllowed(@object, out value);
				return errCode;
			}

			public JavaScriptErrorCode JsPreventExtension(JavaScriptValueSafeHandle @object)
			{
				var errCode = LibChakraCore.JsPreventExtension(@object);
				return errCode;
			}

			public JavaScriptErrorCode JsGetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptValueSafeHandle value)
			{
				var errCode = LibChakraCore.JsGetProperty(@object, propertyId, out value);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsGetProperty);
				return errCode;
			}

			public JavaScriptErrorCode JsGetOwnPropertyDescriptor(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, out JavaScriptValueSafeHandle propertyDescriptor)
			{
				var errCode = LibChakraCore.JsGetOwnPropertyDescriptor(@object, propertyId, out propertyDescriptor);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(propertyDescriptor, out valueRefCount);
				propertyDescriptor.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertyDescriptor);
				return errCode;
			}

			public JavaScriptErrorCode JsGetOwnPropertyNames(JavaScriptValueSafeHandle @object, out JavaScriptValueSafeHandle propertyNames)
			{
				var errCode = LibChakraCore.JsGetOwnPropertyNames(@object, out propertyNames);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(propertyNames, out valueRefCount);
				propertyNames.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertyNames);
				return errCode;
			}

			public JavaScriptErrorCode JsSetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle value, bool useStrictRules)
			{
				var errCode = LibChakraCore.JsSetProperty(@object, propertyId, value, useStrictRules);
				return errCode;
			}

			public JavaScriptErrorCode JsHasProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, out bool hasProperty)
			{
				var errCode = LibChakraCore.JsHasProperty(@object, propertyId, out hasProperty);
				return errCode;
			}

			public JavaScriptErrorCode JsDeleteProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, bool useStrictRules, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsDeleteProperty(@object, propertyId, useStrictRules, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsDeleteProperty);
				return errCode;
			}

			public JavaScriptErrorCode JsDefineProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle propertyDescriptor, out bool result)
			{
				var errCode = LibChakraCore.JsDefineProperty(@object, propertyId, propertyDescriptor, out result);
				return errCode;
			}

			public JavaScriptErrorCode JsHasIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, out bool result)
			{
				var errCode = LibChakraCore.JsHasIndexedProperty(@object, index, out result);
				return errCode;
			}

			public JavaScriptErrorCode JsGetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsGetIndexedProperty(@object, index, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsGetIndexedProperty);
				return errCode;
			}

			public JavaScriptErrorCode JsSetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, JavaScriptValueSafeHandle value)
			{
				var errCode = LibChakraCore.JsSetIndexedProperty(@object, index, value);
				return errCode;
			}

			public JavaScriptErrorCode JsDeleteIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index)
			{
				var errCode = LibChakraCore.JsDeleteIndexedProperty(@object, index);
				return errCode;
			}

			public JavaScriptErrorCode JsHasIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, out bool value)
			{
				var errCode = LibChakraCore.JsHasIndexedPropertiesExternalData(@object, out value);
				return errCode;
			}

			public JavaScriptErrorCode JsGetIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, IntPtr data, out JavaScriptTypedArrayType arrayType, out uint elementLength)
			{
				var errCode = LibChakraCore.JsGetIndexedPropertiesExternalData(@object, data, out arrayType, out elementLength);
				return errCode;
			}

			public JavaScriptErrorCode JsSetIndexedPropertiesToExternalData(JavaScriptValueSafeHandle @object, IntPtr data, JavaScriptTypedArrayType arrayType, uint elementLength)
			{
				var errCode = LibChakraCore.JsSetIndexedPropertiesToExternalData(@object, data, arrayType, elementLength);
				return errCode;
			}

			public JavaScriptErrorCode JsEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result)
			{
				var errCode = LibChakraCore.JsEquals(object1, object2, out result);
				return errCode;
			}

			public JavaScriptErrorCode JsStrictEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2, out bool result)
			{
				var errCode = LibChakraCore.JsStrictEquals(object1, object2, out result);
				return errCode;
			}

			public JavaScriptErrorCode JsHasExternalData(JavaScriptValueSafeHandle @object, out bool value)
			{
				var errCode = LibChakraCore.JsHasExternalData(@object, out value);
				return errCode;
			}

			public JavaScriptErrorCode JsGetExternalData(JavaScriptValueSafeHandle @object, out IntPtr externalData)
			{
				var errCode = LibChakraCore.JsGetExternalData(@object, out externalData);
				return errCode;
			}

			public JavaScriptErrorCode JsSetExternalData(JavaScriptValueSafeHandle @object, IntPtr externalData)
			{
				var errCode = LibChakraCore.JsSetExternalData(@object, externalData);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateArray(uint length, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsCreateArray(length, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCreateArray);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateArrayBuffer(uint byteLength, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsCreateArrayBuffer(byteLength, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCreateArrayBuffer);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateExternalArrayBuffer(IntPtr data, uint byteLength, JavaScriptObjectFinalizeCallback finalizeCallback, IntPtr callbackState, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsCreateExternalArrayBuffer(data, byteLength, finalizeCallback, callbackState, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCreateExternalArrayBuffer);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateTypedArray(JavaScriptTypedArrayType arrayType, JavaScriptValueSafeHandle baseArray, uint byteOffset, uint elementLength, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsCreateTypedArray(arrayType, baseArray, byteOffset, elementLength, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCreateTypedArray);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateDataView(JavaScriptValueSafeHandle arrayBuffer, uint byteOffset, uint byteLength, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsCreateDataView(arrayBuffer, byteOffset, byteLength, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCreateDataView);
				return errCode;
			}

			public JavaScriptErrorCode JsGetTypedArrayInfo(JavaScriptValueSafeHandle typedArray, out JavaScriptTypedArrayType arrayType, out JavaScriptValueSafeHandle arrayBuffer, out uint byteOffset, out uint byteLength)
			{
				var errCode = LibChakraCore.JsGetTypedArrayInfo(typedArray, out arrayType, out arrayBuffer, out byteOffset, out byteLength);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(arrayBuffer, out valueRefCount);
				arrayBuffer.NativeFunctionSource = nameof(LibChakraCore.JsGetTypedArrayInfo);
				return errCode;
			}

			public JavaScriptErrorCode JsGetArrayBufferStorage(JavaScriptValueSafeHandle arrayBuffer, out IntPtr buffer, out uint bufferLength)
			{
				var errCode = LibChakraCore.JsGetArrayBufferStorage(arrayBuffer, out buffer, out bufferLength);
				return errCode;
			}

			public JavaScriptErrorCode JsGetTypedArrayStorage(JavaScriptValueSafeHandle typedArray, out IntPtr buffer, out uint bufferLength, out JavaScriptTypedArrayType arrayType, out int elementSize)
			{
				var errCode = LibChakraCore.JsGetTypedArrayStorage(typedArray, out buffer, out bufferLength, out arrayType, out elementSize);
				return errCode;
			}

			public JavaScriptErrorCode JsGetDataViewStorage(JavaScriptValueSafeHandle dataView, out IntPtr buffer, out uint bufferLength)
			{
				var errCode = LibChakraCore.JsGetDataViewStorage(dataView, out buffer, out bufferLength);
				return errCode;
			}

			public JavaScriptErrorCode JsCallFunction(JavaScriptValueSafeHandle function, IntPtr[] arguments, ushort argumentCount, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsCallFunction(function, arguments, argumentCount, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCallFunction);
				return errCode;
			}

			public JavaScriptErrorCode JsConstructObject(JavaScriptValueSafeHandle function, IntPtr[] arguments, ushort argumentCount, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsConstructObject(function, arguments, argumentCount, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsConstructObject);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateFunction(JavaScriptNativeFunction nativeFunction, IntPtr callbackState, out JavaScriptValueSafeHandle function)
			{
				var errCode = LibChakraCore.JsCreateFunction(nativeFunction, callbackState, out function);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(function, out valueRefCount);
				function.NativeFunctionSource = nameof(LibChakraCore.JsCreateFunction);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateNamedFunction(JavaScriptValueSafeHandle name, JavaScriptNativeFunction nativeFunction, IntPtr callbackState, out JavaScriptValueSafeHandle function)
			{
				var errCode = LibChakraCore.JsCreateNamedFunction(name, nativeFunction, callbackState, out function);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(function, out valueRefCount);
				function.NativeFunctionSource = nameof(LibChakraCore.JsCreateNamedFunction);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				var errCode = LibChakraCore.JsCreateError(message, out error);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(error, out valueRefCount);
				error.NativeFunctionSource = nameof(LibChakraCore.JsCreateError);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateRangeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				var errCode = LibChakraCore.JsCreateRangeError(message, out error);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(error, out valueRefCount);
				error.NativeFunctionSource = nameof(LibChakraCore.JsCreateRangeError);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateReferenceError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				var errCode = LibChakraCore.JsCreateReferenceError(message, out error);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(error, out valueRefCount);
				error.NativeFunctionSource = nameof(LibChakraCore.JsCreateReferenceError);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateSyntaxError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				var errCode = LibChakraCore.JsCreateSyntaxError(message, out error);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(error, out valueRefCount);
				error.NativeFunctionSource = nameof(LibChakraCore.JsCreateSyntaxError);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateTypeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				var errCode = LibChakraCore.JsCreateTypeError(message, out error);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(error, out valueRefCount);
				error.NativeFunctionSource = nameof(LibChakraCore.JsCreateTypeError);
				return errCode;
			}

			public JavaScriptErrorCode JsCreateURIError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle error)
			{
				var errCode = LibChakraCore.JsCreateURIError(message, out error);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(error, out valueRefCount);
				error.NativeFunctionSource = nameof(LibChakraCore.JsCreateURIError);
				return errCode;
			}

			public JavaScriptErrorCode JsHasException(out bool hasException)
			{
				var errCode = LibChakraCore.JsHasException(out hasException);
				return errCode;
			}

			public JavaScriptErrorCode JsGetAndClearException(out JavaScriptValueSafeHandle exception)
			{
				var errCode = LibChakraCore.JsGetAndClearException(out exception);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(exception, out valueRefCount);
				exception.NativeFunctionSource = nameof(LibChakraCore.JsGetAndClearException);
				return errCode;
			}

			public JavaScriptErrorCode JsSetException(JavaScriptValueSafeHandle exception)
			{
				var errCode = LibChakraCore.JsSetException(exception);
				return errCode;
			}

			public JavaScriptErrorCode JsDisableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime)
			{
				var errCode = LibChakraCore.JsDisableRuntimeExecution(runtime);
				return errCode;
			}

			public JavaScriptErrorCode JsEnableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime)
			{
				var errCode = LibChakraCore.JsEnableRuntimeExecution(runtime);
				return errCode;
			}

			public JavaScriptErrorCode JsIsRuntimeExecutionDisabled(JavaScriptRuntimeSafeHandle runtime, out bool isDisabled)
			{
				var errCode = LibChakraCore.JsIsRuntimeExecutionDisabled(runtime, out isDisabled);
				return errCode;
			}

			public JavaScriptErrorCode JsSetPromiseContinuationCallback(JavaScriptPromiseContinuationCallback promiseContinuationCallback, IntPtr callbackState)
			{
				var errCode = LibChakraCore.JsSetPromiseContinuationCallback(promiseContinuationCallback, callbackState);
				return errCode;
			}

			public JavaScriptErrorCode JsParseScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsParseScript(script, sourceContext, sourceUrl, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsParseScript);
				return errCode;
			}

			public JavaScriptErrorCode JsParseScriptWithAttributes(string script, JavaScriptSourceContext sourceContext, string sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsParseScriptWithAttributes(script, sourceContext, sourceUrl, parseAttributes, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsParseScriptWithAttributes);
				return errCode;
			}

			public JavaScriptErrorCode JsRunScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsRunScript(script, sourceContext, sourceUrl, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsRunScript);
				return errCode;
			}

			public JavaScriptErrorCode JsExperimentalApiRunModule(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsExperimentalApiRunModule(script, sourceContext, sourceUrl, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsExperimentalApiRunModule);
				return errCode;
			}

			public JavaScriptErrorCode JsSerializeScript(string script, byte[] buffer, ref ulong bufferSize)
			{
				var errCode = LibChakraCore.JsSerializeScript(script, buffer, ref bufferSize);
				return errCode;
			}

			public JavaScriptErrorCode JsParseSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsParseSerializedScriptWithCallback(scriptLoadCallback, scriptUnloadCallback, buffer, sourceContext, sourceUrl, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerializedScriptWithCallback);
				return errCode;
			}

			public JavaScriptErrorCode JsRunSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsRunSerializedScriptWithCallback(scriptLoadCallback, scriptUnloadCallback, buffer, sourceContext, sourceUrl, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerializedScriptWithCallback);
				return errCode;
			}

			public JavaScriptErrorCode JsParseSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsParseSerializedScript(script, buffer, sourceContext, sourceUrl, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerializedScript);
				return errCode;
			}

			public JavaScriptErrorCode JsRunSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValueSafeHandle result)
			{
				var errCode = LibChakraCore.JsRunSerializedScript(script, buffer, sourceContext, sourceUrl, out result);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerializedScript);
				return errCode;
			}

			public JavaScriptErrorCode JsGetPropertyIdFromName(string name, out JavaScriptPropertyIdSafeHandle propertyId)
			{
				var errCode = LibChakraCore.JsGetPropertyIdFromName(name, out propertyId);
				return errCode;
			}

			public JavaScriptErrorCode JsGetPropertyNameFromId(JavaScriptPropertyIdSafeHandle propertyId, out string name)
			{
				var errCode = LibChakraCore.JsGetPropertyNameFromId(propertyId, out name);
				return errCode;
			}

			public JavaScriptErrorCode JsPointerToString(string stringValue, ulong stringLength, out JavaScriptValueSafeHandle value)
			{
				var errCode = LibChakraCore.JsPointerToString(stringValue, stringLength, out value);
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsPointerToString);
				return errCode;
			}

			public JavaScriptErrorCode JsStringToPointer(JavaScriptValueSafeHandle value, out IntPtr stringValue, out ulong stringLength)
			{
				var errCode = LibChakraCore.JsStringToPointer(value, out stringValue, out stringLength);
				return errCode;
			}

			public JavaScriptErrorCode JsDiagStartDebugging(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagDebugEventCallback debugEventCallback, IntPtr callbackState)
			{
				var errCode = LibChakraCore.JsDiagStartDebugging(runtimeHandle, debugEventCallback, callbackState);
				return errCode;
			}

	}
}
