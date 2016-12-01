namespace BaristaLabs.BaristaCore.JavaScript.Extensions
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public static class IJavaScriptEngineExtensions
    {
        private const string EvalSourceUrl = "[eval code]";

        public static string GetStringUtf8(this IJavaScriptEngine jsrt, JavaScriptValueSafeHandle stringHandle, bool autoConvert = false)
        {
            bool stringHandleWasCreated = false;
            if (stringHandle == null || stringHandle == JavaScriptValueSafeHandle.Invalid)
                throw new ArgumentNullException(nameof(stringHandle));

            //If Auto Convert is specified, ensure that the type is a string, otherwise convert it.
            if (autoConvert)
            {
                var handleValueType = jsrt.JsGetValueType(stringHandle);

                if (handleValueType != JavaScriptValueType.String)
                {
                    var convertedToStringHandle = jsrt.JsConvertValueToString(stringHandle);

                    stringHandle = convertedToStringHandle;
                    stringHandleWasCreated = true;
                }
            }

            //Get the size
            var size = jsrt.JsCopyStringUtf8(stringHandle, null, UIntPtr.Zero);
 
            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            var written = jsrt.JsCopyStringUtf8(stringHandle, result, new UIntPtr((uint)result.Length));

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
        public static async Task<JavaScriptValueSafeHandle> JsParseScriptAsync(this IJavaScriptEngine jsrt, IScriptSource scriptSource, JavaScriptParseScriptAttributes attributes)
        {
            if (scriptSource == null)
                throw new ArgumentNullException(nameof(scriptSource));

            var script = await scriptSource.GetScriptAsync();

            var sourceUrl = jsrt.JsCreateStringUtf8(scriptSource.Description, new UIntPtr((uint)scriptSource.Description.Length));

            try
            {
                return JsParseScript(jsrt, script, JavaScriptSourceContext.FromInt(scriptSource.Cookie), sourceUrl, attributes);
            }
            finally
            {
                sourceUrl.Dispose();
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
        public static JavaScriptValueSafeHandle JsParseScript(this IJavaScriptEngine jsrt, string script, JavaScriptSourceContext sourceContext = default(JavaScriptSourceContext), JavaScriptValueSafeHandle sourceUrl = null, JavaScriptParseScriptAttributes parseAttributes = JavaScriptParseScriptAttributes.None)
        {
            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            bool createdSourceUrl = false;

            var scriptHandle = jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero);

            if (sourceContext == default(JavaScriptSourceContext))
            {
                sourceContext = JavaScriptSourceContext.None;
            }

            if (sourceUrl == null)
            {
                sourceUrl = jsrt.JsCreateStringUtf8(EvalSourceUrl, new UIntPtr((uint)EvalSourceUrl.Length));
                createdSourceUrl = true;
            }

            if (parseAttributes == default(JavaScriptParseScriptAttributes))
            {
                parseAttributes = JavaScriptParseScriptAttributes.None;
            }

            try
            {
                return jsrt.JsParse(scriptHandle, sourceContext, sourceUrl, parseAttributes);
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
        public static async Task<JavaScriptValueSafeHandle> JsRunScriptAsync(this IJavaScriptEngine jsrt, IScriptSource scriptSource, JavaScriptParseScriptAttributes attributes)
        {
            if (scriptSource == null)
                throw new ArgumentNullException(nameof(scriptSource));

            var script = await scriptSource.GetScriptAsync();

            var sourceUrl = jsrt.JsCreateStringUtf8(scriptSource.Description, new UIntPtr((uint)scriptSource.Description.Length));

            try
            {
                return JsRunScript(jsrt, script, JavaScriptSourceContext.FromInt(scriptSource.Cookie), sourceUrl, attributes);
            }
            finally
            {
                sourceUrl.Dispose();
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
        public static JavaScriptValueSafeHandle JsRunScript(this IJavaScriptEngine jsrt, string script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes)
        {
            var ptrScript = Marshal.StringToHGlobalAnsi(script);
            var createdSourceUrl = false;

            var scriptHandle = jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero);

            if (sourceContext == default(JavaScriptSourceContext))
            {
                sourceContext = JavaScriptSourceContext.None;
            }

            if (sourceUrl == null)
            {
                sourceUrl = jsrt.JsCreateStringUtf8(EvalSourceUrl, new UIntPtr((uint)EvalSourceUrl.Length));
                createdSourceUrl = true;
            }

            if (parseAttributes == default(JavaScriptParseScriptAttributes))
            {
                parseAttributes = JavaScriptParseScriptAttributes.None;
            }

            try
            {
                return jsrt.JsRun(scriptHandle, sourceContext, sourceUrl, parseAttributes);
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
        public static JavaScriptValueSafeHandle JsRunScript(this IJavaScriptEngine jsrt, string script, JavaScriptSourceContext sourceContext = default(JavaScriptSourceContext), string sourceUrl = "[eval code]", JavaScriptParseScriptAttributes parseAttributes = JavaScriptParseScriptAttributes.None)
        {
            var ptrScript = Marshal.StringToHGlobalAnsi(script);

            JavaScriptObjectFinalizeCallback callback = (IntPtr ptr) =>
            {
                //Release pinned string.
                Marshal.FreeHGlobal(ptrScript);
            };

            var scriptHandle = jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, callback, IntPtr.Zero);

            if (sourceContext == default(JavaScriptSourceContext))
            {
                sourceContext = JavaScriptSourceContext.None;
            }

            JavaScriptValueSafeHandle sourceUrlHandle;
            if (string.IsNullOrWhiteSpace(sourceUrl))
            {
                sourceUrl = "[eval code]";
            }

            sourceUrlHandle = jsrt.JsCreateStringUtf8(EvalSourceUrl, new UIntPtr((uint)EvalSourceUrl.Length));

            if (parseAttributes == default(JavaScriptParseScriptAttributes))
            {
                parseAttributes = JavaScriptParseScriptAttributes.None;
            }

            try
            {
                return jsrt.JsRun(scriptHandle, sourceContext, sourceUrlHandle, parseAttributes);
            }
            finally
            {
                //Release variables created during this operation.
                sourceUrlHandle.Dispose();
                scriptHandle.Dispose();
            }
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
        public static void JsSerializeScript(this IJavaScriptEngine jsrt, string script, byte[] buffer, ref ulong bufferSize, JavaScriptParseScriptAttributes parseAttributes)
        {
            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);

            using (var scriptHandle = jsrt.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero))
            {
                try
                {
                    jsrt.JsSerialize(scriptHandle, buffer, ref bufferSize, parseAttributes);
                }
                finally
                {
                    //Release pinned string.
                    Marshal.FreeHGlobal(ptrScript);
                }
            }
        }
    }
}
