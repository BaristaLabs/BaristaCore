namespace BaristaLabs.BaristaCore.JavaScript.SafeHandles
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal class JavaScriptContextSafeHandle : SafeHandle
    {
        public JavaScriptContextSafeHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {

        }

        public JavaScriptContextSafeHandle(IntPtr handle) :
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

            uint count;
            var error = ChakraApi.Instance.JsRelease(handle, out count);

            Debug.Assert(error == JsErrorCode.JsNoError);
            return true;
        }

        public static readonly JavaScriptContextSafeHandle Invalid = new JavaScriptContextSafeHandle();
    }
}
