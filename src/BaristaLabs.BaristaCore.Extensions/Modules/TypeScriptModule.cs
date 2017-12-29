namespace BaristaLabs.BaristaCore.Modules
{
    using System.Threading.Tasks;

    //https://cdnjs.cloudflare.com/ajax/libs/typescript/2.6.2/typescript.min.js
    [BaristaModule("typescript", "TypeScript is a language for application scale JavaScript development", Version = "2.6.2")]
    public class TypeScriptModule : INodeModule
    {
        public async Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return await EmbeddedResourceHelper.LoadResource(this, "BaristaLabs.BaristaCore.Scripts.typescript.min.js");
        }
    }
}
