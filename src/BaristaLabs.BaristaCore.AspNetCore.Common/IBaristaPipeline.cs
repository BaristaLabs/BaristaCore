namespace BaristaLabs.BaristaCore.AspNetCore
{
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using Microsoft.AspNetCore.Http;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IBaristaPipeline
    {
        /// <summary>
        /// Takes the order. E.g. From the http request, gets the script that needs to be brewed.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<BrewOrder> TakeOrder(string path, HttpRequest req);

        /// <summary>
        /// Tamps the brew request. E.g. Sets up any module loaders that need to be associated as part of the brew.
        /// </summary>
        /// <param name="brewOrder"></param>
        /// <returns></returns>
        IBaristaModuleLoader Tamp(BrewOrder brewOrder, HttpRequest req);

        /// <summary>
        /// Pulls the shot of espresso
        /// </summary>
        /// <param name="brewOrder"></param>
        /// <param name="baristaRuntimeFactory"></param>
        /// <param name="moduleLoader"></param>
        /// <returns></returns>
        HttpResponseMessage Brew(BrewOrder brewOrder, IBaristaRuntimeFactory baristaRuntimeFactory, IBaristaModuleLoader moduleLoader);

        /// <summary>
        /// Serves the brew. E.g. Now that the coffee, now in liquid form, has been extracted, provide the http response back to the requestor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="brewResult"></param>
        /// <returns></returns>
        HttpResponseMessage Serve(BaristaContext context, JsValue brewResult);
    }
}
