namespace BaristaLabs.BaristaCore.Modules
{
    using System.Threading.Tasks;

    //The script needs to be built by creating an empty project, npm install react-dom envify and using browserify as follows:
    //browserify -r react-dom/server -g [ envify --NODE_ENV production  ] --standalone react-dom-server | uglifyjs > react-dom-server.browser.production.min.js
    [BaristaModule("react-dom-server", "Eenables you to render react components to static markup.", Version = "16.2")]
    public class ReactDomServerModule : INodeModule
    {
        public async Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return await EmbeddedResourceHelper.LoadResourceAsync(this, "BaristaLabs.BaristaCore.Scripts.react-dom-server.browser.production.min.js");
        }
    }
}
