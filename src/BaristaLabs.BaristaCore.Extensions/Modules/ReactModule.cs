namespace BaristaLabs.BaristaCore.Modules
{
    using System.Threading.Tasks;

    [BaristaModule("react", "Allows for server-side rendering in Barista via React", Version = "16.2")]
    public class ReactModule : INodeModule
    {
        public async Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return await EmbeddedResourceHelper.LoadResource(this, "BaristaLabs.BaristaCore.Scripts.react.production.min.js");
        }
    }
}
