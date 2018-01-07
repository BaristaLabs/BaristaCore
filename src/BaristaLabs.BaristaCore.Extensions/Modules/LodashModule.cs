namespace BaristaLabs.BaristaCore.Modules
{
    //https://cdnjs.cloudflare.com/ajax/libs/lodash.js/4.17.4/lodash.min.js
    [BaristaModule("lodash", "A utility library delivering consistency, customization, performance, & extras.", Version = "4.17.4")]
    public class LodashModule : IBaristaModule
    {
        private const string ResourceName = "BaristaLabs.BaristaCore.Scripts.lodash.min.js";

        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var buffer = SerializedScriptService.GetSerializedScript(ResourceName, context, mapWindowToGlobal: true);
            var fnScript = context.ParseSerializedScript(buffer, () => EmbeddedResourceHelper.LoadResource(ResourceName), "[eval lodash]");
            var jsLodash = fnScript.Call<JsObject>();
            return jsLodash;
        }
    }
}
