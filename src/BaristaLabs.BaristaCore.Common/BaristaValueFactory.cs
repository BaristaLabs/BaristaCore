namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Threading;

    public sealed class BaristaValueFactory : IBaristaValueFactory
    {
        private BaristaObjectPool<JsValue, JavaScriptValueSafeHandle> m_valuePool;

        private readonly IJavaScriptEngine m_engine;
        private readonly BaristaContext m_context;

        public BaristaValueFactory(IJavaScriptEngine engine, BaristaContext context)
        {
            m_engine = engine ?? throw new ArgumentNullException(nameof(engine));
            m_context = context ?? throw new ArgumentNullException(nameof(context));
            m_valuePool = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>();
        }

        /// <summary>
        /// Gets the context associated with the value factory.
        /// </summary>
        private BaristaContext Context
        {
            get
            {
                if (m_context.IsDisposed)
                    throw new ObjectDisposedException(nameof(Context));

                return m_context;
            }
        }

        /// <summary>
        /// Gets the number of JsValues currently interned in the factory's value pool.
        /// </summary>
        public int Count
        {
            get { return m_valuePool.Count; }
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
                var flyweight = new JsManagedExternalArrayBuffer(m_engine, Context, externalArrayHandle, ptrData, (ptr) =>
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

        public JsError CreateError(string message)
        {
            var messageHandle = CreateString(message);
            var errorHandle = m_engine.JsCreateError(messageHandle.Handle);
            return CreateValue<JsError>(errorHandle);
        }

        public JsError CreateError(Exception ex)
        {
            //TODO: Add more exception properties.
            var messageHandle = CreateString(ex.Message);
            var errorHandle = m_engine.JsCreateError(messageHandle.Handle);
            return CreateValue<JsError>(errorHandle);
        }

        public JsFunction CreateFunction(Delegate func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            //This is crazy fun.
            var funcParams = func.Method.GetParameters();
            JavaScriptNativeFunction fnDelegate = (IntPtr callee, bool isConstructCall, IntPtr[] arguments, ushort argumentCount, IntPtr callbackData) =>
            {
                //Make sure that we have argument values for each parameter.
                var nativeArgs = new object[funcParams.Length];
                for(int i = 0; i < funcParams.Length; i++)
                {
                    var targetParameterType = funcParams[i].ParameterType;

                    var currentArgument = arguments.ElementAtOrDefault(i);
                    if (currentArgument == default(IntPtr))
                    {
                        //If the argument wasn't specified, use the default value for the target parameter.
                        nativeArgs[i] = targetParameterType.GetDefaultValue();
                    }
                    else
                    {
                        //As the argument has been specified, convert the JsValue back to an Object using
                        //the conversion strategy associated with the context.

                        var argValueHandle = new JavaScriptValueSafeHandle(currentArgument);
                        var jsValue = CreateValue(argValueHandle);
                        if (Context.Converter.TryToObject(Context, jsValue, out object obj))
                        {
                            try
                            {
                                nativeArgs[i] = Convert.ChangeType(obj, targetParameterType);
                            }
                            catch(Exception)
                            {
                                //Something went wrong, use the default value.
                                nativeArgs[i] = targetParameterType.GetDefaultValue();
                            }
                        }
                        else
                        {
                            //If we couldn't convert the type, use the default value.
                            nativeArgs[i] = targetParameterType.GetDefaultValue();
                        }
                    }
                }

                try
                {
                    var nativeResult = func.DynamicInvoke(nativeArgs);
                    if (Context.Converter.TryFromObject(Context, nativeResult, out JsValue valueResult))
                    {
                        return valueResult.Handle.DangerousGetHandle();
                    }
                    else
                    {
                        return Context.Undefined.Handle.DangerousGetHandle();
                    }
                }
                catch(TargetInvocationException exceptionResult)
                {
                    var jsError = CreateError(exceptionResult.InnerException);
                    m_engine.JsSetException(jsError.Handle);
                    return Context.Undefined.Handle.DangerousGetHandle();
                }
            };

            var fnHandle = m_engine.JsCreateFunction(fnDelegate, IntPtr.Zero);

            //this is a special case where we cannot use our CreateValue<> method.
            return m_valuePool.GetOrAdd(fnHandle, () =>
            {
                var jsNativeFunction = new JsNativeFunction(m_engine, Context, fnHandle, fnDelegate);
                jsNativeFunction.BeforeCollect += JsValueBeforeCollectCallback;
                return jsNativeFunction;
            }) as JsNativeFunction;
        }

        public JsNumber CreateNumber(double number)
        {
            var numberHandle = m_engine.JsDoubleToNumber(number);
            return CreateValue<JsNumber>(numberHandle);
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

        public JsObject CreatePromise(out JsFunction resolve, out JsFunction reject)
        {
            var promiseHandle = m_engine.JsCreatePromise(out JavaScriptValueSafeHandle resolveHandle, out JavaScriptValueSafeHandle rejectHandle);
            resolve = CreateValue<JsFunction>(resolveHandle);
            reject = CreateValue<JsFunction>(rejectHandle);
            return CreateValue<JsObject>(promiseHandle);
        }

        public JsObject CreatePromise(Task task)
        {
            //Create a promise
            var promise = CreatePromise(out JsFunction resolve, out JsFunction reject);
            task.ContinueWith((t) =>
            {
                if (t.IsCanceled || t.IsFaulted)
                {
                    if (Context.Converter.TryFromObject(Context, t.Exception, out JsValue rejectValue))
                    {
                        reject.Call(GetGlobalObject(), rejectValue);
                    }
                    else
                    {
                        reject.Call(GetGlobalObject(), GetUndefinedValue());
                    }
                }

                var resultType = t.GetType();
                var resultProperty = resultType.GetProperty("Result");
                if (resultProperty == null)
                {
                    resolve.Call(GetGlobalObject(), GetNullValue());
                    return;
                }

                var result = resultProperty.GetValue(t);

                //If we got an object back attempt to convert it into a JsValue and call the resolve method with the value.
                if (Context.Converter.TryFromObject(Context, result, out JsValue resolveValue))
                {
                    resolve.Call(GetGlobalObject(), resolveValue);
                }
                else
                {
                    resolve.Call(GetGlobalObject(), GetUndefinedValue());
                }

            }, Context.TaskFactory.Scheduler);

            //Start the task.
            
            Context.TaskFactory.StartNew(
                () => { return task; },
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                Context.TaskFactory.Scheduler);

            return promise;
        }

        public JsString CreateString(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            var stringHandle = m_engine.JsCreateString(str, (ulong)str.Length);
            return CreateValue<JsString>(stringHandle);
        }

        public JsSymbol CreateSymbol(string description)
        {
            JavaScriptValueSafeHandle descriptionHandle = JavaScriptValueSafeHandle.Invalid;
            if (description != null)
            {
                var descriptionValue = CreateString(description);
                descriptionHandle = descriptionValue.Handle;
            }

            var symbolHandle = m_engine.JsCreateSymbol(descriptionHandle);
            return CreateValue<JsSymbol>(symbolHandle);
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
                        result = new JsArray(m_engine, Context, valueHandle);
                        break;
                    case JavaScriptValueType.ArrayBuffer:
                        result = new JsArrayBuffer(m_engine, Context, valueHandle);
                        break;
                    case JavaScriptValueType.Boolean:
                        result = new JsBoolean(m_engine, Context, valueHandle);
                        break;
                    case JavaScriptValueType.DataView:
                        result = new JsDataView(m_engine, Context, valueHandle);
                        break;
                    case JavaScriptValueType.Error:
                        result = new JsError(m_engine, Context, valueHandle);
                        break;
                    case JavaScriptValueType.Function:
                        result = new JsFunction(m_engine, Context, valueHandle);
                        break;
                    case JavaScriptValueType.Null:
                        result = new JsNull(m_engine, Context, valueHandle);
                        break;
                    case JavaScriptValueType.Number:
                        result = new JsNumber(m_engine, Context, valueHandle);
                        break;
                    case JavaScriptValueType.Object:
                        result = new JsObject(m_engine, Context, valueHandle);
                        break;
                    case JavaScriptValueType.String:
                        result = new JsString(m_engine, Context, valueHandle);
                        break;
                    case JavaScriptValueType.Symbol:
                        result = new JsSymbol(m_engine, Context, valueHandle);
                        break;
                    case JavaScriptValueType.TypedArray:
                        result = new JsTypedArray(m_engine, Context, valueHandle);
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

        public JsValue CreateValue(Type targetType, JavaScriptValueSafeHandle valueHandle)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            if (typeof(JsValue).IsSameOrSubclass(targetType) == false)
                throw new ArgumentOutOfRangeException(nameof(targetType));

            return m_valuePool.GetOrAdd(valueHandle, () =>
            {
                JsValue result;

                if (typeof(JsObject).IsSameOrSubclass(targetType))
                {
                    result = Activator.CreateInstance(targetType, new object[] { m_engine, Context, valueHandle }) as JsValue;
                }
                else if (typeof(JsSymbol).IsSameOrSubclass(targetType))
                {
                    result = new JsSymbol(m_engine, Context, valueHandle);
                }
                else if (typeof(JsUndefined).IsSameOrSubclass(targetType))
                {
                    result = new JsUndefined(m_engine, Context, valueHandle);
                }
                else if (typeof(JsNull).IsSameOrSubclass(targetType))
                {
                    result = new JsNull(m_engine, Context, valueHandle);
                }
                else
                {
                    throw new InvalidOperationException($"Type {targetType} must subclass JsValue");
                }

                if (result == null)
                    throw new BaristaException($"Unable to create an object of type {targetType}.");

                result.BeforeCollect += JsValueBeforeCollectCallback;
                return result;
            });
        }

        public T CreateValue<T>(JavaScriptValueSafeHandle valueHandle)
            where T : JsValue
        {
            var targetType = typeof(T);
            return CreateValue(targetType, valueHandle) as T;
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

        /// <summary>
        /// Raised by JsValues created via the factory. Cleans up the pool.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void JsValueBeforeCollectCallback(object sender, BaristaObjectBeforeCollectEventArgs args)
        {
            ((JsValue)sender).BeforeCollect -= JsValueBeforeCollectCallback;
            Debug.Assert(m_valuePool != null);
            m_valuePool.RemoveHandle(new JavaScriptValueSafeHandle(args.Handle));
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

        ~BaristaValueFactory()
        {
            Dispose(false);
        }
        #endregion
    }
}
