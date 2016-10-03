namespace BaristaLabs.BaristaCore.JavaScript.SafeHandles
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

            var toRelease = this.handle;

            var error = ChakraApi.Instance.JsReleaseCurrentContext();
            Debug.Assert(error == JsErrorCode.JsNoError);

            error = ChakraApi.Instance.JsDisposeRuntime(toRelease);
            Debug.Assert(error == JsErrorCode.JsNoError);
            return true;
        }
    }
}
