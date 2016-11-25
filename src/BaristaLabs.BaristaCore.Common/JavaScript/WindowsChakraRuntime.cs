namespace BaristaLabs.BaristaCore.JavaScript
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
			exceptionValueRef.NativeFunctionSource = nameof(LibChakraCore.JsParseModuleSource);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(exceptionValueRef);
			return exceptionValueRef;
		}

		public JavaScriptValueSafeHandle JsModuleEvaluation(IntPtr requestModule)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsModuleEvaluation(requestModule, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsModuleEvaluation);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
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
			value.NativeFunctionSource = nameof(LibChakraCore.JsCreateString);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(value);
			return value;
		}

		public JavaScriptValueSafeHandle JsCreateStringUtf8(string content, UIntPtr length)
		{
			JavaScriptValueSafeHandle value;
			Errors.ThrowIfError(LibChakraCore.JsCreateStringUtf8(content, length, out value));
			value.NativeFunctionSource = nameof(LibChakraCore.JsCreateStringUtf8);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(value);
			return value;
		}

		public JavaScriptValueSafeHandle JsCreateStringUtf16(string content, UIntPtr length)
		{
			JavaScriptValueSafeHandle value;
			Errors.ThrowIfError(LibChakraCore.JsCreateStringUtf16(content, length, out value));
			value.NativeFunctionSource = nameof(LibChakraCore.JsCreateStringUtf16);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(value);
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
			result.NativeFunctionSource = nameof(LibChakraCore.JsParse);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsRun(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsRun(script, sourceContext, sourceUrl, parseAttributes, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsRun);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptPropertyIdSafeHandle JsCreatePropertyIdUtf8(string name, UIntPtr length)
		{
			JavaScriptPropertyIdSafeHandle propertyId;
			Errors.ThrowIfError(LibChakraCore.JsCreatePropertyIdUtf8(name, length, out propertyId));
			propertyId.NativeFunctionSource = nameof(LibChakraCore.JsCreatePropertyIdUtf8);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(propertyId);
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
			result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerialized);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsRunSerialized(byte[] buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsRunSerialized(buffer, scriptLoadCallback, sourceContext, sourceUrl, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerialized);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptRuntimeSafeHandle JsCreateRuntime(JavaScriptRuntimeAttributes attributes, JavaScriptThreadServiceCallback threadService)
		{
			JavaScriptRuntimeSafeHandle runtime;
			Errors.ThrowIfError(LibChakraCore.JsCreateRuntime(attributes, threadService, out runtime));
			runtime.NativeFunctionSource = nameof(LibChakraCore.JsCreateRuntime);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(runtime);
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

		public uint JsAddRef(SafeHandle @ref)
		{
			uint count;
			Errors.ThrowIfError(LibChakraCore.JsAddRef(@ref, out count));
			return count;
		}

		public uint JsRelease(SafeHandle @ref)
		{
			uint count;
			Errors.ThrowIfError(LibChakraCore.JsRelease(@ref, out count));
			return count;
		}

		public void JsSetObjectBeforeCollectCallback(SafeHandle @ref, IntPtr callbackState, JavaScriptObjectBeforeCollectCallback objectBeforeCollectCallback)
		{
			Errors.ThrowIfError(LibChakraCore.JsSetObjectBeforeCollectCallback(@ref, callbackState, objectBeforeCollectCallback));
		}

		public JavaScriptContextSafeHandle JsCreateContext(JavaScriptRuntimeSafeHandle runtime)
		{
			JavaScriptContextSafeHandle newContext;
			Errors.ThrowIfError(LibChakraCore.JsCreateContext(runtime, out newContext));
			newContext.NativeFunctionSource = nameof(LibChakraCore.JsCreateContext);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(newContext);
			return newContext;
		}

		public JavaScriptContextSafeHandle JsGetCurrentContext()
		{
			JavaScriptContextSafeHandle currentContext;
			Errors.ThrowIfError(LibChakraCore.JsGetCurrentContext(out currentContext));
			currentContext.NativeFunctionSource = nameof(LibChakraCore.JsGetCurrentContext);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(currentContext);
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
			context.NativeFunctionSource = nameof(LibChakraCore.JsGetContextOfObject);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(context);
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
			runtime.NativeFunctionSource = nameof(LibChakraCore.JsGetRuntime);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(runtime);
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
			symbol.NativeFunctionSource = nameof(LibChakraCore.JsGetSymbolFromPropertyId);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(symbol);
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
			propertyId.NativeFunctionSource = nameof(LibChakraCore.JsGetPropertyIdFromSymbol);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(propertyId);
			return propertyId;
		}

		public JavaScriptValueSafeHandle JsCreateSymbol(JavaScriptValueSafeHandle description)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsCreateSymbol(description, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsCreateSymbol);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsGetOwnPropertySymbols(JavaScriptValueSafeHandle @object)
		{
			JavaScriptValueSafeHandle propertySymbols;
			Errors.ThrowIfError(LibChakraCore.JsGetOwnPropertySymbols(@object, out propertySymbols));
			propertySymbols.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertySymbols);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(propertySymbols);
			return propertySymbols;
		}

		public JavaScriptValueSafeHandle JsGetUndefinedValue()
		{
			JavaScriptValueSafeHandle undefinedValue;
			Errors.ThrowIfError(LibChakraCore.JsGetUndefinedValue(out undefinedValue));
			undefinedValue.NativeFunctionSource = nameof(LibChakraCore.JsGetUndefinedValue);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(undefinedValue);
			return undefinedValue;
		}

		public JavaScriptValueSafeHandle JsGetNullValue()
		{
			JavaScriptValueSafeHandle nullValue;
			Errors.ThrowIfError(LibChakraCore.JsGetNullValue(out nullValue));
			nullValue.NativeFunctionSource = nameof(LibChakraCore.JsGetNullValue);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(nullValue);
			return nullValue;
		}

		public JavaScriptValueSafeHandle JsGetTrueValue()
		{
			JavaScriptValueSafeHandle trueValue;
			Errors.ThrowIfError(LibChakraCore.JsGetTrueValue(out trueValue));
			trueValue.NativeFunctionSource = nameof(LibChakraCore.JsGetTrueValue);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(trueValue);
			return trueValue;
		}

		public JavaScriptValueSafeHandle JsGetFalseValue()
		{
			JavaScriptValueSafeHandle falseValue;
			Errors.ThrowIfError(LibChakraCore.JsGetFalseValue(out falseValue));
			falseValue.NativeFunctionSource = nameof(LibChakraCore.JsGetFalseValue);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(falseValue);
			return falseValue;
		}

		public JavaScriptValueSafeHandle JsBoolToBoolean(bool value)
		{
			JavaScriptValueSafeHandle booleanValue;
			Errors.ThrowIfError(LibChakraCore.JsBoolToBoolean(value, out booleanValue));
			booleanValue.NativeFunctionSource = nameof(LibChakraCore.JsBoolToBoolean);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(booleanValue);
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
			booleanValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToBoolean);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(booleanValue);
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
			value.NativeFunctionSource = nameof(LibChakraCore.JsDoubleToNumber);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(value);
			return value;
		}

		public JavaScriptValueSafeHandle JsIntToNumber(int intValue)
		{
			JavaScriptValueSafeHandle value;
			Errors.ThrowIfError(LibChakraCore.JsIntToNumber(intValue, out value));
			value.NativeFunctionSource = nameof(LibChakraCore.JsIntToNumber);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(value);
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
			numberValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToNumber);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(numberValue);
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
			stringValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToString);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(stringValue);
			return stringValue;
		}

		public JavaScriptValueSafeHandle JsGetGlobalObject()
		{
			JavaScriptValueSafeHandle globalObject;
			Errors.ThrowIfError(LibChakraCore.JsGetGlobalObject(out globalObject));
			globalObject.NativeFunctionSource = nameof(LibChakraCore.JsGetGlobalObject);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(globalObject);
			return globalObject;
		}

		public JavaScriptValueSafeHandle JsCreateObject()
		{
			JavaScriptValueSafeHandle @object;
			Errors.ThrowIfError(LibChakraCore.JsCreateObject(out @object));
			@object.NativeFunctionSource = nameof(LibChakraCore.JsCreateObject);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(@object);
			return @object;
		}

		public JavaScriptValueSafeHandle JsCreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback)
		{
			JavaScriptValueSafeHandle @object;
			Errors.ThrowIfError(LibChakraCore.JsCreateExternalObject(data, finalizeCallback, out @object));
			@object.NativeFunctionSource = nameof(LibChakraCore.JsCreateExternalObject);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(@object);
			return @object;
		}

		public JavaScriptValueSafeHandle JsConvertValueToObject(JavaScriptValueSafeHandle value)
		{
			JavaScriptValueSafeHandle @object;
			Errors.ThrowIfError(LibChakraCore.JsConvertValueToObject(value, out @object));
			@object.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToObject);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(@object);
			return @object;
		}

		public JavaScriptValueSafeHandle JsGetPrototype(JavaScriptValueSafeHandle @object)
		{
			JavaScriptValueSafeHandle prototypeObject;
			Errors.ThrowIfError(LibChakraCore.JsGetPrototype(@object, out prototypeObject));
			prototypeObject.NativeFunctionSource = nameof(LibChakraCore.JsGetPrototype);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(prototypeObject);
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
			value.NativeFunctionSource = nameof(LibChakraCore.JsGetProperty);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(value);
			return value;
		}

		public JavaScriptValueSafeHandle JsGetOwnPropertyDescriptor(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId)
		{
			JavaScriptValueSafeHandle propertyDescriptor;
			Errors.ThrowIfError(LibChakraCore.JsGetOwnPropertyDescriptor(@object, propertyId, out propertyDescriptor));
			propertyDescriptor.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertyDescriptor);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(propertyDescriptor);
			return propertyDescriptor;
		}

		public JavaScriptValueSafeHandle JsGetOwnPropertyNames(JavaScriptValueSafeHandle @object)
		{
			JavaScriptValueSafeHandle propertyNames;
			Errors.ThrowIfError(LibChakraCore.JsGetOwnPropertyNames(@object, out propertyNames));
			propertyNames.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertyNames);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(propertyNames);
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
			result.NativeFunctionSource = nameof(LibChakraCore.JsDeleteProperty);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
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
			result.NativeFunctionSource = nameof(LibChakraCore.JsGetIndexedProperty);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
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
			result.NativeFunctionSource = nameof(LibChakraCore.JsCreateArray);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsCreateArrayBuffer(uint byteLength)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsCreateArrayBuffer(byteLength, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsCreateArrayBuffer);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsCreateExternalArrayBuffer(IntPtr data, uint byteLength, JavaScriptObjectFinalizeCallback finalizeCallback, IntPtr callbackState)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsCreateExternalArrayBuffer(data, byteLength, finalizeCallback, callbackState, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsCreateExternalArrayBuffer);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsCreateTypedArray(JavaScriptTypedArrayType arrayType, JavaScriptValueSafeHandle baseArray, uint byteOffset, uint elementLength)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsCreateTypedArray(arrayType, baseArray, byteOffset, elementLength, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsCreateTypedArray);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsCreateDataView(JavaScriptValueSafeHandle arrayBuffer, uint byteOffset, uint byteLength)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsCreateDataView(arrayBuffer, byteOffset, byteLength, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsCreateDataView);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptTypedArrayType JsGetTypedArrayInfo(JavaScriptValueSafeHandle typedArray, out JavaScriptValueSafeHandle arrayBuffer, out uint byteOffset, out uint byteLength)
		{
			JavaScriptTypedArrayType arrayType;
			Errors.ThrowIfError(LibChakraCore.JsGetTypedArrayInfo(typedArray, out arrayType, out arrayBuffer, out byteOffset, out byteLength));
			arrayBuffer.NativeFunctionSource = nameof(LibChakraCore.JsGetTypedArrayInfo);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(arrayBuffer);
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
			result.NativeFunctionSource = nameof(LibChakraCore.JsCallFunction);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsConstructObject(JavaScriptValueSafeHandle function, IntPtr[] arguments, ushort argumentCount)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsConstructObject(function, arguments, argumentCount, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsConstructObject);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsCreateFunction(JavaScriptNativeFunction nativeFunction, IntPtr callbackState)
		{
			JavaScriptValueSafeHandle function;
			Errors.ThrowIfError(LibChakraCore.JsCreateFunction(nativeFunction, callbackState, out function));
			function.NativeFunctionSource = nameof(LibChakraCore.JsCreateFunction);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(function);
			return function;
		}

		public JavaScriptValueSafeHandle JsCreateNamedFunction(JavaScriptValueSafeHandle name, JavaScriptNativeFunction nativeFunction, IntPtr callbackState)
		{
			JavaScriptValueSafeHandle function;
			Errors.ThrowIfError(LibChakraCore.JsCreateNamedFunction(name, nativeFunction, callbackState, out function));
			function.NativeFunctionSource = nameof(LibChakraCore.JsCreateNamedFunction);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(function);
			return function;
		}

		public JavaScriptValueSafeHandle JsCreateError(JavaScriptValueSafeHandle message)
		{
			JavaScriptValueSafeHandle error;
			Errors.ThrowIfError(LibChakraCore.JsCreateError(message, out error));
			error.NativeFunctionSource = nameof(LibChakraCore.JsCreateError);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(error);
			return error;
		}

		public JavaScriptValueSafeHandle JsCreateRangeError(JavaScriptValueSafeHandle message)
		{
			JavaScriptValueSafeHandle error;
			Errors.ThrowIfError(LibChakraCore.JsCreateRangeError(message, out error));
			error.NativeFunctionSource = nameof(LibChakraCore.JsCreateRangeError);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(error);
			return error;
		}

		public JavaScriptValueSafeHandle JsCreateReferenceError(JavaScriptValueSafeHandle message)
		{
			JavaScriptValueSafeHandle error;
			Errors.ThrowIfError(LibChakraCore.JsCreateReferenceError(message, out error));
			error.NativeFunctionSource = nameof(LibChakraCore.JsCreateReferenceError);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(error);
			return error;
		}

		public JavaScriptValueSafeHandle JsCreateSyntaxError(JavaScriptValueSafeHandle message)
		{
			JavaScriptValueSafeHandle error;
			Errors.ThrowIfError(LibChakraCore.JsCreateSyntaxError(message, out error));
			error.NativeFunctionSource = nameof(LibChakraCore.JsCreateSyntaxError);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(error);
			return error;
		}

		public JavaScriptValueSafeHandle JsCreateTypeError(JavaScriptValueSafeHandle message)
		{
			JavaScriptValueSafeHandle error;
			Errors.ThrowIfError(LibChakraCore.JsCreateTypeError(message, out error));
			error.NativeFunctionSource = nameof(LibChakraCore.JsCreateTypeError);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(error);
			return error;
		}

		public JavaScriptValueSafeHandle JsCreateURIError(JavaScriptValueSafeHandle message)
		{
			JavaScriptValueSafeHandle error;
			Errors.ThrowIfError(LibChakraCore.JsCreateURIError(message, out error));
			error.NativeFunctionSource = nameof(LibChakraCore.JsCreateURIError);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(error);
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
			exception.NativeFunctionSource = nameof(LibChakraCore.JsGetAndClearException);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(exception);
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
			result.NativeFunctionSource = nameof(LibChakraCore.JsParseScript);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsParseScriptWithAttributes(string script, JavaScriptSourceContext sourceContext, string sourceUrl, JavaScriptParseScriptAttributes parseAttributes)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsParseScriptWithAttributes(script, sourceContext, sourceUrl, parseAttributes, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsParseScriptWithAttributes);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsRunScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsRunScript(script, sourceContext, sourceUrl, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsRunScript);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsExperimentalApiRunModule(string script, JavaScriptSourceContext sourceContext, string sourceUrl)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsExperimentalApiRunModule(script, sourceContext, sourceUrl, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsExperimentalApiRunModule);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
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
			result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerializedScriptWithCallback);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsRunSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsRunSerializedScriptWithCallback(scriptLoadCallback, scriptUnloadCallback, buffer, sourceContext, sourceUrl, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerializedScriptWithCallback);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsParseSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsParseSerializedScript(script, buffer, sourceContext, sourceUrl, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerializedScript);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptValueSafeHandle JsRunSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsRunSerializedScript(script, buffer, sourceContext, sourceUrl, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerializedScript);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(result);
			return result;
		}

		public JavaScriptPropertyIdSafeHandle JsGetPropertyIdFromName(string name)
		{
			JavaScriptPropertyIdSafeHandle propertyId;
			Errors.ThrowIfError(LibChakraCore.JsGetPropertyIdFromName(name, out propertyId));
			propertyId.NativeFunctionSource = nameof(LibChakraCore.JsGetPropertyIdFromName);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(propertyId);
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
			value.NativeFunctionSource = nameof(LibChakraCore.JsPointerToString);
			JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(value);
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
