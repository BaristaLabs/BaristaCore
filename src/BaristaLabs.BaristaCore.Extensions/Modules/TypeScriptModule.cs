namespace BaristaLabs.BaristaCore.Modules
{
    using System.Threading.Tasks;

    [BaristaModule("typescript", "TypeScript is a language for application scale JavaScript development", Version = "2.6.2")]
    public class TypeScriptModule : IBaristaScriptModule
    {
        public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return Task.FromResult<object>(Properties.Resources.react_production_min);
        }
    }
}
