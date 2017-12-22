namespace BaristaLabs.BaristaCore.Tests.ModuleLoader
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using Microsoft.Extensions.DependencyInjection;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class InMemoryModuleLoader_Facts
    {
        public InMemoryModuleLoader_Facts()
        {
        }

        public IBaristaRuntimeFactory GetRuntimeFactory(IBaristaModuleLoader moduleLoader)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore(moduleLoader: moduleLoader);

            var provider = serviceCollection.BuildServiceProvider();
            return provider.GetRequiredService<IBaristaRuntimeFactory>();
        }

        [Fact]
        public void JsInMemoryModuleLoaderReturnsModules()
        {
            var script = @"
        import helloworld from 'hello_world';
        export default helloworld;
        ";

            var inMemoryModuleLoader = new InMemoryModuleLoader();
            inMemoryModuleLoader.RegisterModule(new HelloWorldModule());

            var baristaRuntime = GetRuntimeFactory(inMemoryModuleLoader);

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var result = ctx.EvaluateModule(script);

                    Assert.Equal("Hello, World!", result.ToString());
                }
            }
        }
        
        [BaristaModule("hello_world", "Only the best module ever.")]
        private sealed class HelloWorldModule : IBaristaModule
        {
            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>("Hello, World!");
            }
        }
    }
}