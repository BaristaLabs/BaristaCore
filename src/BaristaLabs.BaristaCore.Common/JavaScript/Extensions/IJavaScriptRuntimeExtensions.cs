namespace BaristaLabs.BaristaCore.JavaScript.Extensions
{
    using Internal;
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    internal static class IChakraApiExtensions
    {
        /// <summary>
        /// Parses a script and returns a function representing a script contained in the specified script source.
        /// </summary>
        /// <param name="jsrt"></param>
        /// <param name="scriptSource"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static async Task<JavaScriptValueSafeHandle> JsParseScriptAsync(this IJavaScriptRuntime jsrt, IScriptSource scriptSource, JavaScriptParseScriptAttributes attributes)
        {
            if (scriptSource == null)
                throw new ArgumentNullException(nameof(scriptSource));

            var script = await scriptSource.GetScriptAsync();

            JavaScriptValueSafeHandle sourceUrl;
            Errors.ThrowIfError(jsrt.JsCreateStringUtf8(scriptSource.Description, new UIntPtr((uint)scriptSource.Description.Length), out sourceUrl));

            try
            {
                JavaScriptValueSafeHandle result;
                Errors.ThrowIfError(JsParseScript(jsrt, script, JavaScriptSourceContext.FromInt(scriptSource.Cookie), sourceUrl, attributes, out result));
                return result;
            }
            finally
            {
                uint releaseCount;
                Errors.ThrowIfError(jsrt.JsReleaseValue(sourceUrl, out releaseCount));
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
        public static JavaScriptErrorCode JsParseScript(this IJavaScriptRuntime jsrt, string script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
        {
            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            bool createdSourceUrl = false;

            JavaScriptValueSafeHandle scriptHandle;
            Errors.ThrowIfError(jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

            if (sourceContext == default(JavaScriptSourceContext))
            {
                sourceContext = new JavaScriptSourceContext();
            }

            if (sourceUrl == null)
            {
                string strSourceUrl = "[eval code]";
                Errors.ThrowIfError(jsrt.JsCreateStringUtf8(strSourceUrl, new UIntPtr((uint)strSourceUrl.Length), out sourceUrl));
                createdSourceUrl = true;
            }

            if (parseAttributes == default(JavaScriptParseScriptAttributes))
            {
                parseAttributes = JavaScriptParseScriptAttributes.None;
            }

            try
            {
                return jsrt.JsParse(scriptHandle, sourceContext, sourceUrl, parseAttributes, out result);
            }
            finally
            {
                //Release variables created during this operation.
                uint releaseCount;

                if (createdSourceUrl)
                    Errors.ThrowIfError(jsrt.JsReleaseValue(sourceUrl, out releaseCount));

                Errors.ThrowIfError(jsrt.JsReleaseValue(scriptHandle, out releaseCount));

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
        public static async Task<JavaScriptValueSafeHandle> JsRunScriptAsync(this IJavaScriptRuntime jsrt, IScriptSource scriptSource, JavaScriptParseScriptAttributes attributes)
        {
            if (scriptSource == null)
                throw new ArgumentNullException(nameof(scriptSource));

            var script = await scriptSource.GetScriptAsync();

            JavaScriptValueSafeHandle sourceUrl;
            Errors.ThrowIfError(jsrt.JsCreateStringUtf8(scriptSource.Description, new UIntPtr((uint)scriptSource.Description.Length), out sourceUrl));

            try
            {
                JavaScriptValueSafeHandle result;
                Errors.ThrowIfError(JsRunScript(jsrt, script, JavaScriptSourceContext.FromInt(scriptSource.Cookie), sourceUrl, attributes, out result));
                return result;
            }
            finally
            {
                uint releaseCount;
                Errors.ThrowIfError(jsrt.JsReleaseValue(sourceUrl, out releaseCount));
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
        public static JavaScriptErrorCode JsRunScript(this IJavaScriptRuntime jsrt, string script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
        {
            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            bool createdSourceUrl = false;

            JavaScriptValueSafeHandle scriptHandle;
            Errors.ThrowIfError(jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

            if (sourceContext == default(JavaScriptSourceContext))
            {
                sourceContext = new JavaScriptSourceContext();
            }

            if (sourceUrl == null)
            {
                string strSourceUrl = "[eval code]";
                Errors.ThrowIfError(jsrt.JsCreateStringUtf8(strSourceUrl, new UIntPtr((uint)strSourceUrl.Length), out sourceUrl));
                createdSourceUrl = true;
            }

            if (parseAttributes == default(JavaScriptParseScriptAttributes))
            {
                parseAttributes = JavaScriptParseScriptAttributes.None;
            }

            try
            {
                return jsrt.JsRun(scriptHandle, sourceContext, sourceUrl, parseAttributes, out result);
            }
            finally
            {
                //Release variables created during this operation.
                uint releaseCount;

                if (createdSourceUrl)
                    Errors.ThrowIfError(jsrt.JsReleaseValue(sourceUrl, out releaseCount));

                Errors.ThrowIfError(jsrt.JsReleaseValue(scriptHandle, out releaseCount));

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
        public static JavaScriptErrorCode JsSerializeScript(this IJavaScriptRuntime jsrt, string script, byte[] buffer, ref ulong bufferSize, JavaScriptParseScriptAttributes parseAttributes)
        {
            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);

            JavaScriptValueSafeHandle scriptHandle;
            Errors.ThrowIfError(jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

            try
            {
                return jsrt.JsSerialize(scriptHandle, buffer, ref bufferSize, parseAttributes);
            }
            finally
            {
                //Release variables created during this operation.
                uint releaseCount;

                Errors.ThrowIfError(jsrt.JsReleaseValue(scriptHandle, out releaseCount));

                //Release pinned string.
                Marshal.ZeroFreeGlobalAllocAnsi(ptrScript);
            }
        }
    }
}
