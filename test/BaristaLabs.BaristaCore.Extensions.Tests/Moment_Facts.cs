namespace BaristaLabs.BaristaCore.Extensions.Tests
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.Modules;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class Moment_Facts
    {
        public IBaristaRuntimeFactory GetRuntimeFactory()
        {
            var myMemoryModuleLoader = new InMemoryModuleLoader();
            myMemoryModuleLoader.RegisterModule(new MomentModule());

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore(moduleLoader: myMemoryModuleLoader);

            var provider = serviceCollection.BuildServiceProvider();
            return provider.GetRequiredService<IBaristaRuntimeFactory>();
        }

        [Fact]
        public void CanUseMomentMethods()
        {
            var script = @"
import moment from 'moment';
export default moment('20111031', 'YYYYMMDD').format('MMMM Do YYYY');
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
                        Assert.Equal("October 31st 2011", response.ToString());
                    }
                }
            }
        }
    }
}
