namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    public class JavaScriptValue : JavaScriptReference<JavaScriptValue>
    {
        public JavaScriptValue() :
            base()
        {
        }

        protected JavaScriptValue(IntPtr handle) :
            base(handle)
        {
        }

        /// <summary>
        /// Returns a new JavaScriptValueSafeHandle for the specified handle, ensuring that the handle's lifecycle is monitored.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static JavaScriptValue CreateJavaScriptValueFromHandle(IntPtr handle)
        {
            var safeHandle = new JavaScriptValue(handle);
            JavaScriptObjectManager.MonitorJavaScriptObjectLifetime(safeHandle);
            return safeHandle;
        }

        /// <summary>
        /// Gets an invalid value.
        /// </summary>
        public static readonly JavaScriptValue Invalid = new JavaScriptValue();
    }
}
