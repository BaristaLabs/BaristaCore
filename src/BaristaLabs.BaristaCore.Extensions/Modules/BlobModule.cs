namespace BaristaLabs.BaristaCore.Modules
{
    /// <summary>
    /// Module that provides the blob type.
    /// </summary>
    [BaristaModule("barista-blob", "Provides a Blob implementation")]
    public class BlobModule : IBaristaModule
    {
        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            context.Converter.TryFromObject(context, typeof(Blob), out JsValue value);
            return value;
        }
    }
}
