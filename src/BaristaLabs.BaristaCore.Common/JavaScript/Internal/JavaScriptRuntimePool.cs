namespace BaristaLabs.BaristaCore.JavaScript.Internal
{
    internal class JavaScriptRuntimePool : JavaScriptReferencePool<BaristaRuntime, JavaScriptRuntimeSafeHandle>
    {
        public JavaScriptRuntimePool(IJavaScriptEngine engine)
            : base(engine)
        {
        }

        protected override BaristaRuntime FlyweightFactory(JavaScriptRuntimeSafeHandle runtimeHandle)
        {
            var target = new BaristaRuntime(Engine, runtimeHandle);
            return target;
        }
    }
}
