namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System;

    internal sealed class JavaScriptContextPool : JavaScriptReferencePool<JavaScriptContext, JavaScriptContextSafeHandle>
    {
        public JavaScriptContextPool(IJavaScriptEngine engine)
            : base(engine)
        {
            ReleaseJavaScriptReference = (target) =>
            {
                Engine.JsSetObjectBeforeCollectCallback(target.Handle, IntPtr.Zero, null);
            };
        }

        protected override JavaScriptContext FlyweightFactory(JavaScriptContextSafeHandle contextHandle)
        {
            var target = new JavaScriptContext(Engine, contextHandle);
            Engine.JsSetObjectBeforeCollectCallback(contextHandle, IntPtr.Zero, OnBeforeCollectCallback);
            return target;
        }

        private void OnBeforeCollectCallback(IntPtr handle, IntPtr callbackState)
        {
            RemoveHandle(handle);
        }
    }
}
