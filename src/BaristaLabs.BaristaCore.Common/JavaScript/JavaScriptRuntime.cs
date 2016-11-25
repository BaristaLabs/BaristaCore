namespace BaristaLabs.BaristaCore.JavaScript
{
    using Internal;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Represents a JavaScript Runtime object.
    /// </summary>
    public sealed class JavaScriptRuntime : JavaScriptRuntimeSafeHandle
    {
        public event EventHandler<JavaScriptMemoryEventArgs> MemoryChanging;

        /// <summary>
        /// Private constructor. Runtimes are only creatable through the static factory method.
        /// </summary>
        private JavaScriptRuntime()
        {
        }

        public IJavaScriptEngine Engine
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the runtime's current memory usage.
        /// </summary>
        public ulong RuntimeMemoryUsage
        {
            get
            {
                if (IsCollected)
                    throw new ObjectDisposedException("The runtime has been collected.");

                return Engine.JsGetRuntimeMemoryUsage(this);
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
            //TODO: Implement this.
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

        protected override void Dispose(bool disposing)
        {
            if (!IsCollected && !IsClosed)
            {
                //Ensure that a context is not active, otherwise the runtime will throw a "Runtime In Use" exception.
                var error = LibChakraCore.JsSetCurrentContext(JavaScriptContextSafeHandle.Invalid);
                Debug.Assert(error == JavaScriptErrorCode.NoError);

                error = LibChakraCore.JsDisposeRuntime(handle);
                Debug.Assert(error == JavaScriptErrorCode.NoError);
            }

            base.Dispose(disposing);
        }

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

            throw new NotImplementedException();
            //JavaScriptRuntime javaScriptRuntime = engine.JsCreateRuntime(attributes, null);
            //javaScriptRuntime.Engine = engine;
            //engine.JsSetRuntimeBeforeCollectCallback(javaScriptRuntime, IntPtr.Zero, javaScriptRuntime.RuntimeBeforeCollectCallback);
            //engine.JsSetRuntimeMemoryAllocationCallback(javaScriptRuntime, IntPtr.Zero, javaScriptRuntime.RuntimeMemoryAllocationCallback);
            //return javaScriptRuntime;
        }

        /// <summary>
        /// Gets an invalid runtime.
        /// </summary>
        //public static readonly JavaScriptRuntime Invalid = new JavaScriptRuntime();
    }
}
