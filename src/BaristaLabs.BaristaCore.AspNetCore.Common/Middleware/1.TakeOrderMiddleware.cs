namespace BaristaLabs.BaristaCore.AspNetCore.Middleware
{
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public sealed class TakeOrderMiddleware
    {
        /// <summary>
        /// The HttpClient used by the middleware.
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

        private readonly RequestDelegate m_next;

        public TakeOrderMiddleware(RequestDelegate next)
        {
            m_next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var brewOrder = await Invoke("", context.Request);
            context.Items[BrewKeys.BrewOrderKey] = brewOrder;
            await m_next(context);
            context.Items.Remove(BrewKeys.BrewOrderKey);
        }

        public static async Task<BrewOrder> Invoke(string path, HttpRequest req)
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

        /// <summary>
        /// For the given HttpRequest, returns the base url.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
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
