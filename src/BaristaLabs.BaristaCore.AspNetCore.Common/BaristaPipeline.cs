namespace BaristaLabs.BaristaCore.AspNetCore
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.TypeScript;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class BaristaPipeline : IBaristaPipeline
    {
        /// <summary>
        /// The HttpClient used by the pipeline.
        /// </summary>
        private static HttpClient s_httpClient = new HttpClient();

        private const string GithubRawUserContentUrl = "https://raw.githubusercontent.com/";

        private const string WebResourceLoader_BaseUrl_EnvironmentVariableName = "Barista_WebResourceLoader_DefaultBaseUrl";
        private const string WebResourceLoader_BaseUrl_HeaderName = "X-Barista-WebResourceLoader-BaseUrl";

        private const string Language_HeaderName_EnvironmentVariableName = "Barista_Language_DefaultLanguage";
        private const string Language_HeaderName = "X-Barista-Language";
        private const string Language_Override_HeaderName = "X-Barista-Language-Override";

        private const string Code_Url_HeaderName = "X-Barista-Code-Url";
        private const string Code_Url_QueryName = "Barista-Code-Url";
        private const string Code_Url_FormName = "Barista-Code-Url";

        public async Task<BrewOrder> TakeOrder(string path, HttpRequest req)
        {
            //Init a standard order.
            var brewOrder = new BrewOrder()
            {
                BaseUrl = GetBrewBaseUrl(req),
                Path = path,
                Code = "export default 6 * 7;",
                Language = GetResourceKindOverride(req),
            };

            //Attempt to use the path as the path to the resource.
            if (await TryGetResource(brewOrder, new Uri(new Uri(brewOrder.BaseUrl, UriKind.Absolute), brewOrder.Path)))
            {
                brewOrder.IsCodeSet = true;
            }

            //Failing that, determine if there's a query string value for Barista-Code-Url
            else if (req.QueryString.HasValue && !String.IsNullOrWhiteSpace(req.Query[Code_Url_QueryName].FirstOrDefault()))
            {
                var queryCodeUrl = req.Query[Code_Url_QueryName].FirstOrDefault();
                if (await TryGetResourceContentByRelativeOrAbsoluteUrl(brewOrder, queryCodeUrl))
                {
                    brewOrder.IsCodeSet = true;
                }
            }
            //Failing that, if it's a form and there's a Barista-Code-Url value
            else if (req.HasFormContentType && req.Form.ContainsKey(Code_Url_FormName))
            {
                var formCodeUrl = req.Form[Code_Url_FormName].FirstOrDefault();
                if (await TryGetResourceContentByRelativeOrAbsoluteUrl(brewOrder, formCodeUrl))
                {
                    brewOrder.IsCodeSet = true;
                }

            }
            //Failing that, determine if there's a value for a header named X-Barista-Code
            else if (req.Headers.ContainsKey(Code_Url_HeaderName))
            {
                var headerCodeValue = req.Headers[Code_Url_HeaderName].FirstOrDefault();
                if (await TryGetResourceContentByRelativeOrAbsoluteUrl(brewOrder, headerCodeValue))
                {
                    brewOrder.IsCodeSet = true;
                }
            }

            //If there is a language override header, use the language header instead.
            if (req.Headers.ContainsKey(Language_Override_HeaderName))
                brewOrder.Language = GetResourceKindOverride(req);

            return brewOrder;
        }

        public IBaristaModuleLoader Tamp(BrewOrder order, HttpRequest req)
        {
            //Set up the module loader for the request.
            var moduleLoader = new PrioritizedAggregateModuleLoader();

            //Register all the modules within the BaristaLabs.BaristaCore.Extensions assembly.
            moduleLoader.RegisterModuleLoader(new AssemblyModuleLoader(typeof(TypeScriptTranspiler).Assembly), 1);

            //Register modules needing context.
            var currentContextModule = new BaristaContextModule(req);

            var contextModuleLoader = new InMemoryModuleLoader();
            contextModuleLoader.RegisterModule(currentContextModule);

            moduleLoader.RegisterModuleLoader(contextModuleLoader, 2);

            //Register the web resource module loader rooted at the target path.
            var path = Path.Combine(order.BaseUrl, order.Path);
            var fileName = Path.GetFileName(path);
            if (Uri.TryCreate(path, UriKind.Absolute, out Uri targetUri))
            {
                var targetPath = targetUri.GetLeftPart(UriPartial.Authority) + String.Join("", targetUri.Segments.Take(targetUri.Segments.Length - 1));
                var webResourceModuleLoader = new WebResourceModuleLoader(targetPath);
                moduleLoader.RegisterModuleLoader(webResourceModuleLoader, 100);
            }

            return moduleLoader;
        }

        public HttpResponseMessage Brew(BrewOrder brewOrder, IBaristaRuntimeFactory baristaRuntimeFactory, IBaristaModuleLoader moduleLoader)
        {
            if (brewOrder == null)
                throw new ArgumentNullException(nameof(brewOrder));

            if (baristaRuntimeFactory == null)
                throw new ArgumentNullException(nameof(baristaRuntimeFactory));

            //Brew:
            using (var rt = baristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            using (var scope = ctx.Scope())
            {
                JsValue result;
                switch (brewOrder.Language)
                {
                    case ResourceKind.Tsx:
                    case ResourceKind.Jsx:
                        result = ctx.EvaluateTypeScriptModule(brewOrder.Code, moduleLoader, true);
                        break;
                    case ResourceKind.TypeScript:
                        result = ctx.EvaluateTypeScriptModule(brewOrder.Code, moduleLoader);
                        break;
                    case ResourceKind.Json:
                        result = ctx.JSON.Parse(ctx.CreateString(brewOrder.Code));
                        break;
                    case ResourceKind.JavaScript:
                        result = ctx.EvaluateModule(brewOrder.Code, moduleLoader);
                        break;
                    default:
                        //Just pass-through the response.
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(Encoding.UTF8.GetBytes(brewOrder.Code.ToString()))
                        };
                }

                return Serve(ctx, result);
            }
        }


        public HttpResponseMessage Serve(BaristaContext ctx, JsValue result)
        {
            if (result is JsObject jsObject &&
                jsObject.TryGetBean(out JsExternalObject exObj) &&
                exObj.Target is BrewResponse brewResponseObj
                )
            {
                return brewResponseObj.CreateResponseMessage(ctx);
            }

            return ResponseValueConverter.CreateResponseMessageForValue(ctx, result);
        }

        /// <summary>
        /// For the given HttpRequest, returns the base url.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string GetBrewBaseUrl(HttpRequest req)
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

        private static ResourceKind GetResourceKindOverride(HttpRequest req)
        {
            var brewLanguage = ResourceKind.JavaScript;

            var appSettingsLanguage = GetAppSetting(Language_HeaderName_EnvironmentVariableName, "JavaScript");
            if (Enum.TryParse(appSettingsLanguage, out ResourceKind appSettingBrewLanguage))
            {
                brewLanguage = appSettingBrewLanguage;
            }

            if (req.Headers.ContainsKey(Language_HeaderName))
            {
                var headersLanguage = req.Headers[Language_HeaderName].FirstOrDefault().ToString();
                if (Enum.TryParse(headersLanguage, out ResourceKind headersBrewLanguage))
                {
                    brewLanguage = headersBrewLanguage;
                }
            }

            return brewLanguage;
        }

        private async Task<bool> TryGetResourceContentByRelativeOrAbsoluteUrl(BrewOrder brewOrder, string absOrRelUrl)
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
        private async Task<bool> TryGetResource(BrewOrder brewOrder, Uri requestUri)
        {
            using (var response = await s_httpClient.GetAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    brewOrder.Code = await response.Content.ReadAsStringAsync();
                    brewOrder.Language = WebResourceModuleLoader.GetCanonicalResourceKind(brewOrder.Path, response.Content.Headers.ContentType.ToString());
                    return true;
                }
            }

            brewOrder.Code = null;
            brewOrder.Language = ResourceKind.Binary;
            return false;
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
