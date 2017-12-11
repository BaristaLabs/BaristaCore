namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.JavaScript.Extensions;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    ///     Represents a JavaScript Context
    /// </summary>
    /// <remarks>
    ///     Each script context has its own global object that is isolated from all other script contexts.
    /// </remarks>
    public sealed class BaristaContext : BaristaObject<JavaScriptContextSafeHandle>
    {
        private const string ParseScriptSourceUrl = "[eval code]";

        private readonly Lazy<JsUndefined> m_undefinedValue;
        private readonly Lazy<JsNull> m_nullValue;
        private readonly Lazy<JsBoolean> m_trueValue;
        private readonly Lazy<JsBoolean> m_falseValue;
        private readonly Lazy<JsObject> m_globalValue;

        private readonly IPromiseTaskQueue m_promiseTaskQueue;
        private readonly IBaristaModuleService m_moduleService;

        private IBaristaValueFactory m_valueFactory;
        private BaristaExecutionScope m_currentExecutionScope;

        /// <summary>
        /// Creates a new instance of a Barista Context.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="contextHandle"></param>
        public BaristaContext(IJavaScriptEngine engine, IBaristaValueFactory valueFactory, IPromiseTaskQueue taskQueue, IBaristaModuleService moduleService, JavaScriptContextSafeHandle contextHandle)
            : base(engine, contextHandle)
        {
            m_valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
            m_valueFactory.Context = this;

            m_undefinedValue = new Lazy<JsUndefined>(() => m_valueFactory.GetUndefinedValue());
            m_nullValue = new Lazy<JsNull>(() => m_valueFactory.GetNullValue());
            m_trueValue = new Lazy<JsBoolean>(() => m_valueFactory.GetTrueValue());
            m_falseValue = new Lazy<JsBoolean>(() => m_valueFactory.GetFalseValue());
            m_globalValue = new Lazy<JsObject>(() => m_valueFactory.GetGlobalObject());

            m_moduleService = moduleService;

            m_promiseTaskQueue = taskQueue;
        }

        #region Properties
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
        /// Gets the value factory associated with the context.
        /// </summary>
        public IBaristaValueFactory ValueFactory
        {
            get { return m_valueFactory; }
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
            var result = EvaluateModuleInternal(script);
            return m_valueFactory.CreateValue(result);
        }

        /// <summary>
        /// Evaluates the specified script as a module, the default export will be the returned value.
        /// </summary>
        /// <param name="script">Script to evaluate.</param>
        /// <returns></returns>
        public T EvaluateModule<T>(string script)
            where T : JsValue
        {
            var result = EvaluateModuleInternal(script);
            return m_valueFactory.CreateValue<T>(result);
        }

        private JavaScriptValueSafeHandle EvaluateModuleInternal(string script)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(BaristaRuntime));

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
global.$EXPORTS = child; }});
";
            }
            else
            {
                mainModuleScript = $@"
import child from '{subModuleName}';
let global = (new Function('return this;'))();
(async () => await child)().then((result) => {{ global.$EXPORTS = result; }});
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
            IntPtr fetchCallbackPtr = Marshal.GetFunctionPointerForDelegate(fetchImportedModule);
            Engine.JsSetModuleHostInfo(mainModuleHandle, JavaScriptModuleHostInfoKind.FetchImportedModuleCallback, fetchCallbackPtr);

            //Set the notify callback.
            bool mainModuleNotifyCallback(IntPtr referencingModule, IntPtr exceptionVar)
            {
                if (exceptionVar != IntPtr.Zero)
                {
                    JsError error = m_valueFactory.CreateValue<JsError>(new JavaScriptValueSafeHandle(exceptionVar));
                    throw new BaristaScriptException(error);
                }

                mainModuleReady = true;
                return false;
            }

            IntPtr notifyCallbackPtr = Marshal.GetFunctionPointerForDelegate((JavaScriptNotifyModuleReadyCallback)mainModuleNotifyCallback);
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

            if (!mainModuleReady)
            {
                throw new InvalidOperationException("Main module is not ready. Ensure all script modules are loaded.");
            }

            //Now we're ready, evaluate the main module.

            //Clear and set the task queue if specified
            if (m_promiseTaskQueue != null)
            {
                m_promiseTaskQueue.Clear();
                Engine.JsSetPromiseContinuationCallback(PromiseContinuationCallback, IntPtr.Zero);
            }

            Engine.JsModuleEvaluation(mainModuleHandle);

            //Evaluate any pending promises.
            if (m_promiseTaskQueue != null)
            {
                var args = new IntPtr[] { Undefined.Handle.DangerousGetHandle() };
                while (m_promiseTaskQueue.Count > 0)
                {
                    var promise = m_promiseTaskQueue.Dequeue();
                    try
                    {
                        var promiseResult = Engine.JsCallFunction(promise.Handle, args, (ushort)args.Length);
                        promiseResult.Dispose();
                    }
                    finally
                    {
                        promise.Dispose();
                    }
                }
            }

            //Return the result.
            var result = Engine.GetGlobalVariable("$EXPORTS");

            //Unset the Promise callback.
            if (m_promiseTaskQueue != null)
            {
                Engine.JsSetPromiseContinuationCallback(null, IntPtr.Zero);
            }

            return result;
        }

        /// <summary>
        /// Returns a new JavaScript Execution Scope to perform work in.
        /// </summary>
        /// <returns></returns>
        public BaristaExecutionScope Scope()
        {
            //TODO: Interlock this.
            if (m_currentExecutionScope != null)
                return m_currentExecutionScope;

            Engine.JsSetCurrentContext(Handle);
            m_currentExecutionScope = new BaristaExecutionScope(ReleaseScope);
            return m_currentExecutionScope;
        }

        private JavaScriptFetchImportedModuleCallback GetFetchImportedModuleDelegate(string childModuleName, JavaScriptModuleRecord childModuleRecord)
        {
            return (IntPtr referencingModule, IntPtr specifier, out IntPtr dependentModule) =>
            {
                var specifierHandle = new JavaScriptValueSafeHandle(specifier);
                var moduleName = Engine.GetStringUtf8(specifierHandle);
                if (moduleName == childModuleName)
                {
                    dependentModule = childModuleRecord.DangerousGetHandle();
                }
                else if (m_moduleService != null)
                {
                    var module = m_moduleService.GetModule(moduleName);

                    if (module != null)
                    {
                        var referencingModuleRecord = new JavaScriptModuleRecord(referencingModule);

                        switch(module)
                        {
                            //For the built-in Script Module type, parse the string returned by InstallModule and install it as a module.
                            case BaristaScriptModule scriptModule:
                                var script = scriptModule.InstallModule(this, referencingModuleRecord) as string;
                                if (script == null)
                                    throw new InvalidOperationException("A Barista Script Module must provide a non-null script to use as the module.");
                                var moduleRecord = Engine.JsInitializeModuleRecord(referencingModuleRecord, specifierHandle);
                                var scriptBuffer = Encoding.UTF8.GetBytes(script);
                                Engine.JsParseModuleSource(moduleRecord, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)scriptBuffer.LongLength, JavaScriptParseModuleSourceFlags.DataIsUTF8);
                                dependentModule = moduleRecord.DangerousGetHandle();
                                return false;
                            //Otherwise, install the module.
                            default:
                                var result = InstallModule(module, referencingModuleRecord, specifierHandle, out JavaScriptModuleRecord dependentModuleRecord);
                                dependentModule = dependentModuleRecord.DangerousGetHandle();
                                return result;

                        }
                    }
                }

                dependentModule = referencingModule;
                return false;
            };
        }

        private bool InstallModule(IBaristaModule module, JavaScriptModuleRecord referencingModuleRecord, JavaScriptValueSafeHandle specifierHandle, out JavaScriptModuleRecord dependentModuleRecord)
        {
            try
            {
                var obj = module.InstallModule(this, referencingModuleRecord);

                if (obj == null)
                    throw new InvalidOperationException("The value returned from InstallModule should not be null.");

                //Based on the object's type, make some decisions on how to handle projecting the value to a module.
                switch (obj)
                {
                    case JsValue jsValue:
                        return CreateSingleValueModule(jsValue.Handle, referencingModuleRecord, specifierHandle, out dependentModuleRecord);
                    case JavaScriptValueSafeHandle valueSafeHandle:
                        return CreateSingleValueModule(valueSafeHandle, referencingModuleRecord, specifierHandle, out dependentModuleRecord);
                    default:
                        //TODO: Coerce the .Net object into a safe handle using a IMemberProjectionStrategy.
                        throw new NotImplementedException();
                }
            }
            catch(Exception ex)
            {
                throw new BaristaException($"An error occurred while installing module {module.Name}.", ex);
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
let value = global['$IMPORT_{globalId.ToString()}'];
export default value;
";
            var moduleRecord = Engine.JsInitializeModuleRecord(referencingModuleRecord, specifierHandle);

            Engine.SetGlobalVariable($"$IMPORT_{globalId.ToString()}", valueSafeHandle);
            var scriptBuffer = Encoding.UTF8.GetBytes(exposeNativeValueScript);
            Engine.JsParseModuleSource(moduleRecord, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)scriptBuffer.LongLength, JavaScriptParseModuleSourceFlags.DataIsUTF8);
            dependentModuleRecord = moduleRecord;
            return false;
        }

        private void PromiseContinuationCallback(IntPtr taskHandle, IntPtr callbackState)
        {
            if (IsDisposed || m_promiseTaskQueue == null)
            {
                return;
            }
            var task = new JavaScriptValueSafeHandle(taskHandle);
            var promise = m_valueFactory.CreateValue<JsFunction>(task);
            m_promiseTaskQueue.Enqueue(promise);
        }

        private void ReleaseScope()
        {
            Engine.JsSetCurrentContext(JavaScriptContextSafeHandle.Invalid);
            m_currentExecutionScope = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                if (m_valueFactory != null || m_promiseTaskQueue != null)
                {
                    BaristaExecutionScope scope = null;
                    if (!HasCurrentScope)
                        scope = Scope();
                    try
                    {
                        if (m_promiseTaskQueue != null)
                        {
                            Engine.JsSetPromiseContinuationCallback(null, IntPtr.Zero);
                        }

                        if (m_valueFactory != null)
                        {
                            m_valueFactory.Dispose();
                            m_valueFactory = null;
                        }
                    }
                    finally
                    {
                        if (scope != null)
                            scope.Dispose();
                    }
                }
            }

            base.Dispose(disposing);
        }
    }
}
