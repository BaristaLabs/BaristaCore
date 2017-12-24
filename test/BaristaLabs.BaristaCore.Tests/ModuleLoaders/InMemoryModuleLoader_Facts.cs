namespace BaristaLabs.BaristaCore.Tests.ModuleLoaders
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using Microsoft.Extensions.DependencyInjection;
    using System;
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

        [Fact]
        public void JsInMemoryModuleLoaderThrowsOnInvalidArguments()
        {
            var inMemoryModuleLoader = new InMemoryModuleLoader();
            Assert.Throws<ArgumentNullException>(() =>
            {
                inMemoryModuleLoader.RegisterModule(null);
            });

            Assert.Throws<InvalidOperationException>(() =>
            {
                inMemoryModuleLoader.RegisterModule(new PhantomModule());
            });

            inMemoryModuleLoader.RegisterModule(new HelloWorldModule());
            Assert.Throws<InvalidOperationException>(() =>
            {
                inMemoryModuleLoader.RegisterModule(new HelloWorldModule());
            });
        }

        [Fact]
        public void ModuleLoadersThatThrowThrowErrors()
        {
            var script = @"
        import helloworld from 'hello_world';
        export default helloworld;
        ";

            var fawltyModuleLoader = new FawltyModuleLoader();

            var baristaRuntime = GetRuntimeFactory(fawltyModuleLoader);

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

        [BaristaModule("hello_world", "Only the best module ever.")]
        private sealed class HelloWorldModule : IBaristaModule
        {
            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>("Hello, World!");
            }
        }

        [BaristaModule("", "If it weren't for you meddling kids...")]
        private sealed class PhantomModule : IBaristaModule
        {
            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>("Spooky.");
            }
        }

        private sealed class FawltyModuleLoader : IBaristaModuleLoader
        {
            public IBaristaModule GetModule(string name)
            {
                throw new Exception("Derp!");
            }
        }
    }
}