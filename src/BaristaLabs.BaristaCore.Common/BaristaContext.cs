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

        private readonly IBaristaModuleService m_moduleService;

        private IBaristaValueFactory m_valueFactory;
        private BaristaExecutionScope m_currentExecutionScope;

        /// <summary>
        /// Creates a new instance of a Barista Context.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="contextHandle"></param>
        public BaristaContext(IJavaScriptEngine engine, IBaristaValueFactory valueFactory, IBaristaModuleService moduleService, JavaScriptContextSafeHandle contextHandle)
            : base(engine, contextHandle)
        {
            m_valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));

            m_undefinedValue = new Lazy<JsUndefined>(() => m_valueFactory.GetUndefinedValue(this));
            m_nullValue = new Lazy<JsNull>(() => m_valueFactory.GetNullValue(this));
            m_trueValue = new Lazy<JsBoolean>(() => m_valueFactory.GetTrueValue(this));
            m_falseValue = new Lazy<JsBoolean>(() => m_valueFactory.GetFalseValue(this));

            m_moduleService = moduleService;
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
        /// Gets a value that indicates if a current execution scope exists.
        /// </summary>
        public bool HasCurrentScope
        {
            get { return m_currentExecutionScope != null; }
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
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(BaristaRuntime));

            var mainModuleName = "";
            var subModuleId = Guid.NewGuid();
            var subModuleName = subModuleId.ToString();
            var mainModuleScript = $@"
import child from '{subModuleName}';
let global = (new Function('return this;'))();
global.$EXPORTS = child;
";
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
            JavaScriptNotifyModuleReadyCallback mainModuleNotifyCallback = (IntPtr referencingModule, IntPtr exceptionVar) =>
            {
                if (exceptionVar != IntPtr.Zero)
                {
                    //TODO: Um. get the error and add to stack.
                }

                mainModuleReady = true;
                return false;
            };

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

            if (!mainModuleReady)
            {
                throw new InvalidOperationException("Main module is not ready. Ensure all script modules are loaded.");
            }

            //Now we're ready, evaluate the main module.
            Engine.JsModuleEvaluation(mainModuleHandle);

            //Return the result.
            var result = Engine.GetGlobalVariable("$EXPORTS");
            return m_valueFactory.CreateValue(this, result);
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
            return (IntPtr referencingModule, IntPtr specifier, out IntPtr dependentModuleRecord) =>
            {
                var specifierHandle = new JavaScriptValueSafeHandle(specifier);
                var moduleName = Engine.GetStringUtf8(specifierHandle);
                if (moduleName == childModuleName)
                {
                    dependentModuleRecord = childModuleRecord.DangerousGetHandle();
                }
                else if (m_moduleService != null)
                {
                    var module = m_moduleService.GetModule(moduleName);

                    if (module != null)
                    {
                        var referencingModuleRecord = new JavaScriptModuleRecord(referencingModule);

                        //For the built-in Script Module type, parse the script.
                        if (module is BaristaScriptModule scriptModule)
                        {
                            var moduleRecord = Engine.JsInitializeModuleRecord(referencingModuleRecord, specifierHandle);
                            var scriptBuffer = Encoding.UTF8.GetBytes((string)scriptModule.InstallModule(this, referencingModuleRecord));
                            Engine.JsParseModuleSource(moduleRecord, JavaScriptSourceContext.GetNextSourceContext(), scriptBuffer, (uint)scriptBuffer.LongLength, JavaScriptParseModuleSourceFlags.DataIsUTF8);
                            dependentModuleRecord = moduleRecord.DangerousGetHandle();
                            return false;
                        }
                        else
                        {
                            var obj = module.InstallModule(this, referencingModuleRecord);

                            if (obj is JavaScriptValueSafeHandle valueSafeHandle)
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
                                dependentModuleRecord = moduleRecord.DangerousGetHandle();
                                return false;
                            }
                            else
                            {
                                //TODO: Coerce the .Net object into a safe handle.
                                throw new NotImplementedException();
                            }
                        }
                    }
                }

                dependentModuleRecord = referencingModule;
                return false;
            };
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
                if (m_valueFactory != null)
                {
                    BaristaExecutionScope scope = null;
                    if (!HasCurrentScope)
                        scope = Scope();
                    try
                    {
                        m_valueFactory.Dispose();
                        m_valueFactory = null;
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
