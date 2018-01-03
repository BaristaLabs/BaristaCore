namespace BaristaLabs.BaristaCore.Tests.Extensions
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public static class IJavaScriptEngineExtensions
    {
        private const string EvalSourceUrl = "[eval code]";

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
        public static JavaScriptValueSafeHandle JsRunScript(this IJavaScriptEngine jsrt, string script, JavaScriptSourceContext sourceContext = default(JavaScriptSourceContext), string sourceUrl = null, JavaScriptParseScriptAttributes parseAttributes = JavaScriptParseScriptAttributes.None)
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

            sourceUrlHandle = jsrt.JsCreateString(EvalSourceUrl, (ulong)EvalSourceUrl.Length);

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

        public static string GetStringUtf8(this IJavaScriptEngine jsrt, JavaScriptValueSafeHandle stringHandle, bool autoConvert = false)
        {
            bool stringHandleWasCreated = false;
            if (stringHandle == null || stringHandle == JavaScriptValueSafeHandle.Invalid)
                throw new ArgumentNullException(nameof(stringHandle));

            //If Auto Convert is specified, ensure that the type is a string, otherwise convert it.
            if (autoConvert)
            {
                var handleValueType = jsrt.JsGetValueType(stringHandle);

                if (handleValueType != JsValueType.String)
                {
                    var convertedToStringHandle = jsrt.JsConvertValueToString(stringHandle);

                    stringHandle = convertedToStringHandle;
                    stringHandleWasCreated = true;
                }
            }

            //Get the size
            var size = jsrt.JsCopyString(stringHandle, null, 0);

            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            var written = jsrt.JsCopyString(stringHandle, result, (ulong)result.Length);

            var strResult = Encoding.UTF8.GetString(result, 0, result.Length);
            if (stringHandleWasCreated)
                stringHandle.Dispose();

            return strResult;
        }

        public static JavaScriptValueSafeHandle GetGlobalVariable(this IJavaScriptEngine jsrt, string name)
        {
            var globalObjectHandle = jsrt.JsGetGlobalObject();
            var propertyIdHandle = default(JavaScriptPropertyIdSafeHandle);

            try
            {
                propertyIdHandle = jsrt.JsCreatePropertyId(name, (ulong)name.Length);
                return jsrt.JsGetProperty(globalObjectHandle, propertyIdHandle);
            }
            finally
            {
                if (propertyIdHandle != default(JavaScriptPropertyIdSafeHandle))
                    propertyIdHandle.Dispose();
                globalObjectHandle.Dispose();
            }
        }

        public static void SetGlobalVariable(this IJavaScriptEngine jsrt, string name, JavaScriptValueSafeHandle value)
        {
            var globalObjectHandle = jsrt.JsGetGlobalObject();
            var propertyIdHandle = default(JavaScriptPropertyIdSafeHandle);

            try
            {
                propertyIdHandle = jsrt.JsCreatePropertyId(name, (ulong)name.Length);
                jsrt.JsSetProperty(globalObjectHandle, propertyIdHandle, value, true);
            }
            finally
            {
                if (propertyIdHandle != default(JavaScriptPropertyIdSafeHandle))
                    propertyIdHandle.Dispose();
                globalObjectHandle.Dispose();
            }
        }
    }
}
