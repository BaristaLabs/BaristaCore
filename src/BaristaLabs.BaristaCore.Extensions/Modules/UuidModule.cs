namespace BaristaLabs.BaristaCore.Modules
{
    //https://cdnjs.cloudflare.com/ajax/libs/node-uuid/1.4.8/uuid.min.js
    [BaristaModule("uuid", "Rigorous implementation of RFC4122 (v1 and v4) UUIDs.", Version = "1.4.8")]
    public class UuidModule : IBaristaModule
    {
        private const string ResourceName = "BaristaLabs.BaristaCore.Scripts.uuid.min.js";

        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var buffer = SerializedScriptService.GetSerializedScript(ResourceName, context, mapWindowToGlobal: true);
            var fnScript = context.ParseSerializedScript(buffer, () => EmbeddedResourceHelper.LoadResource(ResourceName), "[uuid]");
            return fnScript.Call<JsObject>();
        }
    }
}
