namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    /// Represents a safe handle to a Shared Array Buffer that can be used between contexts.
    /// </summary>
    /// <remarks>
    ///  SharedArrayBuffers must be explicitly freed with Release
    /// </remarks>
    public sealed class JavaScriptSharedArrayBufferSafeHandle : JavaScriptReference<JavaScriptRuntimeSafeHandle>
    {
        public JavaScriptSharedArrayBufferSafeHandle()
            : base()
        {
        }

        public JavaScriptSharedArrayBufferSafeHandle(IntPtr ptr)
            : base(ptr)
        {
        }

        protected override void Dispose(bool disposing)
        {
            //LibChakraCore.JsReleaseSharedArrayBufferContentHandle(this);

            //Do not call the base implementation as we have no references to free.
            //base.Dispose(disposing);
        }

        /// <summary>
        /// Gets an invalid weak reference.
        /// </summary>
        public static readonly JavaScriptSharedArrayBufferSafeHandle Invalid = new JavaScriptSharedArrayBufferSafeHandle();
    }
}
