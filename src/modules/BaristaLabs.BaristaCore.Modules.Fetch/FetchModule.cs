namespace BaristaLabs.BaristaCore.Modules.Fetch
{
    using System;
    using System.Threading.Tasks;

    [BaristaModule("barista-fetch", "Provides a subset of the standard Fetch Specification.")]
    public class FetchModule : IBaristaModule
    {
        public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var fnFetch = context.ValueFactory.CreateFunction(new Func<JsObject, JsObject, JsObject, object>((thisObj, input, init) =>
            {
                //TODO: If input is JsObject

                var request = new Request(input.ToString(), init);
                return request.Execute(context);
            }));

            if (context.Converter.TryFromObject(context, typeof(Body), out JsValue fnBody))
            {
                fnFetch["Body"] = fnBody;
            }

            if (context.Converter.TryFromObject(context, typeof(Headers), out JsValue fnHeaders))
            {
                fnFetch["Headers"] = fnHeaders;
            }

            if (context.Converter.TryFromObject(context, typeof(Request), out JsValue fnRequest))
            {
                fnFetch["Request"] = fnRequest;
            }

            if (context.Converter.TryFromObject(context, typeof(Request), out JsValue fnResponse))
            {
                fnFetch["Response"] = fnResponse;
            }

            return Task.FromResult<object>(fnFetch);
        }
    }
}
