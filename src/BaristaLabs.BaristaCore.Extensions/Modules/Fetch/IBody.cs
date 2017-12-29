namespace BaristaLabs.BaristaCore.Modules.Fetch
{
    using System.Threading.Tasks;

    public interface IBody
    {
        bool BodyUsed
        {
            get;
        }

        [BaristaProperty(Configurable = false, Writable = false)]
        Task<JsArrayBuffer> ArrayBuffer();

        [BaristaProperty(Configurable = false, Writable = false)]
        Task<JsObject> Blob();

        [BaristaProperty(Configurable = false, Writable = false)]
        Task<JsObject> FormData();

        [BaristaProperty(Configurable = false, Writable = false)]
        Task<JsValue> Json();

        [BaristaProperty(Configurable = false, Writable = false)]
        Task<JsString> Text();
    }
}
