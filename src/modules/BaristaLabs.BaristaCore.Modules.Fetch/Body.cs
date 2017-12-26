namespace BaristaLabs.BaristaCore.Modules.Fetch
{
    using System;
    using System.Threading.Tasks;

    public class Body : IBody
    {
        private bool m_bodyUsed = false;

        public bool BodyUsed
        {
            get { return m_bodyUsed; }
        }

        public Task<JsArrayBuffer> ArrayBuffer()
        {
            throw new NotImplementedException();
        }

        public Task<JsObject> Blob()
        {
            throw new NotImplementedException();
        }

        public Task<JsObject> FormData()
        {
            throw new NotImplementedException();
        }
        public Task<JsObject> Json()
        {
            throw new NotImplementedException();
        }

        public Task<JsString> Text()
        {
            throw new NotImplementedException();
        }
    }
}
