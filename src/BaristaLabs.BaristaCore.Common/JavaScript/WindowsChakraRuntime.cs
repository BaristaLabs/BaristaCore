﻿namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

	using System;
	using System.Runtime.InteropServices;

	public sealed class WindowsChakraRuntime : IJavaScriptRuntime, ICommonWindowsJavaScriptRuntime
	{
			public IntPtr JsInitializeModuleRecord(IntPtr referencingModule, JavaScriptValueSafeHandle normalizedSpecifier)
			{
				IntPtr moduleRecord;
				Errors.ThrowIfError(LibChakraCore.JsInitializeModuleRecord(referencingModule, normalizedSpecifier, out moduleRecord));
				return moduleRecord;
			}

			public JavaScriptValueSafeHandle JsParseModuleSource(IntPtr requestModule, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags sourceFlag)
			{
				JavaScriptValueSafeHandle exceptionValueRef;
				Errors.ThrowIfError(LibChakraCore.JsParseModuleSource(requestModule, sourceContext, script, scriptLength, sourceFlag, out exceptionValueRef));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(exceptionValueRef, out valueRefCount);
				exceptionValueRef.NativeFunctionSource = nameof(LibChakraCore.JsParseModuleSource);
				return exceptionValueRef;
			}

			public JavaScriptValueSafeHandle JsModuleEvaluation(IntPtr requestModule)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsModuleEvaluation(requestModule, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsModuleEvaluation);
				return result;
			}

			public void JsSetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, IntPtr hostInfo)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetModuleHostInfo(requestModule, moduleHostInfo, hostInfo));
			}

			public IntPtr JsGetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo)
			{
				IntPtr hostInfo;
				Errors.ThrowIfError(LibChakraCore.JsGetModuleHostInfo(requestModule, moduleHostInfo, out hostInfo));
				return hostInfo;
			}

			public JavaScriptValueSafeHandle JsCreateString(string content, UIntPtr length)
			{
				JavaScriptValueSafeHandle value;
				Errors.ThrowIfError(LibChakraCore.JsCreateString(content, length, out value));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsCreateString);
				return value;
			}

			public JavaScriptValueSafeHandle JsCreateStringUtf8(string content, UIntPtr length)
			{
				JavaScriptValueSafeHandle value;
				Errors.ThrowIfError(LibChakraCore.JsCreateStringUtf8(content, length, out value));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsCreateStringUtf8);
				return value;
			}

			public JavaScriptValueSafeHandle JsCreateStringUtf16(string content, UIntPtr length)
			{
				JavaScriptValueSafeHandle value;
				Errors.ThrowIfError(LibChakraCore.JsCreateStringUtf16(content, length, out value));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsCreateStringUtf16);
				return value;
			}

			public UIntPtr JsCopyString(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer)
			{
				UIntPtr written;
				Errors.ThrowIfError(LibChakraCore.JsCopyString(value, start, length, buffer, out written));
				return written;
			}

			public UIntPtr JsCopyStringUtf8(JavaScriptValueSafeHandle value, byte[] buffer, UIntPtr bufferSize)
			{
				UIntPtr written;
				Errors.ThrowIfError(LibChakraCore.JsCopyStringUtf8(value, buffer, bufferSize, out written));
				return written;
			}

			public UIntPtr JsCopyStringUtf16(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer)
			{
				UIntPtr written;
				Errors.ThrowIfError(LibChakraCore.JsCopyStringUtf16(value, start, length, buffer, out written));
				return written;
			}

			public JavaScriptValueSafeHandle JsParse(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsParse(script, sourceContext, sourceUrl, parseAttributes, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsParse);
				return result;
			}

			public JavaScriptValueSafeHandle JsRun(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsRun(script, sourceContext, sourceUrl, parseAttributes, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsRun);
				return result;
			}

			public JavaScriptPropertyIdSafeHandle JsCreatePropertyIdUtf8(string name, UIntPtr length)
			{
				JavaScriptPropertyIdSafeHandle propertyId;
				Errors.ThrowIfError(LibChakraCore.JsCreatePropertyIdUtf8(name, length, out propertyId));
				return propertyId;
			}

			public UIntPtr JsCopyPropertyIdUtf8(JavaScriptPropertyIdSafeHandle propertyId, byte[] buffer, UIntPtr bufferSize)
			{
				UIntPtr written;
				Errors.ThrowIfError(LibChakraCore.JsCopyPropertyIdUtf8(propertyId, buffer, bufferSize, out written));
				return written;
			}

			public void JsSerialize(JavaScriptValueSafeHandle script, byte[] buffer, ref ulong bufferSize, JavaScriptParseScriptAttributes parseAttributes)
			{
				Errors.ThrowIfError(LibChakraCore.JsSerialize(script, buffer, ref bufferSize, parseAttributes));
			}

			public JavaScriptValueSafeHandle JsParseSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsParseSerialized(buffer, scriptLoadCallback, sourceContext, sourceUrl, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerialized);
				return result;
			}

			public JavaScriptValueSafeHandle JsRunSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsRunSerialized(buffer, scriptLoadCallback, sourceContext, sourceUrl, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerialized);
				return result;
			}

			public JavaScriptRuntimeSafeHandle JsCreateRuntime(JavaScriptRuntimeAttributes attributes, JavaScriptThreadServiceCallback threadService)
			{
				JavaScriptRuntimeSafeHandle runtime;
				Errors.ThrowIfError(LibChakraCore.JsCreateRuntime(attributes, threadService, out runtime));
				return runtime;
			}

			public void JsCollectGarbage(JavaScriptRuntimeSafeHandle handle)
			{
				Errors.ThrowIfError(LibChakraCore.JsCollectGarbage(handle));
			}

			public void JsDisposeRuntime(IntPtr runtime)
			{
				Errors.ThrowIfError(LibChakraCore.JsDisposeRuntime(runtime));
			}

			public ulong JsGetRuntimeMemoryUsage(JavaScriptRuntimeSafeHandle runtime)
			{
				ulong memoryUsage;
				Errors.ThrowIfError(LibChakraCore.JsGetRuntimeMemoryUsage(runtime, out memoryUsage));
				return memoryUsage;
			}

			public ulong JsGetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime)
			{
				ulong memoryLimit;
				Errors.ThrowIfError(LibChakraCore.JsGetRuntimeMemoryLimit(runtime, out memoryLimit));
				return memoryLimit;
			}

			public void JsSetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, ulong memoryLimit)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetRuntimeMemoryLimit(runtime, memoryLimit));
			}

			public void JsSetRuntimeMemoryAllocationCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptMemoryAllocationCallback allocationCallback)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetRuntimeMemoryAllocationCallback(runtime, callbackState, allocationCallback));
			}

			public void JsSetRuntimeBeforeCollectCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptBeforeCollectCallback beforeCollectCallback)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetRuntimeBeforeCollectCallback(runtime, callbackState, beforeCollectCallback));
			}

			public uint JsAddContextRef(JavaScriptContextSafeHandle @ref)
			{
				uint count;
				Errors.ThrowIfError(LibChakraCore.JsAddContextRef(@ref, out count));
				return count;
			}

			public uint JsAddValueRef(JavaScriptValueSafeHandle @ref)
			{
				uint count;
				Errors.ThrowIfError(LibChakraCore.JsAddValueRef(@ref, out count));
				return count;
			}

			public uint JsAddRef(IntPtr @ref)
			{
				uint count;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(@ref, out count));
				return count;
			}

			public uint JsReleaseContext(JavaScriptContextSafeHandle context)
			{
				uint count;
				Errors.ThrowIfError(LibChakraCore.JsReleaseContext(context, out count));
				return count;
			}

			public uint JsReleasePropertyId(JavaScriptPropertyIdSafeHandle propertyId)
			{
				uint count;
				Errors.ThrowIfError(LibChakraCore.JsReleasePropertyId(propertyId, out count));
				return count;
			}

			public uint JsReleaseValue(JavaScriptValueSafeHandle value)
			{
				uint count;
				Errors.ThrowIfError(LibChakraCore.JsReleaseValue(value, out count));
				return count;
			}

			public uint JsRelease(IntPtr @ref)
			{
				uint count;
				Errors.ThrowIfError(LibChakraCore.JsRelease(@ref, out count));
				return count;
			}

			public void JsSetObjectBeforeCollectCallback(JavaScriptValueSafeHandle @ref, IntPtr callbackState, JavaScriptObjectBeforeCollectCallback objectBeforeCollectCallback)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetObjectBeforeCollectCallback(@ref, callbackState, objectBeforeCollectCallback));
			}

			public JavaScriptContextSafeHandle JsCreateContext(JavaScriptRuntimeSafeHandle runtime)
			{
				JavaScriptContextSafeHandle newContext;
				Errors.ThrowIfError(LibChakraCore.JsCreateContext(runtime, out newContext));
				uint contextRefCount;
				LibChakraCore.JsAddContextRef(newContext, out contextRefCount);
				newContext.NativeFunctionSource = nameof(LibChakraCore.JsCreateContext);
				return newContext;
			}

			public JavaScriptContextSafeHandle JsGetCurrentContext()
			{
				JavaScriptContextSafeHandle currentContext;
				Errors.ThrowIfError(LibChakraCore.JsGetCurrentContext(out currentContext));
				uint contextRefCount;
				LibChakraCore.JsAddContextRef(currentContext, out contextRefCount);
				currentContext.NativeFunctionSource = nameof(LibChakraCore.JsGetCurrentContext);
				return currentContext;
			}

			public void JsSetCurrentContext(JavaScriptContextSafeHandle context)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetCurrentContext(context));
			}

			public JavaScriptContextSafeHandle JsGetContextOfObject(JavaScriptValueSafeHandle @object)
			{
				JavaScriptContextSafeHandle context;
				Errors.ThrowIfError(LibChakraCore.JsGetContextOfObject(@object, out context));
				uint contextRefCount;
				LibChakraCore.JsAddContextRef(context, out contextRefCount);
				context.NativeFunctionSource = nameof(LibChakraCore.JsGetContextOfObject);
				return context;
			}

			public IntPtr JsGetContextData(JavaScriptContextSafeHandle context)
			{
				IntPtr data;
				Errors.ThrowIfError(LibChakraCore.JsGetContextData(context, out data));
				return data;
			}

			public void JsSetContextData(JavaScriptContextSafeHandle context, IntPtr data)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetContextData(context, data));
			}

			public JavaScriptRuntimeSafeHandle JsGetRuntime(JavaScriptContextSafeHandle context)
			{
				JavaScriptRuntimeSafeHandle runtime;
				Errors.ThrowIfError(LibChakraCore.JsGetRuntime(context, out runtime));
				return runtime;
			}

			public uint JsIdle()
			{
				uint nextIdleTick;
				Errors.ThrowIfError(LibChakraCore.JsIdle(out nextIdleTick));
				return nextIdleTick;
			}

			public JavaScriptValueSafeHandle JsGetSymbolFromPropertyId(JavaScriptPropertyIdSafeHandle propertyId)
			{
				JavaScriptValueSafeHandle symbol;
				Errors.ThrowIfError(LibChakraCore.JsGetSymbolFromPropertyId(propertyId, out symbol));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(symbol, out valueRefCount);
				symbol.NativeFunctionSource = nameof(LibChakraCore.JsGetSymbolFromPropertyId);
				return symbol;
			}

			public JavaScriptPropertyIdType JsGetPropertyIdType(JavaScriptPropertyIdSafeHandle propertyId)
			{
				JavaScriptPropertyIdType propertyIdType;
				Errors.ThrowIfError(LibChakraCore.JsGetPropertyIdType(propertyId, out propertyIdType));
				return propertyIdType;
			}

			public JavaScriptPropertyIdSafeHandle JsGetPropertyIdFromSymbol(JavaScriptValueSafeHandle symbol)
			{
				JavaScriptPropertyIdSafeHandle propertyId;
				Errors.ThrowIfError(LibChakraCore.JsGetPropertyIdFromSymbol(symbol, out propertyId));
				return propertyId;
			}

			public JavaScriptValueSafeHandle JsCreateSymbol(JavaScriptValueSafeHandle description)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsCreateSymbol(description, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCreateSymbol);
				return result;
			}

			public JavaScriptValueSafeHandle JsGetOwnPropertySymbols(JavaScriptValueSafeHandle @object)
			{
				JavaScriptValueSafeHandle propertySymbols;
				Errors.ThrowIfError(LibChakraCore.JsGetOwnPropertySymbols(@object, out propertySymbols));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(propertySymbols, out valueRefCount);
				propertySymbols.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertySymbols);
				return propertySymbols;
			}

			public JavaScriptValueSafeHandle JsGetUndefinedValue()
			{
				JavaScriptValueSafeHandle undefinedValue;
				Errors.ThrowIfError(LibChakraCore.JsGetUndefinedValue(out undefinedValue));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(undefinedValue, out valueRefCount);
				undefinedValue.NativeFunctionSource = nameof(LibChakraCore.JsGetUndefinedValue);
				return undefinedValue;
			}

			public JavaScriptValueSafeHandle JsGetNullValue()
			{
				JavaScriptValueSafeHandle nullValue;
				Errors.ThrowIfError(LibChakraCore.JsGetNullValue(out nullValue));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(nullValue, out valueRefCount);
				nullValue.NativeFunctionSource = nameof(LibChakraCore.JsGetNullValue);
				return nullValue;
			}

			public JavaScriptValueSafeHandle JsGetTrueValue()
			{
				JavaScriptValueSafeHandle trueValue;
				Errors.ThrowIfError(LibChakraCore.JsGetTrueValue(out trueValue));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(trueValue, out valueRefCount);
				trueValue.NativeFunctionSource = nameof(LibChakraCore.JsGetTrueValue);
				return trueValue;
			}

			public JavaScriptValueSafeHandle JsGetFalseValue()
			{
				JavaScriptValueSafeHandle falseValue;
				Errors.ThrowIfError(LibChakraCore.JsGetFalseValue(out falseValue));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(falseValue, out valueRefCount);
				falseValue.NativeFunctionSource = nameof(LibChakraCore.JsGetFalseValue);
				return falseValue;
			}

			public JavaScriptValueSafeHandle JsBoolToBoolean(bool value)
			{
				JavaScriptValueSafeHandle booleanValue;
				Errors.ThrowIfError(LibChakraCore.JsBoolToBoolean(value, out booleanValue));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(booleanValue, out valueRefCount);
				booleanValue.NativeFunctionSource = nameof(LibChakraCore.JsBoolToBoolean);
				return booleanValue;
			}

			public bool JsBooleanToBool(JavaScriptValueSafeHandle value)
			{
				bool boolValue;
				Errors.ThrowIfError(LibChakraCore.JsBooleanToBool(value, out boolValue));
				return boolValue;
			}

			public JavaScriptValueSafeHandle JsConvertValueToBoolean(JavaScriptValueSafeHandle value)
			{
				JavaScriptValueSafeHandle booleanValue;
				Errors.ThrowIfError(LibChakraCore.JsConvertValueToBoolean(value, out booleanValue));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(booleanValue, out valueRefCount);
				booleanValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToBoolean);
				return booleanValue;
			}

			public JavaScriptValueType JsGetValueType(JavaScriptValueSafeHandle value)
			{
				JavaScriptValueType type;
				Errors.ThrowIfError(LibChakraCore.JsGetValueType(value, out type));
				return type;
			}

			public JavaScriptValueSafeHandle JsDoubleToNumber(double doubleValue)
			{
				JavaScriptValueSafeHandle value;
				Errors.ThrowIfError(LibChakraCore.JsDoubleToNumber(doubleValue, out value));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsDoubleToNumber);
				return value;
			}

			public JavaScriptValueSafeHandle JsIntToNumber(int intValue)
			{
				JavaScriptValueSafeHandle value;
				Errors.ThrowIfError(LibChakraCore.JsIntToNumber(intValue, out value));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsIntToNumber);
				return value;
			}

			public double JsNumberToDouble(JavaScriptValueSafeHandle value)
			{
				double doubleValue;
				Errors.ThrowIfError(LibChakraCore.JsNumberToDouble(value, out doubleValue));
				return doubleValue;
			}

			public int JsNumberToInt(JavaScriptValueSafeHandle value)
			{
				int intValue;
				Errors.ThrowIfError(LibChakraCore.JsNumberToInt(value, out intValue));
				return intValue;
			}

			public JavaScriptValueSafeHandle JsConvertValueToNumber(JavaScriptValueSafeHandle value)
			{
				JavaScriptValueSafeHandle numberValue;
				Errors.ThrowIfError(LibChakraCore.JsConvertValueToNumber(value, out numberValue));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(numberValue, out valueRefCount);
				numberValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToNumber);
				return numberValue;
			}

			public int JsGetStringLength(JavaScriptValueSafeHandle stringValue)
			{
				int length;
				Errors.ThrowIfError(LibChakraCore.JsGetStringLength(stringValue, out length));
				return length;
			}

			public JavaScriptValueSafeHandle JsConvertValueToString(JavaScriptValueSafeHandle value)
			{
				JavaScriptValueSafeHandle stringValue;
				Errors.ThrowIfError(LibChakraCore.JsConvertValueToString(value, out stringValue));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(stringValue, out valueRefCount);
				stringValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToString);
				return stringValue;
			}

			public JavaScriptValueSafeHandle JsGetGlobalObject()
			{
				JavaScriptValueSafeHandle globalObject;
				Errors.ThrowIfError(LibChakraCore.JsGetGlobalObject(out globalObject));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(globalObject, out valueRefCount);
				globalObject.NativeFunctionSource = nameof(LibChakraCore.JsGetGlobalObject);
				return globalObject;
			}

			public JavaScriptValueSafeHandle JsCreateObject()
			{
				JavaScriptValueSafeHandle @object;
				Errors.ThrowIfError(LibChakraCore.JsCreateObject(out @object));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(@object, out valueRefCount);
				@object.NativeFunctionSource = nameof(LibChakraCore.JsCreateObject);
				return @object;
			}

			public JavaScriptValueSafeHandle JsCreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback)
			{
				JavaScriptValueSafeHandle @object;
				Errors.ThrowIfError(LibChakraCore.JsCreateExternalObject(data, finalizeCallback, out @object));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(@object, out valueRefCount);
				@object.NativeFunctionSource = nameof(LibChakraCore.JsCreateExternalObject);
				return @object;
			}

			public JavaScriptValueSafeHandle JsConvertValueToObject(JavaScriptValueSafeHandle value)
			{
				JavaScriptValueSafeHandle @object;
				Errors.ThrowIfError(LibChakraCore.JsConvertValueToObject(value, out @object));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(@object, out valueRefCount);
				@object.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToObject);
				return @object;
			}

			public JavaScriptValueSafeHandle JsGetPrototype(JavaScriptValueSafeHandle @object)
			{
				JavaScriptValueSafeHandle prototypeObject;
				Errors.ThrowIfError(LibChakraCore.JsGetPrototype(@object, out prototypeObject));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(prototypeObject, out valueRefCount);
				prototypeObject.NativeFunctionSource = nameof(LibChakraCore.JsGetPrototype);
				return prototypeObject;
			}

			public void JsSetPrototype(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle prototypeObject)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetPrototype(@object, prototypeObject));
			}

			public bool JsInstanceOf(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle constructor)
			{
				bool result;
				Errors.ThrowIfError(LibChakraCore.JsInstanceOf(@object, constructor, out result));
				return result;
			}

			public bool JsGetExtensionAllowed(JavaScriptValueSafeHandle @object)
			{
				bool value;
				Errors.ThrowIfError(LibChakraCore.JsGetExtensionAllowed(@object, out value));
				return value;
			}

			public void JsPreventExtension(JavaScriptValueSafeHandle @object)
			{
				Errors.ThrowIfError(LibChakraCore.JsPreventExtension(@object));
			}

			public JavaScriptValueSafeHandle JsGetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId)
			{
				JavaScriptValueSafeHandle value;
				Errors.ThrowIfError(LibChakraCore.JsGetProperty(@object, propertyId, out value));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsGetProperty);
				return value;
			}

			public JavaScriptValueSafeHandle JsGetOwnPropertyDescriptor(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId)
			{
				JavaScriptValueSafeHandle propertyDescriptor;
				Errors.ThrowIfError(LibChakraCore.JsGetOwnPropertyDescriptor(@object, propertyId, out propertyDescriptor));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(propertyDescriptor, out valueRefCount);
				propertyDescriptor.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertyDescriptor);
				return propertyDescriptor;
			}

			public JavaScriptValueSafeHandle JsGetOwnPropertyNames(JavaScriptValueSafeHandle @object)
			{
				JavaScriptValueSafeHandle propertyNames;
				Errors.ThrowIfError(LibChakraCore.JsGetOwnPropertyNames(@object, out propertyNames));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(propertyNames, out valueRefCount);
				propertyNames.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertyNames);
				return propertyNames;
			}

			public void JsSetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle value, bool useStrictRules)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetProperty(@object, propertyId, value, useStrictRules));
			}

			public bool JsHasProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId)
			{
				bool hasProperty;
				Errors.ThrowIfError(LibChakraCore.JsHasProperty(@object, propertyId, out hasProperty));
				return hasProperty;
			}

			public JavaScriptValueSafeHandle JsDeleteProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, bool useStrictRules)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsDeleteProperty(@object, propertyId, useStrictRules, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsDeleteProperty);
				return result;
			}

			public bool JsDefineProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle propertyDescriptor)
			{
				bool result;
				Errors.ThrowIfError(LibChakraCore.JsDefineProperty(@object, propertyId, propertyDescriptor, out result));
				return result;
			}

			public bool JsHasIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index)
			{
				bool result;
				Errors.ThrowIfError(LibChakraCore.JsHasIndexedProperty(@object, index, out result));
				return result;
			}

			public JavaScriptValueSafeHandle JsGetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsGetIndexedProperty(@object, index, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsGetIndexedProperty);
				return result;
			}

			public void JsSetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, JavaScriptValueSafeHandle value)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetIndexedProperty(@object, index, value));
			}

			public void JsDeleteIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index)
			{
				Errors.ThrowIfError(LibChakraCore.JsDeleteIndexedProperty(@object, index));
			}

			public bool JsHasIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object)
			{
				bool value;
				Errors.ThrowIfError(LibChakraCore.JsHasIndexedPropertiesExternalData(@object, out value));
				return value;
			}

			public JavaScriptTypedArrayType JsGetIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, IntPtr data, out uint elementLength)
			{
				JavaScriptTypedArrayType arrayType;
				Errors.ThrowIfError(LibChakraCore.JsGetIndexedPropertiesExternalData(@object, data, out arrayType, out elementLength));
				return arrayType;
			}

			public void JsSetIndexedPropertiesToExternalData(JavaScriptValueSafeHandle @object, IntPtr data, JavaScriptTypedArrayType arrayType, uint elementLength)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetIndexedPropertiesToExternalData(@object, data, arrayType, elementLength));
			}

			public bool JsEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2)
			{
				bool result;
				Errors.ThrowIfError(LibChakraCore.JsEquals(object1, object2, out result));
				return result;
			}

			public bool JsStrictEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2)
			{
				bool result;
				Errors.ThrowIfError(LibChakraCore.JsStrictEquals(object1, object2, out result));
				return result;
			}

			public bool JsHasExternalData(JavaScriptValueSafeHandle @object)
			{
				bool value;
				Errors.ThrowIfError(LibChakraCore.JsHasExternalData(@object, out value));
				return value;
			}

			public IntPtr JsGetExternalData(JavaScriptValueSafeHandle @object)
			{
				IntPtr externalData;
				Errors.ThrowIfError(LibChakraCore.JsGetExternalData(@object, out externalData));
				return externalData;
			}

			public void JsSetExternalData(JavaScriptValueSafeHandle @object, IntPtr externalData)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetExternalData(@object, externalData));
			}

			public JavaScriptValueSafeHandle JsCreateArray(uint length)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsCreateArray(length, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCreateArray);
				return result;
			}

			public JavaScriptValueSafeHandle JsCreateArrayBuffer(uint byteLength)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsCreateArrayBuffer(byteLength, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCreateArrayBuffer);
				return result;
			}

			public JavaScriptValueSafeHandle JsCreateExternalArrayBuffer(IntPtr data, uint byteLength, JavaScriptObjectFinalizeCallback finalizeCallback, IntPtr callbackState)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsCreateExternalArrayBuffer(data, byteLength, finalizeCallback, callbackState, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCreateExternalArrayBuffer);
				return result;
			}

			public JavaScriptValueSafeHandle JsCreateTypedArray(JavaScriptTypedArrayType arrayType, JavaScriptValueSafeHandle baseArray, uint byteOffset, uint elementLength)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsCreateTypedArray(arrayType, baseArray, byteOffset, elementLength, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCreateTypedArray);
				return result;
			}

			public JavaScriptValueSafeHandle JsCreateDataView(JavaScriptValueSafeHandle arrayBuffer, uint byteOffset, uint byteLength)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsCreateDataView(arrayBuffer, byteOffset, byteLength, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCreateDataView);
				return result;
			}

			public JavaScriptTypedArrayType JsGetTypedArrayInfo(JavaScriptValueSafeHandle typedArray, out JavaScriptValueSafeHandle arrayBuffer, out uint byteOffset, out uint byteLength)
			{
				JavaScriptTypedArrayType arrayType;
				Errors.ThrowIfError(LibChakraCore.JsGetTypedArrayInfo(typedArray, out arrayType, out arrayBuffer, out byteOffset, out byteLength));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(arrayBuffer, out valueRefCount);
				arrayBuffer.NativeFunctionSource = nameof(LibChakraCore.JsGetTypedArrayInfo);
				return arrayType;
			}

			public IntPtr JsGetArrayBufferStorage(JavaScriptValueSafeHandle arrayBuffer, out uint bufferLength)
			{
				IntPtr buffer;
				Errors.ThrowIfError(LibChakraCore.JsGetArrayBufferStorage(arrayBuffer, out buffer, out bufferLength));
				return buffer;
			}

			public IntPtr JsGetTypedArrayStorage(JavaScriptValueSafeHandle typedArray, out uint bufferLength, out JavaScriptTypedArrayType arrayType, out int elementSize)
			{
				IntPtr buffer;
				Errors.ThrowIfError(LibChakraCore.JsGetTypedArrayStorage(typedArray, out buffer, out bufferLength, out arrayType, out elementSize));
				return buffer;
			}

			public IntPtr JsGetDataViewStorage(JavaScriptValueSafeHandle dataView, out uint bufferLength)
			{
				IntPtr buffer;
				Errors.ThrowIfError(LibChakraCore.JsGetDataViewStorage(dataView, out buffer, out bufferLength));
				return buffer;
			}

			public JavaScriptValueSafeHandle JsCallFunction(JavaScriptValueSafeHandle function, IntPtr[] arguments, ushort argumentCount)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsCallFunction(function, arguments, argumentCount, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsCallFunction);
				return result;
			}

			public JavaScriptValueSafeHandle JsConstructObject(JavaScriptValueSafeHandle function, IntPtr[] arguments, ushort argumentCount)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsConstructObject(function, arguments, argumentCount, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsConstructObject);
				return result;
			}

			public JavaScriptValueSafeHandle JsCreateFunction(JavaScriptNativeFunction nativeFunction, IntPtr callbackState)
			{
				JavaScriptValueSafeHandle function;
				Errors.ThrowIfError(LibChakraCore.JsCreateFunction(nativeFunction, callbackState, out function));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(function, out valueRefCount);
				function.NativeFunctionSource = nameof(LibChakraCore.JsCreateFunction);
				return function;
			}

			public JavaScriptValueSafeHandle JsCreateNamedFunction(JavaScriptValueSafeHandle name, JavaScriptNativeFunction nativeFunction, IntPtr callbackState)
			{
				JavaScriptValueSafeHandle function;
				Errors.ThrowIfError(LibChakraCore.JsCreateNamedFunction(name, nativeFunction, callbackState, out function));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(function, out valueRefCount);
				function.NativeFunctionSource = nameof(LibChakraCore.JsCreateNamedFunction);
				return function;
			}

			public JavaScriptValueSafeHandle JsCreateError(JavaScriptValueSafeHandle message)
			{
				JavaScriptValueSafeHandle error;
				Errors.ThrowIfError(LibChakraCore.JsCreateError(message, out error));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(error, out valueRefCount);
				error.NativeFunctionSource = nameof(LibChakraCore.JsCreateError);
				return error;
			}

			public JavaScriptValueSafeHandle JsCreateRangeError(JavaScriptValueSafeHandle message)
			{
				JavaScriptValueSafeHandle error;
				Errors.ThrowIfError(LibChakraCore.JsCreateRangeError(message, out error));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(error, out valueRefCount);
				error.NativeFunctionSource = nameof(LibChakraCore.JsCreateRangeError);
				return error;
			}

			public JavaScriptValueSafeHandle JsCreateReferenceError(JavaScriptValueSafeHandle message)
			{
				JavaScriptValueSafeHandle error;
				Errors.ThrowIfError(LibChakraCore.JsCreateReferenceError(message, out error));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(error, out valueRefCount);
				error.NativeFunctionSource = nameof(LibChakraCore.JsCreateReferenceError);
				return error;
			}

			public JavaScriptValueSafeHandle JsCreateSyntaxError(JavaScriptValueSafeHandle message)
			{
				JavaScriptValueSafeHandle error;
				Errors.ThrowIfError(LibChakraCore.JsCreateSyntaxError(message, out error));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(error, out valueRefCount);
				error.NativeFunctionSource = nameof(LibChakraCore.JsCreateSyntaxError);
				return error;
			}

			public JavaScriptValueSafeHandle JsCreateTypeError(JavaScriptValueSafeHandle message)
			{
				JavaScriptValueSafeHandle error;
				Errors.ThrowIfError(LibChakraCore.JsCreateTypeError(message, out error));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(error, out valueRefCount);
				error.NativeFunctionSource = nameof(LibChakraCore.JsCreateTypeError);
				return error;
			}

			public JavaScriptValueSafeHandle JsCreateURIError(JavaScriptValueSafeHandle message)
			{
				JavaScriptValueSafeHandle error;
				Errors.ThrowIfError(LibChakraCore.JsCreateURIError(message, out error));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(error, out valueRefCount);
				error.NativeFunctionSource = nameof(LibChakraCore.JsCreateURIError);
				return error;
			}

			public bool JsHasException()
			{
				bool hasException;
				Errors.ThrowIfError(LibChakraCore.JsHasException(out hasException));
				return hasException;
			}

			public JavaScriptValueSafeHandle JsGetAndClearException()
			{
				JavaScriptValueSafeHandle exception;
				Errors.ThrowIfError(LibChakraCore.JsGetAndClearException(out exception));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(exception, out valueRefCount);
				exception.NativeFunctionSource = nameof(LibChakraCore.JsGetAndClearException);
				return exception;
			}

			public void JsSetException(JavaScriptValueSafeHandle exception)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetException(exception));
			}

			public void JsDisableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime)
			{
				Errors.ThrowIfError(LibChakraCore.JsDisableRuntimeExecution(runtime));
			}

			public void JsEnableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime)
			{
				Errors.ThrowIfError(LibChakraCore.JsEnableRuntimeExecution(runtime));
			}

			public bool JsIsRuntimeExecutionDisabled(JavaScriptRuntimeSafeHandle runtime)
			{
				bool isDisabled;
				Errors.ThrowIfError(LibChakraCore.JsIsRuntimeExecutionDisabled(runtime, out isDisabled));
				return isDisabled;
			}

			public void JsSetPromiseContinuationCallback(JavaScriptPromiseContinuationCallback promiseContinuationCallback, IntPtr callbackState)
			{
				Errors.ThrowIfError(LibChakraCore.JsSetPromiseContinuationCallback(promiseContinuationCallback, callbackState));
			}

			public JavaScriptValueSafeHandle JsParseScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsParseScript(script, sourceContext, sourceUrl, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsParseScript);
				return result;
			}

			public JavaScriptValueSafeHandle JsParseScriptWithAttributes(string script, JavaScriptSourceContext sourceContext, string sourceUrl, JavaScriptParseScriptAttributes parseAttributes)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsParseScriptWithAttributes(script, sourceContext, sourceUrl, parseAttributes, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsParseScriptWithAttributes);
				return result;
			}

			public JavaScriptValueSafeHandle JsRunScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsRunScript(script, sourceContext, sourceUrl, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsRunScript);
				return result;
			}

			public JavaScriptValueSafeHandle JsExperimentalApiRunModule(string script, JavaScriptSourceContext sourceContext, string sourceUrl)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsExperimentalApiRunModule(script, sourceContext, sourceUrl, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsExperimentalApiRunModule);
				return result;
			}

			public void JsSerializeScript(string script, byte[] buffer, ref ulong bufferSize)
			{
				Errors.ThrowIfError(LibChakraCore.JsSerializeScript(script, buffer, ref bufferSize));
			}

			public JavaScriptValueSafeHandle JsParseSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsParseSerializedScriptWithCallback(scriptLoadCallback, scriptUnloadCallback, buffer, sourceContext, sourceUrl, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerializedScriptWithCallback);
				return result;
			}

			public JavaScriptValueSafeHandle JsRunSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsRunSerializedScriptWithCallback(scriptLoadCallback, scriptUnloadCallback, buffer, sourceContext, sourceUrl, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerializedScriptWithCallback);
				return result;
			}

			public JavaScriptValueSafeHandle JsParseSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsParseSerializedScript(script, buffer, sourceContext, sourceUrl, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerializedScript);
				return result;
			}

			public JavaScriptValueSafeHandle JsRunSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
			{
				JavaScriptValueSafeHandle result;
				Errors.ThrowIfError(LibChakraCore.JsRunSerializedScript(script, buffer, sourceContext, sourceUrl, out result));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(result, out valueRefCount);
				result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerializedScript);
				return result;
			}

			public JavaScriptPropertyIdSafeHandle JsGetPropertyIdFromName(string name)
			{
				JavaScriptPropertyIdSafeHandle propertyId;
				Errors.ThrowIfError(LibChakraCore.JsGetPropertyIdFromName(name, out propertyId));
				return propertyId;
			}

			public string JsGetPropertyNameFromId(JavaScriptPropertyIdSafeHandle propertyId)
			{
				string name;
				Errors.ThrowIfError(LibChakraCore.JsGetPropertyNameFromId(propertyId, out name));
				return name;
			}

			public JavaScriptValueSafeHandle JsPointerToString(string stringValue, ulong stringLength)
			{
				JavaScriptValueSafeHandle value;
				Errors.ThrowIfError(LibChakraCore.JsPointerToString(stringValue, stringLength, out value));
				uint valueRefCount;
				LibChakraCore.JsAddValueRef(value, out valueRefCount);
				value.NativeFunctionSource = nameof(LibChakraCore.JsPointerToString);
				return value;
			}

			public IntPtr JsStringToPointer(JavaScriptValueSafeHandle value, out ulong stringLength)
			{
				IntPtr stringValue;
				Errors.ThrowIfError(LibChakraCore.JsStringToPointer(value, out stringValue, out stringLength));
				return stringValue;
			}

			public void JsDiagStartDebugging(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagDebugEventCallback debugEventCallback, IntPtr callbackState)
			{
				Errors.ThrowIfError(LibChakraCore.JsDiagStartDebugging(runtimeHandle, debugEventCallback, callbackState));
			}

	}
}
