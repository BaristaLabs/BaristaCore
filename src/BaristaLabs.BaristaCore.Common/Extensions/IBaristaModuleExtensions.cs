namespace BaristaLabs.BaristaCore.Extensions
{
    using BaristaLabs.BaristaCore.Modules;

    public static class IBaristaModuleExtensions
    {
        public static string GetModuleName(this IBaristaModule module)
        {
            string name;
            switch (module)
            {
                case BaristaScriptModule scriptModule:
                    name = scriptModule.Name;
                    break;
                case BaristaResourceScriptModule resourceScriptModule:
                    name = resourceScriptModule.Name;
                    break;
                default:
                    var baristaModuleAttribute = BaristaModuleAttribute.GetBaristaModuleAttributeFromType(module.GetType());
                    name = baristaModuleAttribute.Name;
                    break;
            }

            return name;
        }
    }
}
