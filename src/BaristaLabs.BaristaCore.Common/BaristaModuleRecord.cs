namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Represents a JavaScript Module
    /// </summary>
    public sealed class BaristaModuleRecord : BaristaObject<JavaScriptModuleRecord>
    {
        private readonly string m_name;
        private readonly BaristaContext m_context;
        private readonly IBaristaModuleRecordFactory m_moduleRecordFactory;
        private readonly IBaristaModuleLoader m_moduleLoader;

        private readonly GCHandle m_fetchImportedModuleCallbackHandle = default(GCHandle);
        private readonly GCHandle m_notifyCallbackHandle = default(GCHandle);

        private readonly Dictionary<string, BaristaModuleRecord> m_importedModules = new Dictionary<string, BaristaModuleRecord>();

        private readonly BaristaModuleRecord m_parentModule; 

        public BaristaModuleRecord(string name, BaristaModuleRecord parentModule, IJavaScriptEngine engine, BaristaContext context, IBaristaModuleRecordFactory moduleRecordFactory, IBaristaModuleLoader moduleLoader, JavaScriptModuleRecord moduleRecord)
            : base(engine, moduleRecord)
        {
            m_name = name ?? throw new ArgumentNullException(nameof(name));
            m_parentModule = parentModule;
            m_context = context ?? throw new ArgumentNullException(nameof(context));
            m_moduleRecordFactory = moduleRecordFactory ?? throw new ArgumentNullException(nameof(moduleRecordFactory));

            m_moduleLoader = moduleLoader;

            //Associate functions that will handle module loading

            if (m_parentModule == null)
            {
                //Set the fetch module callback for the module.
                m_fetchImportedModuleCallbackHandle = InitFetchImportedModuleCallback(Handle);

                //Set the notify callback for the module.
                m_notifyCallbackHandle = InitNotifyModuleReadyCallback(Handle);
            }
        }

        private GCHandle InitFetchImportedModuleCallback(JavaScriptModuleRecord moduleRecord)
        {
            JavaScriptFetchImportedModuleCallback fetchImportedModule = (IntPtr referencingModule, IntPtr specifier, out IntPtr dependentModule) =>
            {
                try
                {
                    return FetchImportedModule(new JavaScriptModuleRecord(referencingModule), new JavaScriptValueSafeHandle(specifier), out dependentModule);
                }
                catch (Exception ex)
                {
                    if (Engine.JsHasException() == false)
                    {
                        Engine.JsSetException(Context.ValueFactory.CreateError(ex.Message).Handle);
                    }

                    dependentModule = referencingModule;
                    return true;
                }
            };

            var handle = GCHandle.Alloc(fetchImportedModule);
            IntPtr fetchCallbackPtr = Marshal.GetFunctionPointerForDelegate(handle.Target);
            Engine.JsSetModuleHostInfo(moduleRecord, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback, fetchCallbackPtr);
            Engine.JsSetModuleHostInfo(moduleRecord, JavaScriptModuleHostInfoKind.FetchImportedModuleFromScriptCallback, fetchCallbackPtr);
            return handle;
        }

        private GCHandle InitNotifyModuleReadyCallback(JavaScriptModuleRecord moduleRecord)
        {
            JavaScriptNotifyModuleReadyCallback moduleNotifyCallback = (IntPtr referencingModule, IntPtr exceptionVar) =>
            {
                if (exceptionVar != IntPtr.Zero)
                {
                    if (!Engine.JsHasException())
                    {
                        Engine.JsSetException(new JavaScriptValueSafeHandle(exceptionVar));
                    }
                    var ex = Context.ValueFactory.CreateValue(new JavaScriptValueSafeHandle(exceptionVar));
                    return true;
                }

                IsReady = true;
                return false;
            };

            var handle = GCHandle.Alloc(moduleNotifyCallback);
            IntPtr notifyCallbackPtr = Marshal.GetFunctionPointerForDelegate(handle.Target);
            Engine.JsSetModuleHostInfo(moduleRecord, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback, notifyCallbackPtr);
            return handle;
        }

        #region Properties
        /// <summary>
        /// Gets a value that indicates if the module's notify ready callback has been called.
        /// </summary>
        public bool IsReady
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        private BaristaContext Context
        {
            get { return m_context; }
        }
        #endregion

        public JsError ParseModuleSource(string script)
        {
            var scriptBuffer = Encoding.UTF8.GetBytes(script);
            var parseResultHandle = Engine.JsParseModuleSource(Handle, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)scriptBuffer.Length, JavaScriptParseModuleSourceFlags.DataIsUTF8);
            return Context.ValueFactory.CreateValue<JsError>(parseResultHandle);
        }

        private bool FetchImportedModule(JavaScriptModuleRecord jsReferencingModuleRecord, JavaScriptValueSafeHandle specifier, out IntPtr dependentModule)
        {
            var moduleName = Context.ValueFactory.CreateValue(specifier).ToString();
            var referencingModuleRecord = m_moduleRecordFactory.GetBaristaModuleRecord(jsReferencingModuleRecord);

            //If the current module name is equal to the fetching module name, return this value.
            if (Name == moduleName)
            {
                //Top-level self-referencing module. Reference itself.
                dependentModule = jsReferencingModuleRecord.DangerousGetHandle();
                return false;
            }

            if (m_importedModules.ContainsKey(moduleName))
            {
                //The module has already been imported, return the existing JavaScriptModuleRecord
                dependentModule = m_importedModules[moduleName].Handle.DangerousGetHandle();
                return false;
            }
            else if (m_moduleLoader != null)
            {
                var module = m_moduleLoader.GetModule(moduleName);

                if (module != null)
                {
                    var newModuleRecord = m_moduleRecordFactory.CreateBaristaModuleRecord(Context, moduleName, this, false);
                    m_importedModules.Add(moduleName, newModuleRecord);
                    
                    switch (module)
                    {
                        //For the built-in Script Module type, parse the string returned by ExportDefault and install it as a module.
                        case IBaristaScriptModule scriptModule:
                            var script = (scriptModule.ExportDefault(Context, newModuleRecord)).GetAwaiter().GetResult() as string;
                            if (script == null)
                                script = "export default null";

                            dependentModule = newModuleRecord.Handle.DangerousGetHandle();
                            newModuleRecord.ParseModuleSource(script);
                            return false;
                        //Otherwise, install the module.
                        default:
                            var result = InstallModule(newModuleRecord, referencingModuleRecord, module, specifier);

                            dependentModule = newModuleRecord.Handle.DangerousGetHandle();
                            return result;
                    }
                }
            }

            dependentModule = jsReferencingModuleRecord.DangerousGetHandle();
            return true;
        }

        private bool InstallModule(BaristaModuleRecord newModuleRecord, BaristaModuleRecord referencingModuleRecord, IBaristaModule module, JavaScriptValueSafeHandle specifier)
        {
            object moduleValue;
            try
            {
                moduleValue = module.ExportDefault(Context, referencingModuleRecord).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new BaristaException($"An error occurred while obtaining the default export of the native module named {module.Name}: {ex.Message}", ex);
            }

            if (Context.Converter.TryFromObject(Context, moduleValue, out JsValue convertedValue))
            {
                return CreateSingleValueModule(newModuleRecord, specifier, convertedValue);
            }
            else
            {
                throw new BaristaException($"Unable to install module {module.Name}: the default exported value could not be converted into a JavaScript object.");
            }
        }

        /// <summary>
        /// Creates a module that returns the specified value.
        /// </summary>
        /// <param name="valueSafeHandle"></param>
        /// <param name="referencingModuleRecord"></param>
        /// <param name="specifierHandle"></param>
        /// <param name="dependentModuleRecord"></param>
        /// <returns></returns>
        private bool CreateSingleValueModule(BaristaModuleRecord moduleRecord, JavaScriptValueSafeHandle specifier, JsValue defaultExportedValue)
        {
            var globalId = Guid.NewGuid();
            var exposeNativeValueScript = $@"
let global = (new Function('return this;'))();
let defaultExport = global['$DEFAULTEXPORT_{globalId.ToString()}'];
export default defaultExport;
";

            Context.GlobalObject.SetProperty($"$DEFAULTEXPORT_{globalId.ToString()}", defaultExportedValue);
            moduleRecord.ParseModuleSource(exposeNativeValueScript);
            return false;
        }

        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // free managed resources
                    foreach(var importedModule in m_importedModules.Values)
                    {
                        importedModule.Dispose();
                    }
                }

                // free unmanaged resources (unmanaged objects)
                if (m_parentModule == null)
                {
                    try
                    {
                        Engine.JsSetModuleHostInfo(Handle, JavaScriptModuleHostInfoKind.FetchImportedModuleFromScriptCallback, IntPtr.Zero);
                        Engine.JsSetModuleHostInfo(Handle, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback, IntPtr.Zero);
                        Engine.JsSetModuleHostInfo(Handle, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback, IntPtr.Zero);
                    }
                    catch
                    {
                        //Do Nothing...
                    }
                }

                if (m_fetchImportedModuleCallbackHandle != default(GCHandle) && m_fetchImportedModuleCallbackHandle.IsAllocated)
                {
                    m_fetchImportedModuleCallbackHandle.Free();
                }

                if (m_notifyCallbackHandle != default(GCHandle) && m_notifyCallbackHandle.IsAllocated)
                {
                    m_notifyCallbackHandle.Free();
                }
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
