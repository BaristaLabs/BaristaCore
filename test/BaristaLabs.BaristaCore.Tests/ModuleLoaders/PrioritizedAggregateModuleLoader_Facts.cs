namespace BaristaLabs.BaristaCore.Tests.ModuleLoaders
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    [Collection("BaristaCore Tests")]
    public class PrioritizedAggregateModuleLoader_Facts
    {
        public PrioritizedAggregateModuleLoader_Facts()
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
        public void PrioritizedModuleLoaderUsesRegisteredModuleLoaders()
        {
            var script = @"
        import helloworld from 'hello_world';
        export default helloworld;
        ";

            var inMemoryModuleLoader = new InMemoryModuleLoader();
            inMemoryModuleLoader.RegisterModule(new HelloWorldModule());

            var aggregateModuleLoader = new PrioritizedAggregateModuleLoader();
            aggregateModuleLoader.RegisterModuleLoader(inMemoryModuleLoader);

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
        public void PrioritizedModuleLoaderUsesHigherPriorityModuleLoadersOverLowerPriority()
        {
            var script = @"
        import helloworld from 'hello_world';
        export default helloworld;
        ";

            var inMemoryModuleLoader = new InMemoryModuleLoader();
            inMemoryModuleLoader.RegisterModule(new HelloWorldModule());

            var inMemoryModuleLoader1 = new InMemoryModuleLoader();
            inMemoryModuleLoader1.RegisterModule(new AnotherHelloWorldModule());

            var aggregateModuleLoader = new PrioritizedAggregateModuleLoader();
            aggregateModuleLoader.RegisterModuleLoader(inMemoryModuleLoader1, priority: 100);
            aggregateModuleLoader.RegisterModuleLoader(inMemoryModuleLoader, priority: 1);

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
        public void PrioritizedModuleLoaderExaminesAllModuleLoaders()
        {
            var script = @"
        import goodnightMoon from 'goodnight_moon';
        export default goodnightMoon;
        ";

            var inMemoryModuleLoader = new InMemoryModuleLoader();
            inMemoryModuleLoader.RegisterModule(new HelloWorldModule());

            var inMemoryModuleLoader1 = new InMemoryModuleLoader();
            inMemoryModuleLoader1.RegisterModule(new GoodnightMoonModule());

            var aggregateModuleLoader = new PrioritizedAggregateModuleLoader();
            aggregateModuleLoader.RegisterModuleLoader(inMemoryModuleLoader1, priority: 100);
            aggregateModuleLoader.RegisterModuleLoader(inMemoryModuleLoader, priority: 1);

            var baristaRuntime = GetRuntimeFactory(aggregateModuleLoader);

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var result = ctx.EvaluateModule(script);

                    Assert.Equal("Goodnight, moon.", result.ToString());
                }
            }
        }
        
        [Fact]
        public void PrioritizedModuleLoaderThrowsWhenNoLoaderCouldBeFound()
        {
            var script = @"
        import helloworld from 'hello_world';
        export default helloworld;
        ";

            var aggregateModuleLoader = new PrioritizedAggregateModuleLoader();

            var baristaRuntime = GetRuntimeFactory(aggregateModuleLoader);

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    Assert.Throws<JsScriptException>(() =>
                    {
                        var result = ctx.EvaluateModule(script);
                    });
                }
            }
        }

        [Fact]
        public void RegisterPrioritizedModuleLoaderThrowsWhenNoModuleLoaderSpecified()
        {
            var aggregateModuleLoader = new PrioritizedAggregateModuleLoader();
            Assert.Throws<ArgumentNullException>(() =>
            {
                aggregateModuleLoader.RegisterModuleLoader(null, -1, null);
            });
        }

        [BaristaModule("hello_world", "Only the best module ever.")]
        private sealed class HelloWorldModule : IBaristaModule
        {
            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>("Hello, World!");
            }
        }

        [BaristaModule("hello_world", "Only the best module ever.")]
        private sealed class AnotherHelloWorldModule : IBaristaModule
        {
            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>("Another Hello, World!");
            }
        }

        [BaristaModule("goodnight_moon", "Yaaaawwwwwn")]
        private sealed class GoodnightMoonModule : IBaristaModule
        {
            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>("Goodnight, moon.");
            }
        }
    }
}
