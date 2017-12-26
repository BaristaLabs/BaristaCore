namespace BaristaLabs.BaristaCore.Modules.Fetch
{
    using RestSharp;
    using System;
    using System.Threading.Tasks;

    public sealed class Response : IBody
    {
        private readonly BaristaContext m_context;
        private readonly IRestResponse m_response;
        public Response()
        {
        }

        public Response(BaristaContext context, IRestResponse response)
        {
            m_context = context ?? throw new ArgumentNullException(nameof(context));
            m_response = response ?? throw new ArgumentNullException(nameof(response));
        }

        [BaristaProperty(Configurable = false, Writable = false)]
        public bool Ok
        {
            get
            {
                return ((int)m_response.StatusCode >= 200 && (int)m_response.StatusCode < 300);
            }
        }

        [BaristaProperty(Configurable = false, Writable = false)]
        public string StatusText
        {
            get
            {
                return m_response.StatusDescription;
            }
        }

        public bool BodyUsed
        {
            get;
            private set;
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
            return Task.FromResult(m_context.ValueFactory.CreateString(m_response.Content));
        }
    }
}
