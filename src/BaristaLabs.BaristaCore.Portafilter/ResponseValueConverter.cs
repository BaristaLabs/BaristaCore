namespace BaristaLabs.BaristaCore.Portafilter
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Text;

    public static class ResponseValueConverter
    {
        public static HttpResponseMessage CreateResponseMessageForValue(BaristaContext context, JsValue value)
        {
            HttpResponseMessage response;

            switch (value)
            {
                case JsError errorValue:
                    response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        ReasonPhrase = value.ToString()
                    };
                    break;
                case JsArrayBuffer arrayBufferValue:
                    response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(arrayBufferValue.GetArrayBufferStorage())
                    };
                    response.Content.Headers.Add("Content-Type", "application/octet-stream");
                    break;
                case JsBoolean booleanValue:
                case JsNumber numberValue:
                case JsString stringValue:
                    response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(Encoding.UTF8.GetBytes(value.ToString()))
                    };
                    response.Content.Headers.Add("Content-Type", "text/plain");
                    break;
                case JsObject objValue:
                    //if the object contains a bean - attempt to serialize that.
                    if (objValue.Type == JsValueType.Object && objValue.TryGetBean(out JsExternalObject exObj))
                    {
                        switch (exObj.Target)
                        {
                            case Blob blobObj:
                                response = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new ByteArrayContent(blobObj.Data)
                                };
                                response.Content.Headers.Add("Content-Type", blobObj.Type);
                                if (!String.IsNullOrWhiteSpace(blobObj.Disposition))
                                    response.Content.Headers.Add("Content-Disposition", blobObj.Disposition);
                                break;
                            default:
                                response = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new ObjectContent(exObj.Target.GetType(), exObj, new JsonMediaTypeFormatter())
                                };
                                break;
                        }
                    }
                    else
                    {
                        var json = context.JSON.Stringify(objValue, null, null);
                        response = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(Encoding.UTF8.GetBytes(json.ToString()))
                        };
                        response.Content.Headers.Add("Content-Type", "application/json");
                    }
                    break;
                default:
                    response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(Encoding.UTF8.GetBytes(value.ToString()))
                    };
                    response.Content.Headers.Add("Content-Type", "text/plain");
                    break;
            }

            return response;
        }
    }
}
