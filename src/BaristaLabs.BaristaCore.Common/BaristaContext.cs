namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.JavaScript.Extensions;
    using BaristaLabs.BaristaCore.Tasks;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Represents a JavaScript Context
    /// </summary>
    /// <remarks>
    ///     Each script context has its own global object that is isolated from all other script contexts.
    /// </remarks>
    public sealed class BaristaContext : BaristaObject<JavaScriptContextSafeHandle>
    {
        private readonly Lazy<JsUndefined> m_undefinedValue;
        private readonly Lazy<JsNull> m_nullValue;
        private readonly Lazy<JsBoolean> m_trueValue;
        private readonly Lazy<JsBoolean> m_falseValue;
        private readonly Lazy<JsJSON> m_jsonValue;
        private readonly Lazy<JsObject> m_globalValue;
        private readonly Lazy<JsObjectConstructor> m_objectValue;
        private readonly Lazy<JsPromise> m_promiseValue;
        
        private readonly IBaristaValueFactory m_valueFactory;
        private readonly IBaristaConversionStrategy m_conversionStrategy;
        private readonly IPromiseTaskQueue m_promiseTaskQueue;
        private readonly IBaristaModuleLoader m_moduleLoader;

        private readonly TaskFactory m_taskFactory;
        private readonly GCHandle m_beforeCollectCallbackDelegateHandle;

        //0 for false, 1 for true.
        private int m_withinScope = 0;
        private BaristaExecutionScope m_currentExecutionScope;

        /// <summary>
        /// Creates a new instance of a Barista Context.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="contextHandle"></param>
        public BaristaContext(IJavaScriptEngine engine, IBaristaValueFactoryBuilder valueFactoryBuilder, IBaristaConversionStrategy conversionStrategy, IPromiseTaskQueue taskQueue, IBaristaModuleLoader moduleLoader, JavaScriptContextSafeHandle contextHandle)
            : base(engine, contextHandle)
        {
            if (valueFactoryBuilder == null)
                throw new ArgumentNullException(nameof(valueFactoryBuilder));

            m_taskFactory = new TaskFactory(
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                TaskContinuationOptions.None,
                new CurrentThreadTaskScheduler());

            m_conversionStrategy = conversionStrategy ?? throw new ArgumentNullException(nameof(conversionStrategy));

            m_valueFactory = valueFactoryBuilder.CreateValueFactory(this);

            m_undefinedValue = new Lazy<JsUndefined>(() => m_valueFactory.GetUndefinedValue());
            m_nullValue = new Lazy<JsNull>(() => m_valueFactory.GetNullValue());
            m_trueValue = new Lazy<JsBoolean>(() => m_valueFactory.GetTrueValue());
            m_falseValue = new Lazy<JsBoolean>(() => m_valueFactory.GetFalseValue());
            m_globalValue = new Lazy<JsObject>(() => m_valueFactory.GetGlobalObject());
            m_jsonValue = new Lazy<JsJSON>(() =>
            {
                var global = m_globalValue.Value;
                return global.GetProperty<JsJSON>("JSON");
            });
            m_objectValue = new Lazy<JsObjectConstructor>(() =>
            {
                var global = m_globalValue.Value;
                return global.GetProperty<JsObjectConstructor>("Object");
            });
            m_promiseValue = new Lazy<JsPromise>(() =>
            {
                var global = m_globalValue.Value;
                return global.GetProperty<JsPromise>("Promise");
            });


            m_promiseTaskQueue = taskQueue;
            m_moduleLoader = moduleLoader;

            //Set the event that will be called prior to the engine collecting the context.
            JavaScriptObjectBeforeCollectCallback beforeCollectCallback = (IntPtr handle, IntPtr callbackState) =>
            {
                OnBeforeCollect(handle, callbackState);
            };

            m_beforeCollectCallbackDelegateHandle = GCHandle.Alloc(beforeCollectCallback);
            Engine.JsSetObjectBeforeCollectCallback(contextHandle, IntPtr.Zero, beforeCollectCallback);
        }

        #region Properties

        /// <summary>
        /// Gets the conversion strategy associated with the context.
        /// </summary>
        public IBaristaConversionStrategy Converter
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_conversionStrategy;
            }
        }

        /// <summary>
        /// Gets the current execution scope.
        /// </summary>
        public BaristaExecutionScope CurrentScope
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_currentExecutionScope;
            }
        }

        /// <summary>
        /// Gets the False Value associated with the context.
        /// </summary>
        public JsBoolean False
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_falseValue.Value;
            }
        }

        /// <summary>
        /// Gets the Global Object associated with the context.
        /// </summary>
        public JsObject GlobalObject
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_globalValue.Value;
            }
        }

        /// <summary>
        /// Gets the global JSON built-in.
        /// </summary>
        public JsJSON JSON
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_jsonValue.Value;
            }
        }

        /// <summary>
        /// Gets a value that indicates if a current execution scope exists.
        /// </summary>
        public bool HasCurrentScope
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_currentExecutionScope != null;
            }
        }

        /// <summary>
        /// Gets the Null Value associated with the context.
        /// </summary>
        public JsNull Null
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_nullValue.Value;
            }
        }

        /// <summary>
        /// Gets the global Object built-in.
        /// </summary>
        public JsObjectConstructor Object
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_objectValue.Value;
            }
        }

        /// <summary>
        /// Gets the global Promise built-in.
        /// </summary>
        public JsPromise Promise
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_promiseValue.Value;
            }
        }

        /// <summary>
        /// Gets the True Value associated with the context.
        /// </summary>
        public JsBoolean True
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_trueValue.Value;
            }
        }

        /// <summary>
        /// Gets a task factory that supports creating and scheduling Task objects associated with the context. 
        /// </summary>
        public TaskFactory TaskFactory
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_taskFactory;
            }
        }

        /// <summary>
        /// Gets the value factory associated with the context.
        /// </summary>
        public IBaristaValueFactory ValueFactory
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_valueFactory;
            }
        }

        /// <summary>
        /// Gets the Undefined Value associated with the context.
        /// </summary>
        public JsUndefined Undefined
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_undefinedValue.Value;
            }
        }
        #endregion

        /// <summary>
        /// Evaluates the specified script as a module, the default export will be the returned value.
        /// </summary>
        /// <param name="script">Script to evaluate.</param>
        /// <returns></returns>
        public JsValue EvaluateModule(string script)
        {
            //Create a scope if we're not currently in one.
            BaristaExecutionScope scope = null;
            if (!HasCurrentScope)
                scope = Scope();

            try
            {
                var globalName = EvaluateModuleInternal(script);
                return GlobalObject.GetProperty(globalName);
            }
            finally
            {
                if (scope != null)
                    scope.Dispose();
            }
        }

        /// <summary>
        /// Evaluates the specified script as a module, the default export will be the returned value.
        /// </summary>
        /// <param name="script">Script to evaluate.</param>
        /// <returns></returns>
        public T EvaluateModule<T>(string script)
            where T : JsValue
        {
            //Create a scope if we're not currently in one.
            BaristaExecutionScope scope = null;
            if (!HasCurrentScope)
                scope = Scope();

            try
            {
                var globalName = EvaluateModuleInternal(script);
                return GlobalObject.GetProperty<T>(globalName);
            }
            finally
            {
                if (scope != null)
                    scope.Dispose();
            }
        }

        private string EvaluateModuleInternal(string script)
        {
            var mainModuleName = "";
            var subModuleId = Guid.NewGuid();
            var subModuleName = subModuleId.ToString();

            //Define a shim script that will set a global to the result of the script run as a module.
            //If there is a promise task queue defined, have the script auto-resolve any promises.
            string mainModuleScript;
            if (m_promiseTaskQueue == null)
            {
                mainModuleScript = $@"
import child from '{subModuleName}';
let global = (new Function('return this;'))();
global.$EXPORTS = child;
";
            }
            else
            {
                mainModuleScript = $@"
import child from '{subModuleName}';
let global = (new Function('return this;'))();
(async () => await child)().then((result) => {{ global.$EXPORTS = result; }}, (reject) => {{ global.$ERROR = reject }});
";
            }

            var mainModuleReady = false;

            var mainModuleNameHandle = Engine.JsCreateString(mainModuleName, (ulong)mainModuleName.Length);
            var mainModuleHandle = Engine.JsInitializeModuleRecord(JavaScriptModuleRecord.Invalid, mainModuleNameHandle);
            var subModuleNameHandle = Engine.JsCreateString(subModuleName, (ulong)subModuleName.Length);
            var subModuleHandle = Engine.JsInitializeModuleRecord(mainModuleHandle, subModuleNameHandle);

            //Associate functions that will handle module loading

            //Set the fetch callback.
            JavaScriptFetchImportedModuleCallback fetchImportedModule = GetFetchImportedModuleDelegate(subModuleName, subModuleHandle);
            var fetchImportedModuleCallbackHandle = GCHandle.Alloc(fetchImportedModule);
            IntPtr fetchCallbackPtr = Marshal.GetFunctionPointerForDelegate(fetchImportedModule);
            Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback, fetchCallbackPtr);

            //Set the notify callback.
            JavaScriptNotifyModuleReadyCallback mainModuleNotifyCallback = (IntPtr referencingModule, IntPtr exceptionVar) =>
            {
                if (exceptionVar != IntPtr.Zero)
                {
                    JsError error = m_valueFactory.CreateValue<JsError>(new JavaScriptValueSafeHandle(exceptionVar));
                    throw new BaristaScriptException(error);
                }

                mainModuleReady = true;
                return false;
            };

            var mainModuleNotifyCallbackHandle = GCHandle.Alloc(mainModuleNotifyCallback);
            IntPtr notifyCallbackPtr = Marshal.GetFunctionPointerForDelegate(mainModuleNotifyCallback);
            Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback, notifyCallbackPtr);

            //Indicate the host-defined, main module.
            Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.HostDefined, mainModuleNameHandle.DangerousGetHandle());

            //Now start the parsing.

            //First, parse our main module script.
            var mainModuleScriptBuffer = Encoding.UTF8.GetBytes(mainModuleScript);
            Engine.JsParseModuleSource(mainModuleHandle, JavaScriptSourceContext.GetNextSourceContext(), mainModuleScriptBuffer, (uint)mainModuleScriptBuffer.LongLength, JavaScriptParseModuleSourceFlags.DataIsUTF8);

            //Now Parse the user-provided script.
            var scriptBuffer = Encoding.UTF8.GetBytes(script);
            Engine.JsParseModuleSource(subModuleHandle, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)scriptBuffer.Length, JavaScriptParseModuleSourceFlags.DataIsUTF8);


            Debug.Assert(mainModuleReady, "Main module is not ready. Ensure all script modules are loaded.");
            
            //Now we're ready, evaluate the main module.

            try
            {
                Engine.JsModuleEvaluation(mainModuleHandle);

                //Evaluate any pending promises.
                CurrentScope.ResolvePendingPromises();

                if (m_promiseTaskQueue != null && GlobalObject.HasOwnProperty("$ERROR"))
                {
                    throw new BaristaScriptException(GlobalObject.GetProperty<JsObject>("$ERROR"));
                }

                //Return the name of the global.
                return "$EXPORTS";
            }
            finally
            {
                Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback, IntPtr.Zero);
                fetchImportedModuleCallbackHandle.Free();

                //Unset the Notify callback.
                Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.NotifyModuleReadyCallback, IntPtr.Zero);
                mainModuleNotifyCallbackHandle.Free();
            }
        }

        /// <summary>
        /// Returns a new JavaScript Execution Scope to perform work in.
        /// </summary>
        /// <returns></returns>
        public BaristaExecutionScope Scope()
        {
            if (0 == Interlocked.Exchange(ref m_withinScope, 1))
            {
                Engine.JsSetCurrentContext(Handle);
                m_currentExecutionScope = new BaristaExecutionScope(this, m_promiseTaskQueue, ReleaseScope);
                return m_currentExecutionScope;
            }
            else
            {
                throw new BaristaException("This context already has an active execution scope.");
            }
        }

        private JavaScriptFetchImportedModuleCallback GetFetchImportedModuleDelegate(string childModuleName, JavaScriptModuleRecord childModuleRecord)
        {
            var importedModules = new Dictionary<string, JavaScriptModuleRecord>();

            return (IntPtr referencingModule, IntPtr specifier, out IntPtr dependentModule) =>
            {
                var specifierHandle = new JavaScriptValueSafeHandle(specifier);
                var moduleName = Engine.GetStringUtf8(specifierHandle);

                // Leaving this code here for postarity -- the way we do module loading would cause the following scenario to be exceedingly rare.
                // That is, the top-level module would need to know a-priori the guid-based sub-module name. If this happens, better go buy a scratch-off.
                //if (moduleName == childModuleName)
                //{
                //    //Top-level self-referencing module. Reference itself.
                //    dependentModule = childModuleRecord.DangerousGetHandle();
                //    return false;
                //}

                if (importedModules.ContainsKey(moduleName))
                {
                    //The module has already been imported, return the existing JavaScriptModuleRecord
                    dependentModule = importedModules[moduleName].DangerousGetHandle();
                    return false;
                }
                else if (m_moduleLoader != null)
                {
                    var module = m_moduleLoader.GetModule(moduleName);

                    if (module != null)
                    {
                        var referencingModuleRecord = new JavaScriptModuleRecord(referencingModule);

                        switch (module)
                        {
                            //For the built-in Script Module type, parse the string returned by InstallModule and install it as a module.
                            case BaristaScriptModule scriptModule:
                                var script = (scriptModule.ExportDefault(this, referencingModuleRecord)).GetAwaiter().GetResult() as string;
                                if (script == null)
                                    script = "export default null";
                                var moduleRecord = Engine.JsInitializeModuleRecord(referencingModuleRecord, specifierHandle);
                                importedModules.Add(moduleName, moduleRecord);
                                var scriptBuffer = Encoding.UTF8.GetBytes(script);
                                Engine.JsParseModuleSource(moduleRecord, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)scriptBuffer.LongLength, JavaScriptParseModuleSourceFlags.DataIsUTF8);
                                dependentModule = moduleRecord.DangerousGetHandle();
                                return false;
                            //Otherwise, install the module.
                            default:
                                var result = InstallModule(module, referencingModuleRecord, specifierHandle, out JavaScriptModuleRecord dependentModuleRecord);
                                importedModules.Add(moduleName, dependentModuleRecord);
                                dependentModule = dependentModuleRecord.DangerousGetHandle();
                                return result;
                        }
                    }
                }

                dependentModule = referencingModule;
                return true;
            };
        }

        private bool InstallModule(IBaristaModule module, JavaScriptModuleRecord referencingModuleRecord, JavaScriptValueSafeHandle specifierHandle, out JavaScriptModuleRecord dependentModuleRecord)
        {
            object moduleValue;
            try
            {
                moduleValue = module.ExportDefault(this, referencingModuleRecord).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new BaristaException($"An error occurred while obtaining a module's default export: {module.Name}.", ex);
            }

            if (Converter.TryFromObject(this, moduleValue, out JsValue convertedValue))
            {
                return CreateSingleValueModule(convertedValue.Handle, referencingModuleRecord, specifierHandle, out dependentModuleRecord);
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
        private bool CreateSingleValueModule(JavaScriptValueSafeHandle valueSafeHandle, JavaScriptModuleRecord referencingModuleRecord, JavaScriptValueSafeHandle specifierHandle, out JavaScriptModuleRecord dependentModuleRecord)
        {
            var globalId = Guid.NewGuid();
            var exposeNativeValueScript = $@"
let global = (new Function('return this;'))();
let defaultExport = global['$DEFAULTEXPORT_{globalId.ToString()}'];
export default defaultExport;
";
            var moduleRecord = Engine.JsInitializeModuleRecord(referencingModuleRecord, specifierHandle);

            Engine.SetGlobalVariable($"$DEFAULTEXPORT_{globalId.ToString()}", valueSafeHandle);
            var scriptBuffer = Encoding.UTF8.GetBytes(exposeNativeValueScript);
            Engine.JsParseModuleSource(moduleRecord, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)scriptBuffer.LongLength, JavaScriptParseModuleSourceFlags.DataIsUTF8);
            dependentModuleRecord = moduleRecord;
            return false;
        }

        private void ReleaseScope()
        {
            Engine.JsSetCurrentContext(JavaScriptContextSafeHandle.Invalid);
            m_currentExecutionScope = null;

            //Release the lock
            Interlocked.Exchange(ref m_withinScope, 0);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                BaristaExecutionScope scope = null;
                if (!HasCurrentScope)
                    scope = Scope();
                try
                {
                    m_valueFactory.Dispose();
                }
                finally
                {
                    if (scope != null)
                        scope.Dispose();
                }
            }

            if (!IsDisposed)
            {
                //Unset the before collect callback.
                Engine.JsSetObjectBeforeCollectCallback(Handle, IntPtr.Zero, null);
            }

            base.Dispose(disposing);
        }
    }
}
