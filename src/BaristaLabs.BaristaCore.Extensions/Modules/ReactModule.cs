namespace BaristaLabs.BaristaCore.Modules
{
    using System.Threading.Tasks;

    [BaristaModule("react", "Allows for server-side rendering in Barista via React", Version = "16.2")]
    public class ReactModule : INodeModule
    {
        public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return Task.FromResult<object>(Properties.Resources.react_production_min);
        }
    }
}
