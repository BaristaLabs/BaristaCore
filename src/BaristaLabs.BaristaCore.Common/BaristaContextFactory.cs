namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public sealed class BaristaContextFactory : IBaristaContextFactory
    {
        private BaristaObjectPool<BaristaContext, JavaScriptContextSafeHandle> m_contextPool;

        private readonly IJavaScriptEngine m_engine;
        private readonly IServiceProvider m_serviceProvider;

        public BaristaContextFactory(IJavaScriptEngine engine, IServiceProvider serviceProvider)
        {
            m_engine = engine ?? throw new ArgumentNullException(nameof(engine));
            m_serviceProvider = serviceProvider;
            m_contextPool = new BaristaObjectPool<BaristaContext, JavaScriptContextSafeHandle>((target) =>
            {
                m_engine.JsSetObjectBeforeCollectCallback(target.Handle, IntPtr.Zero, null);
            });
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
                var moduleService = m_serviceProvider.GetRequiredService<IBaristaModuleService>();
                var valueFactory = m_serviceProvider.GetRequiredService<IBaristaValueFactory>();

                //For flexability, a promise task queue is not required.
                var promiseTaskQueue = m_serviceProvider.GetService<IPromiseTaskQueue>();

                m_engine.JsSetObjectBeforeCollectCallback(contextHandle, IntPtr.Zero, OnBeforeCollectCallback);
                return new BaristaContext(m_engine, valueFactory, promiseTaskQueue, moduleService, contextHandle);
            });
        }

        private void OnBeforeCollectCallback(IntPtr handle, IntPtr callbackState)
        {
            //If the contextpool is null, this factory has already been disposed.
            if (m_contextPool == null)
                return;

            m_contextPool.RemoveHandle(new JavaScriptContextSafeHandle(handle));
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