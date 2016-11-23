namespace BaristaLabs.BaristaCore.JavaScript
{
    using Internal;
    using System;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Contains helper methods using only native calls
    /// </summary>
    /// <remarks>
    /// These methods serve as helper functions to internal classes and aren't meant for general use. IJavaScriptRuntimeExtensions are better suited for public consumption.
    /// </remarks>
    internal static class Helpers
    {
        public static string GetStringUtf8(JavaScriptValueSafeHandle stringHandle, bool autoConvert = false, bool releaseHandle = false)
        {
            bool stringHandleWasCreated = false;
            if (stringHandle == null || stringHandle == JavaScriptValueSafeHandle.Invalid)
                throw new ArgumentNullException(nameof(stringHandle));

            //Don't use our helper error class in order to prevent recursive errors.
            JavaScriptErrorCode innerError;

            //If Auto Convert is specified, ensure that the type is a string, otherwise convert it.
            if (autoConvert)
            {
                
                JavaScriptValueType handleValueType;
                innerError = LibChakraCore.JsGetValueType(stringHandle, out handleValueType);
                Debug.Assert(innerError == JavaScriptErrorCode.NoError);

                if (handleValueType != JavaScriptValueType.String)
                {
                    JavaScriptValueSafeHandle convertedToStringHandle;
                    LibChakraCore.JsConvertValueToString(stringHandle, out convertedToStringHandle);
                    if (releaseHandle)
                        stringHandle.Dispose();

                    stringHandle = convertedToStringHandle;
                    stringHandleWasCreated = true;
                }
            }

            //Get the size
            UIntPtr size;
            innerError = LibChakraCore.JsCopyStringUtf8(stringHandle, null, UIntPtr.Zero, out size);
            Debug.Assert(innerError == JavaScriptErrorCode.NoError);

            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            UIntPtr written;
            innerError = LibChakraCore.JsCopyStringUtf8(stringHandle, result, new UIntPtr((uint)result.Length), out written);
            Debug.Assert(innerError == JavaScriptErrorCode.NoError);

            var strResult = Encoding.UTF8.GetString(result, 0, result.Length);
            if (stringHandleWasCreated)
                stringHandle.Dispose();

            return strResult;
        }
    }
}
