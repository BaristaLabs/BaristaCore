﻿namespace BaristaLabs.BaristaCore.ModuleLoaders
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using BaristaLabs.BaristaCore.Modules;
    using RestSharp;

    /// <summary>
    /// Represents a module loader that loads script modules from a web-based resource.
    /// </summary>
    public class WebResourceModuleLoader : IBaristaModuleLoader
    {
        const string ModuleDescription = "Script loaded via WebResourceModuleLoader";

        private readonly Func<IRestClient> m_restClientBuilder;

        public WebResourceModuleLoader(string baseUrl)
        {
            m_restClientBuilder = new Func<IRestClient>(() =>
            {
                var client = new RestClient(baseUrl);
                client.Proxy = new DummyProxy();
                return client;
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
                var filename = Path.GetFileName(response.ResponseUri.ToString());

                //Based on file's type, return an appropriate module.
                switch (GetCanonicalFileType(response.ResponseUri.ToString(), response.ContentType))
                {
                    case ".js":
                        var scriptModule = new BaristaScriptModule(name, ModuleDescription)
                        {
                            Script = response.Content
                        };

                        return scriptModule;
                    case ".ts":
                    case ".tsx":
                    case ".jsx":
                        var typeScriptModule = new BaristaTypeScriptModule(name, ModuleDescription, String.IsNullOrWhiteSpace(filename) ? "script.js" : filename)
                        {
                            Script = response.Content
                        };
                        return typeScriptModule;
                    case ".txt":
                        return new RawTextModule(name, ModuleDescription, response.Content);
                    case ".bin":
                    default:
                        return new RawBlobModule(name, ModuleDescription, new Blob(response.RawBytes, response.ContentType));
                }
            }

            return null;
        }

        private string GetCanonicalFileType(string path, string contentType)
        {
            var filename = Path.GetFileName(path);
            if (!String.IsNullOrWhiteSpace(filename))
                return Path.GetExtension(path);

            contentType = contentType.Split(";").FirstOrDefault();

            if (contentType == null)
                contentType = String.Empty;

            switch(contentType.ToLowerInvariant())
            {
                case "text/javascript":
                case "application/javascript":
                    return ".js";
                case "application/x-typescript":
                    return ".ts";
                case "text/plain":
                    return ".txt";
                default:
                    return ".bin";
            }
        }
    }
}
