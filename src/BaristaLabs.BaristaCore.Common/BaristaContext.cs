namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.Tasks;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Represents a JavaScript Context
    /// </summary>
    /// <remarks>
    ///     Each script context has its own global object that is isolated from all other script contexts.
    /// </remarks>
    public sealed partial class BaristaContext : BaristaObject<JavaScriptContextSafeHandle>, IBaristaValueFactory
    {
        private readonly Lazy<JsUndefined> m_undefinedValue;
        private readonly Lazy<JsNull> m_nullValue;
        private readonly Lazy<JsBoolean> m_trueValue;
        private readonly Lazy<JsBoolean> m_falseValue;
        private readonly Lazy<JsJSON> m_jsonValue;
        private readonly Lazy<JsObject> m_globalValue;
        private readonly Lazy<JsObjectConstructor> m_objectValue;
        private readonly Lazy<JsPromiseConstructor> m_promiseValue;
        private readonly Lazy<JsSymbolConstructor> m_symbolValue;

        private readonly IBaristaValueFactory m_valueFactory;
        private readonly IBaristaConversionStrategy m_conversionStrategy;
        private readonly IPromiseTaskQueue m_promiseTaskQueue;
        private readonly IBaristaModuleRecordFactory m_moduleRecordFactory;

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
        public BaristaContext(IJavaScriptEngine engine, IBaristaValueFactoryBuilder valueFactoryBuilder, IBaristaConversionStrategy conversionStrategy, IBaristaModuleRecordFactory moduleRecordFactory, IPromiseTaskQueue taskQueue, JavaScriptContextSafeHandle contextHandle)
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

            m_undefinedValue = new Lazy<JsUndefined>(() => m_valueFactory.Undefined);
            m_nullValue = new Lazy<JsNull>(() => m_valueFactory.Null);
            m_trueValue = new Lazy<JsBoolean>(() => m_valueFactory.True);
            m_falseValue = new Lazy<JsBoolean>(() => m_valueFactory.False);
            m_globalValue = new Lazy<JsObject>(() => m_valueFactory.GlobalObject);
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
            m_promiseValue = new Lazy<JsPromiseConstructor>(() =>
            {
                var global = m_globalValue.Value;
                return global.GetProperty<JsPromiseConstructor>("Promise");
            });
            m_symbolValue = new Lazy<JsSymbolConstructor>(() =>
            {
                var global = m_globalValue.Value;
                return global.GetProperty<JsSymbolConstructor>("Symbol");
            });

            m_promiseTaskQueue = taskQueue;
            m_moduleRecordFactory = moduleRecordFactory ?? throw new ArgumentNullException(nameof(moduleRecordFactory));

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
        public JsPromiseConstructor Promise
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_promiseValue.Value;
            }
        }

        /// <summary>
        /// Gets the global Symbol built-in.
        /// </summary>
        public JsSymbolConstructor Symbol
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(BaristaContext));

                return m_symbolValue.Value;
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
        private IBaristaValueFactory ValueFactory
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

        public JsArray CreateArray(uint length)
        {
            return ValueFactory.CreateArray(length);
        }

        public JsArrayBuffer CreateArrayBuffer(string data)
        {
            return ValueFactory.CreateArrayBuffer(data);
        }

        public JsArrayBuffer CreateArrayBuffer(byte[] data)
        {
            return ValueFactory.CreateArrayBuffer(data);
        }

        public JsError CreateError(string message)
        {
            return ValueFactory.CreateError(message);
        }

        public JsExternalObject CreateExternalObject(object obj)
        {
            return ValueFactory.CreateExternalObject(obj);
        }

        public JsFunction CreateFunction(Delegate func, string name = null)
        {
            return ValueFactory.CreateFunction(func, name);
        }

        public JsIterator CreateIterator(IEnumerator enumerator)
        {
            return ValueFactory.CreateIterator(enumerator);
        }

        public JsNumber CreateNumber(double number)
        {
            return ValueFactory.CreateNumber(number);
        }

        public JsNumber CreateNumber(int number)
        {
            return ValueFactory.CreateNumber(number);
        }

        public JsObject CreateObject()
        {
            return ValueFactory.CreateObject();
        }

        public JsObject CreatePromise(out JsFunction resolve, out JsFunction reject)
        {
            return ValueFactory.CreatePromise(out resolve, out reject);
        }

        public JsObject CreatePromise(Task task)
        {
            return ValueFactory.CreatePromise(task);
        }

        public JsString CreateString(string str)
        {
            return ValueFactory.CreateString(str);
        }

        public JsSymbol CreateSymbol(string description)
        {
            return ValueFactory.CreateSymbol(description);
        }

        public JsError CreateTypeError(string message)
        {
            return ValueFactory.CreateTypeError(message);
        }

        public JsValue CreateValue(JavaScriptValueSafeHandle valueHandle, JsValueType? valueType = null)
        {
            return ValueFactory.CreateValue(valueHandle, valueType);
        }

        public JsValue CreateValue(Type targetType, JavaScriptValueSafeHandle valueHandle)
        {
            return ValueFactory.CreateValue(targetType, valueHandle);
        }

        public T CreateValue<T>(JavaScriptValueSafeHandle valueHandle) where T : JsValue
        {
            return ValueFactory.CreateValue<T>(valueHandle);
        }

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

            var mainModule = m_moduleRecordFactory.CreateBaristaModuleRecord(this, "", null, true);
            var subModule = m_moduleRecordFactory.CreateBaristaModuleRecord(this, subModuleName, mainModule);

            //Now start the parsing.
            try
            {
                //First, parse our main module script.
                mainModule.ParseModuleSource(mainModuleScript);

                //Now Parse the user-provided script.
                subModule.ParseModuleSource(script);
                
                //Now we're ready, evaluate the main module.
                Engine.JsModuleEvaluation(mainModule.Handle);
                
                //Evaluate any pending promises.
                CurrentScope.ResolvePendingPromises();

                if (m_promiseTaskQueue != null && GlobalObject.HasOwnProperty("$ERROR"))
                {
                    var errorValue = GlobalObject.GetProperty("$ERROR");
                    throw new JsScriptException(JsErrorCode.ScriptException, errorValue.Handle);
                }

                //Return the name of the global.
                return "$EXPORTS";
            }
            finally
            {
                mainModule.Dispose();
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
                throw new InvalidOperationException("This context already has an active execution scope.");
            }
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
                    m_moduleRecordFactory.Dispose();
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
                m_beforeCollectCallbackDelegateHandle.Free();
            }

            base.Dispose(disposing);
        }
    }
}
