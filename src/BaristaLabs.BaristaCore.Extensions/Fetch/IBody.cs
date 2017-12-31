namespace BaristaLabs.BaristaCore
{
    using System.Threading.Tasks;

    public interface IBody
    {
        bool BodyUsed
        {
            get;
        }

        [BaristaProperty(Configurable = false, Writable = false)]
        Task<JsValue> ArrayBuffer();

        [BaristaProperty(Configurable = false, Writable = false)]
        Task<JsValue> Blob();

        [BaristaProperty(Configurable = false, Writable = false)]
        Task<JsValue> FormData();

        [BaristaProperty(Configurable = false, Writable = false)]
        Task<JsValue> Json();

        [BaristaProperty(Configurable = false, Writable = false)]
        Task<JsValue> Text();
    }
}
