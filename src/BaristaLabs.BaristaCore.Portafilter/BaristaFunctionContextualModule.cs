namespace BaristaLabs.BaristaCore.Portafilter
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs.Host;
    using System;
    using System.Threading.Tasks;

    [BaristaModule("barista-context", "Contains the current context of the Barista Function, including the current request object.")]
    public sealed class BaristaFunctionContextualModule : IBaristaModule
    {
        private readonly HttpRequest m_request;
        private readonly TraceWriter m_log;

        public BaristaFunctionContextualModule(HttpRequest request, TraceWriter log)
        {
            m_request = request ?? throw new ArgumentNullException(nameof(request));
            m_log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var brewRequest = new BrewRequest(context, m_request);

            context.Converter.TryFromObject(context, brewRequest, out JsValue requestObj);
            context.Converter.TryFromObject(context, new BrewResponse(), out JsValue responseObj);
            context.Converter.TryFromObject(context, m_log, out JsValue logObj);

            var contextObj = context.CreateObject();
            context.Object.DefineProperty(contextObj, "request", new JsPropertyDescriptor()
            {
                Configurable = false,
                Writable = false,
                Value = requestObj
            });

            context.Object.DefineProperty(contextObj, "response", new JsPropertyDescriptor()
            {
                Configurable = false,
                Writable = false,
                Value = responseObj
            });

            context.Object.DefineProperty(contextObj, "log", new JsPropertyDescriptor()
            {
                Configurable = false,
                Writable = false,
                Value = logObj
            });

            return contextObj;
        }
    }
}
