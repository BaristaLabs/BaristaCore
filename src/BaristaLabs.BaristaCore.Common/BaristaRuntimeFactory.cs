namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// Represents a class that is used to instantiate new BaristaRuntimes.
    /// </summary>
    public sealed class BaristaRuntimeFactory : IBaristaRuntimeFactory
    {
        private JavaScriptReferencePool<BaristaRuntime, JavaScriptRuntimeSafeHandle> m_runtimePool = new JavaScriptReferencePool<BaristaRuntime, JavaScriptRuntimeSafeHandle>();
        private readonly IJavaScriptEngine m_engine;
        private readonly IServiceProvider m_serviceProvider;

        public BaristaRuntimeFactory(IJavaScriptEngine engine, IServiceProvider serviceProvider)
        {
            m_engine = engine ?? throw new ArgumentNullException(nameof(engine));
            m_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Creates a new JavaScript Runtime.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public BaristaRuntime CreateRuntime(JavaScriptRuntimeAttributes attributes = JavaScriptRuntimeAttributes.None)
        {
            var contextFactory = m_serviceProvider.GetRequiredService<IBaristaContextFactory>();

            var runtimeHandle = m_engine.JsCreateRuntime(attributes, null);
            return m_runtimePool.GetOrAdd(runtimeHandle, () => new BaristaRuntime(m_engine, contextFactory, runtimeHandle));
        }

        #region IDisposable
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_runtimePool != null)
                {
                    m_runtimePool.Dispose();
                    m_runtimePool = null;
                }
            }
        }

        /// <summary>
        /// Disposes of the factory and all references contained within.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BaristaRuntimeFactory()
        {
            Dispose(false);
        }
        #endregion
    }
}
