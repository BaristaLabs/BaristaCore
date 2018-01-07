namespace BaristaLabs.BaristaCore.Portafilter
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class BrewResponse
    {
        public BrewResponse(BaristaContext context)
        {
            Headers = new Headers(context);
        }

        public JsObject Body
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }

        public string ContentDisposition
        {
            get;
            set;
        }

        public Headers Headers
        {
            get;
            set;
        }

        public int StatusCode
        {
            get;
            set;
        }

        public string StatusDescription
        {
            get;
            set;
        }

        [BaristaIgnore]
        public HttpResponseMessage CreateResponseMessage(BaristaContext ctx)
        {
            var result = ResponseValueConverter.CreateResponseMessageForValue(ctx, Body);

            result.StatusCode = (HttpStatusCode)StatusCode;
            result.ReasonPhrase = StatusDescription;
            foreach(var header in Headers.AllHeaders)
            {
                result.Headers.Add(header.Key, header.Value);
            }

            if (!string.IsNullOrWhiteSpace(ContentType))
            {
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            }

            if (!string.IsNullOrWhiteSpace(ContentDisposition))
            {
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue(ContentDisposition);
            }
            
            return result;
        }
    }
}
