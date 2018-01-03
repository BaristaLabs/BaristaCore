namespace BaristaLabs.BaristaCore.Tests.ModuleLoaders
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Reflection;
    using Xunit;

    [Collection("BaristaCore Tests")]
    public class AssemblyModuleLoader_Facts
    {
        public AssemblyModuleLoader_Facts()
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
        public void AssemblyModuleLoaderLoadsModules()
        {
            var script = @"
        import helloworld from 'hello_world';
        export default helloworld;
        ";

            using (var aml = new AssemblyModuleLoader(Assembly.GetExecutingAssembly()))
            {
                var baristaRuntime = GetRuntimeFactory(aml);

                using (var rt = baristaRuntime.CreateRuntime())
                {
                    using (var ctx = rt.CreateContext())
                    {
                        var result = ctx.EvaluateModule(script);

                        Assert.Equal("Hello, World!", result.ToString());
                    }
                }
            }
        }

        [Fact]
        public void AssemblyModuleLoaderDoesntFindNonExistentModules()
        {
            var script = @"
        import helloworld from 'this_is_not_a_real_module_name';
        export default helloworld;
        ";

            using (var aml = new AssemblyModuleLoader(Assembly.GetExecutingAssembly()))
            {
                var baristaRuntime = GetRuntimeFactory(aml);

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
        }

        [Fact]
        public void AssemblyModuleLoaderThrowsWhenNoAssembly()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var aml = new AssemblyModuleLoader(null);
            });
        }
    }
}