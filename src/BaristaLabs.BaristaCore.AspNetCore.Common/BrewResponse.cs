namespace BaristaLabs.BaristaCore.AspNetCore
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.Extensions.Primitives;
    using System.Linq;
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

        [BaristaIgnore]
        public static void PopulateHttpResponse(HttpResponse response, BaristaContext brewContext, BrewResponse brewResponse)
        {
            response.StatusCode = brewResponse.StatusCode;
            response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = brewResponse.StatusDescription;

            foreach(var header in brewResponse.Headers.AllHeaders)
            {
                var values = new StringValues(header.Value.ToArray());
                response.Headers.Add(header.Key, values);
            }

            if (!string.IsNullOrWhiteSpace(brewResponse.ContentType))
            {
                response.ContentType = brewResponse.ContentType;
            }

            if (!string.IsNullOrWhiteSpace(brewResponse.ContentDisposition))
            {
                response.Headers["Content-Disposition"] = brewResponse.ContentDisposition;
            }

            ResponseValueConverter.PopulateResponseForValue(response, brewContext, brewResponse.Body, false);
        }
    }
}
