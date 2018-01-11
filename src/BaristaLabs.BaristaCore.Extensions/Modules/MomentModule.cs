namespace BaristaLabs.BaristaCore.Modules
{
    //https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.20.1/moment.min.js
    [BaristaModule("moment", "Parse, validate, manipulate, and display dates", Version = "2.20.1")]
    public class MomentModule : IBaristaModule
    {
        private const string ResourceName = "BaristaLabs.BaristaCore.Scripts.moment.min.js";

        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var buffer = SerializedScriptService.GetSerializedScript(ResourceName, context);
            var fnScript = context.ParseSerializedScript(buffer, () => EmbeddedResourceHelper.LoadResource(ResourceName), "[moment]");
            var jsMoment = fnScript.Call<JsObject>();
            return jsMoment;
        }
    }
}
