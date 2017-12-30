namespace BaristaLabs.BaristaCore.Modules.Fetch
{
    using RestSharp;
    using RestSharp.Authenticators;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Cache;
    using System.Threading.Tasks;

    public sealed class Request
    {
        private readonly Uri m_inputUri;
        private readonly JsObject m_initObject;

        public Request(string input, JsObject initObject)
        {
            if (!Uri.TryCreate(input, UriKind.Absolute, out Uri inputUri))
                throw new ArgumentException("Input argument must indicate the absolute url of the resource to fetch.");

            m_inputUri = inputUri;
            m_initObject = initObject;
        }

        public Request(JsObject jsObject)
        {
            if (jsObject.TryGetBean(out JsExternalObject exObj) && exObj.Target is Request request)
            {
                m_inputUri = request.m_inputUri;
                m_initObject = request.m_initObject;
            }
            else
            {
                throw new InvalidOperationException("The specified object must be a request object.");
            }
        }

        [BaristaIgnore]
        public Request(Request request)
        {
            m_inputUri = request.m_inputUri;
            m_initObject = request.m_initObject;
        }

        [BaristaProperty(Configurable = false, Writable = false)]
        public Request Clone()
        {
            return new Request(this);
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
                UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36 BaristaCore/1.0",
                Proxy = DummyProxy.GetDefaultProxy()
            };

            var restRequest = new RestRequest
            {
                Method = Method.GET,
                UseDefaultCredentials = true
            };

            ProcessInitObject(m_initObject, restClient, restRequest);

            var restResponse = await restClient.ExecuteTaskAsync(restRequest);
            return new Response(context, restResponse);
        }

        private void ProcessInitObject(JsObject init, RestClient client, IRestRequest request)
        {
            if (init == null)
                return;

            if (init.HasProperty("method"))
            {
                var methodValue = init["method"].ToString();
                if (!String.IsNullOrWhiteSpace(methodValue) && Enum.TryParse(methodValue.ToUpperInvariant(), out Method requestMethod))
                {
                    request.Method = requestMethod;
                }
            }

            if (init.HasProperty("headers") && init["headers"] is JsObject headersValue && headersValue.Type == JsValueType.Object)
            {
                //var headersValue = init["headers"];
                IDictionary<string, IList<string>> headers = null;
                //If it's an instance of the Headers object, sweet.
                if (headersValue.TryGetBean(out JsExternalObject bean) && bean.Target is Headers exHeaders)
                {
                    headers = exHeaders.AllHeaders;
                }
                else if (headersValue is JsObject jsObj && headersValue.Type == JsValueType.Object)
                {
                    //iterate through the keys and set the headers.
                    headers = new Dictionary<string, IList<string>>();
                    foreach (var keyValue in jsObj.Keys)
                    {
                        var key = keyValue.ToString();
                        var value = jsObj[keyValue].ToString();
                        headers.Add(key, new List<string>() { value });
                    }
                }

                if (headers != null)
                {
                    foreach(var header in headers)
                    {
                        request = request.AddHeader(header.Key, String.Join(", ", header.Value));
                    }
                }
            }

            if (init.HasProperty("body"))
            {
                var bodyValue = init["body"];
                switch (bodyValue)
                {
                    case JsArrayBuffer arrayBuffer:
                        var buffer = arrayBuffer.GetArrayBufferStorage();
                        request = request.AddParameter("application/octet-stream", buffer, ParameterType.RequestBody);
                        break;
                    //TODO: Blob, FormData...
                    default:
                        request = request.AddParameter("text/plain", bodyValue.ToString(), ParameterType.RequestBody);
                        break;
                }
            }

            if (init.HasProperty("mode"))
            {
                //Always no-cors.
            }

            if (init.HasProperty("credentials"))
            {
                var credentialsValue = init["credentials"].ToString();
                //TODO: allow for objects that specify an authenticator.
                if (!String.IsNullOrWhiteSpace(credentialsValue))
                {
                    switch (credentialsValue.ToLowerInvariant())
                    {
                        case "omit":
                            request.UseDefaultCredentials = false;
                            break;
                        case "same-origin":
                        case "include":
                            request.UseDefaultCredentials = true;
                            break;
                        //Non-standard
                        case "ntlm":
                            client.Authenticator = new NtlmAuthenticator(CredentialCache.DefaultNetworkCredentials);
                            break;
                    }
                }
            }

            if (init.HasProperty("cache"))
            {
                var cacheValue = init["cache"].ToString();
                if (!String.IsNullOrWhiteSpace(cacheValue))
                {
                    switch (cacheValue.ToLowerInvariant())
                    {
                        case "default":
                            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Default);
                            break;
                        case "no-store":
                            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                            break;
                        case "reload":
                            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Reload);
                            break;
                        case "no-cache":
                            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Revalidate);
                            break;
                        case "force-cache":
                            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
                            break;
                        case "only-if-cached":
                            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheOnly);
                            break;
                        //Non-standard
                        case "no-cache-no-store":
                            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                            break;
                    }
                }
            }

            if (init.HasProperty("redirect"))
            {
                var redirectValue = init["redirect"].ToString();
                if (!String.IsNullOrWhiteSpace(redirectValue))
                {
                    switch (redirectValue.ToLowerInvariant())
                    {
                        case "follow":
                            client.FollowRedirects = true;
                            break;
                        case "error":
                            client.FollowRedirects = false;
                            break;
                        case "manual":
                            client.FollowRedirects = true;
                            client.MaxRedirects = 0;
                            break;
                    }
                }
            }

            if (init.HasProperty("referrer"))
            {
                var referrerValue = init["referrer"].ToString();
                if (!String.IsNullOrWhiteSpace(referrerValue))
                {
                    switch (referrerValue.ToLowerInvariant())
                    {
                        case "no-referrer":
                        case "client":
                            break;
                        default:
                            request = request.AddHeader("Referer", referrerValue);
                            break;
                    }
                }
            }

            if (init.HasProperty("integrity"))
            {
                //n/a
            }

            //Allow some non-standard settings as well.

            if (init.HasProperty("cookies"))
            {
                if (init["cookies"] is JsObject cookiesValue && cookiesValue.Type == JsValueType.Object)
                {
                    foreach (var keyValue in cookiesValue.Keys)
                    {
                        var key = keyValue.ToString();
                        request = request.AddCookie(key, cookiesValue[keyValue].ToString());
                    }
                }
            }

            if (init.HasProperty("proxy"))
            {
                var proxyValue = init["proxy"];
                switch (proxyValue)
                {
                    case JsObject proxyObj:
                        string address = proxyObj["address"].ToString();
                        bool bypassLocal = proxyObj["bypassLocal"].ToBoolean();
                        var credentials = CredentialCache.DefaultNetworkCredentials;
                        if (proxyObj.HasProperty("username") && proxyObj.HasProperty("password"))
                        {
                            credentials = new NetworkCredential(proxyObj["username"].ToString(), proxyObj["username"].ToString());
                        }
                        client.Proxy = new WebProxy(address, bypassLocal, null, credentials);
                        break;
                    default:
                        client.Proxy = new WebProxy(proxyValue.ToString(), true, null, CredentialCache.DefaultNetworkCredentials);
                        break;
                }
            }

            if (init.HasProperty("timeout"))
            {
                var timeoutValue = init["timeout"];
                var timeout = timeoutValue.ToInt32();
                request.Timeout = timeout;
            }

            if (init.HasProperty("user-agent"))
            {
                var userAgentValue = init["user-agent"];
                var userAgent = userAgentValue.ToString();
                client.UserAgent = userAgent;
            }
        }

        private class DummyProxy : IWebProxy
        {
            private static DummyProxy s_defaultDummyProxy = new DummyProxy();

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

            public static DummyProxy GetDefaultProxy()
            {
                return s_defaultDummyProxy;
            }
        }
    }
}
