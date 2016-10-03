namespace BaristaLabs.BaristaCore.JavaScript
{
    using SafeHandles;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;

    public delegate void JavaScriptExternalObjectFinalizeCallback(object additionalData);
    public delegate JavaScriptValue JavaScriptCallableFunction(JavaScriptEngine callingEngine, bool asConstructor, JavaScriptValue thisValue, IEnumerable<JavaScriptValue> arguments);

    public sealed class JavaScriptEngine : IDisposable
    {
        private class NativeFunctionThunkData
        {
            public JavaScriptCallableFunction callback;
            public WeakReference<JavaScriptEngine> engine;
        }

        private class ExternalObjectThunkData
        {
            public WeakReference<JavaScriptEngine> engine;
            public WeakReference<object> userData;
            public JavaScriptExternalObjectFinalizeCallback callback;
        }

        private JavaScriptEngineSafeHandle m_handle;
        private WeakReference<JavaScriptRuntime> m_runtime;
        private JavaScriptConverter m_converter;
        private List<NativeFunctionThunkData> m_nativeFunctionThunks;
        private static NativeFunctionThunkCallback NativeCallback;
        private static IntPtr NativeCallbackPtr;
        private static JsFinalizeCallback FinalizerCallback;
        private static IntPtr FinalizerCallbackPtr;
        private HashSet<ExternalObjectThunkData> m_externalObjects;
        private ChakraApi m_api;

        private List<IntPtr> m_handlesToRelease;
        private object m_handleReleaseLock;

        private JavaScriptValue m_undefined, m_true, m_false;
        private JavaScriptObject m_global, m_null;

        static JavaScriptEngine()
        {
            NativeCallback = NativeCallbackThunk;
            NativeCallbackPtr = Marshal.GetFunctionPointerForDelegate(NativeCallback);

            FinalizerCallback = FinalizerCallbackThunk;
            FinalizerCallbackPtr = Marshal.GetFunctionPointerForDelegate(FinalizerCallback);
        }

        internal JavaScriptEngine(JavaScriptEngineSafeHandle handle, JavaScriptRuntime runtime, ChakraApi api)
        {
            Debug.Assert(handle != null);
            Debug.Assert(runtime != null);
            Debug.Assert(api != null);
            m_api = api;

            m_handle = handle;
            m_runtime = new WeakReference<JavaScriptRuntime>(runtime);
            m_converter = new JavaScriptConverter(this);
            m_nativeFunctionThunks = new List<NativeFunctionThunkData>();
            m_externalObjects = new HashSet<ExternalObjectThunkData>();

            m_handlesToRelease = new List<IntPtr>();
            m_handleReleaseLock = new object();

            ClaimContextPrivate();
            JavaScriptValueSafeHandle global;
            Errors.ThrowIfIs(m_api.JsGetGlobalObject(out global));
            m_global = CreateObjectFromHandle(global);
            JavaScriptValueSafeHandle undef;
            Errors.ThrowIfIs(m_api.JsGetUndefinedValue(out undef));
            m_undefined = CreateValueFromHandle(undef);
            JavaScriptValueSafeHandle @null;
            Errors.ThrowIfIs(m_api.JsGetNullValue(out @null));
            m_null = CreateObjectFromHandle(@null);
            JavaScriptValueSafeHandle @true;
            Errors.ThrowIfIs(m_api.JsGetTrueValue(out @true));
            m_true = CreateValueFromHandle(@true);
            JavaScriptValueSafeHandle @false;
            Errors.ThrowIfIs(m_api.JsGetFalseValue(out @false));
            m_false = CreateValueFromHandle(@false);
            ReleaseContextPrivate();
        }

        public JavaScriptRuntime Runtime
        {
            get
            {
                JavaScriptRuntime result;
                if (!m_runtime.TryGetTarget(out result))
                    throw new ObjectDisposedException(nameof(JavaScriptEngine));

                return result;
            }
        }

        public JavaScriptConverter Converter
        {
            get
            {
                return m_converter;
            }
        }

        #region Internal methods
        internal ChakraApi Api
        {
            get
            {
                return m_api;
            }
        }

        internal void EnqueueRelease(IntPtr handle)
        {
            lock (m_handleReleaseLock)
            {
                m_handlesToRelease.Add(handle);
            }
        }

        public JavaScriptExecutionContext AcquireContext()
        {
            ClaimContextPrivate();
            return new JavaScriptExecutionContext(this, ReleaseContextPrivate);
        }

        private void ClaimContextPrivate()
        {
            if (m_handle == null)
                throw new ObjectDisposedException(nameof(JavaScriptEngine));

            Errors.ThrowIfIs(m_api.JsSetCurrentContext(m_handle));

            if (m_handlesToRelease.Count > 0)
            {
                lock (m_handleReleaseLock)
                {
                    foreach (IntPtr handle in m_handlesToRelease)
                    {
                        uint count;
                        var error = m_api.JsRelease(handle, out count);
                        Debug.Assert(error == JsErrorCode.JsNoError);
                    }

                    m_handlesToRelease.Clear();
                }
            }
        }

        private void ReleaseContextPrivate()
        {
            Errors.ThrowIfIs(m_api.JsReleaseCurrentContext());
        }

        internal JavaScriptValue CreateValueFromHandle(JavaScriptValueSafeHandle handle)
        {
            Debug.Assert(!(handle.IsClosed || handle.IsInvalid));

            JsValueType kind;
            Errors.ThrowIfIs(m_api.JsGetValueType(handle, out kind));

            JavaScriptValue result = null;
            switch (kind)
            {
                case JsValueType.JsArray:
                    result = new JavaScriptArray(handle, JavaScriptValueType.Array, this);
                    break;

                case JsValueType.JsFunction:
                    result = new JavaScriptFunction(handle, JavaScriptValueType.Function, this);
                    break;

                case JsValueType.JsObject:
                case JsValueType.JsNull:
                case JsValueType.JsError:
                    result = new JavaScriptObject(handle, JavaScriptValueType.Object, this);
                    break;

                case JsValueType.JsSymbol:
                    result = new JavaScriptSymbol(handle, JavaScriptValueType.Symbol, this);
                    break;

                case JsValueType.JsArrayBuffer:
                    result = new JavaScriptArrayBuffer(handle, JavaScriptValueType.ArrayBuffer, this);
                    break;

                case JsValueType.JsTypedArray:
                    result = new JavaScriptTypedArray(handle, JavaScriptValueType.TypedArray, this);
                    break;

                case JsValueType.JsDataView:
                    result = new JavaScriptDataView(handle, JavaScriptValueType.DataView, this);
                    break;

                case JsValueType.JsBoolean:
                case JsValueType.JsNumber:
                case JsValueType.JsString:
                case JsValueType.JsUndefined:
                default:
                    result = new JavaScriptValue(handle, kind.ToApiValueType(), this);
                    break;
            }

            return result;
        }

        internal JavaScriptObject CreateObjectFromHandle(JavaScriptValueSafeHandle handle)
        {
            JsValueType kind;
            Errors.ThrowIfIs(m_api.JsGetValueType(handle, out kind));

            JavaScriptObject result = null;
            switch (kind)
            {
                case JsValueType.JsArray:
                    result = new JavaScriptArray(handle, JavaScriptValueType.Array, this);
                    break;

                case JsValueType.JsFunction:
                    result = new JavaScriptFunction(handle, JavaScriptValueType.Function, this);
                    break;

                case JsValueType.JsObject:
                case JsValueType.JsError:
                case JsValueType.JsNull:
                    result = new JavaScriptObject(handle, JavaScriptValueType.Object, this);
                    break;

                case JsValueType.JsArrayBuffer:
                    result = new JavaScriptArrayBuffer(handle, JavaScriptValueType.ArrayBuffer, this);
                    break;

                case JsValueType.JsTypedArray:
                    result = new JavaScriptTypedArray(handle, JavaScriptValueType.TypedArray, this);
                    break;

                case JsValueType.JsDataView:
                    result = new JavaScriptDataView(handle, JavaScriptValueType.DataView, this);
                    break;

                case JsValueType.JsBoolean:
                case JsValueType.JsNumber:
                case JsValueType.JsString:
                case JsValueType.JsUndefined:
                case JsValueType.JsSymbol:
                default:
                    throw new ArgumentException();
            }

            return result;
        }

        internal JavaScriptArray CreateArrayFromHandle(JavaScriptValueSafeHandle handle)
        {
            JsValueType kind;
            Errors.ThrowIfIs(m_api.JsGetValueType(handle, out kind));

            switch (kind)
            {
                case JsValueType.JsArray:
                    var result = new JavaScriptArray(handle, JavaScriptValueType.Array, this);
                    return result;

                case JsValueType.JsFunction:
                case JsValueType.JsObject:
                case JsValueType.JsError:
                case JsValueType.JsNull:
                case JsValueType.JsArrayBuffer:
                case JsValueType.JsTypedArray:
                case JsValueType.JsDataView:
                case JsValueType.JsBoolean:
                case JsValueType.JsNumber:
                case JsValueType.JsString:
                case JsValueType.JsUndefined:
                case JsValueType.JsSymbol:
                default:
                    throw new ArgumentException();
            }
        }
        #endregion

        #region Base properties
        public JavaScriptObject GlobalObject
        {
            get
            {
                return m_global;
            }
        }

        public JavaScriptValue UndefinedValue
        {
            get
            {
                return m_undefined;
            }
        }

        public JavaScriptObject NullValue
        {
            get
            {
                return m_null;
            }
        }

        public JavaScriptValue TrueValue
        {
            get
            {
                return m_true;
            }
        }

        public JavaScriptValue FalseValue
        {
            get
            {
                return m_false;
            }
        }

        public bool HasException
        {
            get
            {
                bool has;
                Errors.ThrowIfIs(m_api.JsHasException(out has));

                return has;
            }
        }

        public event EventHandler RuntimeExceptionRaised;
        internal void OnRuntimeExceptionRaised()
        {
            var rer = RuntimeExceptionRaised;
            if (rer != null)
                rer(this, EventArgs.Empty);
        }
        #endregion

        #region Code execution
        public JavaScriptFunction EvaluateScriptText(string code)
        {
            return Evaluate(new ScriptSource("[eval code]", code));
        }

        public JavaScriptFunction Evaluate(ScriptSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            JavaScriptValueSafeHandle handle;
            Errors.ThrowIfIs(m_api.JsParseScript(source.SourceText, source.SourceContextId, source.SourceLocation, out handle));

            return CreateObjectFromHandle(handle) as JavaScriptFunction;
        }

        public JavaScriptFunction Evaluate(ScriptSource source, Stream compiledCode)
        {
            throw new NotSupportedException();
        }

        public unsafe void Compile(ScriptSource source, Stream compiledCodeDestination)
        {
            uint bufferSize = 0;
            Errors.ThrowIfIs(m_api.JsSerializeScript(source.SourceText, IntPtr.Zero, ref bufferSize));
            if (bufferSize > int.MaxValue)
                throw new OutOfMemoryException();

            IntPtr mem = Marshal.AllocCoTaskMem(unchecked((int)bufferSize));
            var error = m_api.JsSerializeScript(source.SourceText, mem, ref bufferSize);
            if (error != JsErrorCode.JsNoError)
            {
                Marshal.FreeCoTaskMem(mem);
                Errors.ThrowFor(error);
            }

            using (UnmanagedMemoryStream ums = new UnmanagedMemoryStream((byte*)mem.ToPointer(), bufferSize))
            {
                ums.CopyTo(compiledCodeDestination);
            }
            Marshal.FreeCoTaskMem(mem);
        }

        public JavaScriptValue Execute(ScriptSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            JavaScriptValueSafeHandle handle;
            Errors.CheckForScriptExceptionOrThrow(m_api.JsRunScript(source.SourceText, source.SourceContextId, source.SourceLocation, out handle), this);
            if (handle.IsInvalid)
                return m_undefined;

            return CreateValueFromHandle(handle);
        }

        public JavaScriptValue Execute(ScriptSource source, Stream compiledCode)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region Main interaction functions
        public JavaScriptObject CreateObject(JavaScriptObject prototype = null)
        {
            JavaScriptValueSafeHandle handle;
            Errors.ThrowIfIs(m_api.JsCreateObject(out handle));

            if (prototype != null)
            {
                Errors.ThrowIfIs(m_api.JsSetPrototype(handle, prototype.m_handle));
            }

            return CreateObjectFromHandle(handle);
        }

        private static void FinalizerCallbackThunk(IntPtr externalData)
        {
            if (externalData == IntPtr.Zero)
                return;

            GCHandle handle = GCHandle.FromIntPtr(externalData);
            var thunkData = handle.Target as ExternalObjectThunkData;
            if (thunkData == null)
                return;

            var engine = thunkData.engine;
            var callback = thunkData.callback;
            object userData;
            thunkData.userData.TryGetTarget(out userData);

            if (callback != null)
                callback(userData);

            JavaScriptEngine eng;
            if (engine.TryGetTarget(out eng))
            {
                eng.m_externalObjects.Remove(thunkData);
            }
        }

        public JavaScriptObject CreateExternalObject(object externalData, JavaScriptExternalObjectFinalizeCallback finalizeCallback)
        {
            ExternalObjectThunkData thunk = new ExternalObjectThunkData() { callback = finalizeCallback, engine = new WeakReference<JavaScriptEngine>(this), userData = new WeakReference<object>(externalData), };
            GCHandle handle = GCHandle.Alloc(thunk);
            m_externalObjects.Add(thunk);

            JavaScriptValueSafeHandle result;
            Errors.ThrowIfIs(m_api.JsCreateExternalObject(GCHandle.ToIntPtr(handle), FinalizerCallbackPtr, out result));

            return CreateObjectFromHandle(result);
        }

        internal object GetExternalObjectFrom(JavaScriptValue value)
        {
            Debug.Assert(value != null);

            IntPtr handlePtr;
            Errors.ThrowIfIs(m_api.JsGetExternalData(value.m_handle, out handlePtr));
            GCHandle gcHandle = GCHandle.FromIntPtr(handlePtr);
            ExternalObjectThunkData thunk = gcHandle.Target as ExternalObjectThunkData;
            if (thunk == null)
                return null;

            object result;
            thunk.userData.TryGetTarget(out result);
            return result;
        }

        public JavaScriptSymbol CreateSymbol(string description)
        {
            JavaScriptValueSafeHandle handle;
            using (var str = m_converter.FromString(description))
            {
                Errors.ThrowIfIs(m_api.JsCreateSymbol(str.m_handle, out handle));
            }

            return CreateValueFromHandle(handle) as JavaScriptSymbol;
        }

        public TimeSpan RunIdleWork()
        {
            uint nextTick;
            Errors.ThrowIfIs(m_api.JsIdle(out nextTick));

            return TimeSpan.FromTicks(nextTick);
        }

        public bool HasGlobalVariable(string name)
        {
            return GlobalObject.HasOwnProperty(name);
        }

        public JavaScriptValue GetGlobalVariable(string name)
        {
            return GlobalObject.GetPropertyByName(name);
        }

        public void SetGlobalVariable(string name, JavaScriptValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            GlobalObject.SetPropertyByName(name, value);
        }

        public JavaScriptValue CallGlobalFunction(string functionName)
        {
            return CallGlobalFunction(functionName, Enumerable.Empty<JavaScriptValue>());
        }

        public JavaScriptValue CallGlobalFunction(string functionName, IEnumerable<JavaScriptValue> args)
        {
            var global = GlobalObject;
            var fn = global.GetPropertyByName(functionName) as JavaScriptFunction;
            return fn.Call(global, args);
        }

        public void SetGlobalFunction(string functionName, JavaScriptCallableFunction hostFunction)
        {
            if (hostFunction == null)
                throw new ArgumentNullException(nameof(hostFunction));

            GlobalObject.SetPropertyByName(functionName, CreateFunction(hostFunction, functionName));
        }

        public JavaScriptArray CreateArray(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            JavaScriptValueSafeHandle handle;
            Errors.ThrowIfIs(m_api.JsCreateArray(unchecked((uint)length), out handle));

            return CreateArrayFromHandle(handle);
        }

        private static IntPtr NativeCallbackThunk(
            IntPtr callee,
            [MarshalAs(UnmanagedType.U1)] bool asConstructor,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] IntPtr[] args,
            ushort argCount,
            IntPtr data)
        {
            // callee and args[0] are the same
            if (data == IntPtr.Zero)
                return IntPtr.Zero;

            try
            {
                GCHandle handle = GCHandle.FromIntPtr(data);
                var nativeThunk = handle.Target as NativeFunctionThunkData;
                JavaScriptEngine engine;
                if (!nativeThunk.engine.TryGetTarget(out engine))
                    return IntPtr.Zero;

                JavaScriptValue thisValue = null;
                if (argCount > 0)
                {
                    thisValue = engine.CreateValueFromHandle(new JavaScriptValueSafeHandle(args[0]));
                }

                try
                {
                    var result = nativeThunk.callback(engine, asConstructor, thisValue, args.Skip(1).Select(h => engine.CreateValueFromHandle(new JavaScriptValueSafeHandle(h))));
                    return result.m_handle.DangerousGetHandle();
                }
                catch (Exception ex)
                {
                    var error = engine.CreateError(ex.Message);
                    engine.SetException(error);

                    return engine.UndefinedValue.m_handle.DangerousGetHandle();
                }
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        public JavaScriptFunction CreateFunction(JavaScriptCallableFunction hostFunction)
        {
            if (hostFunction == null)
                throw new ArgumentNullException(nameof(hostFunction));

            NativeFunctionThunkData td = new NativeFunctionThunkData() { callback = hostFunction, engine = new WeakReference<JavaScriptEngine>(this) };
            GCHandle handle = GCHandle.Alloc(td, GCHandleType.Weak);
            m_nativeFunctionThunks.Add(td);

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(m_api.JsCreateFunction(NativeCallbackPtr, GCHandle.ToIntPtr(handle), out resultHandle));

            return CreateObjectFromHandle(resultHandle) as JavaScriptFunction;
        }

        public JavaScriptFunction CreateFunction(JavaScriptCallableFunction hostFunction, string name)
        {
            if (hostFunction == null)
                throw new ArgumentNullException(nameof(hostFunction));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var nameVal = Converter.FromString(name);

            NativeFunctionThunkData td = new NativeFunctionThunkData() { callback = hostFunction, engine = new WeakReference<JavaScriptEngine>(this) };
            GCHandle handle = GCHandle.Alloc(td, GCHandleType.Weak);
            m_nativeFunctionThunks.Add(td);

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(m_api.JsCreateNamedFunction(nameVal.m_handle, NativeCallbackPtr, GCHandle.ToIntPtr(handle), out resultHandle));

            return CreateObjectFromHandle(resultHandle) as JavaScriptFunction;
        }

        public JavaScriptValue GetAndClearException()
        {
            JavaScriptValueSafeHandle handle;
            Errors.ThrowIfIs(m_api.JsGetAndClearException(out handle));

            return CreateValueFromHandle(handle);
        }

        public void SetException(JavaScriptValue exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            Errors.ThrowIfIs(m_api.JsSetException(exception.m_handle));
        }
        #endregion

        #region Errors
        public JavaScriptObject CreateError(string message = "")
        {
            var str = Converter.FromString(message ?? "");
            JavaScriptValueSafeHandle handle;
            Errors.ThrowIfIs(m_api.JsCreateError(str.m_handle, out handle));

            return CreateObjectFromHandle(handle);
        }

        public JavaScriptObject CreateRangeError(string message = "")
        {
            var str = Converter.FromString(message ?? "");
            JavaScriptValueSafeHandle handle;
            Errors.ThrowIfIs(m_api.JsCreateRangeError(str.m_handle, out handle));

            return CreateObjectFromHandle(handle);
        }

        public JavaScriptObject CreateReferenceError(string message = "")
        {
            var str = Converter.FromString(message ?? "");
            JavaScriptValueSafeHandle handle;
            Errors.ThrowIfIs(m_api.JsCreateReferenceError(str.m_handle, out handle));

            return CreateObjectFromHandle(handle);
        }

        public JavaScriptObject CreateSyntaxError(string message = "")
        {
            var str = Converter.FromString(message ?? "");
            JavaScriptValueSafeHandle handle;
            Errors.ThrowIfIs(m_api.JsCreateSyntaxError(str.m_handle, out handle));

            return CreateObjectFromHandle(handle);
        }

        public JavaScriptObject CreateTypeError(string message = "")
        {
            var str = Converter.FromString(message ?? "");
            JavaScriptValueSafeHandle handle;
            Errors.ThrowIfIs(m_api.JsCreateTypeError(str.m_handle, out handle));

            return CreateObjectFromHandle(handle);
        }

        public JavaScriptObject CreateUriError(string message = "")
        {
            var str = Converter.FromString(message ?? "");
            JavaScriptValueSafeHandle handle;
            Errors.ThrowIfIs(m_api.JsCreateURIError(str.m_handle, out handle));

            return CreateObjectFromHandle(handle);
        }
        #endregion

        public bool CanEnableDebugging
        {
            get
            {
                return m_api.JsStartDebugging != null;
            }
        }

        public void EnableDebugging()
        {
            if (m_api.JsStartDebugging == null)
                throw new NotSupportedException("Debugging is not supported with ChakraCore.  Check the CanEnableDebugging property before attempting to enable debugging.");

            Errors.ThrowIfIs(m_api.JsStartDebugging());
        }

        public void AddTypeToGlobal<T>(string name = null)
        {
            Type t = typeof(T);
            if (null == name)
            {
                name = t.Name;
            }

            var proj = Converter.GetProjectionPrototypeForType(t);
            SetGlobalVariable(name, proj.GetPropertyByName("constructor"));
        }

        #region IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_handle != null)
                {
                    m_handle.Dispose();
                    m_handle = null;
                }
            }
        }

        ~JavaScriptEngine()
        {
            Dispose(false);
        }
        #endregion
    }
}
