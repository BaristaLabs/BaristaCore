namespace BaristaLabs.BaristaCore.JavaScript.SafeHandles
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal class JavaScriptValueSafeHandle : SafeHandle
    {
        private WeakReference<JavaScriptContext> m_context;

        public JavaScriptValueSafeHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {

        }

        public JavaScriptValueSafeHandle(IntPtr handle) :
            base(handle, true)
        {

        }

        internal void SetContext(JavaScriptContext context)
        {
            Debug.Assert(context != null);

            m_context = new WeakReference<JavaScriptContext>(context);
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
            if (IsInvalid || m_context == null)
                return false;

            JavaScriptContext eng;
            if (m_context.TryGetTarget(out eng))
            {
                eng.EnqueueRelease(handle);
                return true;
            }

            return false;
        }
    }
}
