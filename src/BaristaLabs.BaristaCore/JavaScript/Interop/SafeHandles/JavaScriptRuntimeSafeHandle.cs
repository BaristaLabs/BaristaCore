namespace BaristaLabs.BaristaCore.JavaScript.Interop.SafeHandles
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal sealed class JavaScriptRuntimeSafeHandle : SafeHandle
    {
        public JavaScriptRuntimeSafeHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {

        }

        public JavaScriptRuntimeSafeHandle(IntPtr handle) :
            base(handle, true)
        {

        }

        public override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero;
            }
        }
        
        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
                return false;
            
            var error = ChakraApi.Instance.JsSetCurrentContext(JavaScriptContextSafeHandle.Invalid);
            Debug.Assert(error == JsErrorCode.JsNoError);

            error = ChakraApi.Instance.JsDisposeRuntime(handle);
            Debug.Assert(error == JsErrorCode.JsNoError);
            return true;
        }

        /// <summary>
        /// Gets an invalid runtime.
        /// </summary>
        public static readonly JavaScriptRuntimeSafeHandle Invalid = new JavaScriptRuntimeSafeHandle();
    }
}
