namespace BaristaLabs.BaristaCore.JavaScript
{
    using Internal;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Represents a JavaScript Context
    /// </summary>
    /// <remarks>
    ///     Each script context has its own global object that is isolated from all other script contexts.
    /// </remarks>
    public sealed class JavaScriptContext : JavaScriptReferenceFlyweight<JavaScriptContextSafeHandle>
    {
        private const string ParseScriptSourceUrl = "[eval code]";

        private readonly Lazy<JavaScriptUndefined> m_undefinedValue;
        private readonly Lazy<JavaScriptNull> m_nullValue;
        private readonly Lazy<JavaScriptBoolean> m_trueValue;
        private readonly Lazy<JavaScriptBoolean> m_falseValue;

        private JavaScriptValuePool m_valuePool;
        private JavaScriptExecutionScope m_currentExecutionScope;

        #region Properties
        /// <summary>
        /// Gets the False Value associated with the context.
        /// </summary>
        public JavaScriptBoolean False
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(JavaScriptContext));

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
                    throw new ObjectDisposedException(nameof(JavaScriptContext));

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
                    throw new ObjectDisposedException(nameof(JavaScriptContext));

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
                    throw new ObjectDisposedException(nameof(JavaScriptContext));

                return m_undefinedValue.Value;
            }
        }
        #endregion

        /// <summary>
        /// Gets the pool of jsvalue flyweight objects associated with the context.
        /// </summary>
        internal JavaScriptValuePool ValuePool
        {
            get { return m_valuePool; }
        }

        internal JavaScriptContext(IJavaScriptEngine engine, JavaScriptContextSafeHandle contextHandle)
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
        /// Returns a JavaScript function that evaluates the given script.
        /// </summary>
        /// <param name="scriptText"></param>
        /// <returns></returns>
        public JavaScriptFunction ParseScriptText(string scriptText)
        {
            var externalArrayBuffer = CreateExternalArrayBufferFromString(scriptText);
            var sourceUrl = CreateString(ParseScriptSourceUrl);

            var resultHandle = Engine.JsParse(externalArrayBuffer.Handle, JavaScriptSourceContext.GetNextSourceContext(), sourceUrl.Handle, JavaScriptParseScriptAttributes.None);
            return (JavaScriptFunction)m_valuePool.GetOrAdd(resultHandle);
        }

        /// <summary>
        /// Returns a new JavaScript Execution Scope to perform work in.
        /// </summary>
        /// <returns></returns>
        public JavaScriptExecutionScope Scope()
        {
            //TODO: Interlock this.
            if (m_currentExecutionScope != null)
                return m_currentExecutionScope;

            Engine.JsSetCurrentContext(Handle);
            m_currentExecutionScope = new JavaScriptExecutionScope(ReleaseScope);
            return m_currentExecutionScope;
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
                    JavaScriptExecutionScope scope = null;
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
