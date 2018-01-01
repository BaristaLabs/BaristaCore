namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using BaristaLabs.BaristaCore.Modules;
    using BaristaLabs.BaristaCore.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a class that uses a BaristaRuntime on a worker thread to transpile TypeScript.
    /// </summary>
    public sealed class TypeScriptTranspiler : IDisposable
    {
        private readonly IServiceProvider m_provider;
        private readonly IBaristaRuntimeFactory m_runtimeFactory;
        const string TranspileScript = @"
import ts from 'typescript';

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
            m_runtimeFactory = m_provider.GetRequiredService<IBaristaRuntimeFactory>();
        }

        public Task<string> Transpile(string scriptToTranspile, string fileName = "main.ts")
        {
            return Transpile(new TypeScriptTranspilerOptions() { ScriptToTranspile = scriptToTranspile, Filename = fileName });
        }

        public Task<string> Transpile(TypeScriptTranspilerOptions options)
        {
            if (m_isDisposed)
                throw new ObjectDisposedException(nameof(TypeScriptTranspiler));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return Task.Factory.StartNew(new Func<object, string>((transpilerOptions) =>
            {
                return PerformTranspilation(transpilerOptions as TypeScriptTranspilerOptions);
            }),
            options,
            CancellationToken.None,
            TaskCreationOptions.DenyChildAttach,
            new ThreadPerTaskScheduler());
        }

        private string PerformTranspilation(TypeScriptTranspilerOptions options)
        {
            using (var rt = m_runtimeFactory.CreateRuntime(JavaScriptRuntimeAttributes.DisableBackgroundWork | JavaScriptRuntimeAttributes.DisableEval))
            using (var ctx = rt.CreateContext())
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

        #region IDisposable Support
        private bool m_isDisposed = false;

        private void Dispose(bool disposing)
        {
            if (!m_isDisposed)
            {
                if (disposing)
                {
                    m_runtimeFactory.Dispose();
                }
                m_isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        private static TypeScriptTranspiler m_defaultTranspiler;
        private static readonly object m_defaultLock = new object();

        /// <summary>
        /// Gets the default TypeScript transpiler.
        /// </summary>
        public static TypeScriptTranspiler Default
        {
            get
            {
                if (null == m_defaultTranspiler)
                {
                    lock (m_defaultLock)
                    {
                        if (null == m_defaultTranspiler)
                        {
                            m_defaultTranspiler = new TypeScriptTranspiler();
                        }
                    }
                }

                return m_defaultTranspiler;
            }
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
