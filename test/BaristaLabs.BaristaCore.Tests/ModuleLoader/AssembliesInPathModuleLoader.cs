﻿namespace BaristaLabs.BaristaCore.Tests.ModuleLoader
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using Microsoft.Extensions.DependencyInjection;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class AssembliesInPathModuleLoader_Facts
    {
        public AssembliesInPathModuleLoader_Facts()
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
        public void AssembliesInPathModuleLoaderLoadsModules()
        {
            var script = @"
        import helloworld from 'MyTestModule';
        export default helloworld;
        ";

            var inPathModuleLoader = new AssembliesInPathModuleLoader();
            var baristaRuntime = GetRuntimeFactory(inPathModuleLoader);

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var result = ctx.EvaluateModule(script);

                    Assert.Equal("The maze isn't meant for you.", result.ToString());
                }
            }
        }

        [Fact]
        public void AssembliesInPathModuleLoaderLoadsDisposableModules()
        {
            var script = @"
        import helloworld from 'MyDisposableTestModule';
        export default helloworld;
        ";

            var inPathModuleLoader = new AssembliesInPathModuleLoader();
            var baristaRuntime = GetRuntimeFactory(inPathModuleLoader);

            using (var rt = baristaRuntime.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    var result = ctx.EvaluateModule(script);

                    Assert.Equal("The maze still isn't meant for you.", result.ToString());
                }
            }


            inPathModuleLoader.Dispose();
        }

        [Fact]
        public void AssembliesInPathModuleLoaderOkToNotFindModules()
        {
            var script = @"
        import helloworld from 'ThisIsntAModule';
        export default helloworld;
        ";

            var inPathModuleLoader = new AssembliesInPathModuleLoader();
            var baristaRuntime = GetRuntimeFactory(inPathModuleLoader);

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


            inPathModuleLoader.Dispose();
        }
    }
}