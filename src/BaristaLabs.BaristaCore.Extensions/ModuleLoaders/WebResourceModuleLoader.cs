namespace BaristaLabs.BaristaCore.ModuleLoaders
{
    using System;
    using System.Threading.Tasks;
    using RestSharp;

    /// <summary>
    /// Represents a module loader that loads script modules from a web-based resource.
    /// </summary>
    public class WebResourceModuleLoader : IBaristaModuleLoader
    {
        private readonly Func<IRestClient> m_restClientBuilder;

        public WebResourceModuleLoader(string baseUrl)
        {
            m_restClientBuilder = new Func<IRestClient>(() =>
            {
                return new RestClient(baseUrl);
            });
        }

        public WebResourceModuleLoader(Func<IRestClient> fnRestClientBuilder)
        {
            m_restClientBuilder = fnRestClientBuilder ?? throw new ArgumentNullException(nameof(fnRestClientBuilder));
        }

        public async Task<IBaristaModule> GetModule(string name)
        {
            var restClient = m_restClientBuilder();
            var request = new RestRequest(name, Method.GET);
            var response = await restClient.ExecuteTaskAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 400)
            {
                var scriptModule = new BaristaScriptModule(name, "Script loaded via WebResourceModuleLoader")
                {
                    Script = response.Content
                };

                return scriptModule;
            }

            return null;
        }
    }
}
