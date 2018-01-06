namespace BaristaLabs.BaristaCore.Modules
{
    using System.Threading.Tasks;

    //https://cdnjs.cloudflare.com/ajax/libs/lodash.js/4.17.4/lodash.min.js
    [BaristaModule("lodash", "A utility library delivering consistency, customization, performance, & extras.", Version = "4.17.4")]
    public class LodashModule : INodeModule
    {
        public async Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return await EmbeddedResourceHelper.LoadResourceAsync(this, "BaristaLabs.BaristaCore.Scripts.lodash.min.js");
        }
    }
}
