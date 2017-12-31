namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.Modules;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a class that uses a BaristaRuntime on a worker thread to transpile TypeScript.
    /// </summary>
    public sealed class TypeScriptTranspiler
    {
        private readonly IServiceProvider m_provider;
        const string TranspileScript = @"
import ts from 'typescript';

const global = (new Function('return this;'))();

let transpiled = ts.transpileModule(global.scriptToTranspile, {
    compilerOptions: {
        target: ts.ScriptTarget.Latest,
        module: ts.ModuleKind.ESNext,
        jsx: ts.JsxEmit.React,
        importHelpers: true
    },
    fileName: global.filename
});

export default transpiled.outputText;
        ";

        public TypeScriptTranspiler()
        {
            var myMemoryModuleLoader = new InMemoryModuleLoader();
            myMemoryModuleLoader.RegisterModule(new TypeScriptModule());

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore(moduleLoader: myMemoryModuleLoader);

            m_provider = serviceCollection.BuildServiceProvider();
        }

        public Task<string> Transpile(string scriptToTranspile, string fileName = "main.ts")
        {
            return Transpile(new TypeScriptTranspilerOptions() { ScriptToTranspile = scriptToTranspile, Filename = fileName });
        }

        public async Task<string> Transpile(TypeScriptTranspilerOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return await Task.Factory.StartNew(new Func<object, string>((transpilerOptions) =>
            {
                return PerformTranspilation(transpilerOptions as TypeScriptTranspilerOptions);
            }), options);
        }

        private string PerformTranspilation(TypeScriptTranspilerOptions options)
        {
            var runtimeFactory = m_provider.GetRequiredService<IBaristaRuntimeFactory>();

            using(var rt = runtimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            {
                using (ctx.Scope())
                {
                    var jsScriptToTranspile = ctx.CreateString(options.ScriptToTranspile);
                    var jsFilename = ctx.CreateString(options.Filename);
                    ctx.GlobalObject["scriptToTranspile"] = jsScriptToTranspile;
                    ctx.GlobalObject["filename"] = jsFilename;

                    var text = ctx.EvaluateModule<JsString>(TranspileScript);
                    return text.ToString();
                }
            }
        }

        private static TypeScriptTranspiler s_defaultTranspiler = new TypeScriptTranspiler();
        
        /// <summary>
        /// Gets the default transpiler
        /// </summary>
        public static TypeScriptTranspiler Default
        {
            get { return s_defaultTranspiler; }
        }
    }

    public class TypeScriptTranspilerOptions
    {
        public string ScriptToTranspile
        {
            get;
            set;
        }

        public string Filename
        {
            get;
            set;
        }
    }
}
