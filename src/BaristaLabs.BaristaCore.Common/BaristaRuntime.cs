namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a JavaScript Runtime
    /// </summary>
    /// <remarks> 
    /// A runtime represents a complete JavaScript execution environment. Each runtime that is created has its own isolated garbage-collected heap and, 
    /// by default, its own just-in-time (JIT) compiler thread and garbage collector (GC) thread. An execution context represents a JavaScript environment 
    /// that has its own JavaScript global object distinct from all other execution contexts. One runtime may contain multiple execution contexts, and 
    /// in such cases, all the execution contexts share the JIT compiler and GC thread associated with the runtime.
    /// 
    /// Runtimes represent a single thread of execution. Only one runtime can be active on a particular thread at a time, and a runtime can only be active 
    /// on one thread at a time. Runtimes are rental threaded, so a runtime that is not currently active on a thread 
    /// (i.e.isn’t running any JavaScript code or responding to any calls from the host) can be used on any thread that doesn’t already have an 
    /// active runtime on it.
    /// 
    /// Execution contexts are tied to a particular runtime and execute code within that runtime. Unlike runtimes, multiple execution contexts can be 
    /// active on a thread at the same time. So a host can make a call into an execution context, that execution context can call back to the host, 
    /// and the host can make a call into a different execution context.
    /// 
    /// In practice, unless a host needs to run code in separated environments, a single execution context can be used.
    /// Similarly, unless a host needs to run multiple pieces of code concurrently, a single runtime is sufficient.
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
                try
                {
                    return OnRuntimeMemoryAllocationChanging(callbackState, allocationEvent, allocationSize);
                }
                catch
                {
                    //Do Nothing.
                    return true;
                }
            };

            m_runtimeMemoryAllocationChangingDelegateHandle = GCHandle.Alloc(runtimeMemoryAllocationChanging);
            Engine.JsSetRuntimeMemoryAllocationCallback(runtimeHandle, IntPtr.Zero, runtimeMemoryAllocationChanging);
            
            JavaScriptBeforeCollectCallback beforeCollectCallback = (IntPtr callbackState) =>
            {
                try
                {
                    OnBeforeCollect(IntPtr.Zero, callbackState);
                }
                catch
                {
                    //Do Nothing.
                }
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
