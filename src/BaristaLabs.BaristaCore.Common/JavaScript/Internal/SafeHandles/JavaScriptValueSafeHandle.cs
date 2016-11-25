namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System;
    using System.Diagnostics;

    public class JavaScriptValueSafeHandle : JavaScriptSafeHandle<JavaScriptValueSafeHandle>
    {
        public JavaScriptValueSafeHandle() :
            base()
        {
        }

        protected JavaScriptValueSafeHandle(IntPtr handle) :
            base(handle)
        {
        }

        /// <summary>
        /// Returns a new JavaScriptValueSafeHandle for the specified handle, ensuring that the handle's lifecycle is monitored.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static JavaScriptValueSafeHandle CreateJavaScriptValueFromHandle(IntPtr handle)
        {
            var safeHandle = new JavaScriptValueSafeHandle(handle);
            JavaScriptSafeHandleManager.MonitorJavaScriptSafeHandle(safeHandle);
            return safeHandle;
        }

        /// <summary>
        /// Gets an invalid value.
        /// </summary>
        public static readonly JavaScriptValueSafeHandle Invalid = new JavaScriptValueSafeHandle();
    }
}
