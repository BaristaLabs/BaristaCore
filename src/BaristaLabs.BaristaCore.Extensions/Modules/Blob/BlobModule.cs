namespace BaristaLabs.BaristaCore.Modules.Blob
{
    using System;
    using System.Threading.Tasks;

    [BaristaModule("barista-blob", "Provides a Blob implementation")]
    public class BlobModule : IBaristaModule
    {
        public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            throw new NotImplementedException();
        }
    }
}
