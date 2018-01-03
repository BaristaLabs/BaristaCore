namespace BaristaLabs.BaristaCore.Extensions.Tests
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.Modules;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class Lodash_Facts
    {
        public IBaristaRuntimeFactory GetRuntimeFactory()
        {
            var myMemoryModuleLoader = new InMemoryModuleLoader();
            myMemoryModuleLoader.RegisterModule(new LodashModule());

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore(moduleLoader: myMemoryModuleLoader);

            var provider = serviceCollection.BuildServiceProvider();
            return provider.GetRequiredService<IBaristaRuntimeFactory>();
        }

        [Fact]
        public void CanUseLodashMethods()
        {
            var script = @"
import _ from 'lodash';
export default _.capitalize('FRED');
";

            var baristaRuntime = GetRuntimeFactory();

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var response = ctx.EvaluateModule(script);
                        Assert.NotNull(response);
                        Assert.IsType<JsString>(response);
                        Assert.Equal("Fred", response.ToString());
                    }
                }
            }
        }
    }
}
