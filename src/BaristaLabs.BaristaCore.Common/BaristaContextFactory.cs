namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// Represents a factory that creates Barista Contexts.
    /// </summary>
    public sealed class BaristaContextFactory : IBaristaContextFactory
    {
        private BaristaObjectPool<BaristaContext, JavaScriptContextSafeHandle> m_contextPool;

        private readonly IJavaScriptEngine m_engine;
        private readonly IServiceProvider m_serviceProvider;

        public BaristaContextFactory(IJavaScriptEngine engine, IServiceProvider serviceProvider)
        {
            m_engine = engine ?? throw new ArgumentNullException(nameof(engine));
            m_serviceProvider = serviceProvider;
            m_contextPool = new BaristaObjectPool<BaristaContext, JavaScriptContextSafeHandle>();
        }

        public BaristaContext CreateContext(BaristaRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));

            if (runtime.IsDisposed)
                throw new ObjectDisposedException(nameof(runtime));

            var contextHandle = m_engine.JsCreateContext(runtime.Handle);
            return m_contextPool.GetOrAdd(contextHandle, () =>
            {
                var moduleRecordFactory = m_serviceProvider.GetRequiredService<IBaristaModuleRecordFactory>();
                var valueFactoryBuilder = m_serviceProvider.GetRequiredService<IBaristaValueFactoryBuilder>();
                var conversionStrategy = m_serviceProvider.GetRequiredService<IBaristaConversionStrategy>();

                //For flexability, a promise task queue is not required.
                var promiseTaskQueue = m_serviceProvider.GetService<IPromiseTaskQueue>();

                //Set the handle that will be called prior to the engine collecting the context.
                var context = new BaristaContext(m_engine, valueFactoryBuilder, conversionStrategy, moduleRecordFactory, promiseTaskQueue, contextHandle);

                void beforeCollect(object sender, BaristaObjectBeforeCollectEventArgs args)
                {
                    context.BeforeCollect -= beforeCollect;
                    m_contextPool.RemoveHandle(new JavaScriptContextSafeHandle(args.Handle));
                }
                context.BeforeCollect += beforeCollect;
                return context;
            });
        }

        #region IDisposable
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_contextPool != null)
                {
                    m_contextPool.Dispose();
                    m_contextPool = null;
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

        ~BaristaContextFactory()
        {
            Dispose(false);
        }
        #endregion
    }
}