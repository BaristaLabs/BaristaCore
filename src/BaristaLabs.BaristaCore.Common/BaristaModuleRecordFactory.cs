namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public sealed class BaristaModuleRecordFactory : IBaristaModuleRecordFactory
    {
        private BaristaObjectPool<BaristaModuleRecord, JavaScriptModuleRecord> m_moduleReferencePool;

        private readonly IJavaScriptEngine m_engine;
        private readonly IServiceProvider m_serviceProvider;

        public BaristaModuleRecordFactory(IJavaScriptEngine engine, IServiceProvider serviceProvider)
        {
            m_engine = engine ?? throw new ArgumentNullException(nameof(engine));
            m_serviceProvider = serviceProvider;
            m_moduleReferencePool = new BaristaObjectPool<BaristaModuleRecord, JavaScriptModuleRecord>();
        }

        public BaristaModuleRecord GetBaristaModuleRecord(JavaScriptModuleRecord moduleRecord)
        {
            if (moduleRecord == null)
                throw new ArgumentNullException(nameof(moduleRecord));

            if (m_moduleReferencePool.TryGet(moduleRecord, out BaristaModuleRecord existingModuleRecord))
            {
                return existingModuleRecord;
            }

            return null;
        }

        public BaristaModuleRecord CreateBaristaModuleRecord(BaristaContext context, string moduleName, BaristaModuleRecord parentModule = null, bool setAsHost = false)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var moduleNameValue = context.ValueFactory.CreateString(moduleName);
            var moduleRecord = m_engine.JsInitializeModuleRecord(parentModule == null ? JavaScriptModuleRecord.Invalid : parentModule.Handle, moduleNameValue.Handle);

            return m_moduleReferencePool.GetOrAdd(moduleRecord, () =>
            {
                var moduleLoader = m_serviceProvider.GetRequiredService<IBaristaModuleLoader>();
                var module = new BaristaModuleRecord(moduleName, parentModule, m_engine, context, this, moduleLoader, moduleRecord);

                //If specified, indicate as host module.
                if (setAsHost == true)
                {
                    m_engine.JsSetModuleHostInfo(moduleRecord, JavaScriptModuleHostInfoKind.HostDefined, moduleNameValue.Handle.DangerousGetHandle());
                }

                return module;
            });
        }

        #region IDisposable
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_moduleReferencePool != null)
                {
                    m_moduleReferencePool.Dispose();
                    m_moduleReferencePool = null;
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

        ~BaristaModuleRecordFactory()
        {
            Dispose(false);
        }
        #endregion
    }
}
