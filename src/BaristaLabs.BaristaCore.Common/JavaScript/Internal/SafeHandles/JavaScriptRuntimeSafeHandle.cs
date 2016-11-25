namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System.Diagnostics;

    public class JavaScriptRuntimeSafeHandle : JavaScriptSafeHandle<JavaScriptRuntimeSafeHandle>
    {
        protected override void Dispose(bool disposing)
        {
            if (!IsCollected && !IsClosed)
            {
                //Ensure that a context is not active, otherwise the runtime will throw a "Runtime In Use" exception.
                var error = LibChakraCore.JsSetCurrentContext(JavaScriptContextSafeHandle.Invalid);
                Debug.Assert(error == JavaScriptErrorCode.NoError);

                error = LibChakraCore.JsDisposeRuntime(handle);
                Debug.Assert(error == JavaScriptErrorCode.NoError);
                IsCollected = true;
                SetHandleAsInvalid();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets an invalid runtime.
        /// </summary>
        public static readonly JavaScriptRuntimeSafeHandle Invalid = new JavaScriptRuntimeSafeHandle();
    }
}
