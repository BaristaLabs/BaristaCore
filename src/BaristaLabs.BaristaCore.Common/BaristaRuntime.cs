namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a JavaScript Runtime
    /// </summary>
    /// <remarks>
    ///     A runtime in which contexts are created and JavaScript is executed.
    /// </remarks>
    public sealed class BaristaRuntime : BaristaObject<JavaScriptRuntimeSafeHandle>
    {
        /// <summary>
        /// Event that is raised when the JavaScript engine indicates that memory allocation is changing
        /// </summary>
        public event EventHandler<JavaScriptMemoryEventArgs> MemoryAllocationChanging;

        /// <summary>
        /// Event that is raised after the runtime handle has been released.
        /// </summary>
        public event EventHandler<EventArgs> AfterDispose;

        private IBaristaContextFactory m_contextFactory;

        private readonly GCHandle m_runtimeMemoryAllocationChangingDelegateHandle;
        private readonly GCHandle m_beforeCollectCallbackDelegateHandle;

        /// <summary>
        /// Creates a new instance of a Barista Runtime.
        /// </summary>
        public BaristaRuntime(IJavaScriptEngine engine, IBaristaContextFactory contextFactory, JavaScriptRuntimeSafeHandle runtimeHandle)
            : base(engine, runtimeHandle)
        {
            m_contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));

            JavaScriptMemoryAllocationCallback runtimeMemoryAllocationChanging = (IntPtr callbackState, JavaScriptMemoryEventType allocationEvent, UIntPtr allocationSize) =>
            {
                return OnRuntimeMemoryAllocationChanging(callbackState, allocationEvent, allocationSize);
            };

            m_runtimeMemoryAllocationChangingDelegateHandle = GCHandle.Alloc(runtimeMemoryAllocationChanging);
            Engine.JsSetRuntimeMemoryAllocationCallback(runtimeHandle, IntPtr.Zero, runtimeMemoryAllocationChanging);
            
            JavaScriptBeforeCollectCallback beforeCollectCallback = (IntPtr callbackState) =>
            {
                OnBeforeCollect(IntPtr.Zero, callbackState);
            };

            m_beforeCollectCallbackDelegateHandle = GCHandle.Alloc(beforeCollectCallback);
            Engine.JsSetRuntimeBeforeCollectCallback(runtimeHandle, IntPtr.Zero, beforeCollectCallback);
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

            return m_contextFactory.CreateContext(this);
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

        private void OnAfterDispose()
        {
            AfterDispose?.Invoke(this, new EventArgs());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                if (m_contextFactory != null)
                {
                    m_contextFactory.Dispose();
                    m_contextFactory = null;
                }
            }

            if (!IsDisposed)
            {
                //We don't need no more steekin' memory monitoring.
                Engine.JsSetRuntimeMemoryAllocationCallback(Handle, IntPtr.Zero, null);
                m_runtimeMemoryAllocationChangingDelegateHandle.Free();

                //Don't need no before collect monitoring either!
                Engine.JsSetRuntimeBeforeCollectCallback(Handle, IntPtr.Zero, null);
                m_beforeCollectCallbackDelegateHandle.Free();
            }

            base.Dispose(disposing);

            OnAfterDispose();
        }
    }
}
