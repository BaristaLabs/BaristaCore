namespace BaristaLabs.BaristaCore.Modules
{
    using BaristaLabs.BaristaCore.TypeScript;

    //https://cdnjs.cloudflare.com/ajax/libs/typescript/2.6.2/typescript.min.js
    [BaristaModule("typescript", "TypeScript is a language for application scale JavaScript development", Version = "2.6.2")]
    public class TypeScriptModule : IBaristaModule
    {
        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var tsBuffer = TypeScriptTranspiler.GetSerializedTypeScriptCompiler(context);
            var fnTypeScript = context.ParseSerializedScript(tsBuffer, "[typescript]");
            return fnTypeScript.Call<JsObject>();
        }
    }
}
