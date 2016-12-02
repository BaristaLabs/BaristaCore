namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    using System;
    using System.Runtime.InteropServices;

    internal class JavaScriptRuntimePool : JavaScriptReferencePool<JavaScriptRuntime, JavaScriptRuntimeSafeHandle>
    {
        public JavaScriptRuntimePool(IJavaScriptEngine engine)
            : base(engine)
        {
        }

        protected override JavaScriptRuntime FlyweightFactory(JavaScriptRuntimeSafeHandle runtimeHandle)
        {
            var target = new JavaScriptRuntime(Engine, runtimeHandle);
            
            Engine.JsSetRuntimeMemoryAllocationCallback(runtimeHandle, IntPtr.Zero, (IntPtr callbackState, JavaScriptMemoryEventType allocationEvent, UIntPtr allocationSize) =>
            {
                return target.RuntimeMemoryAllocationChanging(allocationEvent, allocationSize);
            });

            Engine.JsSetRuntimeBeforeCollectCallback(runtimeHandle, IntPtr.Zero, (callbackState) =>
            {
                RemoveHandle(runtimeHandle);
            });
            return target;
        }

        protected override void ReleaseJavaScriptReference(JavaScriptRuntime target)
        {
            //We don't need no more steekin' memory monitoring.
            Engine.JsSetRuntimeMemoryAllocationCallback(target.Handle, IntPtr.Zero, null);

            //Don't need no before collect monitoring either!
            Engine.JsSetRuntimeBeforeCollectCallback(target.Handle, IntPtr.Zero, null);
        }
    }
}
