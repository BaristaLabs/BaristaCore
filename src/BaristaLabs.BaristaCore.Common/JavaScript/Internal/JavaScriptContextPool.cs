namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System;

    internal sealed class JavaScriptContextPool : JavaScriptReferencePool<BaristaContext, JavaScriptContextSafeHandle>
    {
        public JavaScriptContextPool(IJavaScriptEngine engine)
            : base(engine)
        {
            ReleaseJavaScriptReference = (target) =>
            {
                Engine.JsSetObjectBeforeCollectCallback(target.Handle, IntPtr.Zero, null);
            };
        }

        protected override BaristaContext FlyweightFactory(JavaScriptContextSafeHandle contextHandle)
        {
            var target = new BaristaContext(Engine, contextHandle);
            Engine.JsSetObjectBeforeCollectCallback(contextHandle, IntPtr.Zero, OnBeforeCollectCallback);
            return target;
        }

        private void OnBeforeCollectCallback(IntPtr handle, IntPtr callbackState)
        {
            RemoveHandle(handle);
        }
    }
}
