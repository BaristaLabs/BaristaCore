namespace BaristaLabs.BaristaCore.AspNetCore.Middleware
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ServeMiddleware
    {
        private readonly RequestDelegate m_next;

        public ServeMiddleware(RequestDelegate next)
        {
            m_next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var brewContext = context.Items[BrewKeys.BrewContext] as BaristaContext;
            if (brewContext == null)
                throw new InvalidOperationException("BrewContext was not defined within the http context.");

            var brewResult = context.Items[BrewKeys.BrewResult] as JsValue;
            if (brewResult is JsObject jsObject &&
                jsObject.TryGetBean(out JsExternalObject exObj) &&
                exObj.Target is BrewResponse brewResponseObj
                )
            {
                BrewResponse.PopulateHttpResponse(context.Response, brewContext, brewResponseObj);
                return;
            }
            else if (brewResult != null)
            {
                ResponseValueConverter.PopulateResponseForValue(context.Response, brewContext, brewResult);
                return;
            }

            await m_next(context);
        }

        public static HttpResponseMessage Invoke(BaristaContext brewContext, JsValue brewResult)
        {
            if (brewResult is JsObject jsObject &&
                jsObject.TryGetBean(out JsExternalObject exObj) &&
                exObj.Target is BrewResponse brewResponseObj
                )
            {
                return BrewResponse.CreateResponseMessage(brewContext, brewResponseObj);
            }

            return ResponseValueConverter.CreateResponseMessageForValue(brewContext, brewResult);
        }
    }
}
