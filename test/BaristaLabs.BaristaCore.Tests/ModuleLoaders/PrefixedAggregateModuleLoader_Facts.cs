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
    [Collection("BaristaCore Tests")]
    public class PrefixedAggregateModuleLoader_Facts
    {
        public PrefixedAggregateModuleLoader_Facts()
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
        public void JsAggregateModuleLoaderReturnsFallbackModules()
        {
            var script = @"
        import helloworld from 'hello_world';
        export default helloworld;
        ";

            var inMemoryModuleLoader = new InMemoryModuleLoader();
            var aggregateModuleLoader = new PrefixedAggregateModuleLoader
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

            var aggregateModuleLoader = new PrefixedAggregateModuleLoader();
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

        [Fact]
        public void JsAggregateModuleLoaderPrefixesMustBeUnique()
        {
            var inMemoryModuleLoader1 = new InMemoryModuleLoader();
            inMemoryModuleLoader1.RegisterModule(new HelloWorldModule());

            var inMemoryModuleLoader2 = new InMemoryModuleLoader();
            inMemoryModuleLoader2.RegisterModule(new GoodnightMoonModule());

            var aggregateModuleLoader = new PrefixedAggregateModuleLoader();
            aggregateModuleLoader.RegisterModuleLoader("daytime", inMemoryModuleLoader1);
            Assert.Throws<ArgumentException>(() =>
            {
                aggregateModuleLoader.RegisterModuleLoader("daytime", inMemoryModuleLoader2);
            });
        }

        [Fact]
        public void JsAggregateModuleLoaderThrowsWhenUnregisteredPrefixesAreUsed()
        {
            var script = @"
        import helloworld from 'football!hello_world';
        export default helloworld;
        ";

            var inMemoryModuleLoader1 = new InMemoryModuleLoader();
            inMemoryModuleLoader1.RegisterModule(new HelloWorldModule());

            var aggregateModuleLoader = new PrefixedAggregateModuleLoader();
            aggregateModuleLoader.RegisterModuleLoader("daytime", inMemoryModuleLoader1);

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
        public void JsAggregateModuleLoaderThrowsWhenWhitespacePrefixesAreUsed()
        {
            var script = @"
        import helloworld from '       !hello_world';
        export default helloworld;
        ";

            var inMemoryModuleLoader1 = new InMemoryModuleLoader();
            inMemoryModuleLoader1.RegisterModule(new HelloWorldModule());

            var aggregateModuleLoader = new PrefixedAggregateModuleLoader();
            Assert.Throws<ArgumentNullException>(() =>
            {
                aggregateModuleLoader.RegisterModuleLoader("       ", inMemoryModuleLoader1);
            });

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
        public void JsAggregateModuleLoaderThrowsWhenNoLoaderCouldBeFound()
        {
            var script = @"
        import helloworld from 'hello_world';
        export default helloworld;
        ";

            var aggregateModuleLoader = new PrefixedAggregateModuleLoader();

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
        public void JsAggregateModuleLoaderPrefixesMustBeSpecified()
        {
            var inMemoryModuleLoader1 = new InMemoryModuleLoader();
            inMemoryModuleLoader1.RegisterModule(new HelloWorldModule());

            var inMemoryModuleLoader2 = new InMemoryModuleLoader();
            inMemoryModuleLoader2.RegisterModule(new GoodnightMoonModule());

            var aggregateModuleLoader = new PrefixedAggregateModuleLoader();

            Assert.Throws<ArgumentNullException>(() =>
            {
                aggregateModuleLoader.RegisterModuleLoader("", inMemoryModuleLoader1);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                aggregateModuleLoader.RegisterModuleLoader(null, inMemoryModuleLoader2);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                aggregateModuleLoader.RegisterModuleLoader("foobar", null);
            });
        }

        [Fact]
        public void JsAggregateModuleLoaderCanRemoveLoader()
        {
            var inMemoryModuleLoader1 = new InMemoryModuleLoader();
            inMemoryModuleLoader1.RegisterModule(new HelloWorldModule());

            var inMemoryModuleLoader2 = new InMemoryModuleLoader();
            inMemoryModuleLoader2.RegisterModule(new GoodnightMoonModule());

            var aggregateModuleLoader = new PrefixedAggregateModuleLoader();

            Assert.False(aggregateModuleLoader.HasModuleLoader("foo"));
            aggregateModuleLoader.RegisterModuleLoader("foo", inMemoryModuleLoader1);
            Assert.True(aggregateModuleLoader.HasModuleLoader("foo"));
            aggregateModuleLoader.RemoveModuleLoader("foo");
            Assert.False(aggregateModuleLoader.HasModuleLoader("foo"));
        }

        [BaristaModule("hello_world", "Only the best module ever.")]
        private sealed class HelloWorldModule : IBaristaModule
        {
            public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
            {
                return Task.FromResult<object>("Hello, World!");
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
