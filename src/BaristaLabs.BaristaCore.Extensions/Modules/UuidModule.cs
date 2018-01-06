namespace BaristaLabs.BaristaCore.Modules
{
    //https://cdnjs.cloudflare.com/ajax/libs/node-uuid/1.4.8/uuid.min.js
    [BaristaModule("uuid", "Rigorous implementation of RFC4122 (v1 and v4) UUIDs.", Version = "1.4.8")]
    public class UuidModule : IBaristaModule
    {
        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var buffer = SerializedScriptService.GetSerializedScript("BaristaLabs.BaristaCore.Scripts.uuid.min.js", context, mapWindowToGlobal: true);
            var fnScript = context.ParseSerializedScript(buffer, "[uuid]");
            return fnScript.Call<JsObject>();
        }
    }
}
