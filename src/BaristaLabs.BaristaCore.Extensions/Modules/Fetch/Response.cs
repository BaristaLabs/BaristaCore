namespace BaristaLabs.BaristaCore.Modules.Fetch
{
    using RestSharp;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class Response : IBody
    {
        private readonly BaristaContext m_context;
        private readonly IRestResponse m_response;
        private readonly IBody m_body;
        private readonly Headers m_headers;

        public Response(BaristaContext context, Body body, JsObject initObject)
        {
            m_context = context;
            m_body = body;
            throw new NotImplementedException();
        }

        [BaristaIgnore]
        public Response(BaristaContext context, IRestResponse response)
        {
            m_context = context ?? throw new ArgumentNullException(nameof(context));
            m_response = response ?? throw new ArgumentNullException(nameof(response));
            m_body = new Body(m_context, m_response);
            m_headers = new Headers(context);
            foreach (var header in m_response.Headers.Where(h => h.Type == ParameterType.HttpHeader))
            {
                m_headers.Append(header.Name, header.Value.ToString());
            }
        }

        [BaristaIgnore]
        public Response(Response response)
        {
            m_context = response.m_context;
            m_response = response.m_response;
            m_body = new Body(m_context, m_response);
            m_headers = new Headers(m_context);
            foreach (var header in m_response.Headers.Where(h => h.Type == ParameterType.HttpHeader))
            {
                m_headers.Append(header.Name, header.Value.ToString());
            }
        }

        [BaristaProperty(Configurable = false, Writable = false)]
        public Headers Headers
        {
            get
            {
                return m_headers;
            }
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

        [BaristaProperty(Configurable = false, Writable = false)]
        public bool BodyUsed
        {
            get { return m_body.BodyUsed; }
        }

        public Response Clone()
        {
            return new Response(this);
        }

        public Task<JsArrayBuffer> ArrayBuffer()
        {
            return m_body.ArrayBuffer();
        }

        public Task<JsObject> Blob()
        {
            return m_body.Blob();
        }

        public Task<JsObject> FormData()
        {
            return m_body.FormData();
        }

        public Task<JsValue> Json()
        {
            return m_body.Json();
        }

        public Task<JsString> Text()
        {
            return m_body.Text();
        }
    }
}
