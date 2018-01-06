namespace BaristaLabs.BaristaCore.Modules
{
    //https://cdnjs.cloudflare.com/ajax/libs/lodash.js/4.17.4/lodash.min.js
    [BaristaModule("lodash", "A utility library delivering consistency, customization, performance, & extras.", Version = "4.17.4")]
    public class LodashModule : IBaristaScriptModule
    {
        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            //TODO: Something about lodash doesn't like pre-parsing the script.
            //var buffer = SerializedScriptService.GetSerializedScript("BaristaLabs.BaristaCore.Scripts.lodash.min.js", context, mapWindowToGlobal: true);
            //var fnScript = context.ParseSerializedScript(buffer, "[lodash]");
            //var jsLodash = fnScript.Call<JsObject>();
            //return jsLodash;

            var script = EmbeddedResourceHelper.LoadResource(typeof(LodashModule).Assembly, "BaristaLabs.BaristaCore.Scripts.lodash.min.js");
            script = $@"'use strict';
const window = global;
const module = {{
    exports: {{}}
}};
let exports = module.exports;
(function() {{
{script}
}}).call(global);
export default module.exports";

            return context.CreateString(script);
        }
    }
}
