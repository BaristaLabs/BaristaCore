namespace BaristaLabs.BaristaCore.Modules.Fetch
{
    using System.Threading.Tasks;

    [BaristaModule("barista-fetch", "Provides a subset of the standard Fetch Specification.")]
    public class FetchModule : IBaristaModule
    {
        public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            throw new System.NotImplementedException();
        }
    }
}
