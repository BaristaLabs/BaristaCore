namespace BaristaLabs.BaristaCore.Modules.Fetch.Tests
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using Microsoft.Extensions.DependencyInjection;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class Fetch_Facts
    {
        public IBaristaRuntimeFactory GetRuntimeFactory()
        {
            var myMemoryModuleLoader = new InMemoryModuleLoader();
            myMemoryModuleLoader.RegisterModule(new FetchModule());

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore(moduleLoader: myMemoryModuleLoader);

            var provider = serviceCollection.BuildServiceProvider();
            return provider.GetRequiredService<IBaristaRuntimeFactory>();
        }

        [Fact]
        public void CanFetch()
        {
            var script = @"
        import fetch from 'barista-fetch';
        
        export default fetch('https://httpbin.org/get');
        ";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var response = ctx.EvaluateModule<JsObject>(script);
                        var fnText = response.GetProperty<JsFunction>("text");
                        var textPromise = fnText.Call<JsObject>(response);
                        var text = ctx.Promise.Wait<JsString>(textPromise);
                        Assert.NotNull(text.ToString());
                    }
                }
            }
        }
    }
}
