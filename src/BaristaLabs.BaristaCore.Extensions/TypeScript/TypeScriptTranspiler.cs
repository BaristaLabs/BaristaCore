namespace BaristaLabs.BaristaCore.TypeScript
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

        public TypeScriptTranspiler()
        {
            var myMemoryModuleLoader = new InMemoryModuleLoader();
            myMemoryModuleLoader.RegisterModule(new TypeScriptModule());

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore(moduleLoader: myMemoryModuleLoader);

            m_provider = serviceCollection.BuildServiceProvider();
            m_runtimeFactory = m_provider.GetRequiredService<IBaristaRuntimeFactory>();
        }

        /// <summary>
        /// Returns transpiled javascript according to the specified options.
        /// </summary>
        /// <param name="scriptToTranspile">The script to transpile</param>
        /// <param name="fileName">Filename to use as a ts/js/jsx/tsx specifer</param>
        /// <returns>The output of the transpilation process</returns>
        public Task<TranspileOutput> Transpile(string scriptToTranspile, string fileName = "main.ts")
        {
            return Transpile(new TranspileOptions() { Script = scriptToTranspile, FileName = fileName });
        }

        /// <summary>
        /// Returns transpiled javascript according to the specified options.
        /// </summary>
        /// <param name="options">Transpile options</param>
        /// <returns>The output of the transpilation process</returns>
        public Task<TranspileOutput> Transpile(TranspileOptions options)
        {
            if (m_isDisposed)
                throw new ObjectDisposedException(nameof(TypeScriptTranspiler));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return Task.Factory.StartNew(new Func<object, TranspileOutput>((transpilerOptions) =>
            {
                return TranspileInternal(transpilerOptions as TranspileOptions);
            }),
            options,
            CancellationToken.None,
            TaskCreationOptions.DenyChildAttach,
            new ThreadPerTaskScheduler());
        }

        private TranspileOutput TranspileInternal(TranspileOptions options)
        {
            using (var rt = m_runtimeFactory.CreateRuntime(JavaScriptRuntimeAttributes.DisableBackgroundWork | JavaScriptRuntimeAttributes.DisableEval))
            using (var ctx = rt.CreateContext())
            using (ctx.Scope())
            {
                var tsBuffer = GetSerializedTypeScriptCompiler(ctx);
                var fnTypeScript = ctx.ParseSerializedScript(tsBuffer, "[typescript]");

                //Create the compiler options object that specifies the compiler options.
                var objCompilerOptions = ctx.CreateObject();
                objCompilerOptions["target"] = ctx.CreateNumber((int)options.CompilerOptions.Target);
                objCompilerOptions["module"] = ctx.CreateNumber((int)options.CompilerOptions.Module);
                objCompilerOptions["jsx"] = ctx.CreateNumber((int)options.CompilerOptions.Jsx);
                if (options.CompilerOptions.ImportHelpers.HasValue)
                    objCompilerOptions["importHelpers"] = options.CompilerOptions.ImportHelpers.Value ? ctx.True : ctx.False;

                //Create the transpile options object that specifies the transpilation options
                var objTranspileOptions = ctx.CreateObject();
                objTranspileOptions["compilerOptions"] = objCompilerOptions;
                objTranspileOptions["fileName"] = ctx.CreateString(options.FileName);

                ////All systems go. Perform the transpilation.
                var jsScriptToTranspile = ctx.CreateString(options.Script);

                var jsTs = fnTypeScript.Call<JsObject>();
                var fnTranspileModule = jsTs.GetProperty<JsFunction>("transpileModule");
                var objTranspiled = fnTranspileModule.Call<JsObject>(jsTs, jsScriptToTranspile, objTranspileOptions);

                var output = new TranspileOutput
                {
                    OutputText = objTranspiled["outputText"].ToString(),
                    SourceMapText = objTranspiled["sourceMapText"].ToString()
                };
                return output;
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

        private static object s_serializedTypeScriptCompilerLock = new object();
        private static byte[] s_serializedTypeScriptCompiler = null;

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

        private static byte[] GetSerializedTypeScriptCompiler(BaristaContext context)
        {
            if (null == s_serializedTypeScriptCompiler)
            {
                lock (s_serializedTypeScriptCompilerLock)
                {
                    if (null == s_serializedTypeScriptCompiler)
                    {
                        var ts = EmbeddedResourceHelper.LoadResource(typeof(TypeScriptTranspiler).Assembly, "BaristaLabs.BaristaCore.Scripts.typescript.min.js");
                        var typeScriptCompilerScript = $@"(() => {{
'use strict';
const module = {{
    exports: {{}}
}};
let exports = module.exports;
{ts}
return module.exports;
}})();";
                        s_serializedTypeScriptCompiler = context.SerializeScript(typeScriptCompilerScript);
                    }
                }
            }

            return s_serializedTypeScriptCompiler;
        }
    }
}
