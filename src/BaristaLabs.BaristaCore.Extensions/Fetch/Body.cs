namespace BaristaLabs.BaristaCore
{
    using RestSharp;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the body of the response/request, allowing you to declare what its content type is and how it should be handled.
    /// </summary>
    /// <see cref="https://developer.mozilla.org/en-US/docs/Web/API/Body"/>
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

        public Task<JsValue> ArrayBuffer()
        {
            if (TryCheckBodyUsed())
            {
                return Task.FromResult<JsValue>(m_context.Undefined);
            }

            var foo = m_response.ContentEncoding;
            var result = m_context.CreateArrayBuffer(m_response.RawBytes);
            m_bodyUsed = true;

            return Task.FromResult<JsValue>(result);
        }

        public Task<JsValue> Blob()
        {
            if (TryCheckBodyUsed())
            {
                return Task.FromResult<JsValue>(m_context.Undefined);
            }

            var result = new Blob(m_response.RawBytes, m_response.ContentType);
            m_bodyUsed = true;

            if (m_context.Converter.TryFromObject(m_context, result, out JsValue jsResult))
            {
                return Task.FromResult(jsResult);
            }

            return Task.FromResult<JsValue>(m_context.Undefined);
        }

        public Task<JsValue> FormData()
        {
            if (TryCheckBodyUsed())
            {
                return Task.FromResult<JsValue>(m_context.Undefined);
            }

            throw new NotImplementedException();
            //return Task.FromResult(result);
        }

        public Task<JsValue> Json()
        {
            if (TryCheckBodyUsed())
            {
                return Task.FromResult<JsValue>(m_context.Undefined);
            }

            var result = m_context.JSON.Parse(m_context.CreateString(m_response.Content));
            m_bodyUsed = true;

            return Task.FromResult(result);
        }

        public Task<JsValue> Text()
        {
            if (TryCheckBodyUsed())
            {
                return Task.FromResult<JsValue>(m_context.Undefined);
            }

            var result = m_context.CreateString(m_response.Content);
            m_bodyUsed = true;

            return Task.FromResult<JsValue>(result);
        }

        /// <summary>
        /// Returns a value that indicates if the body object has already been used. If it already has, sets an exception.
        /// </summary>
        /// <returns></returns>
        private bool TryCheckBodyUsed()
        {
            if (m_bodyUsed)
            {
                var err = m_context.CreateTypeError("The body object has already been read.");
                m_context.CurrentScope.SetException(err);
                return true;
            }

            return false;
        }
    }
}
