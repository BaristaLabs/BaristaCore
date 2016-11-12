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
        public static JsErrorCode JsRunScriptUtf8(this IChakraApi api, string script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JsParseScriptAttributes parseAttributes, out JavaScriptValueSafeHandle result)
        {
            IntPtr ptrScript = Marshal.StringToHGlobalAnsi(script);
            try
            {
                
                JavaScriptValueSafeHandle scriptHandle;
                Errors.ThrowIfIs(ChakraApi.Instance.JsCreateExternalArrayBuffer(ptrScript, (uint)script.Length, null, IntPtr.Zero, out scriptHandle));

                if (sourceContext == default(JavaScriptSourceContext))
                {
                    sourceContext = new JavaScriptSourceContext();
                }

                bool createdSourceUrl = false;
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

                var errorCode = api.JsRun(scriptHandle, sourceContext, sourceUrl, parseAttributes, out result);
                
                //Release variables created during this operation.
                uint releaseCount;

                if (createdSourceUrl)
                    Errors.ThrowIfIs(ChakraApi.Instance.JsReleaseValue(sourceUrl, out releaseCount));

                Errors.ThrowIfIs(ChakraApi.Instance.JsReleaseValue(scriptHandle, out releaseCount));

                return errorCode;
            }
            finally
            {                    
                Marshal.ZeroFreeGlobalAllocAnsi(ptrScript);
            }
            
        }
    }
}
