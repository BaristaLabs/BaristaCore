namespace BaristaLabs.BaristaCore.Portafilter
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.DependencyInjection;
    using RestSharp;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public static class BaristaFunction
    {
        private static IBaristaRuntimeFactory m_baristaRuntimeFactory;

        private const string GithubRawUserContentUrl = "https://raw.githubusercontent.com/";

        private const string WebResourceLoader_BaseUrl_EnvironmentVariableName = "Barista_WebResourceLoader_DefaultBaseUrl";
        private const string WebResourceLoader_BaseUrl_HeaderName = "X-Barista-WebResourceLoader-BaseUrl";

        private const string Language_HeaderName_EnvironmentVariableName = "Barista_Language_DefaultLanguage";
        private const string Language_HeaderName = "X-Barista-Language";

        private const string Code_Url_HeaderName = "X-Barista-Code-Url";
        private const string Code_Url_QueryName = "Barista-Code-Url";
        private const string Code_Url_FormName = "Barista-Code-Url";

        static BaristaFunction()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore();

            var provider = serviceCollection.BuildServiceProvider();
            m_baristaRuntimeFactory = provider.GetRequiredService<IBaristaRuntimeFactory>();
        }

        [FunctionName("BaristaFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", "patch", "put", "delete", "options", "head", "brew", Route = "{*path}")]HttpRequest req, string path, TraceWriter log)
        {
            log.Info("Barista Function processed a request.");

            //Init a standard order.
            var brewOrder = new BrewOrder()
            {
                BaseUrl = GetBrewBaseUrl(req),
                Code = "export default 6 * 7;",
                Language = GetBrewLanguage(req)
            };

            if (!TryTakeOrder(brewOrder, req, path))
            {
                //Return an exception.
            }

            var moduleLoader = Tamp(brewOrder, req, log);

            try
            { 
                //Brew:
                using (var rt = m_baristaRuntimeFactory.CreateRuntime())
                using (var ctx = rt.CreateContext())
                using (var scope = ctx.Scope())
                {
                    JsValue result;
                    switch(brewOrder.Language)
                    {
                        default:
                            result = ctx.EvaluateModule(brewOrder.Code, moduleLoader);
                            break;
                    }

                    return Serve(req, result);
                }
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
        private static bool TryTakeOrder(BrewOrder order, HttpRequest req, string path)
        {
            var codeIsSet = false;

            //Attempt to use the path as the path to the resource.
            if (Uri.TryCreate(order.BaseUrl + path, UriKind.Absolute, out Uri resourceUri) && TryGetResource(resourceUri, out string content))
            {
                order.Code = content;
                codeIsSet = true;
            }

            //Failing that, determine if there's a query string value for Barista-Code-Url
            else if (req.QueryString.HasValue && !String.IsNullOrWhiteSpace(req.Query[Code_Url_QueryName].FirstOrDefault()))
            {
                var queryCodeUrl = req.Query[Code_Url_QueryName].FirstOrDefault();
                if (TryGetResourceContentByRelativeOrAbsoluteUrl(queryCodeUrl, order.BaseUrl, out string queryCodeContent))
                {
                    order.Code = queryCodeContent;
                    codeIsSet = true;
                }
            }
            //Failing that, if it's a form and there's a Barista-Code-Url value
            else if (req.HasFormContentType && req.Form.ContainsKey(Code_Url_FormName))
            {
                var formCodeUrl = req.Form[Code_Url_FormName].FirstOrDefault();
                if (TryGetResourceContentByRelativeOrAbsoluteUrl(formCodeUrl, order.BaseUrl, out string formCodeContent))
                {
                    order.Code = formCodeContent;
                    codeIsSet = true;
                }

            }
            //Failing that, determine if there's a value for a header named X-Barista-Code
            else if (req.Headers.ContainsKey(Code_Url_HeaderName))
            {
                var headerCodeValue = req.Headers[Code_Url_HeaderName].FirstOrDefault();
                if (TryGetResourceContentByRelativeOrAbsoluteUrl(headerCodeValue, order.BaseUrl, out string headerCodeContent))
                {
                    order.Code = headerCodeContent;
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
            moduleLoader.RegisterModuleLoader(new AssemblyModuleLoader(typeof(AssemblyModuleLoader).Assembly), 1);

            //Register modules needing context.
            var currentContextModule = new BaristaFunctionContextualModule(req, log);

            var contextModuleLoader = new InMemoryModuleLoader();
            contextModuleLoader.RegisterModule(currentContextModule);

            moduleLoader.RegisterModuleLoader(contextModuleLoader, 2);

            var webResourceModuleLoader = new WebResourceModuleLoader(order.BaseUrl);
            moduleLoader.RegisterModuleLoader(webResourceModuleLoader, 100);

            return moduleLoader;
        }

        /// <summary>
        /// Serves the brew. E.g. Now that the coffee, now in liquid form, has been extracted, provide the http response back to the requestor.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static HttpResponseMessage Serve(HttpRequest req, JsValue result)
        {
            //HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.BadRequest);
            //switch(result)
            //{
            //    case JsError errorValue:
            //        return new BadRequestObjectResult(errorValue.ToString());
            //}
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(Encoding.UTF8.GetBytes(result.ToString()))
            };

            response.Content.Headers.Add("Content-Type", "text/plain");
            
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

        private static bool TryGetResourceContentByRelativeOrAbsoluteUrl(string absOrRelUrl, string baseUrl, out string content)
        {
            if (Uri.TryCreate(absOrRelUrl, UriKind.Absolute, out Uri queryCodeAbsResouceUri) &&
                    TryGetResource(queryCodeAbsResouceUri, out content))
            {
                return true;
            }
            else if (Uri.TryCreate(baseUrl + absOrRelUrl, UriKind.Absolute, out Uri queryCodeRelResouceUri) &&
                     TryGetResource(queryCodeRelResouceUri, out content))
            {
                return true;
            }

            content = null;
            return false;
        }

        private static bool TryGetResource(Uri resource, out string content)
        {
            var client = new RestClient(resource);
            client.Proxy = new DummyProxy();
            var request = new RestRequest();
            var response = client.ExecuteAsGet(request, "get");
            if (response.IsSuccessful)
            {
                content = response.Content;
                return true;
            }
            content = null;
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
