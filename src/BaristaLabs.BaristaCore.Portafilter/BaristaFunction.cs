namespace BaristaLabs.BaristaCore.Portafilter
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.TypeScript;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Text;
    using System.Threading.Tasks;

    public static class BaristaFunction
    {
        private static readonly IBaristaRuntimeFactory m_baristaRuntimeFactory;
        private static readonly HttpClient m_httpClient;

        private const string GithubRawUserContentUrl = "https://raw.githubusercontent.com/";

        private const string WebResourceLoader_BaseUrl_EnvironmentVariableName = "Barista_WebResourceLoader_DefaultBaseUrl";
        private const string WebResourceLoader_BaseUrl_HeaderName = "X-Barista-WebResourceLoader-BaseUrl";

        private const string Language_HeaderName_EnvironmentVariableName = "Barista_Language_DefaultLanguage";
        private const string Language_HeaderName = "X-Barista-Language";
        private const string Language_Override_HeaderName = "X-Barista-Language-Override";

        private const string Code_Url_HeaderName = "X-Barista-Code-Url";
        private const string Code_Url_QueryName = "Barista-Code-Url";
        private const string Code_Url_FormName = "Barista-Code-Url";

        static BaristaFunction()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore();

            var provider = serviceCollection.BuildServiceProvider();
            m_baristaRuntimeFactory = provider.GetRequiredService<IBaristaRuntimeFactory>();
            m_httpClient = new HttpClient();
        }

        [FunctionName("BaristaFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", "patch", "put", "delete", "options", "head", "brew", Route = "{*path}")]HttpRequest req, string path, TraceWriter log)
        {
            log.Info("Barista Function processed a request.");

            //Init a standard order.
            var brewOrder = new BrewOrder()
            {
                BaseUrl = GetBrewBaseUrl(req),
                Path = path,
                Code = "export default 6 * 7;",
                Language = GetBrewLanguage(req),
            };

            if (!await TryTakeOrder(brewOrder, req))
            {
                //Return an exception.
            }

            if (req.Headers.ContainsKey(Language_Override_HeaderName) || brewOrder.Language == BrewLanguage.Unknown)
                brewOrder.Language = GetBrewLanguage(req);

            var moduleLoader = Tamp(brewOrder, req, log);

            try
            {
                return Brew(brewOrder, moduleLoader, req);
            }
            finally
            {
                if (moduleLoader is IDisposable disposableLoader)
                {
                    disposableLoader.Dispose();
                }
            }
        }

        /// <summary>
        /// Takes the order. E.g. From the http request, gets the script that needs to be brewed.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private static async Task<bool> TryTakeOrder(BrewOrder order, HttpRequest req)
        {
            var codeIsSet = false;

            //Attempt to use the path as the path to the resource.
            if (await TryGetResource(order, new Uri(new Uri(order.BaseUrl, UriKind.Absolute), order.Path)))
            {
                codeIsSet = true;
            }

            //Failing that, determine if there's a query string value for Barista-Code-Url
            else if (req.QueryString.HasValue && !String.IsNullOrWhiteSpace(req.Query[Code_Url_QueryName].FirstOrDefault()))
            {
                var queryCodeUrl = req.Query[Code_Url_QueryName].FirstOrDefault();
                if (await TryGetResourceContentByRelativeOrAbsoluteUrl(order, queryCodeUrl))
                {
                    codeIsSet = true;
                }
            }
            //Failing that, if it's a form and there's a Barista-Code-Url value
            else if (req.HasFormContentType && req.Form.ContainsKey(Code_Url_FormName))
            {
                var formCodeUrl = req.Form[Code_Url_FormName].FirstOrDefault();
                if (await TryGetResourceContentByRelativeOrAbsoluteUrl(order, formCodeUrl))
                {
                    codeIsSet = true;
                }

            }
            //Failing that, determine if there's a value for a header named X-Barista-Code
            else if (req.Headers.ContainsKey(Code_Url_HeaderName))
            {
                var headerCodeValue = req.Headers[Code_Url_HeaderName].FirstOrDefault();
                if (await TryGetResourceContentByRelativeOrAbsoluteUrl(order, headerCodeValue))
                {
                    codeIsSet = true;
                }
            }

            return codeIsSet;
        }

        /// <summary>
        /// Tamps the brew request. E.g. Sets up any module loaders that need to be associated as part of the brew.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private static IBaristaModuleLoader Tamp(BrewOrder order, HttpRequest req, TraceWriter log)
        {
            //Set up the module loader for the request.
            var moduleLoader = new PrioritizedAggregateModuleLoader();

            //Register all the modules within the BaristaLabs.BaristaCore.Extensions assembly.
            moduleLoader.RegisterModuleLoader(new AssemblyModuleLoader(typeof(TypeScriptTranspiler).Assembly), 1);

            //Register modules needing context.
            var currentContextModule = new BaristaFunctionContextualModule(req, log);

            var contextModuleLoader = new InMemoryModuleLoader();
            contextModuleLoader.RegisterModule(currentContextModule);

            moduleLoader.RegisterModuleLoader(contextModuleLoader, 2);

            //Register the web resource module loader, using the base url that we obtained previously.
            var webResourceModuleLoader = new WebResourceModuleLoader(order.BaseUrl);
            moduleLoader.RegisterModuleLoader(webResourceModuleLoader, 100);

            return moduleLoader;
        }

        /// <summary>
        /// Pulls the shot of espresso
        /// </summary>
        /// <param name="brewOrder"></param>
        /// <param name="moduleLoader"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        private static HttpResponseMessage Brew(BrewOrder brewOrder, IBaristaModuleLoader moduleLoader, HttpRequest req)
        {
            //Brew:
            using (var rt = m_baristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            using (var scope = ctx.Scope())
            {
                JsValue result;
                switch (brewOrder.Language)
                {
                    case BrewLanguage.Tsx:
                    case BrewLanguage.Jsx:
                        result = ctx.EvaluateTypeScriptModule(brewOrder.Code, moduleLoader, true);
                        break;
                    case BrewLanguage.TypeScript:
                        result = ctx.EvaluateTypeScriptModule(brewOrder.Code, moduleLoader);
                        break;
                    case BrewLanguage.JavaScript:
                    default:
                        result = ctx.EvaluateModule(brewOrder.Code, moduleLoader);
                        break;
                }

                return Serve(ctx, req, result);
            }
        }

        /// <summary>
        /// Serves the brew. E.g. Now that the coffee, now in liquid form, has been extracted, provide the http response back to the requestor.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static HttpResponseMessage Serve(BaristaContext ctx, HttpRequest req, JsValue result)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            switch (result)
            {
                case JsError errorValue:
                    response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        ReasonPhrase = result.ToString()
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
                        Content = new ByteArrayContent(Encoding.UTF8.GetBytes(result.ToString()))
                    };
                    response.Content.Headers.Add("Content-Type", "text/plain");
                    break;
                case JsObject objValue:
                    //if the object contains a bean - attempt to serialize that.
                    if (objValue.Type == JsValueType.Object && objValue.TryGetBean(out JsExternalObject exObj))
                    {
                        switch(exObj.Target)
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
                            case BrewResponse brewResponseObj:
                                //TODO: convert the response obj to a HttpResponseMessage.
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
                        var json = ctx.JSON.Stringify(objValue, null, null);
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
                        Content = new ByteArrayContent(Encoding.UTF8.GetBytes(result.ToString()))
                    };
                    response.Content.Headers.Add("Content-Type", "text/plain");
                    break;
            }

            return response;
        }

        private static string GetBrewBaseUrl(HttpRequest req)
        {
            string resourceLoaderBaseUrl = GetAppSetting(WebResourceLoader_BaseUrl_EnvironmentVariableName, GithubRawUserContentUrl);
            if (req.Headers.ContainsKey(WebResourceLoader_BaseUrl_HeaderName))
            {
                var headerBaseUrl = req.Headers[WebResourceLoader_BaseUrl_HeaderName].FirstOrDefault().ToString();
                if (!string.IsNullOrWhiteSpace(headerBaseUrl))
                    resourceLoaderBaseUrl = headerBaseUrl;
            }
            return resourceLoaderBaseUrl;
        }

        private static BrewLanguage GetBrewLanguage(HttpRequest req)
        {
            var brewLanguage = BrewLanguage.JavaScript;

            var appSettingsLanguage = GetAppSetting(Language_HeaderName_EnvironmentVariableName, "JavaScript");
            if (Enum.TryParse<BrewLanguage>(appSettingsLanguage, out BrewLanguage appSettingBrewLanguage))
            {
                brewLanguage = appSettingBrewLanguage;
            }

            if (req.Headers.ContainsKey(Language_HeaderName))
            {
                var headersLanguage = req.Headers[Language_HeaderName].FirstOrDefault().ToString();
                if (Enum.TryParse<BrewLanguage>(headersLanguage, out BrewLanguage headersBrewLanguage))
                {
                    brewLanguage = headersBrewLanguage;
                }
            }

            return brewLanguage;
        }

        private static async Task<bool> TryGetResourceContentByRelativeOrAbsoluteUrl(BrewOrder brewOrder, string absOrRelUrl)
        {
            if (Uri.TryCreate(absOrRelUrl, UriKind.Absolute, out Uri absoluteResourceUri) &&
                    await TryGetResource(brewOrder, absoluteResourceUri))
            {
                return true;
            }
            else if (Uri.TryCreate(brewOrder.BaseUrl + absOrRelUrl, UriKind.Absolute, out Uri basePlusRelativeResourceUri) &&
                     await TryGetResource(brewOrder, basePlusRelativeResourceUri))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to retrieve the resource using a GET request.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="resource"></param>
        /// <param name="content"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        private static async Task<bool> TryGetResource(BrewOrder brewOrder, Uri requestUri)
        {
            using (var response = await m_httpClient.GetAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    brewOrder.Code = await response.Content.ReadAsStringAsync();
                    brewOrder.Language = GetBrewLanguageFromResponse(response);
                    return true;
                }
            }

            brewOrder.Code = null;
            brewOrder.Language = BrewLanguage.Unknown;
            return false;
        }

        private static BrewLanguage GetBrewLanguageFromResponse(HttpResponseMessage response)
        {
            var path = response.RequestMessage.RequestUri.ToString();

            var filename = Path.GetFileName(path);
            if (!String.IsNullOrWhiteSpace(filename))
            {
                var ext = Path.GetExtension(path);
                switch (ext)
                {
                    case ".js":
                        return BrewLanguage.JavaScript;
                    case ".jsx":
                        return BrewLanguage.Jsx;
                    case ".ts":
                        return BrewLanguage.TypeScript;
                    case ".tsx":
                        return BrewLanguage.Tsx;
                }
            }

            var contentType = response.Content.Headers.ContentType.MediaType;

            if (String.IsNullOrWhiteSpace(contentType))
                contentType = String.Empty;

            switch (contentType.ToLowerInvariant())
            {
                case "text/js":
                case "text/javascript":
                case "application/javascript":
                    return BrewLanguage.JavaScript;
                case "text/jsx":
                case "application/x-javascript+xml":
                case "application/vnd.facebook.javascript+xml":
                    return BrewLanguage.Jsx;
                case "text/ts":
                case "text/typescript":
                case "application/x-typescript":
                case "application/vnd.microsoft.typescript":
                    return BrewLanguage.TypeScript;
                case "text/tsx":
                case "application/x-typescript+xml":
                case "application/vnd.microsoft.typescript+xml":
                    return BrewLanguage.Tsx;
            }

            return BrewLanguage.Unknown;
        }

        private static string GetAppSetting(string name, string defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
            if (String.IsNullOrWhiteSpace(value))
                return defaultValue;
            return value;
        }
    }
}
