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
            return target;
        }
    }
}
