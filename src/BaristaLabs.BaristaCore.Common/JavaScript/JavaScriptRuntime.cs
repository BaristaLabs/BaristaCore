namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using System.Runtime.InteropServices;
    using System.Diagnostics;

    /// <summary>
    /// Represents a JavaScript Runtime
    /// </summary>
    /// <remarks>
    ///     A javascript runtime in which contexts are created and JavaScript is executed.
    /// </remarks>
    public sealed class JavaScriptRuntime : JavaScriptReferenceWrapper<JavaScriptRuntimeSafeHandle>
    {
        public event EventHandler<JavaScriptMemoryEventArgs> MemoryChanging;

        /// <summary>
        /// Private constructor. Runtimes are only creatable through the static factory method.
        /// </summary>
        private JavaScriptRuntime(IJavaScriptEngine engine, JavaScriptRuntimeSafeHandle runtimeHandle)
            : base(engine, runtimeHandle)
        {
            //engine.JsSetRuntimeBeforeCollectCallback(runtimeHandle, IntPtr.Zero, RuntimeBeforeCollectCallback);
            GCHandle handle = GCHandle.Alloc(this, GCHandleType.Weak);
            engine.JsSetRuntimeMemoryAllocationCallback(runtimeHandle, GCHandle.ToIntPtr(handle), RuntimeMemoryAllocationCallback);
        }

        /// <summary>
        /// Gets the runtime's current memory usage.
        /// </summary>
        public ulong RuntimeMemoryUsage
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(JavaScriptRuntime));

                return Engine.JsGetRuntimeMemoryUsage(Handle);
            }
        }

        /// <summary>
        /// Returns a new script context for running scripts.
        /// </summary>
        /// <returns></returns>
        public JavaScriptContext CreateContext()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(JavaScriptRuntime));

            var contextHandle = Engine.JsCreateContext(Handle);
            return new JavaScriptContext(Engine, contextHandle, this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                //We don't need no more steekin' memory monitoring.
                Engine.JsSetRuntimeMemoryAllocationCallback(Handle, IntPtr.Zero, null);
            }
            
            base.Dispose(disposing);
        }

        private void RuntimeBeforeCollectCallback(IntPtr callbackState)
        {
            Dispose();
        }

        private static bool RuntimeMemoryAllocationCallback(IntPtr callbackState, JavaScriptMemoryEventType allocationEvent, UIntPtr allocationSize)
        {
            GCHandle handle = GCHandle.FromIntPtr(callbackState);
            JavaScriptRuntime runtime = handle.Target as JavaScriptRuntime;
            if (runtime == null)
            {
                Debug.Assert(false, "Runtime has been freed.");
                return false;
            }

            if (!runtime.IsDisposed && runtime.MemoryChanging != null)
            {
                var args = new JavaScriptMemoryEventArgs(allocationSize, allocationEvent);
                runtime.MemoryChanging(runtime, args);
                if (args.IsCancelable && args.Cancel)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a new JavaScript Runtime.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static JavaScriptRuntime CreateJavaScriptRuntime(IServiceProvider provider, JavaScriptRuntimeAttributes attributes = JavaScriptRuntimeAttributes.None)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var engine = provider.GetRequiredService<IJavaScriptEngine>();
            var runtimeObserver = provider.GetRequiredService<JavaScriptRuntimeObserver>();

            var runtimeHandle = engine.JsCreateRuntime(attributes, null);
            
            var result = new JavaScriptRuntime(engine, runtimeHandle);
            runtimeObserver.MonitorJavaScriptReference(result);
            return result;
        }

        public sealed class JavaScriptRuntimeObserver : JavaScriptReferenceObserver<JavaScriptRuntime, JavaScriptRuntimeSafeHandle>
        {
            public JavaScriptRuntimeObserver(IJavaScriptEngine engine)
                : base(engine)
            {
            }

            protected override IntPtr StartMonitor(JavaScriptRuntime runtime)
            {
                var handle = runtime.Handle;
                IntPtr handlePtr = handle.DangerousGetHandle();
                GCHandle runtimeHandle = GCHandle.Alloc(this, GCHandleType.Weak);
                if (!IsMonitoringHandle(handlePtr))
                    Engine.JsSetRuntimeBeforeCollectCallback(handle, GCHandle.ToIntPtr(runtimeHandle), OnRuntimeBeforeCollect);
                return handlePtr;
            }

            private void OnRuntimeBeforeCollect(IntPtr callbackState)
            {
                GCHandle runtimeHandle = GCHandle.FromIntPtr(callbackState);
                JavaScriptRuntime targetRuntime = runtimeHandle.Target as JavaScriptRuntime;
                if (targetRuntime == null)
                {
                    Debug.Assert(false, "Runtime has been freed.");
                    return;
                }

                var handle = targetRuntime.Handle;
                IntPtr handlePtr = handle.DangerousGetHandle();

                NotifyAll(handlePtr, (runtime) => {
                    if (runtime == null || runtime.IsDisposed)
                        return;

                    runtime.Dispose(false);
                }, true);


                Engine.JsSetRuntimeBeforeCollectCallback(targetRuntime.Handle, IntPtr.Zero, null);
            }
        }
    }
}
