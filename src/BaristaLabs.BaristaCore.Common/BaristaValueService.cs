namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public sealed class BaristaValueService : IBaristaValueService
    {
        private BaristaObjectPool<JsValue, JavaScriptValueSafeHandle> m_valuePool;

        private readonly IJavaScriptEngine m_engine;
        private readonly BaristaContext m_context;

        public BaristaValueService(IJavaScriptEngine engine, BaristaContext context)
        {
            m_context = context ?? throw new ArgumentNullException(nameof(context));
            m_engine = engine ?? throw new ArgumentNullException(nameof(engine));
            m_valuePool = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>();
        }

        /// <summary>
        /// Gets or sets the context associated with the value service.
        /// </summary>
        public BaristaContext Context
        {
            get
            {
                if (m_context.IsDisposed)
                    throw new ObjectDisposedException(nameof(Context));

                return m_context;
            }
        }

        public JsArray CreateArray(uint length)
        {
            var arrayHandle = m_engine.JsCreateArray(length);
            return CreateValue<JsArray>(arrayHandle);
        }

        public JsArrayBuffer CreateArrayBuffer(string data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            JavaScriptValueSafeHandle externalArrayHandle;
            IntPtr ptrData = Marshal.StringToHGlobalAnsi(data);
            try
            {
                externalArrayHandle = m_engine.JsCreateExternalArrayBuffer(ptrData, (uint)data.Length, null, IntPtr.Zero);
            }
            catch (Exception)
            {
                //If anything goes wrong, free the unmanaged memory.
                //This is not a finally as if success, the memory will be freed automagially.
                Marshal.ZeroFreeGlobalAllocAnsi(ptrData);
                throw;
            }

            var result = m_valuePool.GetOrAdd(externalArrayHandle, () =>
            {
                var flyweight = new JsManagedExternalArrayBuffer(m_engine, Context, this, externalArrayHandle, ptrData, (ptr) =>
                {
                    Marshal.ZeroFreeGlobalAllocAnsi(ptr);
                });

                return flyweight;
            });

            var resultArrayBuffer = result as JsArrayBuffer;
            if (resultArrayBuffer == null)
                throw new InvalidOperationException($"Expected the result object to be a JsArrayBuffer, however the value was {result.GetType()}");

            return (JsArrayBuffer)result;
        }

        public JsNumber CreateNumber(int number)
        {
            var numberHandle = m_engine.JsIntToNumber(number);
            return CreateValue<JsNumber>(numberHandle);
        }

        public JsObject CreateObject()
        {
            var objectHandle = m_engine.JsCreateObject();
            return CreateValue<JsObject>(objectHandle);
        }

        public JsString CreateString(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            var stringHandle = m_engine.JsCreateString(str, (ulong)str.Length);
            return CreateValue<JsString>(stringHandle);
        }

        /// <summary>
        /// Returns a new JavaScriptValue for the specified handle querying for the handle's value type.
        /// </summary>
        /// <remarks>
        /// Use the valueType parameter carefully. If the resulting type does not match the handle type unexpected issues may occur.
        /// </remarks>
        /// <returns>The JavaScript Value that represents the handle</returns>
        public JsValue CreateValue(JavaScriptValueSafeHandle valueHandle, JavaScriptValueType? valueType = null)
        {
            return m_valuePool.GetOrAdd(valueHandle, () =>
            {
                if (valueType.HasValue == false)
                {
                    valueType = m_engine.JsGetValueType(valueHandle);
                }

                JsValue result;
                switch (valueType.Value)
                {
                    case JavaScriptValueType.Array:
                        result = new JsArray(m_engine, Context, this, valueHandle);
                        break;
                    case JavaScriptValueType.ArrayBuffer:
                        result = new JsArrayBuffer(m_engine, Context, this, valueHandle);
                        break;
                    case JavaScriptValueType.Boolean:
                        result = new JsBoolean(m_engine, Context, this, valueHandle);
                        break;
                    case JavaScriptValueType.DataView:
                        result = new JsDataView(m_engine, Context, this, valueHandle);
                        break;
                    case JavaScriptValueType.Error:
                        result = new JsError(m_engine, Context, this, valueHandle);
                        break;
                    case JavaScriptValueType.Function:
                        result = new JsFunction(m_engine, Context, this, valueHandle);
                        break;
                    case JavaScriptValueType.Null:
                        result = new JsNull(m_engine, Context, valueHandle);
                        break;
                    case JavaScriptValueType.Number:
                        result = new JsNumber(m_engine, Context, this, valueHandle);
                        break;
                    case JavaScriptValueType.Object:
                        result = new JsObject(m_engine, Context, this, valueHandle);
                        break;
                    case JavaScriptValueType.String:
                        result = new JsString(m_engine, Context, this, valueHandle);
                        break;
                    case JavaScriptValueType.Symbol:
                        result = new JsSymbol(m_engine, Context, valueHandle);
                        break;
                    case JavaScriptValueType.TypedArray:
                        result = new JsTypedArray(m_engine, Context, this, valueHandle);
                        break;
                    case JavaScriptValueType.Undefined:
                        result = new JsUndefined(m_engine, Context, valueHandle);
                        break;
                    default:
                        throw new NotImplementedException($"Error Creating JavaScript Value: The JavaScript Value Type '{valueType}' is unknown, invalid, or has not been implemented.");
                }

                result.BeforeCollect += JsValueBeforeCollectCallback;

                return result;
            });
        }

        /// <summary>
        /// Returns a new JavaScriptValue for the specified handle using the supplied type information.
        /// </summary>
        /// <returns>The JavaScript Value that represents the Handle</returns>
        public T CreateValue<T>(JavaScriptValueSafeHandle valueHandle)
            where T : JsValue
        {
            return m_valuePool.GetOrAdd(valueHandle, () =>
            {
                var targetType = typeof(T);

                T result;
                //JsObject Derived Value Types first.
                if (typeof(JsObject).IsSameOrSubclass(targetType))
                {
                    result = Activator.CreateInstance(targetType, new object[] { m_engine, Context, this, valueHandle }) as T;
                }
                else if (typeof(JsSymbol).IsSameOrSubclass(targetType))
                {
                    result = new JsSymbol(m_engine, Context, valueHandle) as T;
                }
                else if (typeof(JsUndefined).IsSameOrSubclass(targetType))
                {
                    result = new JsUndefined(m_engine, Context, valueHandle) as T;
                }
                else if (typeof(JsNull).IsSameOrSubclass(targetType))
                {
                    result = new JsNull(m_engine, Context, valueHandle) as T;
                }
                else
                {
                    throw new InvalidOperationException($"Type {targetType} must subclass JsValue");
                }

                if (result == null)
                    throw new BaristaException($"Unable to create an object of type {targetType}.");

                result.BeforeCollect += JsValueBeforeCollectCallback;
                return result;
            }) as T;
        }

        public JsObject GetGlobalObject()
        {
            var globalValueHandle = m_engine.JsGetGlobalObject();
            return CreateValue<JsObject>(globalValueHandle);
        }

        public JsBoolean GetFalseValue()
        {
            var falseValueHandle = m_engine.JsGetFalseValue();
            return CreateValue<JsBoolean>(falseValueHandle);
        }

        public JsNull GetNullValue()
        {
            var nullValueHandle = m_engine.JsGetNullValue();
            return CreateValue<JsNull>(nullValueHandle);
        }

        public JsBoolean GetTrueValue()
        {
            var trueValueHandle = m_engine.JsGetTrueValue();
            return CreateValue<JsBoolean>(trueValueHandle);
        }

        public JsUndefined GetUndefinedValue()
        {
            var undefinedValueHandle = m_engine.JsGetUndefinedValue();
            return CreateValue<JsUndefined>(undefinedValueHandle);
        }

        private void JsValueBeforeCollectCallback(object sender, BaristaObjectBeforeCollectEventArgs args)
        {
            ((JsValue)sender).BeforeCollect -= JsValueBeforeCollectCallback;
            Debug.Assert(m_valuePool != null);
            m_valuePool.RemoveHandle(new JavaScriptValueSafeHandle(args.Handle));
        }

        /// <summary>
        /// Method that all objects created though this service call when the runtime disposes of them.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="callbackState"></param>
        private void OnBeforeCollectCallback(IntPtr handle, IntPtr callbackState)
        {
            //If the valuepool is null, this service has already been disposed.
            if (m_valuePool == null)
                return;

            m_valuePool.RemoveHandle(new JavaScriptValueSafeHandle(handle));
        }

        #region IDisposable
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_valuePool != null)
                {
                    m_valuePool.Dispose();
                    m_valuePool = null;
                }
            }
        }

        /// <summary>
        /// Disposes of the service and all references contained within.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BaristaValueService()
        {
            Dispose(false);
        }
        #endregion
    }
}
