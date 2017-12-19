namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

	using System;
	using System.Runtime.InteropServices;

    public abstract class ChakraEngineBase : IJavaScriptEngine
    {
        public JavaScriptModuleRecord JsInitializeModuleRecord(JavaScriptModuleRecord referencingModule, JavaScriptValueSafeHandle normalizedSpecifier)
        {
            Errors.ThrowIfError(LibChakraCore.JsInitializeModuleRecord(referencingModule, normalizedSpecifier, out JavaScriptModuleRecord moduleRecord));
            moduleRecord.NativeFunctionSource = nameof(LibChakraCore.JsInitializeModuleRecord);
            if (moduleRecord != JavaScriptModuleRecord.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(moduleRecord, out uint valueRefCount));
			}
            return moduleRecord;
        }

        public JavaScriptValueSafeHandle JsParseModuleSource(JavaScriptModuleRecord requestModule, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags sourceFlag)
        {
            Errors.ThrowIfError(LibChakraCore.JsParseModuleSource(requestModule, sourceContext, script, scriptLength, sourceFlag, out JavaScriptValueSafeHandle exceptionValueRef));
            exceptionValueRef.NativeFunctionSource = nameof(LibChakraCore.JsParseModuleSource);
            if (exceptionValueRef != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(exceptionValueRef, out uint valueRefCount));
			}
            return exceptionValueRef;
        }

        public JavaScriptValueSafeHandle JsModuleEvaluation(JavaScriptModuleRecord requestModule)
        {
            Errors.ThrowIfError(LibChakraCore.JsModuleEvaluation(requestModule, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsModuleEvaluation);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public void JsSetModuleHostInfo(JavaScriptModuleRecord requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, IntPtr hostInfo)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetModuleHostInfo(requestModule, moduleHostInfo, hostInfo));
        }

        public IntPtr JsGetModuleHostInfo(JavaScriptModuleRecord requestModule, JavaScriptModuleHostInfoKind moduleHostInfo)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetModuleHostInfo(requestModule, moduleHostInfo, out IntPtr hostInfo));
            return hostInfo;
        }

        public JavaScriptValueSafeHandle JsGetAndClearExceptionWithMetadata()
        {
            Errors.ThrowIfError(LibChakraCore.JsGetAndClearExceptionWithMetadata(out JavaScriptValueSafeHandle metadata));
            metadata.NativeFunctionSource = nameof(LibChakraCore.JsGetAndClearExceptionWithMetadata);
            if (metadata != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(metadata, out uint valueRefCount));
			}
            return metadata;
        }

        public JavaScriptValueSafeHandle JsCreateString(string content, ulong length)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateString(content, length, out JavaScriptValueSafeHandle value));
            value.NativeFunctionSource = nameof(LibChakraCore.JsCreateString);
            if (value != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(value, out uint valueRefCount));
			}
            return value;
        }

        public JavaScriptValueSafeHandle JsCreateStringUtf16(string content, ulong length)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateStringUtf16(content, length, out JavaScriptValueSafeHandle value));
            value.NativeFunctionSource = nameof(LibChakraCore.JsCreateStringUtf16);
            if (value != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(value, out uint valueRefCount));
			}
            return value;
        }

        public ulong JsCopyString(JavaScriptValueSafeHandle value, byte[] buffer, ulong bufferSize)
        {
            Errors.ThrowIfError(LibChakraCore.JsCopyString(value, buffer, bufferSize, out ulong length));
            return length;
        }

        public ulong JsCopyStringUtf16(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer)
        {
            Errors.ThrowIfError(LibChakraCore.JsCopyStringUtf16(value, start, length, buffer, out ulong written));
            return written;
        }

        public JavaScriptValueSafeHandle JsParse(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes)
        {
            Errors.ThrowIfError(LibChakraCore.JsParse(script, sourceContext, sourceUrl, parseAttributes, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsParse);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsRun(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes)
        {
            Errors.ThrowIfError(LibChakraCore.JsRun(script, sourceContext, sourceUrl, parseAttributes, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsRun);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptPropertyIdSafeHandle JsCreatePropertyId(string name, ulong length)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreatePropertyId(name, length, out JavaScriptPropertyIdSafeHandle propertyId));
            propertyId.NativeFunctionSource = nameof(LibChakraCore.JsCreatePropertyId);
            if (propertyId != JavaScriptPropertyIdSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertyId, out uint valueRefCount));
			}
            return propertyId;
        }

        public ulong JsCopyPropertyId(JavaScriptPropertyIdSafeHandle propertyId, byte[] buffer, ulong bufferSize)
        {
            Errors.ThrowIfError(LibChakraCore.JsCopyPropertyId(propertyId, buffer, bufferSize, out ulong length));
            return length;
        }

        public JavaScriptValueSafeHandle JsSerialize(JavaScriptValueSafeHandle script, JavaScriptParseScriptAttributes parseAttributes)
        {
            Errors.ThrowIfError(LibChakraCore.JsSerialize(script, out JavaScriptValueSafeHandle buffer, parseAttributes));
            buffer.NativeFunctionSource = nameof(LibChakraCore.JsSerialize);
            if (buffer != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(buffer, out uint valueRefCount));
			}
            return buffer;
        }

        public JavaScriptValueSafeHandle JsParseSerialized(JavaScriptValueSafeHandle buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl)
        {
            Errors.ThrowIfError(LibChakraCore.JsParseSerialized(buffer, scriptLoadCallback, sourceContext, sourceUrl, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerialized);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsRunSerialized(JavaScriptValueSafeHandle buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl)
        {
            Errors.ThrowIfError(LibChakraCore.JsRunSerialized(buffer, scriptLoadCallback, sourceContext, sourceUrl, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerialized);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsCreatePromise(out JavaScriptValueSafeHandle resolveFunction, out JavaScriptValueSafeHandle rejectFunction)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreatePromise(out JavaScriptValueSafeHandle promise, out resolveFunction, out rejectFunction));
            promise.NativeFunctionSource = nameof(LibChakraCore.JsCreatePromise);
            if (promise != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(promise, out uint valueRefCount));
			}
            resolveFunction.NativeFunctionSource = nameof(LibChakraCore.JsCreatePromise);
            if (resolveFunction != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(resolveFunction, out uint valueRefCount));
			}
            rejectFunction.NativeFunctionSource = nameof(LibChakraCore.JsCreatePromise);
            if (rejectFunction != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(rejectFunction, out uint valueRefCount));
			}
            return promise;
        }

        public JavaScriptWeakReferenceSafeHandle JsCreateWeakReference(JavaScriptValueSafeHandle value)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateWeakReference(value, out JavaScriptWeakReferenceSafeHandle weakRef));
            return weakRef;
        }

        public JavaScriptValueSafeHandle JsGetWeakReferenceValue(JavaScriptWeakReferenceSafeHandle weakRef)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetWeakReferenceValue(weakRef, out JavaScriptValueSafeHandle value));
            value.NativeFunctionSource = nameof(LibChakraCore.JsGetWeakReferenceValue);
            if (value != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(value, out uint valueRefCount));
			}
            return value;
        }

        public JavaScriptValueSafeHandle JsCreateSharedArrayBufferWithSharedContent(IntPtr sharedContents)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateSharedArrayBufferWithSharedContent(sharedContents, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCreateSharedArrayBufferWithSharedContent);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public IntPtr JsGetSharedArrayBufferContent(JavaScriptValueSafeHandle sharedArrayBuffer)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetSharedArrayBufferContent(sharedArrayBuffer, out IntPtr sharedContents));
            return sharedContents;
        }

        public void JsReleaseSharedArrayBufferContentHandle(IntPtr sharedContents)
        {
            Errors.ThrowIfError(LibChakraCore.JsReleaseSharedArrayBufferContentHandle(sharedContents));
        }

        public bool JsHasOwnProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId)
        {
            Errors.ThrowIfError(LibChakraCore.JsHasOwnProperty(@object, propertyId, out bool hasOwnProperty));
            return hasOwnProperty;
        }

        public ulong JsCopyStringOneByte(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer)
        {
            Errors.ThrowIfError(LibChakraCore.JsCopyStringOneByte(value, start, length, buffer, out ulong written));
            return written;
        }

        public JavaScriptValueSafeHandle JsGetDataViewInfo(JavaScriptValueSafeHandle dataView, out uint byteOffset, out uint byteLength)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetDataViewInfo(dataView, out JavaScriptValueSafeHandle arrayBuffer, out byteOffset, out byteLength));
            arrayBuffer.NativeFunctionSource = nameof(LibChakraCore.JsGetDataViewInfo);
            if (arrayBuffer != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(arrayBuffer, out uint valueRefCount));
			}
            return arrayBuffer;
        }

        public JavaScriptRuntimeSafeHandle JsCreateRuntime(JavaScriptRuntimeAttributes attributes, JavaScriptThreadServiceCallback threadService)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateRuntime(attributes, threadService, out JavaScriptRuntimeSafeHandle runtime));
            runtime.NativeFunctionSource = nameof(LibChakraCore.JsCreateRuntime);
            return runtime;
        }

        public void JsCollectGarbage(JavaScriptRuntimeSafeHandle runtime)
        {
            Errors.ThrowIfError(LibChakraCore.JsCollectGarbage(runtime));
        }

        public void JsDisposeRuntime(IntPtr runtime)
        {
            Errors.ThrowIfError(LibChakraCore.JsDisposeRuntime(runtime));
        }

        public ulong JsGetRuntimeMemoryUsage(JavaScriptRuntimeSafeHandle runtime)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetRuntimeMemoryUsage(runtime, out ulong memoryUsage));
            return memoryUsage;
        }

        public ulong JsGetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetRuntimeMemoryLimit(runtime, out ulong memoryLimit));
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
            Errors.ThrowIfError(LibChakraCore.JsAddRef(@ref, out uint count));
            return count;
        }

        public uint JsRelease(SafeHandle @ref)
        {
            Errors.ThrowIfError(LibChakraCore.JsRelease(@ref, out uint count));
            return count;
        }

        public void JsSetObjectBeforeCollectCallback(SafeHandle @ref, IntPtr callbackState, JavaScriptObjectBeforeCollectCallback objectBeforeCollectCallback)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetObjectBeforeCollectCallback(@ref, callbackState, objectBeforeCollectCallback));
        }

        public JavaScriptContextSafeHandle JsCreateContext(JavaScriptRuntimeSafeHandle runtime)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateContext(runtime, out JavaScriptContextSafeHandle newContext));
            newContext.NativeFunctionSource = nameof(LibChakraCore.JsCreateContext);
            if (newContext != JavaScriptContextSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(newContext, out uint valueRefCount));
			}
            return newContext;
        }

        public JavaScriptContextSafeHandle JsGetCurrentContext()
        {
            Errors.ThrowIfError(LibChakraCore.JsGetCurrentContext(out JavaScriptContextSafeHandle currentContext));
            currentContext.NativeFunctionSource = nameof(LibChakraCore.JsGetCurrentContext);
            if (currentContext != JavaScriptContextSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(currentContext, out uint valueRefCount));
			}
            return currentContext;
        }

        public void JsSetCurrentContext(JavaScriptContextSafeHandle context)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetCurrentContext(context));
        }

        public JavaScriptContextSafeHandle JsGetContextOfObject(JavaScriptValueSafeHandle @object)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetContextOfObject(@object, out JavaScriptContextSafeHandle context));
            context.NativeFunctionSource = nameof(LibChakraCore.JsGetContextOfObject);
            if (context != JavaScriptContextSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(context, out uint valueRefCount));
			}
            return context;
        }

        public IntPtr JsGetContextData(JavaScriptContextSafeHandle context)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetContextData(context, out IntPtr data));
            return data;
        }

        public void JsSetContextData(JavaScriptContextSafeHandle context, IntPtr data)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetContextData(context, data));
        }

        public JavaScriptRuntimeSafeHandle JsGetRuntime(JavaScriptContextSafeHandle context)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetRuntime(context, out JavaScriptRuntimeSafeHandle runtime));
            runtime.NativeFunctionSource = nameof(LibChakraCore.JsGetRuntime);
            return runtime;
        }

        public uint JsIdle()
        {
            Errors.ThrowIfError(LibChakraCore.JsIdle(out uint nextIdleTick));
            return nextIdleTick;
        }

        public JavaScriptValueSafeHandle JsGetSymbolFromPropertyId(JavaScriptPropertyIdSafeHandle propertyId)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetSymbolFromPropertyId(propertyId, out JavaScriptValueSafeHandle symbol));
            symbol.NativeFunctionSource = nameof(LibChakraCore.JsGetSymbolFromPropertyId);
            if (symbol != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(symbol, out uint valueRefCount));
			}
            return symbol;
        }

        public JavaScriptPropertyIdType JsGetPropertyIdType(JavaScriptPropertyIdSafeHandle propertyId)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetPropertyIdType(propertyId, out JavaScriptPropertyIdType propertyIdType));
            return propertyIdType;
        }

        public JavaScriptPropertyIdSafeHandle JsGetPropertyIdFromSymbol(JavaScriptValueSafeHandle symbol)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetPropertyIdFromSymbol(symbol, out JavaScriptPropertyIdSafeHandle propertyId));
            propertyId.NativeFunctionSource = nameof(LibChakraCore.JsGetPropertyIdFromSymbol);
            if (propertyId != JavaScriptPropertyIdSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertyId, out uint valueRefCount));
			}
            return propertyId;
        }

        public JavaScriptValueSafeHandle JsCreateSymbol(JavaScriptValueSafeHandle description)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateSymbol(description, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCreateSymbol);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsGetOwnPropertySymbols(JavaScriptValueSafeHandle @object)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetOwnPropertySymbols(@object, out JavaScriptValueSafeHandle propertySymbols));
            propertySymbols.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertySymbols);
            if (propertySymbols != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertySymbols, out uint valueRefCount));
			}
            return propertySymbols;
        }

        public JavaScriptValueSafeHandle JsGetUndefinedValue()
        {
            Errors.ThrowIfError(LibChakraCore.JsGetUndefinedValue(out JavaScriptValueSafeHandle undefinedValue));
            undefinedValue.NativeFunctionSource = nameof(LibChakraCore.JsGetUndefinedValue);
            if (undefinedValue != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(undefinedValue, out uint valueRefCount));
			}
            return undefinedValue;
        }

        public JavaScriptValueSafeHandle JsGetNullValue()
        {
            Errors.ThrowIfError(LibChakraCore.JsGetNullValue(out JavaScriptValueSafeHandle nullValue));
            nullValue.NativeFunctionSource = nameof(LibChakraCore.JsGetNullValue);
            if (nullValue != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(nullValue, out uint valueRefCount));
			}
            return nullValue;
        }

        public JavaScriptValueSafeHandle JsGetTrueValue()
        {
            Errors.ThrowIfError(LibChakraCore.JsGetTrueValue(out JavaScriptValueSafeHandle trueValue));
            trueValue.NativeFunctionSource = nameof(LibChakraCore.JsGetTrueValue);
            if (trueValue != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(trueValue, out uint valueRefCount));
			}
            return trueValue;
        }

        public JavaScriptValueSafeHandle JsGetFalseValue()
        {
            Errors.ThrowIfError(LibChakraCore.JsGetFalseValue(out JavaScriptValueSafeHandle falseValue));
            falseValue.NativeFunctionSource = nameof(LibChakraCore.JsGetFalseValue);
            if (falseValue != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(falseValue, out uint valueRefCount));
			}
            return falseValue;
        }

        public JavaScriptValueSafeHandle JsBoolToBoolean(bool value)
        {
            Errors.ThrowIfError(LibChakraCore.JsBoolToBoolean(value, out JavaScriptValueSafeHandle booleanValue));
            booleanValue.NativeFunctionSource = nameof(LibChakraCore.JsBoolToBoolean);
            if (booleanValue != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(booleanValue, out uint valueRefCount));
			}
            return booleanValue;
        }

        public bool JsBooleanToBool(JavaScriptValueSafeHandle value)
        {
            Errors.ThrowIfError(LibChakraCore.JsBooleanToBool(value, out bool boolValue));
            return boolValue;
        }

        public JavaScriptValueSafeHandle JsConvertValueToBoolean(JavaScriptValueSafeHandle value)
        {
            Errors.ThrowIfError(LibChakraCore.JsConvertValueToBoolean(value, out JavaScriptValueSafeHandle booleanValue));
            booleanValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToBoolean);
            if (booleanValue != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(booleanValue, out uint valueRefCount));
			}
            return booleanValue;
        }

        public JavaScriptValueType JsGetValueType(JavaScriptValueSafeHandle value)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetValueType(value, out JavaScriptValueType type));
            return type;
        }

        public JavaScriptValueSafeHandle JsDoubleToNumber(double doubleValue)
        {
            Errors.ThrowIfError(LibChakraCore.JsDoubleToNumber(doubleValue, out JavaScriptValueSafeHandle value));
            value.NativeFunctionSource = nameof(LibChakraCore.JsDoubleToNumber);
            if (value != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(value, out uint valueRefCount));
			}
            return value;
        }

        public JavaScriptValueSafeHandle JsIntToNumber(int intValue)
        {
            Errors.ThrowIfError(LibChakraCore.JsIntToNumber(intValue, out JavaScriptValueSafeHandle value));
            value.NativeFunctionSource = nameof(LibChakraCore.JsIntToNumber);
            if (value != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(value, out uint valueRefCount));
			}
            return value;
        }

        public double JsNumberToDouble(JavaScriptValueSafeHandle value)
        {
            Errors.ThrowIfError(LibChakraCore.JsNumberToDouble(value, out double doubleValue));
            return doubleValue;
        }

        public int JsNumberToInt(JavaScriptValueSafeHandle value)
        {
            Errors.ThrowIfError(LibChakraCore.JsNumberToInt(value, out int intValue));
            return intValue;
        }

        public JavaScriptValueSafeHandle JsConvertValueToNumber(JavaScriptValueSafeHandle value)
        {
            Errors.ThrowIfError(LibChakraCore.JsConvertValueToNumber(value, out JavaScriptValueSafeHandle numberValue));
            numberValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToNumber);
            if (numberValue != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(numberValue, out uint valueRefCount));
			}
            return numberValue;
        }

        public int JsGetStringLength(JavaScriptValueSafeHandle stringValue)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetStringLength(stringValue, out int length));
            return length;
        }

        public JavaScriptValueSafeHandle JsConvertValueToString(JavaScriptValueSafeHandle value)
        {
            Errors.ThrowIfError(LibChakraCore.JsConvertValueToString(value, out JavaScriptValueSafeHandle stringValue));
            stringValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToString);
            if (stringValue != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(stringValue, out uint valueRefCount));
			}
            return stringValue;
        }

        public JavaScriptValueSafeHandle JsGetGlobalObject()
        {
            Errors.ThrowIfError(LibChakraCore.JsGetGlobalObject(out JavaScriptValueSafeHandle globalObject));
            globalObject.NativeFunctionSource = nameof(LibChakraCore.JsGetGlobalObject);
            if (globalObject != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(globalObject, out uint valueRefCount));
			}
            return globalObject;
        }

        public JavaScriptValueSafeHandle JsCreateObject()
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateObject(out JavaScriptValueSafeHandle @object));
            @object.NativeFunctionSource = nameof(LibChakraCore.JsCreateObject);
            if (@object != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(@object, out uint valueRefCount));
			}
            return @object;
        }

        public JavaScriptValueSafeHandle JsCreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateExternalObject(data, finalizeCallback, out JavaScriptValueSafeHandle @object));
            @object.NativeFunctionSource = nameof(LibChakraCore.JsCreateExternalObject);
            if (@object != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(@object, out uint valueRefCount));
			}
            return @object;
        }

        public JavaScriptValueSafeHandle JsConvertValueToObject(JavaScriptValueSafeHandle value)
        {
            Errors.ThrowIfError(LibChakraCore.JsConvertValueToObject(value, out JavaScriptValueSafeHandle @object));
            @object.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToObject);
            if (@object != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(@object, out uint valueRefCount));
			}
            return @object;
        }

        public JavaScriptValueSafeHandle JsGetPrototype(JavaScriptValueSafeHandle @object)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetPrototype(@object, out JavaScriptValueSafeHandle prototypeObject));
            prototypeObject.NativeFunctionSource = nameof(LibChakraCore.JsGetPrototype);
            if (prototypeObject != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(prototypeObject, out uint valueRefCount));
			}
            return prototypeObject;
        }

        public void JsSetPrototype(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle prototypeObject)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetPrototype(@object, prototypeObject));
        }

        public bool JsInstanceOf(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle constructor)
        {
            Errors.ThrowIfError(LibChakraCore.JsInstanceOf(@object, constructor, out bool result));
            return result;
        }

        public bool JsGetExtensionAllowed(JavaScriptValueSafeHandle @object)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetExtensionAllowed(@object, out bool value));
            return value;
        }

        public void JsPreventExtension(JavaScriptValueSafeHandle @object)
        {
            Errors.ThrowIfError(LibChakraCore.JsPreventExtension(@object));
        }

        public JavaScriptValueSafeHandle JsGetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetProperty(@object, propertyId, out JavaScriptValueSafeHandle value));
            value.NativeFunctionSource = nameof(LibChakraCore.JsGetProperty);
            if (value != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(value, out uint valueRefCount));
			}
            return value;
        }

        public JavaScriptValueSafeHandle JsGetOwnPropertyDescriptor(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetOwnPropertyDescriptor(@object, propertyId, out JavaScriptValueSafeHandle propertyDescriptor));
            propertyDescriptor.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertyDescriptor);
            if (propertyDescriptor != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertyDescriptor, out uint valueRefCount));
			}
            return propertyDescriptor;
        }

        public JavaScriptValueSafeHandle JsGetOwnPropertyNames(JavaScriptValueSafeHandle @object)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetOwnPropertyNames(@object, out JavaScriptValueSafeHandle propertyNames));
            propertyNames.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertyNames);
            if (propertyNames != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertyNames, out uint valueRefCount));
			}
            return propertyNames;
        }

        public void JsSetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle value, bool useStrictRules)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetProperty(@object, propertyId, value, useStrictRules));
        }

        public bool JsHasProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId)
        {
            Errors.ThrowIfError(LibChakraCore.JsHasProperty(@object, propertyId, out bool hasProperty));
            return hasProperty;
        }

        public JavaScriptValueSafeHandle JsDeleteProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, bool useStrictRules)
        {
            Errors.ThrowIfError(LibChakraCore.JsDeleteProperty(@object, propertyId, useStrictRules, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsDeleteProperty);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public bool JsDefineProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle propertyDescriptor)
        {
            Errors.ThrowIfError(LibChakraCore.JsDefineProperty(@object, propertyId, propertyDescriptor, out bool result));
            return result;
        }

        public bool JsHasIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index)
        {
            Errors.ThrowIfError(LibChakraCore.JsHasIndexedProperty(@object, index, out bool result));
            return result;
        }

        public JavaScriptValueSafeHandle JsGetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetIndexedProperty(@object, index, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsGetIndexedProperty);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
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
            Errors.ThrowIfError(LibChakraCore.JsHasIndexedPropertiesExternalData(@object, out bool value));
            return value;
        }

        public IntPtr JsGetIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, out JavaScriptTypedArrayType arrayType, out uint elementLength)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetIndexedPropertiesExternalData(@object, out IntPtr data, out arrayType, out elementLength));
            return data;
        }

        public void JsSetIndexedPropertiesToExternalData(JavaScriptValueSafeHandle @object, IntPtr data, JavaScriptTypedArrayType arrayType, uint elementLength)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetIndexedPropertiesToExternalData(@object, data, arrayType, elementLength));
        }

        public bool JsEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2)
        {
            Errors.ThrowIfError(LibChakraCore.JsEquals(object1, object2, out bool result));
            return result;
        }

        public bool JsStrictEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2)
        {
            Errors.ThrowIfError(LibChakraCore.JsStrictEquals(object1, object2, out bool result));
            return result;
        }

        public bool JsHasExternalData(JavaScriptValueSafeHandle @object)
        {
            Errors.ThrowIfError(LibChakraCore.JsHasExternalData(@object, out bool value));
            return value;
        }

        public IntPtr JsGetExternalData(JavaScriptValueSafeHandle @object)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetExternalData(@object, out IntPtr externalData));
            return externalData;
        }

        public void JsSetExternalData(JavaScriptValueSafeHandle @object, IntPtr externalData)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetExternalData(@object, externalData));
        }

        public JavaScriptValueSafeHandle JsCreateArray(uint length)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateArray(length, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCreateArray);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsCreateArrayBuffer(uint byteLength)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateArrayBuffer(byteLength, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCreateArrayBuffer);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsCreateExternalArrayBuffer(IntPtr data, uint byteLength, JavaScriptObjectFinalizeCallback finalizeCallback, IntPtr callbackState)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateExternalArrayBuffer(data, byteLength, finalizeCallback, callbackState, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCreateExternalArrayBuffer);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsCreateTypedArray(JavaScriptTypedArrayType arrayType, JavaScriptValueSafeHandle baseArray, uint byteOffset, uint elementLength)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateTypedArray(arrayType, baseArray, byteOffset, elementLength, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCreateTypedArray);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsCreateDataView(JavaScriptValueSafeHandle arrayBuffer, uint byteOffset, uint byteLength)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateDataView(arrayBuffer, byteOffset, byteLength, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCreateDataView);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptTypedArrayType JsGetTypedArrayInfo(JavaScriptValueSafeHandle typedArray, out JavaScriptValueSafeHandle arrayBuffer, out uint byteOffset, out uint byteLength)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetTypedArrayInfo(typedArray, out JavaScriptTypedArrayType arrayType, out arrayBuffer, out byteOffset, out byteLength));
            arrayBuffer.NativeFunctionSource = nameof(LibChakraCore.JsGetTypedArrayInfo);
            if (arrayBuffer != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(arrayBuffer, out uint valueRefCount));
			}
            return arrayType;
        }

        public IntPtr JsGetArrayBufferStorage(JavaScriptValueSafeHandle arrayBuffer, out uint bufferLength)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetArrayBufferStorage(arrayBuffer, out IntPtr buffer, out bufferLength));
            return buffer;
        }

        public IntPtr JsGetTypedArrayStorage(JavaScriptValueSafeHandle typedArray, out uint bufferLength, out JavaScriptTypedArrayType arrayType, out int elementSize)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetTypedArrayStorage(typedArray, out IntPtr buffer, out bufferLength, out arrayType, out elementSize));
            return buffer;
        }

        public IntPtr JsGetDataViewStorage(JavaScriptValueSafeHandle dataView, out uint bufferLength)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetDataViewStorage(dataView, out IntPtr buffer, out bufferLength));
            return buffer;
        }

        public JavaScriptValueSafeHandle JsCallFunction(JavaScriptValueSafeHandle function, IntPtr[] arguments, ushort argumentCount)
        {
            Errors.ThrowIfError(LibChakraCore.JsCallFunction(function, arguments, argumentCount, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCallFunction);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsConstructObject(JavaScriptValueSafeHandle function, IntPtr[] arguments, ushort argumentCount)
        {
            Errors.ThrowIfError(LibChakraCore.JsConstructObject(function, arguments, argumentCount, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsConstructObject);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsCreateFunction(JavaScriptNativeFunction nativeFunction, IntPtr callbackState)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateFunction(nativeFunction, callbackState, out JavaScriptValueSafeHandle function));
            function.NativeFunctionSource = nameof(LibChakraCore.JsCreateFunction);
            if (function != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(function, out uint valueRefCount));
			}
            return function;
        }

        public JavaScriptValueSafeHandle JsCreateNamedFunction(JavaScriptValueSafeHandle name, JavaScriptNativeFunction nativeFunction, IntPtr callbackState)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateNamedFunction(name, nativeFunction, callbackState, out JavaScriptValueSafeHandle function));
            function.NativeFunctionSource = nameof(LibChakraCore.JsCreateNamedFunction);
            if (function != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(function, out uint valueRefCount));
			}
            return function;
        }

        public JavaScriptValueSafeHandle JsCreateError(JavaScriptValueSafeHandle message)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateError(message, out JavaScriptValueSafeHandle error));
            error.NativeFunctionSource = nameof(LibChakraCore.JsCreateError);
            if (error != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(error, out uint valueRefCount));
			}
            return error;
        }

        public JavaScriptValueSafeHandle JsCreateRangeError(JavaScriptValueSafeHandle message)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateRangeError(message, out JavaScriptValueSafeHandle error));
            error.NativeFunctionSource = nameof(LibChakraCore.JsCreateRangeError);
            if (error != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(error, out uint valueRefCount));
			}
            return error;
        }

        public JavaScriptValueSafeHandle JsCreateReferenceError(JavaScriptValueSafeHandle message)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateReferenceError(message, out JavaScriptValueSafeHandle error));
            error.NativeFunctionSource = nameof(LibChakraCore.JsCreateReferenceError);
            if (error != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(error, out uint valueRefCount));
			}
            return error;
        }

        public JavaScriptValueSafeHandle JsCreateSyntaxError(JavaScriptValueSafeHandle message)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateSyntaxError(message, out JavaScriptValueSafeHandle error));
            error.NativeFunctionSource = nameof(LibChakraCore.JsCreateSyntaxError);
            if (error != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(error, out uint valueRefCount));
			}
            return error;
        }

        public JavaScriptValueSafeHandle JsCreateTypeError(JavaScriptValueSafeHandle message)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateTypeError(message, out JavaScriptValueSafeHandle error));
            error.NativeFunctionSource = nameof(LibChakraCore.JsCreateTypeError);
            if (error != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(error, out uint valueRefCount));
			}
            return error;
        }

        public JavaScriptValueSafeHandle JsCreateURIError(JavaScriptValueSafeHandle message)
        {
            Errors.ThrowIfError(LibChakraCore.JsCreateURIError(message, out JavaScriptValueSafeHandle error));
            error.NativeFunctionSource = nameof(LibChakraCore.JsCreateURIError);
            if (error != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(error, out uint valueRefCount));
			}
            return error;
        }

        public bool JsHasException()
        {
            Errors.ThrowIfError(LibChakraCore.JsHasException(out bool hasException));
            return hasException;
        }

        public JavaScriptValueSafeHandle JsGetAndClearException()
        {
            Errors.ThrowIfError(LibChakraCore.JsGetAndClearException(out JavaScriptValueSafeHandle exception));
            exception.NativeFunctionSource = nameof(LibChakraCore.JsGetAndClearException);
            if (exception != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(exception, out uint valueRefCount));
			}
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
            Errors.ThrowIfError(LibChakraCore.JsIsRuntimeExecutionDisabled(runtime, out bool isDisabled));
            return isDisabled;
        }

        public void JsSetPromiseContinuationCallback(JavaScriptPromiseContinuationCallback promiseContinuationCallback, IntPtr callbackState)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetPromiseContinuationCallback(promiseContinuationCallback, callbackState));
        }

        public void JsDiagStartDebugging(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagDebugEventCallback debugEventCallback, IntPtr callbackState)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagStartDebugging(runtimeHandle, debugEventCallback, callbackState));
        }

        public IntPtr JsDiagStopDebugging(JavaScriptRuntimeSafeHandle runtimeHandle)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagStopDebugging(runtimeHandle, out IntPtr callbackState));
            return callbackState;
        }

        public void JsDiagRequestAsyncBreak(JavaScriptRuntimeSafeHandle runtimeHandle)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagRequestAsyncBreak(runtimeHandle));
        }

        public JavaScriptValueSafeHandle JsDiagGetBreakpoints()
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagGetBreakpoints(out JavaScriptValueSafeHandle breakpoints));
            breakpoints.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetBreakpoints);
            if (breakpoints != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(breakpoints, out uint valueRefCount));
			}
            return breakpoints;
        }

        public JavaScriptValueSafeHandle JsDiagSetBreakpoint(uint scriptId, uint lineNumber, uint columnNumber)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagSetBreakpoint(scriptId, lineNumber, columnNumber, out JavaScriptValueSafeHandle breakpoint));
            breakpoint.NativeFunctionSource = nameof(LibChakraCore.JsDiagSetBreakpoint);
            if (breakpoint != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(breakpoint, out uint valueRefCount));
			}
            return breakpoint;
        }

        public void JsDiagRemoveBreakpoint(uint breakpointId)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagRemoveBreakpoint(breakpointId));
        }

        public void JsDiagSetBreakOnException(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagBreakOnExceptionAttributes exceptionAttributes)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagSetBreakOnException(runtimeHandle, exceptionAttributes));
        }

        public JavaScriptDiagBreakOnExceptionAttributes JsDiagGetBreakOnException(JavaScriptRuntimeSafeHandle runtimeHandle)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagGetBreakOnException(runtimeHandle, out JavaScriptDiagBreakOnExceptionAttributes exceptionAttributes));
            return exceptionAttributes;
        }

        public void JsDiagSetStepType(JavaScriptDiagStepType stepType)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagSetStepType(stepType));
        }

        public JavaScriptValueSafeHandle JsDiagGetScripts()
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagGetScripts(out JavaScriptValueSafeHandle scriptsArray));
            scriptsArray.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetScripts);
            if (scriptsArray != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(scriptsArray, out uint valueRefCount));
			}
            return scriptsArray;
        }

        public JavaScriptValueSafeHandle JsDiagGetSource(uint scriptId)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagGetSource(scriptId, out JavaScriptValueSafeHandle source));
            source.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetSource);
            if (source != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(source, out uint valueRefCount));
			}
            return source;
        }

        public JavaScriptValueSafeHandle JsDiagGetFunctionPosition(JavaScriptValueSafeHandle function)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagGetFunctionPosition(function, out JavaScriptValueSafeHandle functionPosition));
            functionPosition.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetFunctionPosition);
            if (functionPosition != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(functionPosition, out uint valueRefCount));
			}
            return functionPosition;
        }

        public JavaScriptValueSafeHandle JsDiagGetStackTrace()
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagGetStackTrace(out JavaScriptValueSafeHandle stackTrace));
            stackTrace.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetStackTrace);
            if (stackTrace != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(stackTrace, out uint valueRefCount));
			}
            return stackTrace;
        }

        public JavaScriptValueSafeHandle JsDiagGetStackProperties(uint stackFrameIndex)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagGetStackProperties(stackFrameIndex, out JavaScriptValueSafeHandle properties));
            properties.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetStackProperties);
            if (properties != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(properties, out uint valueRefCount));
			}
            return properties;
        }

        public JavaScriptValueSafeHandle JsDiagGetProperties(uint objectHandle, uint fromCount, uint totalCount)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagGetProperties(objectHandle, fromCount, totalCount, out JavaScriptValueSafeHandle propertiesObject));
            propertiesObject.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetProperties);
            if (propertiesObject != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertiesObject, out uint valueRefCount));
			}
            return propertiesObject;
        }

        public JavaScriptValueSafeHandle JsDiagGetObjectFromHandle(uint objectHandle)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagGetObjectFromHandle(objectHandle, out JavaScriptValueSafeHandle handleObject));
            handleObject.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetObjectFromHandle);
            if (handleObject != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(handleObject, out uint valueRefCount));
			}
            return handleObject;
        }

        public JavaScriptValueSafeHandle JsDiagEvaluate(JavaScriptValueSafeHandle expression, uint stackFrameIndex, JavaScriptParseScriptAttributes parseAttributes, bool forceSetValueProp)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagEvaluate(expression, stackFrameIndex, parseAttributes, forceSetValueProp, out JavaScriptValueSafeHandle evalResult));
            evalResult.NativeFunctionSource = nameof(LibChakraCore.JsDiagEvaluate);
            if (evalResult != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(evalResult, out uint valueRefCount));
			}
            return evalResult;
        }

    }
}