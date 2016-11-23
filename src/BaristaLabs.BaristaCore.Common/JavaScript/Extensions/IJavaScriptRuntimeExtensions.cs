namespace BaristaLabs.BaristaCore.JavaScript.Extensions
{
    using Internal;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public static class IJavaScriptRuntimeExtensions
    {
        private const string EvalSourceUrl = "[eval code]";

        public static string GetStringUtf8(this IJavaScriptRuntime jsrt, JavaScriptValueSafeHandle stringHandle, bool autoConvert = false)
        {
            bool stringHandleWasCreated = false;
            if (stringHandle == null || stringHandle == JavaScriptValueSafeHandle.Invalid)
                throw new ArgumentNullException(nameof(stringHandle));

            //If Auto Convert is specified, ensure that the type is a string, otherwise convert it.
            if (autoConvert)
            {
                JavaScriptValueType handleValueType;
                Errors.ThrowIfError(jsrt.JsGetValueType(stringHandle, out handleValueType));

                if (handleValueType != JavaScriptValueType.String)
                {
                    JavaScriptValueSafeHandle convertedToStringHandle;
                    Errors.ThrowIfError(jsrt.JsConvertValueToString(stringHandle, out convertedToStringHandle));

                    stringHandle = convertedToStringHandle;
                    stringHandleWasCreated = true;
                }
            }

            //Get the size
            UIntPtr size;
            Errors.ThrowIfError(jsrt.JsCopyStringUtf8(stringHandle, null, UIntPtr.Zero, out size));
 
            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            UIntPtr written;
            Errors.ThrowIfError(jsrt.JsCopyStringUtf8(stringHandle, result, new UIntPtr((uint)result.Length), out written));

            var strResult = Encoding.UTF8.GetString(result, 0, result.Length);
            if (stringHandleWasCreated)
                stringHandle.Dispose();

            return strResult;
        }

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
        /// <param name="jsrt"></param>
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
                Errors.ThrowIfError(jsrt.JsCreateStringUtf8(EvalSourceUrl, new UIntPtr((uint)EvalSourceUrl.Length), out sourceUrl));
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
                if (createdSourceUrl)
                    sourceUrl.Dispose();

                scriptHandle.Dispose();

                //Release pinned string.
                Marshal.FreeHGlobal(ptrScript);
            }
        }

        /// <summary>
        /// Executes a script contained in the specified script source.
        /// </summary>
        /// <param name="jsrt"></param>
        /// <param name="scriptSource"></param>
        /// <param name="attributes"></param>
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
        ///     This extension method exists for simplicity.
        /// </remarks>
        /// <param name="jsrt"></param>
        /// <param name="script"></param>
        /// <param name="sourceContext"></param>
        /// <param name="sourceUrl"></param>
        /// <param name="parseAttributes"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static JavaScriptErrorCode JsRunScript(this IJavaScriptRuntime jsrt, string script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
        {
            var ptrScript = Marshal.StringToHGlobalAnsi(script);
            var createdSourceUrl = false;

            JavaScriptValueSafeHandle scriptHandle;
            Errors.ThrowIfError(jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

            if (sourceContext == default(JavaScriptSourceContext))
            {
                sourceContext = new JavaScriptSourceContext();
            }

            if (sourceUrl == null)
            {
                Errors.ThrowIfError(jsrt.JsCreateStringUtf8(EvalSourceUrl, new UIntPtr((uint)EvalSourceUrl.Length), out sourceUrl));
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
                if (createdSourceUrl)
                    sourceUrl.Dispose();

                scriptHandle.Dispose();

                //Release pinned string.
                Marshal.FreeHGlobal(ptrScript);
            }
        }

        /// <summary>
        /// Executes a script, automatically storing the script value within an ExternalArrayBuffer.
        /// </summary>
        /// <remarks>
        ///     This extension method exists for simplicity.
        /// </remarks>
        /// <param name="jsrt"></param>
        /// <param name="script"></param>
        /// <param name="sourceContext"></param>
        /// <param name="sourceUrl"></param>
        /// <param name="parseAttributes"></param>
        /// <param name="result"></param>
        /// <returns>A JavaScriptValueSafeHandle containing the result.</returns>
        public static JavaScriptValueSafeHandle JsRunScript(this IJavaScriptRuntime jsrt, string script, JavaScriptSourceContext sourceContext = default(JavaScriptSourceContext), string sourceUrl = "[eval code]", JavaScriptParseScriptAttributes parseAttributes = JavaScriptParseScriptAttributes.None)
        {
            var ptrScript = Marshal.StringToHGlobalAnsi(script);

            JavaScriptObjectFinalizeCallback callback = (IntPtr ptr) =>
            {
                //Release pinned string.
                Marshal.FreeHGlobal(ptrScript);
            };

            JavaScriptValueSafeHandle scriptHandle;
            Errors.ThrowIfError(jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, callback, IntPtr.Zero, out scriptHandle));

            if (sourceContext == default(JavaScriptSourceContext))
            {
                sourceContext = new JavaScriptSourceContext();
            }

            JavaScriptValueSafeHandle sourceUrlHandle;
            if (string.IsNullOrWhiteSpace(sourceUrl))
            {
                sourceUrl = "[eval code]";
            }

            Errors.ThrowIfError(jsrt.JsCreateStringUtf8(EvalSourceUrl, new UIntPtr((uint)EvalSourceUrl.Length), out sourceUrlHandle));

            if (parseAttributes == default(JavaScriptParseScriptAttributes))
            {
                parseAttributes = JavaScriptParseScriptAttributes.None;
            }

            JavaScriptValueSafeHandle result;
            try
            {
                Errors.ThrowIfError(jsrt.JsRun(scriptHandle, sourceContext, sourceUrlHandle, parseAttributes, out result));
            }
            finally
            {
                //Release variables created during this operation.
                sourceUrlHandle.Dispose();
                scriptHandle.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Serializes a parsed script to a buffer that can be reused.
        /// </summary>
        /// <param name="jsrt"></param>
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
                scriptHandle.Dispose();

                //Release pinned string.
                Marshal.FreeHGlobal(ptrScript);
            }
        }
    }
}
