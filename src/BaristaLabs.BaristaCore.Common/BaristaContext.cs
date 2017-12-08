namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.JavaScript.Extensions;
    using BaristaLabs.BaristaCore.JavaScript.Internal;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    ///     Represents a JavaScript Context
    /// </summary>
    /// <remarks>
    ///     Each script context has its own global object that is isolated from all other script contexts.
    /// </remarks>
    public sealed class BaristaContext : JavaScriptReferenceFlyweight<JavaScriptContextSafeHandle>
    {
        private const string ParseScriptSourceUrl = "[eval code]";

        private readonly Lazy<JavaScriptUndefined> m_undefinedValue;
        private readonly Lazy<JavaScriptNull> m_nullValue;
        private readonly Lazy<JavaScriptBoolean> m_trueValue;
        private readonly Lazy<JavaScriptBoolean> m_falseValue;

        private JavaScriptValuePool m_valuePool;
        private BaristaExecutionScope m_currentExecutionScope;

        #region Properties
        /// <summary>
        /// Gets the False Value associated with the context.
        /// </summary>
        public JavaScriptBoolean False
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
        public JavaScriptValue Null
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
        public JavaScriptBoolean True
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
        public JavaScriptValue Undefined
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_undefinedValue.Value;
            }
        }
        #endregion

        internal IBaristaModuleService ModuleService
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the pool of jsvalue flyweight objects associated with the context.
        /// </summary>
        internal JavaScriptValuePool ValuePool
        {
            get { return m_valuePool; }
        }

        internal BaristaContext(IJavaScriptEngine engine, JavaScriptContextSafeHandle contextHandle)
            : base(engine, contextHandle)
        {
            m_undefinedValue = new Lazy<JavaScriptUndefined>(GetUndefinedValue);
            m_nullValue = new Lazy<JavaScriptNull>(GetNullValue);
            m_trueValue = new Lazy<JavaScriptBoolean>(GetTrueValue);
            m_falseValue = new Lazy<JavaScriptBoolean>(GetFalseValue);

            m_valuePool = new JavaScriptValuePool(engine, this);
        }

        public JavaScriptString CreateString(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            var stringHandle = Engine.JsCreateString(str, (ulong)str.Length);
            var flyweight = new JavaScriptString(Engine, this, stringHandle);
            if (m_valuePool.TryAdd(flyweight))
                return flyweight;

            flyweight.Dispose();
            throw new InvalidOperationException("Could not create string. The string already exists at that location in memory.");
        }

        public JavaScriptExternalArrayBuffer CreateExternalArrayBufferFromString(string data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            JavaScriptValueSafeHandle externalArrayHandle;
            IntPtr ptrData = Marshal.StringToHGlobalAnsi(data);
            try
            {
                externalArrayHandle = Engine.JsCreateExternalArrayBuffer(ptrData, (uint)data.Length, null, IntPtr.Zero);
            }
            catch(Exception)
            {
                //If anything goes wrong, free the unmanaged memory.
                //This is not a finally as if success, the memory will be freed automagially.
                Marshal.ZeroFreeGlobalAllocAnsi(ptrData);
                throw;
            }

            var flyweight = new JavaScriptManagedExternalArrayBuffer(Engine, this, externalArrayHandle, ptrData, (ptr) =>
            {
                Marshal.ZeroFreeGlobalAllocAnsi(ptr);
            });
            if (m_valuePool.TryAdd(flyweight))
                return flyweight;

            //This would be... unexpected.
            flyweight.Dispose();
            throw new InvalidOperationException("Could not create external array buffer. The external array buffer already exists at that location in memory.");
        }

        /// <summary>
        /// Evaluates the specified script as a module, the default export will be the returned value.
        /// </summary>
        /// <param name="script">Script to evaluate.</param>
        /// <param name="onModuleRequested">A callback that can be used to perform custom behavior when a module is requested.</param>
        /// <returns></returns>
        public JavaScriptValue EvaluateModule(string script)
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
            return ValuePool.GetOrAdd(result);
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
                else if (ModuleService != null)
                {
                    var module = ModuleService.GetModule(moduleName);

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

        private JavaScriptBoolean GetFalseValue()
        {
            var falseValue = Engine.JsGetFalseValue();
            var result = new JavaScriptBoolean(Engine, this, falseValue);
            if (m_valuePool.TryAdd(result))
                return result;

            result.Dispose();
            throw new InvalidOperationException("Could not add JsFalse to the Value Pool associated with the context.");
        }

        private JavaScriptNull GetNullValue()
        {
            var nullValue = Engine.JsGetNullValue();
            var result = new JavaScriptNull(Engine, this, nullValue);
            if (m_valuePool.TryAdd(result))
                return result;

            result.Dispose();
            throw new InvalidOperationException("Could not add JsNull to the Value Pool associated with the context.");
        }

        private JavaScriptBoolean GetTrueValue()
        {
            var trueValue = Engine.JsGetTrueValue();
            var result = new JavaScriptBoolean(Engine, this, trueValue);
            if (m_valuePool.TryAdd(result))
                return result;

            result.Dispose();
            throw new InvalidOperationException("Could not add JsTrue to the Value Pool associated with the context.");
        }

        private JavaScriptUndefined GetUndefinedValue()
        {
            var undefinedValue = Engine.JsGetUndefinedValue();
            var result = new JavaScriptUndefined(Engine, this, undefinedValue);
            if (m_valuePool.TryAdd(result))
                return result;

            result.Dispose();
            throw new InvalidOperationException("Could not add JsUndefined to the Value Pool associated with the context.");
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
                if (m_valuePool != null)
                {
                    BaristaExecutionScope scope = null;
                    if (!HasCurrentScope)
                        scope = Scope();
                    try
                    {
                        m_valuePool.Dispose();
                        m_valuePool = null;
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
