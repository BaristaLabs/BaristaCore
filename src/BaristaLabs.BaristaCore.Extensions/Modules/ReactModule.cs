namespace BaristaLabs.BaristaCore.Modules
{
    //https://cdnjs.cloudflare.com/ajax/libs/react/16.2.0/umd/react.production.min.js
    [BaristaModule("react", "Allows for server-side rendering in Barista via React", Version = "16.2")]
    public class ReactModule : IBaristaModule
    {
        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var buffer = SerializedScriptService.GetSerializedScript("BaristaLabs.BaristaCore.Scripts.react.production.min.js", context);
            var fnScript = context.ParseSerializedScript(buffer, "[react]");
            return fnScript.Call<JsObject>();
        }
    }
}
