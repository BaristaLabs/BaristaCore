namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public class BaristaModuleRecordFactory : IBaristaModuleRecordFactory
    {
        private readonly IJavaScriptEngine m_engine;
        private readonly IServiceProvider m_serviceProvider;

        public BaristaModuleRecordFactory(IJavaScriptEngine engine, IServiceProvider serviceProvider)
        {
            m_engine = engine ?? throw new ArgumentNullException(nameof(engine));
            m_serviceProvider = serviceProvider;
        }

        public BaristaModuleRecord CreateBaristaModuleRecord(BaristaContext context, string moduleName, BaristaModuleRecord parentModule = null, bool setAsHost = false)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var moduleLoader = m_serviceProvider.GetRequiredService<IBaristaModuleLoader>();
            var moduleNameValue = context.ValueFactory.CreateString(moduleName);
            var moduleRecord = m_engine.JsInitializeModuleRecord(parentModule == null ? JavaScriptModuleRecord.Invalid : parentModule.ModuleRecord, moduleNameValue.Handle);

            var module = new BaristaModuleRecord(moduleName, parentModule, m_engine, context, moduleLoader, moduleRecord);

            //If specified indicate as host module.
            if (setAsHost)
            {
                m_engine.JsSetModuleHostInfo(moduleRecord, JavaScriptModuleHostInfoKind.HostDefined, moduleNameValue.Handle.DangerousGetHandle());
            }

            return module;
        }
    }
}
