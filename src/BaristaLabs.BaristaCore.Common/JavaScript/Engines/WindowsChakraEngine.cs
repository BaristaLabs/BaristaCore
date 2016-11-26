namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

	using System;
	using System.Runtime.InteropServices;

	public sealed class WindowsChakraEngine : ChakraEngineBase, ICommonWindowsJavaScriptEngine
	{
		public JavaScriptValueSafeHandle JsParseScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsParseScript(script, sourceContext, sourceUrl, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsParseScript);
			if (result != JavaScriptValueSafeHandle.Invalid)
			{
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
			return result;
		}

		public JavaScriptValueSafeHandle JsParseScriptWithAttributes(string script, JavaScriptSourceContext sourceContext, string sourceUrl, JavaScriptParseScriptAttributes parseAttributes)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsParseScriptWithAttributes(script, sourceContext, sourceUrl, parseAttributes, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsParseScriptWithAttributes);
			if (result != JavaScriptValueSafeHandle.Invalid)
			{
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
			return result;
		}

		public JavaScriptValueSafeHandle JsRunScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsRunScript(script, sourceContext, sourceUrl, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsRunScript);
			if (result != JavaScriptValueSafeHandle.Invalid)
			{
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
			return result;
		}

		public JavaScriptValueSafeHandle JsExperimentalApiRunModule(string script, JavaScriptSourceContext sourceContext, string sourceUrl)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsExperimentalApiRunModule(script, sourceContext, sourceUrl, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsExperimentalApiRunModule);
			if (result != JavaScriptValueSafeHandle.Invalid)
			{
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
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
			if (result != JavaScriptValueSafeHandle.Invalid)
			{
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
			return result;
		}

		public JavaScriptValueSafeHandle JsRunSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback, JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsRunSerializedScriptWithCallback(scriptLoadCallback, scriptUnloadCallback, buffer, sourceContext, sourceUrl, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerializedScriptWithCallback);
			if (result != JavaScriptValueSafeHandle.Invalid)
			{
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
			return result;
		}

		public JavaScriptValueSafeHandle JsParseSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsParseSerializedScript(script, buffer, sourceContext, sourceUrl, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerializedScript);
			if (result != JavaScriptValueSafeHandle.Invalid)
			{
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
			return result;
		}

		public JavaScriptValueSafeHandle JsRunSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl)
		{
			JavaScriptValueSafeHandle result;
			Errors.ThrowIfError(LibChakraCore.JsRunSerializedScript(script, buffer, sourceContext, sourceUrl, out result));
			result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerializedScript);
			if (result != JavaScriptValueSafeHandle.Invalid)
			{
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
			return result;
		}

		public JavaScriptPropertyIdSafeHandle JsGetPropertyIdFromName(string name)
		{
			JavaScriptPropertyIdSafeHandle propertyId;
			Errors.ThrowIfError(LibChakraCore.JsGetPropertyIdFromName(name, out propertyId));
			propertyId.NativeFunctionSource = nameof(LibChakraCore.JsGetPropertyIdFromName);
			if (propertyId != JavaScriptPropertyIdSafeHandle.Invalid)
			{
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertyId, out valueRefCount));
			}
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
			if (value != JavaScriptValueSafeHandle.Invalid)
			{
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(value, out valueRefCount));
			}
			return value;
		}

		public IntPtr JsStringToPointer(JavaScriptValueSafeHandle value, out ulong stringLength)
		{
			IntPtr stringValue;
			Errors.ThrowIfError(LibChakraCore.JsStringToPointer(value, out stringValue, out stringLength));
			return stringValue;
		}

	}
}
