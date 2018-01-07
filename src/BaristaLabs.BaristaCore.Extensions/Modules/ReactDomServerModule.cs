namespace BaristaLabs.BaristaCore.Modules
{
    //The script needs to be built by creating an empty project, npm install react-dom envify and using browserify as follows:
    //browserify -r react-dom/server -g [ envify --NODE_ENV production  ] --standalone react-dom-server | uglifyjs > react-dom-server.browser.production.min.js
    [BaristaModule("react-dom-server", "Eenables you to render react components to static markup.", Version = "16.2")]
    public class ReactDomServerModule : IBaristaModule
    {
        private const string ResourceName = "BaristaLabs.BaristaCore.Scripts.react-dom-server.browser.production.min.js";

        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var buffer = SerializedScriptService.GetSerializedScript(ResourceName, context);
            var fnScript = context.ParseSerializedScript(buffer, () => EmbeddedResourceHelper.LoadResource(ResourceName), "[react-dom-server]");
            var jsReact = fnScript.Call<JsObject>();
            return jsReact;
        }
    }
}
