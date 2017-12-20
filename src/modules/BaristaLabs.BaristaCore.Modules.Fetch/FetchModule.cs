namespace BaristaLabs.BaristaCore.Modules.Fetch
{
    using System.Threading.Tasks;
    using BaristaLabs.BaristaCore.JavaScript;

    public class FetchModule : IBaristaModule
    {
        public string Name => "barista-fetch";

        public string Description => "Provides a subset of the standard Fetch Specification";

        public Task<object> ExportDefault(BaristaContext context, JavaScriptModuleRecord referencingModule)
        {
            throw new System.NotImplementedException();
        }
    }
}
