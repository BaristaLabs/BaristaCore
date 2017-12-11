namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    /// <summary>
    /// Represents a JavaScript Runtime
    /// </summary>
    /// <remarks>
    ///     A runtime in which contexts are created and JavaScript is executed.
    /// </remarks>
    public sealed class BaristaRuntime : BaristaObject<JavaScriptRuntimeSafeHandle>
    {
        public event EventHandler<JavaScriptMemoryEventArgs> MemoryAllocationChanging;
        public event EventHandler<EventArgs> GarbageCollecting;

        private IBaristaContextService m_contextService;

        /// <summary>
        /// Creates a new instance of a Barista Runtime.
        /// </summary>
        public BaristaRuntime(IJavaScriptEngine engine, IBaristaContextService contextService, JavaScriptRuntimeSafeHandle runtimeHandle)
            : base(engine, runtimeHandle)
        {
            m_contextService = contextService ?? throw new ArgumentNullException(nameof(contextService));
            
            Engine.JsSetRuntimeMemoryAllocationCallback(runtimeHandle, IntPtr.Zero, OnRuntimeMemoryAllocationChanging);
            Engine.JsSetRuntimeBeforeCollectCallback(runtimeHandle, IntPtr.Zero, OnRuntimeGarbageCollecting);
        }

        /// <summary>
        /// Gets the runtime's current memory usage.
        /// </summary>
        public ulong RuntimeMemoryUsage
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaRuntime));

                return Engine.JsGetRuntimeMemoryUsage(Handle);
            }
        }

        /// <summary>
        /// Returns a new script context for running scripts.
        /// </summary>
        /// <returns></returns>
        public BaristaContext CreateContext()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(BaristaRuntime));

            return m_contextService.CreateContext(this);
        }

        /// <summary>
        /// Performs a full garbage collection.
        /// </summary>
        public void CollectGarbage()
        {
            Engine.JsCollectGarbage(Handle);
        }

        private bool OnRuntimeMemoryAllocationChanging(IntPtr callbackState, JavaScriptMemoryEventType allocationEvent, UIntPtr allocationSize)
        {
            if (!IsDisposed && null != MemoryAllocationChanging)
            {
                lock(MemoryAllocationChanging)
                {
                    if (null != MemoryAllocationChanging)
                    {
                        var args = new JavaScriptMemoryEventArgs(allocationSize, allocationEvent);
                        MemoryAllocationChanging(this, args);
                        if (args.IsCancelable && args.Cancel)
                            return false;
                    }
                }
            }

            return true;
        }

        private void OnRuntimeGarbageCollecting(IntPtr callbackState)
        {
            if (!IsDisposed && null != GarbageCollecting)
            {
                lock(GarbageCollecting)
                {
                    if (null != GarbageCollecting)
                    {
                        var args = new EventArgs();
                        GarbageCollecting(this, args);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                if (m_contextService != null)
                {
                    m_contextService.Dispose();
                    m_contextService = null;
                }

                //We don't need no more steekin' memory monitoring.
                Engine.JsSetRuntimeMemoryAllocationCallback(Handle, IntPtr.Zero, null);

                //Don't need no before collect monitoring either!
                Engine.JsSetRuntimeBeforeCollectCallback(Handle, IntPtr.Zero, null);
            }

            base.Dispose(disposing);
        }
    }
}
