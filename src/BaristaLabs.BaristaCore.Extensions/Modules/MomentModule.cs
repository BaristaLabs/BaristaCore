namespace BaristaLabs.BaristaCore.Modules
{
    using System.Threading.Tasks;

    //https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.20.1/moment.min.js
    [BaristaModule("moment", "Parse, validate, manipulate, and display dates", Version = "2.20.1")]
    public class MomentModule : INodeModule
    {
        public async Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return await EmbeddedResourceHelper.LoadResourceAsync(this, "BaristaLabs.BaristaCore.Scripts.moment.min.js");
        }
    }
}
