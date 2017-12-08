﻿namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.JavaScript.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// Represents a JavaScript Runtime
    /// </summary>
    /// <remarks>
    ///     A javascript runtime in which contexts are created and JavaScript is executed.
    /// </remarks>
    public sealed class BaristaRuntime : JavaScriptReferenceFlyweight<JavaScriptRuntimeSafeHandle>
    {
        public event EventHandler<JavaScriptMemoryEventArgs> MemoryAllocationChanging;
        public event EventHandler<EventArgs> GarbageCollecting;

        private JavaScriptContextPool m_contextPool;

        /// <summary>
        /// Private constructor. Runtimes are only creatable through the static factory method.
        /// </summary>
        internal BaristaRuntime(IJavaScriptEngine engine, JavaScriptRuntimeSafeHandle runtimeHandle)
            : base(engine, runtimeHandle)
        {
            m_contextPool = new JavaScriptContextPool(engine);

            Engine.JsSetRuntimeMemoryAllocationCallback(runtimeHandle, IntPtr.Zero, OnRuntimeMemoryAllocationChanging);
            Engine.JsSetRuntimeBeforeCollectCallback(runtimeHandle, IntPtr.Zero, OnRuntimeGarbageCollecting);
        }

        public IServiceProvider Provider
        {
            get;
            set;
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

            var contextHandle = Engine.JsCreateContext(Handle);
            var context = m_contextPool.GetOrAdd(contextHandle);
            context.ModuleService = Provider.GetService<IBaristaModuleService>();
            return context;
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
            if (!IsDisposed && MemoryAllocationChanging != null)
            {
                var args = new JavaScriptMemoryEventArgs(allocationSize, allocationEvent);
                MemoryAllocationChanging(this, args);
                if (args.IsCancelable && args.Cancel)
                    return false;
            }

            return true;
        }

        private void OnRuntimeGarbageCollecting(IntPtr callbackState)
        {
            if (!IsDisposed && GarbageCollecting != null)
            {
                var args = new EventArgs();
                GarbageCollecting(this, args);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                if (m_contextPool != null)
                {
                    m_contextPool.Dispose();
                    m_contextPool = null;
                }

                //We don't need no more steekin' memory monitoring.
                Engine.JsSetRuntimeMemoryAllocationCallback(Handle, IntPtr.Zero, null);

                //Don't need no before collect monitoring either!
                Engine.JsSetRuntimeBeforeCollectCallback(Handle, IntPtr.Zero, null);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates a new JavaScript Runtime.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static BaristaRuntime CreateRuntime(IServiceProvider provider, JavaScriptRuntimeAttributes attributes = JavaScriptRuntimeAttributes.None)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var engine = provider.GetRequiredService<IJavaScriptEngine>();
            var runtimePool = provider.GetRequiredService<JavaScriptRuntimePool>();

            var runtimeHandle = engine.JsCreateRuntime(attributes, null);
            var runtime = runtimePool.GetOrAdd(runtimeHandle);
            runtime.Provider = provider;
            return runtime;
        }
    }
}