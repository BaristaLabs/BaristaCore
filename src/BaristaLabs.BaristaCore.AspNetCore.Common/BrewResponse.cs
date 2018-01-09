namespace BaristaLabs.BaristaCore.AspNetCore
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

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
        public static HttpResponseMessage CreateResponseMessage(BaristaContext ctx, BrewResponse response)
        {
            var result = ResponseValueConverter.CreateResponseMessageForValue(ctx, response.Body);

            result.StatusCode = (HttpStatusCode)response.StatusCode;
            result.ReasonPhrase = response.StatusDescription;
            foreach(var header in response.Headers.AllHeaders)
            {
                result.Headers.Add(header.Key, header.Value);
            }

            if (!string.IsNullOrWhiteSpace(response.ContentType))
            {
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(response.ContentType);
            }

            if (!string.IsNullOrWhiteSpace(response.ContentDisposition))
            {
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue(response.ContentDisposition);
            }
            
            return result;
        }

        public static void PopulateRequest(HttpRequest request, BaristaContext brewContext, BrewResponse brewResponseObj)
        {
            throw new NotImplementedException();
        }
    }
}
