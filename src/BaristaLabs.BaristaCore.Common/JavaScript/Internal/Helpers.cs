namespace BaristaLabs.BaristaCore.JavaScript.Internal
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
    public static class Helpers
    {
        public static string GetStringUtf8(JavaScriptValueSafeHandle stringHandle, bool releaseHandle = false)
        {
            bool stringHandleWasCreated = false;
            if (stringHandle == null || stringHandle == JavaScriptValueSafeHandle.Invalid)
                throw new ArgumentNullException(nameof(stringHandle));

            //Don't use our helper error class in order to prevent recursive errors.
            JavaScriptErrorCode innerError;

            //Get the size
            innerError = LibChakraCore.JsCopyString(stringHandle, null, 0, out ulong size);
            Debug.Assert(innerError == JavaScriptErrorCode.NoError);

            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            innerError = LibChakraCore.JsCopyString(stringHandle, result, (ulong)result.Length, out ulong written);
            Debug.Assert(innerError == JavaScriptErrorCode.NoError);

            var strResult = Encoding.UTF8.GetString(result, 0, result.Length);
            if (stringHandleWasCreated)
                stringHandle.Dispose();

            if (releaseHandle)
                stringHandle.Dispose();

            return strResult;
        }
    }
}
