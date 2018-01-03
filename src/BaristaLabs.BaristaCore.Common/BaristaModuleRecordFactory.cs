namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.ModuleLoaders;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;

    public sealed class BaristaModuleRecordFactory : IBaristaModuleRecordFactory
    {
        private BaristaObjectPool<BaristaModuleRecord, JavaScriptModuleRecord> m_moduleReferencePool;
        private IDictionary<JavaScriptValueSafeHandle, JavaScriptModuleRecord> m_specifierModuleLookup;

        private readonly IJavaScriptEngine m_engine;
        private readonly IServiceProvider m_serviceProvider;

        public BaristaModuleRecordFactory(IJavaScriptEngine engine, IServiceProvider serviceProvider)
        {
            m_engine = engine ?? throw new ArgumentNullException(nameof(engine));
            m_serviceProvider = serviceProvider;
            m_moduleReferencePool = new BaristaObjectPool<BaristaModuleRecord, JavaScriptModuleRecord>();
            m_specifierModuleLookup = new Dictionary<JavaScriptValueSafeHandle, JavaScriptModuleRecord>();
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

            var moduleNameValue = context.CreateString(moduleName);
            return CreateBaristaModuleRecordInternal(context, moduleName, moduleNameValue.Handle, parentModule, setAsHost);
        }

        public BaristaModuleRecord CreateBaristaModuleRecord(BaristaContext context, JavaScriptValueSafeHandle specifier, BaristaModuleRecord parentModule = null, bool setAsHost = false)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (specifier == null || specifier.IsClosed || specifier.IsInvalid)
                throw new ArgumentNullException(nameof(specifier));

            var moduleNameValue = context.CreateValue<JsString>(specifier);
            if (moduleNameValue == null)
                throw new InvalidOperationException("Specifier is expected to be a string value.");

            return CreateBaristaModuleRecordInternal(context, moduleNameValue.ToString(), specifier, parentModule, setAsHost);
        }

        private BaristaModuleRecord CreateBaristaModuleRecordInternal(BaristaContext context, string moduleName, JavaScriptValueSafeHandle specifier, BaristaModuleRecord parentModule = null, bool setAsHost = false)
        {
            JavaScriptModuleRecord moduleRecord = null;
            if (m_specifierModuleLookup.ContainsKey(specifier))
                moduleRecord = m_specifierModuleLookup[specifier];

            if (moduleRecord == null || moduleRecord.IsClosed || moduleRecord.IsInvalid)
            {
                if (m_specifierModuleLookup.ContainsKey(specifier))
                    m_specifierModuleLookup.Remove(specifier);

                moduleRecord = m_engine.JsInitializeModuleRecord(parentModule == null ? JavaScriptModuleRecord.Invalid : parentModule.Handle, specifier);
            }

            return m_moduleReferencePool.GetOrAdd(moduleRecord, () =>
            {
                var moduleLoader = m_serviceProvider.GetRequiredService<IBaristaModuleLoader>();
                var module = new BaristaModuleRecord(moduleName.ToString(), specifier, parentModule, m_engine, context, this, moduleLoader, moduleRecord);
                m_specifierModuleLookup.Add(specifier, moduleRecord);

                //If specified, indicate as host module.
                if (setAsHost == true)
                {
                    m_engine.JsSetModuleHostInfo(moduleRecord, JavaScriptModuleHostInfoKind.HostDefined, specifier.DangerousGetHandle());
                }

                void beforeCollect(object sender, BaristaObjectBeforeCollectEventArgs args)
                {
                    context.BeforeCollect -= beforeCollect;
                    m_moduleReferencePool.RemoveHandle(new JavaScriptModuleRecord(args.Handle));
                    if (sender is BaristaModuleRecord baristaModuleRecord)
                    {
                        m_specifierModuleLookup.Remove(baristaModuleRecord.Specifier);
                    }
                }
                module.BeforeCollect += beforeCollect;
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
