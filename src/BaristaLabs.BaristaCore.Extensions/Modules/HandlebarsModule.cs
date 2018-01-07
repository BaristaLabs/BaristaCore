namespace BaristaLabs.BaristaCore.Modules
{
    //https://cdnjs.cloudflare.com/ajax/libs/handlebars.js/4.0.11/handlebars.js
    [BaristaModule("handlebars", "Handlebars provides the power necessary to let you build semantic templates effectively with no frustration", Version = "4.0.11")]
    public class HandlebarsModule : IBaristaModule
    {
        private const string ResourceName = "BaristaLabs.BaristaCore.Scripts.handlebars.min.js";

        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var buffer = SerializedScriptService.GetSerializedScript(ResourceName, context, mapWindowToGlobal: true);
            var fnScript = context.ParseSerializedScript(buffer, () => EmbeddedResourceHelper.LoadResource(ResourceName), "[handlebars]");
            var jsHandlebars = fnScript.Call<JsObject>();
            return jsHandlebars;
        }
    }
}
