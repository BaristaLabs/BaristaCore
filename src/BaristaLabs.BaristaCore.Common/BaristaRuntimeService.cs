namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Represents a class that is used to instantiate new BaristaRuntimes.
    /// </summary>
    public sealed class BaristaRuntimeService : IBaristaRuntimeService
    {
        private BaristaObjectPool<BaristaRuntime, JavaScriptRuntimeSafeHandle> m_runtimePool = new BaristaObjectPool<BaristaRuntime, JavaScriptRuntimeSafeHandle>();
        private readonly IJavaScriptEngine m_engine;
        private readonly IServiceProvider m_serviceProvider;

        public BaristaRuntimeService(IJavaScriptEngine engine, IServiceProvider serviceProvider)
        {
            m_engine = engine ?? throw new ArgumentNullException(nameof(engine));
            m_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public int Count
        {
            get { return m_runtimePool.Count; }
        }

        /// <summary>
        /// Creates a new JavaScript Runtime.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public BaristaRuntime CreateRuntime(JavaScriptRuntimeAttributes attributes = JavaScriptRuntimeAttributes.None)
        {
            var contextService = m_serviceProvider.GetRequiredService<IBaristaContextService>();

            var runtimeHandle = m_engine.JsCreateRuntime(attributes, null);
            var result = m_runtimePool.GetOrAdd(runtimeHandle, () => {
                var rt = new BaristaRuntime(m_engine, contextService, runtimeHandle);
                //Do not wire up a runtime's BeforeCollect handler with the the service as this will
                //remove and dispose of the runtime on ANY garbage collect, which will eventually
                //crash the engine.

                //After a runtime handle is disposed, remove the handle from the pool.
                void afterDispose(object sender, EventArgs args)
                {
                    rt.AfterDispose -= afterDispose;
                    Debug.Assert(m_runtimePool != null);
                    m_runtimePool.RemoveHandle(runtimeHandle);
                }

                rt.AfterDispose += afterDispose;
                return rt;
            });

            if (result == null)
            {
                throw new InvalidOperationException("Unable to create or retrieve a new Barista Runtime.");
            }

            return result;
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
        /// Disposes of the service and all references contained within.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BaristaRuntimeService()
        {
            Dispose(false);
        }
        #endregion
    }
}
