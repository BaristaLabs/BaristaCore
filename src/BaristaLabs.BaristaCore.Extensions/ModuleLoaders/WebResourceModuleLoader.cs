namespace BaristaLabs.BaristaCore.ModuleLoaders
{
    using BaristaLabs.BaristaCore.Modules;
    using RestSharp;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a module loader that loads script modules from a web-based resource.
    /// </summary>
    public class WebResourceModuleLoader : IBaristaModuleLoader
    {
        const string ModuleDescription = "Script loaded via WebResourceModuleLoader";

        private readonly Func<IRestClient> m_restClientBuilder;
        private readonly Uri m_baseUri;

        public WebResourceModuleLoader(string baseUrl)
        {
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri baseUri))
            {
                throw new ArgumentException("The base url of a module loader must be an absolute url.", nameof(baseUrl));
            }

            m_baseUri = baseUri;
            m_restClientBuilder = new Func<IRestClient>(() =>
            {
                var client = new RestClient(baseUrl)
                {
                    Proxy = new DummyProxy(),
                    Timeout = 60000
                };
                return client;
            });
        }

        public WebResourceModuleLoader(Func<IRestClient> fnRestClientBuilder)
        {
            m_restClientBuilder = fnRestClientBuilder ?? throw new ArgumentNullException(nameof(fnRestClientBuilder));
        }

        public Task<IBaristaModule> GetModule(string name)
        {
            var fullPath = Path.Combine(m_baseUri.ToString(), name);
            var relativePath = new Uri(fullPath);
            var targetPath = m_baseUri.MakeRelativeUri(relativePath);

            var restClient = m_restClientBuilder();
            var request = new RestRequest(targetPath, Method.GET);
            //RestClient's Async Methods seem to be queued on a single thread
            //If this is async, it conflicts with fetch (if that is async too)
            //So we're making them all syncronous. :-/
            var response = restClient.Execute(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 400)
            {
                var filename = Path.GetFileName(response.ResponseUri.ToString());

                //Based on file's type, return an appropriate module.
                switch (GetCanonicalFileType(response.ResponseUri.ToString(), response.ContentType))
                {
                    case ResourceKind.JavaScript:
                        var scriptModule = new BaristaScriptModule(name, ModuleDescription)
                        {
                            Script = response.Content
                        };

                        return Task.FromResult<IBaristaModule>(scriptModule);
                    case ResourceKind.Jsx:
                    case ResourceKind.Tsx:
                    case ResourceKind.TypeScript:
                        var typeScriptModule = new BaristaTypeScriptModule(name, ModuleDescription, String.IsNullOrWhiteSpace(filename) ? "script.js" : filename)
                        {
                            Script = response.Content
                        };
                        return Task.FromResult<IBaristaModule>(typeScriptModule);
                    case ResourceKind.Json:
                        return Task.FromResult<IBaristaModule>(new JsonModule(name, ModuleDescription, response.Content));
                    case ResourceKind.Text:
                        return Task.FromResult<IBaristaModule>(new RawTextModule(name, ModuleDescription, response.Content));
                    case ResourceKind.Binary:
                    default:
                        return Task.FromResult<IBaristaModule>(new RawBlobModule(name, ModuleDescription, new Blob(response.RawBytes, response.ContentType)));
                }
            }

            return null;
        }

        private ResourceKind GetCanonicalFileType(string path, string contentType)
        {
            var filename = Path.GetFileName(path);
            if (!String.IsNullOrWhiteSpace(filename))
            {
                var ext = Path.GetExtension(path);
                switch (ext)
                {
                    case ".js":
                        return ResourceKind.JavaScript;
                    case ".jsx":
                        return ResourceKind.Jsx;
                    case ".ts":
                        return ResourceKind.TypeScript;
                    case ".tsx":
                        return ResourceKind.Tsx;
                    case ".json":
                    case ".jsonx":
                        return ResourceKind.Json;
                    case ".txt":
                        return ResourceKind.Text;
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".gif":
                    case ".bin":
                        return ResourceKind.Binary;
                }
                
            }

            contentType = contentType.Split(';').FirstOrDefault();

            if (contentType == null)
                contentType = String.Empty;

            switch (contentType.ToLowerInvariant())
            {
                case "text/js":
                case "text/javascript":
                case "application/javascript":
                    return ResourceKind.JavaScript;
                case "text/jsx":
                case "application/x-javascript+xml":
                case "application/vnd.facebook.javascript+xml":
                    return ResourceKind.Jsx;
                case "text/ts":
                case "text/typescript":
                case "application/x-typescript":
                case "application/vnd.microsoft.typescript":
                    return ResourceKind.TypeScript;
                case "text/tsx":
                case "application/x-typescript+xml":
                case "application/vnd.microsoft.typescript+xml":
                    return ResourceKind.Tsx;
                case "text/json":
                case "application/json":
                    return ResourceKind.Json;
                case "text/plain":
                    return ResourceKind.Text;
                default:
                    return ResourceKind.Binary;
            }
        }
    }
}
