namespace BaristaLabs.BaristaCore.Modules
{
    using System.Threading.Tasks;

    /// <summary>
    /// Module that provides the blob type.
    /// </summary>
    [BaristaModule("barista-blob", "Provides a Blob implementation")]
    public class BlobModule : IBaristaModule
    {
        public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return Task.FromResult<object>(typeof(Blob));
        }
    }
}
