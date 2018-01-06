namespace BaristaLabs.BaristaCore.Extensions
{
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.TypeScript;

    public static class BaristaContextExtensions
    {
        public static JsValue EvaluateTypeScriptModule(this BaristaContext context, string script, IBaristaModuleLoader moduleLoader = null, bool isTsx = false)
        {
            var transpileTask = TypeScriptTranspiler.Default.Transpile(script, isTsx ? "main.tsx" : "main.ts");
            var transpilerOutput = transpileTask.GetAwaiter().GetResult();

            return context.EvaluateModule(transpilerOutput.OutputText, moduleLoader);
        }

        public static T EvaluateTypeScriptModule<T>(this BaristaContext context, string script, IBaristaModuleLoader moduleLoader = null, bool isTsx = false)
            where T : JsValue
        {
            var transpileTask = TypeScriptTranspiler.Default.Transpile(script, isTsx ? "main.tsx" : "main.ts");
            var transpilerOutput = transpileTask.GetAwaiter().GetResult();

            return context.EvaluateModule<T>(transpilerOutput.OutputText, moduleLoader);
        }
    }
}
