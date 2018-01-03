namespace BaristaLabs.BaristaCore.Portafilter
{
    using BaristaLabs.BaristaCore.Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.DependencyInjection;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    public static class BaristaFunction
    {
        private static IBaristaRuntimeFactory m_baristaRuntimeFactory;

        static BaristaFunction()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore();

            var provider = serviceCollection.BuildServiceProvider();
            m_baristaRuntimeFactory = provider.GetRequiredService<IBaristaRuntimeFactory>();
        }

        [FunctionName("BaristaFunction")]
        public async static Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", "put", "patch", "delete", "options", "head", Route = "{*path}")]HttpRequest req, TraceWriter log)
        {
            log.Info("Barista Function processed a request.");

            var requestBody = ReadFully(req.Body);

            //Todo: convert the request.

            //Brew:
            using (var rt = m_baristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            {
                var result = ctx.EvaluateModule("export default 6*7");
                return Serve(req, result);
            }
            
            
        }

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

            response.Headers.Add("Content-Type", "text/plain");
            
            return response;
        }

        private static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
