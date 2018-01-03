namespace BaristaLabs.BaristaCore.Modules
{
    using System.Threading.Tasks;

    //https://cdnjs.cloudflare.com/ajax/libs/node-uuid/1.4.8/uuid.min.js
    [BaristaModule("uuid", "Rigorous implementation of RFC4122 (v1 and v4) UUIDs.", Version = "1.4.8")]
    public class UuidModule : INodeModule
    {
        public async Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return await EmbeddedResourceHelper.LoadResource(this, "BaristaLabs.BaristaCore.Scripts.uuid.min.js");
        }
    }
}
