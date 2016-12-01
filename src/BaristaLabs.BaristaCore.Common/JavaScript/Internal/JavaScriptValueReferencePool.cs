namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    /// Represents a pool of JavaScript Value references.
    /// </summary>
    internal sealed class JavaScriptValuePool : JavaScriptReferencePool<JavaScriptValue, JavaScriptValueSafeHandle>
    {
        private readonly JavaScriptContext m_context;

        public JavaScriptContext Context
        {
            get { return m_context; }
        }

        public JavaScriptValuePool(IJavaScriptEngine engine, JavaScriptContext context)
            : base(engine)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            m_context = context;
        }

        protected override JavaScriptValue FlyweightFactory(JavaScriptValueSafeHandle valueHandle)
        {
            var target = JavaScriptValue.CreateJavaScriptValueFromHandle(Engine, Context, valueHandle);

            //Certain types do not participate in collect callback.
            //These throw an invalid argument exception when attempting to set a beforecollectcallback.
            if (target is JavaScriptNumberValue)
                return target;

            Engine.JsSetObjectBeforeCollectCallback(valueHandle, IntPtr.Zero, OnBeforeCollectCallback);
            return target;
        }

        protected override void ReleaseJavaScriptReference(JavaScriptValue target)
        {
            //Certain types do not participate in collect callback.
            //These throw an invalid argument exception when attempting to set a beforecollectcallback.
            if (target is JavaScriptNumberValue)
                return;

            Engine.JsSetObjectBeforeCollectCallback(target.Handle, IntPtr.Zero, null);
        }

        private void OnBeforeCollectCallback(IntPtr handle, IntPtr callbackState)
        {
            RemoveHandle(handle);
        }
    }
}
