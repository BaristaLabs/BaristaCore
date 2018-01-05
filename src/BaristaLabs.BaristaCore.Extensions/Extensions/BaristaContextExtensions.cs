namespace BaristaLabs.BaristaCore.Extensions
{
    using BaristaLabs.BaristaCore.ModuleLoaders;

    public static class BaristaContextExtensions
    {
        public static JsValue EvaluateTypeScriptModule(this BaristaContext context, string script, IBaristaModuleLoader moduleLoader = null, bool isTsx = false)
        {
            var transpileTask = TypeScriptTranspiler.Default.Transpile(script, isTsx ? "main.tsx" : "main.ts");
            var transpiledScript = transpileTask.GetAwaiter().GetResult();

            return context.EvaluateModule(transpiledScript, moduleLoader);
        }

        public static T EvaluateTypeScriptModule<T>(this BaristaContext context, string script, IBaristaModuleLoader moduleLoader = null, bool isTsx = false)
            where T : JsValue
        {
            var transpileTask = TypeScriptTranspiler.Default.Transpile(script, isTsx ? "main.tsx" : "main.ts");
            var transpiledScript = transpileTask.GetAwaiter().GetResult();

            return context.EvaluateModule<T>(transpiledScript, moduleLoader);
        }
    }
}
