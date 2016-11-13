namespace BaristaLabs.BaristaCore.Extensions
{
    using JavaScript;
    using JavaScript.Interfaces;
    using JavaScript.SafeHandles;
    using System;
    using System.Runtime.InteropServices;

    internal static class IChakraApiExtensions
    {
        /// <summary>
        /// Parses a script and returns a function representing a script contained in the specified script source.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="scriptSource"></param>
        /// <param name="attributes"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static JsErrorCode JsParseScript(this IChakraApi api, ScriptSource scriptSource, JsParseScriptAttributes attributes, out JavaScriptValueSafeHandle result)
        {
            if (scriptSource == null)
                throw new ArgumentNullException(nameof(scriptSource));

            JavaScriptValueSafeHandle sourceUrl;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(scriptSource.SourceLocation, new UIntPtr((uint)scriptSource.SourceLocation.Length), out sourceUrl));

            try
            {
                var errorCode = IChakraApiExtensions.JsParseScript(api, scriptSource.SourceText, JavaScriptSourceContext.FromIntPtr(scriptSource.SourceContextId), sourceUrl, attributes, out result);
                return errorCode;
            }
            finally
            {
                uint releaseCount;
                Errors.ThrowIfIs(ChakraApi.Instance.JsReleaseValue(sourceUrl, out releaseCount));
            }
        }

        /// <summary>
        /// Parses a script and returns a function representing the script, automatically storing the script value within an ExternalArrayBuffer.
        /// </summary>
        /// <remarks>
        ///     This extension method exists for simplicity. Often, the
        /// </remarks>
        /// <param name="api"></param>
        /// <param name="script"></param>
        /// <param name="sourceContext"></param>
        /// <param name="sourceUrl"></param>
        /// <param name="parseAttributes"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static JsErrorCode JsParseScript(this IChakraApi api, string script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JsParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
        {
            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            bool createdSourceUrl = false;

            JavaScriptValueSafeHandle scriptHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

            if (sourceContext == default(JavaScriptSourceContext))
            {
                sourceContext = new JavaScriptSourceContext();
            }

            if (sourceUrl == null)
            {
                string strSourceUrl = "[eval code]";
                Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(strSourceUrl, new UIntPtr((uint)strSourceUrl.Length), out sourceUrl));
                createdSourceUrl = true;
            }

            if (parseAttributes == default(JsParseScriptAttributes))
            {
                parseAttributes = JsParseScriptAttributes.JsParseScriptAttributeNone;
            }

            try
            {
                return api.JsParse(scriptHandle, sourceContext, sourceUrl, parseAttributes, out result);
            }
            finally
            {
                //Release variables created during this operation.
                uint releaseCount;

                if (createdSourceUrl)
                    Errors.ThrowIfIs(ChakraApi.Instance.JsReleaseValue(sourceUrl, out releaseCount));

                Errors.ThrowIfIs(ChakraApi.Instance.JsReleaseValue(scriptHandle, out releaseCount));

                //Release pinned string.
                Marshal.ZeroFreeGlobalAllocAnsi(ptrScript);
            }
        }

        /// <summary>
        /// Executes a script contained in the specified script source.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="scriptSource"></param>
        /// <param name="attributes"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static JsErrorCode JsRunScript(this IChakraApi api, ScriptSource scriptSource, JsParseScriptAttributes attributes, out JavaScriptValueSafeHandle result)
        {
            if (scriptSource == null)
                throw new ArgumentNullException(nameof(scriptSource));

            JavaScriptValueSafeHandle sourceUrl;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(scriptSource.SourceLocation, new UIntPtr((uint)scriptSource.SourceLocation.Length), out sourceUrl));

            try
            {
                var errorCode = IChakraApiExtensions.JsRunScript(api, scriptSource.SourceText, JavaScriptSourceContext.FromIntPtr(scriptSource.SourceContextId), sourceUrl, attributes, out result);
                return errorCode;
            }
            finally
            {
                uint releaseCount;
                Errors.ThrowIfIs(ChakraApi.Instance.JsReleaseValue(sourceUrl, out releaseCount));
            }
        }

        /// <summary>
        /// Executes a script, automatically storing the script value within an ExternalArrayBuffer.
        /// </summary>
        /// <remarks>
        ///     This extension method exists for simplicity. Often, the
        /// </remarks>
        /// <param name="api"></param>
        /// <param name="script"></param>
        /// <param name="sourceContext"></param>
        /// <param name="sourceUrl"></param>
        /// <param name="parseAttributes"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static JsErrorCode JsRunScript(this IChakraApi api, string script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JsParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
        {
            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            bool createdSourceUrl = false;

            JavaScriptValueSafeHandle scriptHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

            if (sourceContext == default(JavaScriptSourceContext))
            {
                sourceContext = new JavaScriptSourceContext();
            }

            if (sourceUrl == null)
            {
                string strSourceUrl = "[eval code]";
                Errors.ThrowIfIs(ChakraApi.Instance.JsCreateStringUtf8(strSourceUrl, new UIntPtr((uint)strSourceUrl.Length), out sourceUrl));
                createdSourceUrl = true;
            }

            if (parseAttributes == default(JsParseScriptAttributes))
            {
                parseAttributes = JsParseScriptAttributes.JsParseScriptAttributeNone;
            }

            try
            {
                return api.JsRun(scriptHandle, sourceContext, sourceUrl, parseAttributes, out result);
            }
            finally
            {
                //Release variables created during this operation.
                uint releaseCount;

                if (createdSourceUrl)
                    Errors.ThrowIfIs(ChakraApi.Instance.JsReleaseValue(sourceUrl, out releaseCount));

                Errors.ThrowIfIs(ChakraApi.Instance.JsReleaseValue(scriptHandle, out releaseCount));

                //Release pinned string.
                Marshal.ZeroFreeGlobalAllocAnsi(ptrScript);
            }
        }

        /// <summary>
        /// Serializes a parsed script to a buffer that can be reused.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="script"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferSize"></param>
        /// <param name="parseAttributes"></param>
        /// <returns></returns>
        public static JsErrorCode JsSerializeScript(this IChakraApi api, string script, byte[] buffer, ref ulong bufferSize, JsParseScriptAttributes parseAttributes)
        {
            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);

            JavaScriptValueSafeHandle scriptHandle;
            Errors.ThrowIfIs(ChakraApi.Instance.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

            try
            {
                return api.JsSerialize(scriptHandle, buffer, ref bufferSize, parseAttributes);
            }
            finally
            {
                //Release variables created during this operation.
                uint releaseCount;

                Errors.ThrowIfIs(ChakraApi.Instance.JsReleaseValue(scriptHandle, out releaseCount));

                //Release pinned string.
                Marshal.ZeroFreeGlobalAllocAnsi(ptrScript);
            }
        }
    }
}
