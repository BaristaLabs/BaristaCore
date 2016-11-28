namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    ///     Represents a JavaScript Context
    /// </summary>
    /// <remarks>
    ///     Each script context has its own global object that is isolated from all other script contexts.
    /// </remarks>
    public sealed class JavaScriptContext : JavaScriptReferenceWrapper<JavaScriptContextSafeHandle>
    {
        private readonly JavaScriptRuntime m_runtime;
        private readonly Lazy<JavaScriptUndefinedValue> m_undefinedValue;

        /// <summary>
        /// Gets the Undefined Value associated with the context.
        /// </summary>
        public JavaScriptValue Undefined
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(JavaScriptContext));

                return m_undefinedValue.Value;
            }
        }

        internal JavaScriptContext(IJavaScriptEngine engine, JavaScriptContextSafeHandle contextHandle, JavaScriptRuntime runtime)
            : base(engine, contextHandle)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));

            m_runtime = runtime;
            //engine.JsSetObjectBeforeCollectCallback(contextHandle, IntPtr.Zero, OnObjectBeforeCollect);

            m_undefinedValue = new Lazy<JavaScriptUndefinedValue>(GetUndefinedValue);
        }

        private JavaScriptUndefinedValue GetUndefinedValue()
        {
            var undefined = Engine.JsGetUndefinedValue();
            
            //Global 'const' values don't participate in Object Collection, return the value.
            return new JavaScriptUndefinedValue(Engine, undefined);
        }

        private void OnObjectBeforeCollect(IntPtr handle, IntPtr callbackState)
        {
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                if (m_undefinedValue.IsValueCreated)
                    m_undefinedValue.Value.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
