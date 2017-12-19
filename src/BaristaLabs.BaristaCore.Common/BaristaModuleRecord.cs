namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Represents a JavaScript Module
    /// </summary>
    public sealed class BaristaModuleRecord : IDisposable
    {
        private readonly string m_name;
        private readonly IJavaScriptEngine m_engine;
        private readonly BaristaContext m_context;
        private readonly IBaristaModuleRecordFactory m_moduleRecordFactory;
        private readonly IBaristaModuleLoader m_moduleLoader;
        private readonly JavaScriptModuleRecord m_moduleRecord;

        private readonly GCHandle m_fetchImportedModuleCallbackHandle = default(GCHandle);
        private readonly GCHandle m_notifyCallbackHandle = default(GCHandle);

        private readonly Dictionary<string, BaristaModuleRecord> m_importedModules = new Dictionary<string, BaristaModuleRecord>();

        private readonly BaristaModuleRecord m_parentModule; 

        public BaristaModuleRecord(string name, BaristaModuleRecord parentModule, IJavaScriptEngine engine, BaristaContext context, IBaristaModuleRecordFactory moduleRecordFactory, IBaristaModuleLoader moduleLoader, JavaScriptModuleRecord moduleRecord)
        {
            m_name = name ?? throw new ArgumentNullException(nameof(name));
            m_parentModule = parentModule;
            m_engine = engine ?? throw new ArgumentNullException(nameof(engine));
            m_context = context ?? throw new ArgumentNullException(nameof(context));
            m_moduleRecordFactory = moduleRecordFactory ?? throw new ArgumentNullException(nameof(moduleRecordFactory));
            m_moduleRecord = moduleRecord ?? throw new ArgumentNullException(nameof(moduleRecord));

            m_moduleLoader = moduleLoader;

            //Associate functions that will handle module loading

            if (m_parentModule == null)
            {
                //Set the fetch module callback for the module.
                m_fetchImportedModuleCallbackHandle = InitFetchImportedModuleCallback(m_moduleRecord);

                //Set the notify callback for the module.
                m_notifyCallbackHandle = InitNotifyModuleReadyCallback(m_moduleRecord);
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

                    if (m_fetchImportedModuleCallbackHandle != default(GCHandle) && m_fetchImportedModuleCallbackHandle.IsAllocated)
                    {
                        Engine.JsSetModuleHostInfo(moduleRecord, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback, IntPtr.Zero);
                        m_fetchImportedModuleCallbackHandle.Free();
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

                try
                {
                    IsReady = true;
                    return false;
                }
                catch (Exception ex)
                {
                    if (!Engine.JsHasException())
                    {
                        Engine.JsSetException(Context.ValueFactory.CreateError(ex.Message).Handle);
                    }
                    return true;
                }
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
        /// Gets the underlying JavaScriptModuleRecord
        /// </summary>
        public JavaScriptModuleRecord ModuleRecord
        {
            get { return m_moduleRecord; }
        }

        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        private IJavaScriptEngine Engine
        {
            get { return m_engine; }
        }

        private BaristaContext Context
        {
            get { return m_context; }
        }
        #endregion

        public JsError ParseModuleSource(string script)
        {
            if (!TryParseModuleSource(script, out JsError parseResult))
            {
                if (!Engine.JsHasException())
                {
                    Engine.JsSetException(parseResult.Handle);
                }

                return parseResult;
            }

            return null;
        }

        /// <summary>
        /// Attempts to parse the specified source within the module. If the parse was successful, returns true, otherwise false and the parseResult is set.
        /// </summary>
        /// <remarks>
        /// This method should only be called once. If called multiple times a JavaScriptFatalException of type ModuleParsed occurs.
        /// </remarks>
        /// <param name="script"></param>
        /// <param name="parseResult"></param>
        /// <returns></returns>
        public bool TryParseModuleSource(string script, out JsError parseResult)
        {
            var scriptBuffer = Encoding.UTF8.GetBytes(script);

            var parseResultHandle = Engine.JsParseModuleSource(m_moduleRecord, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)scriptBuffer.Length, JavaScriptParseModuleSourceFlags.DataIsUTF8);
            if (parseResultHandle != JavaScriptValueSafeHandle.Invalid)
            {
                parseResult = Context.ValueFactory.CreateValue<JsError>(parseResultHandle);
                return false;
            }

            parseResult = null;
            return true;
        }

        private bool FetchImportedModule(JavaScriptModuleRecord referencingModule, JavaScriptValueSafeHandle specifier, out IntPtr dependentModule)
        {
            var moduleName = Context.ValueFactory.CreateValue(specifier).ToString();

            //If the current module name is equal to the fetching module name, return this value.
            if (Name == moduleName)
            {
                //Top-level self-referencing module. Reference itself.
                dependentModule = referencingModule.DangerousGetHandle();
                return false;
            }

            if (m_importedModules.ContainsKey(moduleName))
            {
                //The module has already been imported, return the existing JavaScriptModuleRecord
                dependentModule = m_importedModules[moduleName].m_moduleRecord.DangerousGetHandle();
                return false;
            }
            else if (m_moduleLoader != null)
            {
                var module = m_moduleLoader.GetModule(moduleName);

                if (module != null)
                {
                    var newModule = m_moduleRecordFactory.CreateBaristaModuleRecord(Context, moduleName, this, false);
                    m_importedModules.Add(moduleName, newModule);
                    
                    switch (module)
                    {
                        //For the built-in Script Module type, parse the string returned by InstallModule and install it as a module.
                        case BaristaScriptModule scriptModule:
                            var script = (scriptModule.ExportDefault(Context, referencingModule)).GetAwaiter().GetResult() as string;
                            if (script == null)
                                script = "export default null";

                            dependentModule = newModule.ModuleRecord.DangerousGetHandle();
                            if (newModule.TryParseModuleSource(script, out JsError parseResult) == false)
                            {
                                if (!Engine.JsHasException())
                                {
                                    Engine.JsSetException(parseResult.Handle);
                                }
                                return true;
                            }

                            return false;
                        //Otherwise, install the module.
                        default:
                            var result = InstallModule(newModule, module, specifier, referencingModule);

                            dependentModule = newModule.ModuleRecord.DangerousGetHandle();
                            return result;
                    }
                }
            }

            dependentModule = referencingModule.DangerousGetHandle();
            return true;
        }

        private bool InstallModule(BaristaModuleRecord moduleRecord, IBaristaModule module, JavaScriptValueSafeHandle specifier, JavaScriptModuleRecord referencingModuleRecord)
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
                return CreateSingleValueModule(moduleRecord, specifier, convertedValue);
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
            if (!moduleRecord.TryParseModuleSource(exposeNativeValueScript, out JsError parseResult))
            {
                if (!Engine.JsHasException())
                {
                    Engine.JsSetException(parseResult.Handle);
                }
                return true;
            }
            return false;
        }

        #region IDisposable Support
        private bool m_isDisposed = false;

        private void Dispose(bool disposing)
        {
            if (!m_isDisposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                }

                // free unmanaged resources (unmanaged objects)

                if (m_fetchImportedModuleCallbackHandle != default(GCHandle) && m_fetchImportedModuleCallbackHandle.IsAllocated)
                {
                    try
                    {
                        //if (m_fetchImportedModuleCallbackHandle.AddrOfPinnedObject() == m_engine.JsGetModuleHostInfo(m_moduleRecord, JavaScriptModuleHostInfoKind.FetchImportedModuleFromScriptCallback))
                        //{
                            m_engine.JsSetModuleHostInfo(m_moduleRecord, JavaScriptModuleHostInfoKind.FetchImportedModuleFromScriptCallback, IntPtr.Zero);
                        //}

                        //if (m_fetchImportedModuleCallbackHandle.AddrOfPinnedObject() == m_engine.JsGetModuleHostInfo(m_moduleRecord, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback))
                        //{
                            m_engine.JsSetModuleHostInfo(m_moduleRecord, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback, IntPtr.Zero);
                        //}
                    }
                    catch
                    {
                        //Do Nothing...
                    }

                    m_fetchImportedModuleCallbackHandle.Free();
                }

                if (m_notifyCallbackHandle != default(GCHandle) && m_notifyCallbackHandle.IsAllocated)
                {
                    //if (m_notifyCallbackHandle.AddrOfPinnedObject() == m_engine.JsGetModuleHostInfo(m_moduleRecord, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback))
                    //{
                        m_engine.JsSetModuleHostInfo(m_moduleRecord, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback, IntPtr.Zero);
                    //}

                    m_notifyCallbackHandle.Free();
                }

                m_moduleRecord.Dispose();
                // TODO: set large fields to null.

                m_isDisposed = true;
            }
        }

         ~BaristaModuleRecord()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public override string ToString()
        {
            return $"{Name} ({m_moduleRecord.DangerousGetHandle().ToString()})";
        }
    }
}
