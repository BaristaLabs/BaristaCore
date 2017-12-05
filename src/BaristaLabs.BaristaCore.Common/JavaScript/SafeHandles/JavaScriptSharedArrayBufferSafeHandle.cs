using System;

namespace BaristaLabs.BaristaCore.JavaScript
{
    /// <summary>
    /// Represents a safe handle to a Shared Array Buffer that can be used between contexts.
    /// </summary>
    /// <remarks>
    ///  SharedArrayBuffers must be explicitly freed with Release
    /// </remarks>
    public sealed class JavaScriptSharedArrayBufferSafeHandle : JavaScriptReference<JavaScriptRuntimeSafeHandle>
    {
        public JavaScriptSharedArrayBufferSafeHandle(IntPtr ptr)
            : base(ptr)
        {
        }

        protected override void Dispose(bool disposing)
        {
            //Do not call the base implementation as we have no references to free.
            //base.Dispose(disposing);
        }

        /// <summary>
        /// Gets an invalid weak reference.
        /// </summary>
        public static readonly JavaScriptWeakReferenceSafeHandle Invalid = new JavaScriptWeakReferenceSafeHandle();
    }
}
