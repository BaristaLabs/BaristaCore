namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    /// Represents a JavaScript Runtime
    /// </summary>
    /// <remarks>
    ///     A javascript runtime in which contexts are created and JavaScript is executed.
    /// </remarks>
    public sealed class JavaScriptRuntime : IDisposable
    {
        public event EventHandler<JavaScriptMemoryEventArgs> MemoryChanging;

        private readonly IJavaScriptEngine m_javaScriptEngine;
        private JavaScriptRuntimeSafeHandle m_runtimeSafeHandle;

        /// <summary>
        /// Gets the JavaScript Engine associated with the JavaScript Runtime.
        /// </summary>
        public IJavaScriptEngine Engine
        {
            get { return m_javaScriptEngine; }
        }

        /// <summary>
        /// Gets a value that indicates if this runtime has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_runtimeSafeHandle == null || m_runtimeSafeHandle.IsClosed;
            }
        }

        /// <summary>
        /// Private constructor. Runtimes are only creatable through the static factory method.
        /// </summary>
        private JavaScriptRuntime(IJavaScriptEngine engine, JavaScriptRuntimeSafeHandle runtimeHandle)
        {
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));

            if (runtimeHandle == null || runtimeHandle == JavaScriptRuntimeSafeHandle.Invalid)
                throw new ArgumentNullException(nameof(runtimeHandle));

            engine.JsSetRuntimeBeforeCollectCallback(runtimeHandle, IntPtr.Zero, RuntimeBeforeCollectCallback);
            engine.JsSetRuntimeMemoryAllocationCallback(runtimeHandle, IntPtr.Zero, RuntimeMemoryAllocationCallback);
            
            m_javaScriptEngine = engine;
            m_runtimeSafeHandle = runtimeHandle;
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

                return Engine.JsGetRuntimeMemoryUsage(m_runtimeSafeHandle);
            }
        }

        /// <summary>
        /// Returns a new script context for running scripts.
        /// </summary>
        /// <returns></returns>
        public JavaScriptContext CreateContext()
        {
            throw new NotImplementedException();
            //return Engine.JsCreateContext(this);
        }

        private void RuntimeBeforeCollectCallback(IntPtr callbackState)
        {
            Dispose();
        }

        private bool RuntimeMemoryAllocationCallback(IntPtr callbackState, JavaScriptMemoryEventType allocationEvent, UIntPtr allocationSize)
        {
            if (MemoryChanging != null)
            {
                var args = new JavaScriptMemoryEventArgs(allocationSize, allocationEvent);
                MemoryChanging(this, args);
                if (args.IsCancelable && args.Cancel)
                    return false;
            }

            return true;
        }

        #region IDisposable

        private void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                //Dispose of the handle
                m_runtimeSafeHandle.Dispose();
                m_runtimeSafeHandle = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~JavaScriptRuntime()
        {
            Dispose(false);
        }
        #endregion;

        /// <summary>
        /// Creates a new JavaScript Runtime.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static JavaScriptRuntime CreateJavaScriptRuntime(IJavaScriptEngine engine, JavaScriptRuntimeAttributes attributes = JavaScriptRuntimeAttributes.None)
        {
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));

            var runtimeHandle = engine.JsCreateRuntime(attributes, null);
            var result = new JavaScriptRuntime(engine, runtimeHandle);
            return result;
        }
    }
}
