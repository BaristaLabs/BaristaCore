namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

	using System;
	using System.Runtime.InteropServices;

    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class WindowsChakraEngine : ChakraEngineBase, ICommonWindowsJavaScriptEngine, IDebugWindowsJavaScriptEngine
    {
        public JavaScriptValueSafeHandle JsParseScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl)
        {
            Errors.ThrowIfError(LibChakraCore.JsParseScript(script, sourceContext, sourceUrl, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsParseScript);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsParseScriptWithAttributes(string script, JavaScriptSourceContext sourceContext, string sourceUrl, JavaScriptParseScriptAttributes parseAttributes)
        {
            Errors.ThrowIfError(LibChakraCore.JsParseScriptWithAttributes(script, sourceContext, sourceUrl, parseAttributes, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsParseScriptWithAttributes);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsRunScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl)
        {
            Errors.ThrowIfError(LibChakraCore.JsRunScript(script, sourceContext, sourceUrl, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsRunScript);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsExperimentalApiRunModule(string script, JavaScriptSourceContext sourceContext, string sourceUrl)
        {
            Errors.ThrowIfError(LibChakraCore.JsExperimentalApiRunModule(script, sourceContext, sourceUrl, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsExperimentalApiRunModule);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public void JsSerializeScript(string script, byte[] buffer, ref uint bufferSize)
        {
            Errors.ThrowIfError(LibChakraCore.JsSerializeScript(script, buffer, ref bufferSize));
        }

        public JavaScriptValueSafeHandle JsParseSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
        {
            Errors.ThrowIfError(LibChakraCore.JsParseSerializedScriptWithCallback(scriptLoadCallback, scriptUnloadCallback, buffer, sourceContext, sourceUrl, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerializedScriptWithCallback);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsRunSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
        {
            Errors.ThrowIfError(LibChakraCore.JsRunSerializedScriptWithCallback(scriptLoadCallback, scriptUnloadCallback, buffer, sourceContext, sourceUrl, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerializedScriptWithCallback);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsParseSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
        {
            Errors.ThrowIfError(LibChakraCore.JsParseSerializedScript(script, buffer, sourceContext, sourceUrl, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerializedScript);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsRunSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
        {
            Errors.ThrowIfError(LibChakraCore.JsRunSerializedScript(script, buffer, sourceContext, sourceUrl, out JavaScriptValueSafeHandle result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerializedScript);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out uint valueRefCount));
			}
            return result;
        }

        public JavaScriptPropertyIdSafeHandle JsGetPropertyIdFromName(string name)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetPropertyIdFromName(name, out JavaScriptPropertyIdSafeHandle propertyId));
            propertyId.NativeFunctionSource = nameof(LibChakraCore.JsGetPropertyIdFromName);
            if (propertyId != JavaScriptPropertyIdSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertyId, out uint valueRefCount));
			}
            return propertyId;
        }

        public IntPtr JsGetPropertyNameFromId(JavaScriptPropertyIdSafeHandle propertyId)
        {
            Errors.ThrowIfError(LibChakraCore.JsGetPropertyNameFromId(propertyId, out IntPtr name));
            return name;
        }

        public JavaScriptValueSafeHandle JsPointerToString(string stringValue, ulong stringLength)
        {
            Errors.ThrowIfError(LibChakraCore.JsPointerToString(stringValue, stringLength, out JavaScriptValueSafeHandle value));
            value.NativeFunctionSource = nameof(LibChakraCore.JsPointerToString);
            if (value != JavaScriptValueSafeHandle.Invalid)
            {
				Errors.ThrowIfError(LibChakraCore.JsAddRef(value, out uint valueRefCount));
			}
            return value;
        }

        public IntPtr JsStringToPointer(JavaScriptValueSafeHandle value, out ulong stringLength)
        {
            Errors.ThrowIfError(LibChakraCore.JsStringToPointer(value, out IntPtr stringValue, out stringLength));
            return stringValue;
        }

    }
}