namespace BaristaLabs.BaristaCore.Modules.Fetch
{
    using RestSharp;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public sealed class Request
    {
        private readonly Uri m_inputUri;
        private readonly RestRequest m_restRequest;

        public Request(string input, JsObject init)
        {
            if (!Uri.TryCreate(input, UriKind.Absolute, out Uri inputUri))
                throw new ArgumentException("Input argument must indicate the absolute url of the resource to fetch.");

            m_inputUri = inputUri;
            m_restRequest = new RestRequest();
            m_restRequest.Method = Method.GET;

        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <returns></returns>
        [BaristaIgnore]
        public async Task<Response> Execute(BaristaContext context)
        {
            var restClient = new RestClient(m_inputUri)
            {
                Proxy = new DummyProxy()
            };

            var restResponse = await restClient.ExecuteTaskAsync(m_restRequest);
            return new Response(context, restResponse);
        }

        private class DummyProxy : IWebProxy
        {
            public ICredentials Credentials
            {
                get;
                set;
            }

            public Uri GetProxy(Uri destination)
            {
                return null;
            }

            public bool IsBypassed(Uri host)
            {
                return true;
            }
        }
    }
}
