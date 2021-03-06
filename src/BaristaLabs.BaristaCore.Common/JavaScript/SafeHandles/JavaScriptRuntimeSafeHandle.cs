﻿namespace BaristaLabs.BaristaCore.JavaScript
{
    using Internal;
    using System.Diagnostics;

    /// <summary>
    /// Represents a handle to a JavaScript Runtime
    /// </summary>
    public sealed class JavaScriptRuntimeSafeHandle : JavaScriptReference<JavaScriptRuntimeSafeHandle>
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsClosed)
            {
                //Ensure that a context is not active, otherwise the runtime will throw a "Runtime In Use" exception.
                LibChakraCore.JsSetCurrentContext(JavaScriptContextSafeHandle.Invalid);
                LibChakraCore.JsDisposeRuntime(handle);
            }

            //Do not call the base implementation as we have no references to free.
            //base.Dispose(disposing);
        }

        /// <summary>
        /// Gets an invalid runtime.
        /// </summary>
        public static readonly JavaScriptRuntimeSafeHandle Invalid = new JavaScriptRuntimeSafeHandle();
    }
}
