namespace BaristaLabs.BaristaCore.JavaScript.SafeHandles
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal class JavaScriptValueSafeHandle : SafeHandle
    {
        private WeakReference<JavaScriptContext> m_engine;

        public JavaScriptValueSafeHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {

        }

        public JavaScriptValueSafeHandle(IntPtr handle) :
            base(handle, true)
        {

        }

        internal void SetEngine(JavaScriptContext engine)
        {
            Debug.Assert(engine != null);

            m_engine = new WeakReference<JavaScriptContext>(engine);
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
            if (IsInvalid || m_engine == null)
                return false;

            JavaScriptContext eng;
            if (m_engine.TryGetTarget(out eng))
            {
                eng.EnqueueRelease(handle);
                return true;
            }

            return false;
        }
    }
}
