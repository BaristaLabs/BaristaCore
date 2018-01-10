namespace BaristaLabs.BaristaCore.AspNetCore
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    public static class ResponseValueConverter
    {
        public static void PopulateResponseForValue(HttpResponse response, BaristaContext context, JsValue value, bool setHeaders = true)
        {
            Action setHeaderAction;
            byte[] bodyBuffer;

            switch(value)
            {
                case JsError errorValue:
                    setHeaderAction = () =>
                    {
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = value.ToString();
                    };

                    //TODO: Set response body.
                    bodyBuffer = Encoding.UTF8.GetBytes(String.Empty);
                    break;
                case JsArrayBuffer arrayBufferValue:
                    setHeaderAction = () =>
                    {
                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.ContentType = "application/octet-stream";
                    };

                    bodyBuffer = arrayBufferValue.GetArrayBufferStorage();
                    break;
                case JsBoolean booleanValue:
                case JsNumber numberValue:
                case JsString stringValue:
                    setHeaderAction = () =>
                    {
                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.ContentType = new MediaTypeHeaderValue("text/plain").ToString();
                    };

                    bodyBuffer = Encoding.UTF8.GetBytes(value.ToString());
                    break;
                case JsObject objValue:
                    //if the object contains a bean - attempt to serialize that.
                    if (objValue.Type == JsValueType.Object && objValue.TryGetBean(out JsExternalObject exObj))
                    {
                        switch (exObj.Target)
                        {
                            case Blob blobObj:

                                setHeaderAction = () =>
                                {
                                    response.StatusCode = (int)HttpStatusCode.OK;
                                    response.ContentType = blobObj.Type;

                                    if (!String.IsNullOrWhiteSpace(blobObj.Disposition))
                                        response.Headers.Add("Content-Disposition", blobObj.Disposition);
                                };

                                bodyBuffer = blobObj.Data;
                                break;
                            default:

                                setHeaderAction = () =>
                                {
                                    response.StatusCode = (int)HttpStatusCode.OK;
                                    response.ContentType = "application/json; charset=utf-8";
                                };

                                //Use Json.net to serialize the object to a string.
                                var jsonObj = JsonConvert.SerializeObject(exObj);
                                bodyBuffer = Encoding.UTF8.GetBytes(jsonObj);
                                break;
                        }
                    }
                    else
                    {
                        setHeaderAction = () =>
                        {
                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.ContentType = "application/json; charset=utf-8";
                        };

                        //Use the built-in JSON object to serialize the jsValue to a string.
                        var json = context.JSON.Stringify(objValue, null, null);
                        bodyBuffer = Encoding.UTF8.GetBytes(json);
                    }
                    break;
                default:
                    setHeaderAction = () =>
                    {
                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.ContentType = "text/plain; charset=utf-8";
                    };

                    var responseTextBody = Encoding.UTF8.GetBytes(value.ToString());
                    bodyBuffer = responseTextBody;
                    break;
            }

            if (setHeaders)
                setHeaderAction();

            response.Body.Write(bodyBuffer, 0, bodyBuffer.Length);
        }

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
                    //TODO: Set response body
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
                                //Use Json.net to serialize the object to a string.
                                var jsonObj = JsonConvert.SerializeObject(exObj);
                                response = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new StringContent(jsonObj, Encoding.UTF8, "application/json")
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
