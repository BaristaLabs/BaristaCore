namespace BaristaLabs.BaristaCore.Modules.Fetch
{
    using System.Threading.Tasks;

    public interface IBody
    {
        bool BodyUsed
        {
            get;
        }

        Task<JsArrayBuffer> ArrayBuffer();

        Task<JsObject> Blob();

        Task<JsObject> FormData();

        Task<JsObject> Json();

        Task<JsString> Text();
    }
}
