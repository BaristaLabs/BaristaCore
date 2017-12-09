namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    /// Represents a handle to a JavaScript Context
    /// </summary>
    public sealed class JavaScriptContextSafeHandle : JavaScriptReference<JavaScriptContextSafeHandle>
    {
        public JavaScriptContextSafeHandle()
            : base()
        {
        }

        public JavaScriptContextSafeHandle(IntPtr ptr)
            : base(ptr)
        {
        }

        /// <summary>
        /// Gets an invalid context.
        /// </summary>
        public static readonly JavaScriptContextSafeHandle Invalid = new JavaScriptContextSafeHandle();
    }
}
