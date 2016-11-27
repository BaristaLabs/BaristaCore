namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    /// Represents a handle to a JavaScript Value
    /// </summary>
    public sealed class JavaScriptValueSafeHandle : JavaScriptReference<JavaScriptValueSafeHandle>
    {
        public JavaScriptValueSafeHandle()
            : base()
        {
        }

        public JavaScriptValueSafeHandle(IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Gets an invalid value.
        /// </summary>
        public static readonly JavaScriptValueSafeHandle Invalid = new JavaScriptValueSafeHandle();
    }
}
