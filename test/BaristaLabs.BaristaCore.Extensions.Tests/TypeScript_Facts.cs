namespace BaristaLabs.BaristaCore.Extensions.Tests
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.Modules;
    using Microsoft.Extensions.DependencyInjection;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class TypeScript_Facts
    {
        public IBaristaRuntimeFactory GetRuntimeFactory()
        {
            var myMemoryModuleLoader = new InMemoryModuleLoader();
            myMemoryModuleLoader.RegisterModule(new ReactModule());
            myMemoryModuleLoader.RegisterModule(new ReactDomServerModule());
            myMemoryModuleLoader.RegisterModule(new TypeScriptModule());

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore(moduleLoader: myMemoryModuleLoader);

            var provider = serviceCollection.BuildServiceProvider();
            return provider.GetRequiredService<IBaristaRuntimeFactory>();
        }

        [Fact]
        public void CanUseTypeScript()
        {
            var script = @"
import ts from 'typescript';

var code = '<MyCounter count={3 + 5} />;';

let transpiled = ts.transpileModule(code, {
    compilerOptions: {
        target: ts.ScriptTarget.Latest,
        module: ts.ModuleKind.ESNext,
        jsx: ts.JsxEmit.React,
        importHelpers: true
    },
    fileName: 'test.tsx'
});

export default transpiled.outputText;
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
                        //See http://www.typescriptlang.org/docs/handbook/jsx.html
                        Assert.Equal("React.createElement(MyCounter, { count: 3 + 5 });\r\n", response.ToString());
                    }
                }
            }
        }

        [Fact]
        public void CanUseExtensionMethodsToEvaluateTypeScript()
        {
            var script = @"
import React from 'react';
import ReactDOMServer from 'react-dom-server';

function formatName(user) {
  return user.firstName + ' ' + user.lastName;
}

function getGreeting(user) {
  if (user) {
    return <h1>Hello, {formatName(user)}!</h1>;
  }
  return <h1>Hello, Stranger.</h1>;
}

var result = getGreeting({ firstName: 'James', lastName: 'Bond'});

export default ReactDOMServer.renderToStaticMarkup(result);
";

            var runtimeFactory = GetRuntimeFactory();
            //System.Diagnostics.Debugger.Launch();

            using (var rt = runtimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var response = ctx.EvaluateTypeScriptModule(script, true);

                        Assert.Equal("<h1>Hello, James Bond!</h1>", response.ToString());

                        response = ctx.EvaluateTypeScriptModule<JsString>(script, true);

                        Assert.Equal("<h1>Hello, James Bond!</h1>", response.ToString());
                    }
                }
            }
        }
    }
}
