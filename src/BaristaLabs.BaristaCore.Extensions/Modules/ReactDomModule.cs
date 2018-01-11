namespace BaristaLabs.BaristaCore.Modules
{
    //https://cdnjs.cloudflare.com/ajax/libs/react-dom/16.2.0/umd/react-dom.production.min.js
    [BaristaModule("react-dom", "The entry point of the DOM-related rendering paths. It is intended to be paired with the isomorphic React, which is shipped as react to npm.", Version = "16.2")]
    public class ReactDomModule : IBaristaModule
    {
        private const string ResourceName = "BaristaLabs.BaristaCore.Scripts.react-dom.production.min.js";

        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var buffer = SerializedScriptService.GetSerializedScript(ResourceName, context);
            var fnScript = context.ParseSerializedScript(buffer, () => EmbeddedResourceHelper.LoadResource(ResourceName), "[react-dom]");
            var jsReact = fnScript.Call<JsObject>();
            return jsReact;
        }
    }
}
