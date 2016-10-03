namespace BaristaLabs.BaristaCore.JavaScript.SafeHandles
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal class JavaScriptValueSafeHandle : SafeHandle
    {
        private WeakReference<JavaScriptEngine> m_engine;

        public JavaScriptValueSafeHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {

        }

        public JavaScriptValueSafeHandle(IntPtr handle) :
            base(handle, true)
        {

        }

        internal void SetEngine(JavaScriptEngine engine)
        {
            Debug.Assert(engine != null);

            m_engine = new WeakReference<JavaScriptEngine>(engine);
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

            JavaScriptEngine eng;
            if (m_engine.TryGetTarget(out eng))
            {
                eng.EnqueueRelease(handle);
                return true;
            }

            return false;
        }
    }
}
