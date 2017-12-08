namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System;

    /// <summary>
    /// Represents a pool of JavaScript Value references.
    /// </summary>
    internal sealed class JavaScriptValuePool : JavaScriptReferencePool<JavaScriptValue, JavaScriptValueSafeHandle>
    {
        private readonly BaristaContext m_context;

        public BaristaContext Context
        {
            get { return m_context; }
        }

        public JavaScriptValuePool(IJavaScriptEngine engine, BaristaContext context)
            : base(engine)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            m_context = context;

            ReleaseJavaScriptReference = (target) =>
            {
                //Certain types do not participate in collect callback.
                //These throw an invalid argument exception when attempting to set a beforecollectcallback.
                if (target is JavaScriptNumber)
                    return;

                Engine.JsSetObjectBeforeCollectCallback(target.Handle, IntPtr.Zero, null);
            };
        }

        protected override JavaScriptValue FlyweightFactory(JavaScriptValueSafeHandle valueHandle)
        {
            var target = JavaScriptValue.CreateJavaScriptValueFromHandle(Engine, Context, valueHandle);

            //Certain types do not participate in collect callback.
            //These throw an invalid argument exception when attempting to set a beforecollectcallback.
            if (target is JavaScriptNumber)
                return target;

            Engine.JsSetObjectBeforeCollectCallback(valueHandle, IntPtr.Zero, OnBeforeCollectCallback);
            return target;
        }

        private void OnBeforeCollectCallback(IntPtr handle, IntPtr callbackState)
        {
            RemoveHandle(handle);
        }
    }
}
