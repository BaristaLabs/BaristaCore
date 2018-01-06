namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Represents a service that maintains a collection of serialized scripts.
    /// </summary>
    public static class SerializedScriptService
    {
        private static ConcurrentDictionary<string, byte[]> s_serializedScripts = new ConcurrentDictionary<string, byte[]>();

        public static byte[] GetSerializedScript(string resourceName, BaristaContext context, bool mapWindowToGlobal = false)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return s_serializedScripts.GetOrAdd(resourceName, new Func<string, byte[]>((name) =>
             {
                 var script = EmbeddedResourceHelper.LoadResource(typeof(SerializedScriptService).Assembly, name);

                 string mapWindowToGlobalString = mapWindowToGlobal ? "const window = global;" : null;

                 var scriptWithModuleWrapper = $@"(() => {{
'use strict';
{mapWindowToGlobalString}
const module = {{
    exports: {{}}
}};
let exports = module.exports;
{script}
return module.exports;
}})();";

                 return context.SerializeScript(scriptWithModuleWrapper);
             }));
        }
    }
}
