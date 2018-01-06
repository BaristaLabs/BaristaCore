namespace BaristaLabs.BaristaCore.Modules
{
    using System.Threading.Tasks;

    //https://cdnjs.cloudflare.com/ajax/libs/react-dom/16.2.0/umd/react-dom.production.min.js
    [BaristaModule("react-dom", "The entry point of the DOM-related rendering paths. It is intended to be paired with the isomorphic React, which is shipped as react to npm.", Version = "16.2")]
    public class ReactDomModule : INodeModule
    {
        public async Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return await EmbeddedResourceHelper.LoadResourceAsync(this, "BaristaLabs.BaristaCore.Scripts.react-dom.production.min.js");
        }
    }
}
