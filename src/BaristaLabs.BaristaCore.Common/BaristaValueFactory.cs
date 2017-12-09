namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Runtime.InteropServices;

    public sealed class BaristaValueFactory : IBaristaValueFactory
    {
        private BaristaObjectPool<JsValue, JavaScriptValueSafeHandle> m_valuePool;

        private readonly IJavaScriptEngine m_engine;

        public BaristaValueFactory(IJavaScriptEngine engine)
        {
            m_engine = engine ?? throw new ArgumentNullException(nameof(engine));
            m_valuePool = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>((target) =>
            {
                // Certain types do not participate in collect callback.
                //These throw an invalid argument exception when attempting to set a beforecollectcallback.
                if (target is JsNumber)
                    return;

                m_engine.JsSetObjectBeforeCollectCallback(target.Handle, IntPtr.Zero, null);
            });
        }

        public JsString CreateString(BaristaContext context, string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            var stringHandle = m_engine.JsCreateString(str, (ulong)str.Length);
            var flyweight = new JsString(m_engine, context, stringHandle);
            if (m_valuePool.TryAdd(flyweight))
            {
                m_engine.JsSetObjectBeforeCollectCallback(stringHandle, IntPtr.Zero, OnBeforeCollectCallback);
                return flyweight;
            }

            flyweight.Dispose();
            throw new InvalidOperationException("Could not create string. The string already exists at that location in memory.");
        }

        public JsExternalArrayBuffer CreateExternalArrayBufferFromString(BaristaContext context, string data)
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

            var flyweight = new JavaScriptManagedExternalArrayBuffer(m_engine, context, externalArrayHandle, ptrData, (ptr) =>
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
        /// Returns a new JavaScriptValue for the specified handle querying for the handle's value type.
        /// </summary>
        /// <returns>The JavaScript Value that represents the handle</returns>
        public JsValue CreateValue(BaristaContext context, JavaScriptValueSafeHandle valueHandle)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.IsDisposed)
                throw new ObjectDisposedException(nameof(context));

            return m_valuePool.GetOrAdd(valueHandle, () =>
            {
                var valueType = m_engine.JsGetValueType(valueHandle);
                JsValue result;
                switch (valueType)
                {
                    case JavaScriptValueType.Array:
                        result = new JsArray(m_engine, context, valueHandle);
                        break;
                    case JavaScriptValueType.ArrayBuffer:
                        result = new JsArrayBuffer(m_engine, context, valueHandle);
                        break;
                    case JavaScriptValueType.Boolean:
                        result = new JsBoolean(m_engine, context, valueHandle);
                        break;
                    case JavaScriptValueType.DataView:
                        //TODO: Add a dataview
                        throw new NotImplementedException();
                    case JavaScriptValueType.Error:
                        //TODO: Realign exception classes to be JavaScriptValues... or something.
                        throw new NotImplementedException();
                    case JavaScriptValueType.Function:
                        result = new JsFunction(m_engine, context, this, valueHandle);
                        break;
                    case JavaScriptValueType.Null:
                        result = new JsNull(m_engine, context, valueHandle);
                        break;
                    case JavaScriptValueType.Number:
                        result = new JsNumber(m_engine, context, valueHandle);
                        break;
                    case JavaScriptValueType.Object:
                        result = new JsObject(m_engine, context, this, valueHandle);
                        break;
                    case JavaScriptValueType.String:
                        result = new JsString(m_engine, context, valueHandle);
                        break;
                    case JavaScriptValueType.Symbol:
                        //TODO: add symbol class.
                        throw new NotImplementedException();
                    case JavaScriptValueType.TypedArray:
                        result = new JsTypedArray(m_engine, context, this, valueHandle);
                        break;
                    case JavaScriptValueType.Undefined:
                        result = new JsUndefined(m_engine, context, valueHandle);
                        break;
                    default:
                        throw new NotImplementedException($"Error Creating JavaScript Value: The JavaScript Value Type '{valueType}' is unknown, invalid, or has not been implemented.");
                }

                //Certain types do not participate in collect callback.
                //These throw an invalid argument exception when attempting to set a beforecollectcallback.
                if (result is JsNumber)
                    return result;

                m_engine.JsSetObjectBeforeCollectCallback(valueHandle, IntPtr.Zero, OnBeforeCollectCallback);
                return result;
            });
        }

        /// <summary>
        /// Returns a new JavaScriptValue for the specified handle using the supplied type information.
        /// </summary>
        /// <returns>The JavaScript Value that represents the Handle</returns>
        public T CreateValue<T>(BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            where T : JsValue
        {
            return CreateValue(context, valueHandle) as T;
        }


        public JsBoolean GetFalseValue(BaristaContext context)
        {
            var falseValue = m_engine.JsGetFalseValue();
            var result = new JsBoolean(m_engine, context, falseValue);
            if (m_valuePool.TryAdd(result))
                return result;

            result.Dispose();
            throw new InvalidOperationException("Could not add JsFalse to the Value Pool associated with the context.");
        }

        public JsNull GetNullValue(BaristaContext context)
        {
            var nullValue = m_engine.JsGetNullValue();
            var result = new JsNull(m_engine, context, nullValue);
            if (m_valuePool.TryAdd(result))
                return result;

            result.Dispose();
            throw new InvalidOperationException("Could not add JsNull to the Value Pool associated with the context.");
        }

        public JsBoolean GetTrueValue(BaristaContext context)
        {
            var trueValue = m_engine.JsGetTrueValue();
            var result = new JsBoolean(m_engine, context, trueValue);
            if (m_valuePool.TryAdd(result))
                return result;

            result.Dispose();
            throw new InvalidOperationException("Could not add JsTrue to the Value Pool associated with the context.");
        }

        public JsUndefined GetUndefinedValue(BaristaContext context)
        {
            var undefinedValue = m_engine.JsGetUndefinedValue();
            var result = new JsUndefined(m_engine, context, undefinedValue);
            if (m_valuePool.TryAdd(result))
                return result;

            result.Dispose();
            throw new InvalidOperationException("Could not add JsUndefined to the Value Pool associated with the context.");
        }

        private void OnBeforeCollectCallback(IntPtr handle, IntPtr callbackState)
        {
            //If the valuepool is null, this factory has already been disposed.
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
        /// Disposes of the factory and all references contained within.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BaristaValueFactory()
        {
            Dispose(false);
        }
        #endregion
    }
}
