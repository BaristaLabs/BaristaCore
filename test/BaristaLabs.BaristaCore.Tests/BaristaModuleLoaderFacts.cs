namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.JavaScript.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class BaristaModuleLoader_Facts
    {
        public BaristaModuleLoader_Facts()
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

        [Fact]
        public void JsAggregateModuleLoaderReturnsFallbackModules()
        {
            var script = @"
        import helloworld from 'hello_world';
        export default helloworld;
        ";

            var inMemoryModuleLoader = new InMemoryModuleLoader();
            var aggregateModuleLoader = new AggregateModuleLoader
            {
                FallbackModuleLoader = inMemoryModuleLoader
            };
            inMemoryModuleLoader.RegisterModule(new HelloWorldModule());


            var baristaRuntime = GetRuntimeFactory(aggregateModuleLoader);

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var result = ctx.EvaluateModule(script);

                    Assert.Equal("Hello, World!", result.ToString());
                }
            }
        }

        [Fact]
        public void JsAggregateModuleLoaderPrefixedModules()
        {
            var script = @"
        import helloworld from 'daytime!hello_world';
        export default helloworld;
        ";

            var script2 = @"
        import helloworld from 'nighttime!goodnight_moon';
        export default helloworld;
        ";

            var inMemoryModuleLoader1 = new InMemoryModuleLoader();
            inMemoryModuleLoader1.RegisterModule(new HelloWorldModule());

            var inMemoryModuleLoader2 = new InMemoryModuleLoader();
            inMemoryModuleLoader2.RegisterModule(new GoodnightMoonModule());

            var aggregateModuleLoader = new AggregateModuleLoader();
            aggregateModuleLoader.RegisterModuleLoader("daytime", inMemoryModuleLoader1);
            aggregateModuleLoader.RegisterModuleLoader("nighttime", inMemoryModuleLoader2);

            var baristaRuntime = GetRuntimeFactory(aggregateModuleLoader);

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var result = ctx.EvaluateModule(script);

                    Assert.Equal("Hello, World!", result.ToString());

                    result = ctx.EvaluateModule(script2);

                    Assert.Equal("Goodnight, moon.", result.ToString());
                }
            }
        }

        private sealed class HelloWorldModule : IBaristaModule
        {
            public string Name => "hello_world";

            public string Description => "Only the best module ever.";

            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>("Hello, World!");
            }
        }

        private sealed class GoodnightMoonModule : IBaristaModule
        {
            public string Name => "goodnight_moon";

            public string Description => "Yaaaawwwwwn.";

            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>("Goodnight, moon.");
            }
        }
    }
}