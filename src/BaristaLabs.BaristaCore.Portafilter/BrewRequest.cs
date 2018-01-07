namespace BaristaLabs.BaristaCore.Portafilter
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class BrewRequest : IBody
    {
        private bool m_bodyUsed = false;
        private readonly BaristaContext m_context;
        private readonly HttpRequest m_httpRequest;
        private readonly Headers m_headers;

        public BrewRequest()
        {
        }

        [BaristaIgnore]
        public BrewRequest(BaristaContext context, HttpRequest request)
        {
            m_context = context ?? throw new ArgumentNullException(nameof(context));
            m_httpRequest = request ?? throw new ArgumentNullException(nameof(request));

            m_headers = new Headers(context);
            foreach(var header in request.Headers)
            {
                m_headers.Append(header.Key, header.Value);
            }
        }

        public Headers Headers
        {
            get { return m_headers; }
        }

        public bool BodyUsed
        {
            get { return m_bodyUsed; }
        }

        public string QueryString
        {
            get { return m_httpRequest.QueryString.Value; }
        }

        public Task<JsValue> ArrayBuffer()
        {
            var result = m_context.CreateArrayBuffer(ReadFully(m_httpRequest.Body));
            m_bodyUsed = true;

            return Task.FromResult<JsValue>(result);
        }

        public Task<JsValue> Blob()
        {
            if (TryCheckBodyUsed())
            {
                return Task.FromResult<JsValue>(m_context.Undefined);
            }

            var result = new Blob(ReadFully(m_httpRequest.Body), m_httpRequest.ContentType);
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
        }

        public Task<JsValue> Json()
        {
            if (TryCheckBodyUsed())
            {
                return Task.FromResult<JsValue>(m_context.Undefined);
            }

            var bodyBytes = ReadFully(m_httpRequest.Body);
            var headers = m_httpRequest.GetTypedHeaders();
            var bodyText = headers.ContentType.Encoding.GetString(bodyBytes);

            var result = m_context.JSON.Parse(m_context.CreateString(bodyText));
            m_bodyUsed = true;

            return Task.FromResult(result);
        }

        public Task<JsValue> Text()
        {
            if (TryCheckBodyUsed())
            {
                return Task.FromResult<JsValue>(m_context.Undefined);
            }

            var bodyBytes = ReadFully(m_httpRequest.Body);
            var headers = m_httpRequest.GetTypedHeaders();
            var bodyText = headers.ContentType.Encoding.GetString(bodyBytes);

            var result = m_context.CreateString(bodyText);
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

        private byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
