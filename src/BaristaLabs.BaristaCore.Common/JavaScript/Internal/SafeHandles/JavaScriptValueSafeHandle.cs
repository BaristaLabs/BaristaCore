namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System;

    public class JavaScriptValueSafeHandle : JavaScriptSafeHandle<JavaScriptValueSafeHandle>
    {
        public JavaScriptValueSafeHandle() :
            base()
        {
        }

        public JavaScriptValueSafeHandle(IntPtr handle) :
            base(handle)
        {
        }

        /// <summary>
        /// Gets an invalid value.
        /// </summary>
        public static readonly JavaScriptValueSafeHandle Invalid = new JavaScriptValueSafeHandle();
    }
}
