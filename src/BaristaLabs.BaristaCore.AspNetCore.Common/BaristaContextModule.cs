namespace BaristaLabs.BaristaCore.AspNetCore
{
    using Microsoft.AspNetCore.Http;
    using System;

    [BaristaModule("barista-context", "Contains the current context of the Barista Function, including the current request object.")]
    public sealed class BaristaContextModule : IBaristaModule
    {
        private readonly HttpRequest m_request;

        public BaristaContextModule(HttpRequest request)
        {
            m_request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var brewRequest = new BrewRequest(context, m_request);

            context.Converter.TryFromObject(context, brewRequest, out JsValue requestObj);
            context.Converter.TryFromObject(context, typeof(BrewResponse), out JsValue responseObj);
            //context.Converter.TryFromObject(context, m_log, out JsValue logObj);

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

            return contextObj;
        }
    }
}
