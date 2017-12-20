namespace BaristaLabs.BaristaCore.Modules.Fetch
{
    using System;
    using System.Threading.Tasks;

    public class Body
    {
        private bool m_bodyUsed = false;

        public Task<JsObject> Blob()
        {
            throw new NotImplementedException();
        }

        public Task<JsArrayBuffer> ArrayBuffer()
        {
            throw new NotImplementedException();
        }

        public Task<JsString> Text()
        {
            throw new NotImplementedException();
        }

        public Task<JsObject> Json()
        {
            throw new NotImplementedException();
        }
    }
}
