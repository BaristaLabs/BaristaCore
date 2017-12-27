namespace BaristaLabs.BaristaCore.Modules.Fetch
{
    using RestSharp;
    using System;
    using System.Threading.Tasks;

    public sealed class Body : IBody
    {
        private bool m_bodyUsed = false;
        private readonly BaristaContext m_context;
        private readonly IRestResponse m_response;

        [BaristaIgnore]
        public Body(BaristaContext context, IRestResponse response)
        {
            m_context = context ?? throw new ArgumentNullException(nameof(context));
            m_response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public bool BodyUsed
        {
            get { return m_bodyUsed; }
        }

        public Task<JsArrayBuffer> ArrayBuffer()
        {
            if (TryCheckBodyUsed())
            {
                return Task.FromResult<JsArrayBuffer>(null);
            }

            var result = m_context.ValueFactory.CreateArrayBuffer(m_response.RawBytes);
            m_bodyUsed = true;

            return Task.FromResult(result);
        }

        public Task<JsObject> Blob()
        {
            if (TryCheckBodyUsed())
            {
                return Task.FromResult<JsObject>(null);
            }

            throw new NotImplementedException();
        }

        public Task<JsObject> FormData()
        {
            if (TryCheckBodyUsed())
            {
                return Task.FromResult<JsObject>(null);
            }

            throw new NotImplementedException();
            //return Task.FromResult(result);
        }
        public Task<JsValue> Json()
        {
            if (TryCheckBodyUsed())
            {
                return Task.FromResult<JsValue>(null);
            }

            var result = m_context.JSON.Parse(m_context.ValueFactory.CreateString(m_response.Content));
            m_bodyUsed = true;

            return Task.FromResult(result);
        }

        public Task<JsString> Text()
        {
            if (TryCheckBodyUsed())
            {
                return Task.FromResult<JsString>(null);
            }

            var result = m_context.ValueFactory.CreateString(m_response.Content);
            m_bodyUsed = true;

            return Task.FromResult(result);
        }

        /// <summary>
        /// Returns a value that indicates if the body object has already been used. If it already has, sets an exception.
        /// </summary>
        /// <returns></returns>
        private bool TryCheckBodyUsed()
        {
            if (m_bodyUsed)
            {
                var err = m_context.ValueFactory.CreateTypeError("The body object has already been read.");
                m_context.CurrentScope.SetException(err);
                return true;
            }

            return false;
        }
    }
}
