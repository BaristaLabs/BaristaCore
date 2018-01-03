namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class BaristaConversionStrategy : IBaristaConversionStrategy
    {
        private readonly IJsonConverter m_jsonConverter;
        private readonly IBaristaTypeConversionStrategy m_typeConversionStrategy;

        public BaristaConversionStrategy()
            : this(null, null)
        {
        }

        public BaristaConversionStrategy(IJsonConverter jsonConverter)
            : this(null, jsonConverter)
        {
        }

        public BaristaConversionStrategy(IBaristaTypeConversionStrategy typeConversionStrategy)
            : this(typeConversionStrategy, null)
        {
        }

        public BaristaConversionStrategy(IBaristaTypeConversionStrategy typeConversionStrategy, IJsonConverter jsonConverter)
        {
            m_typeConversionStrategy = typeConversionStrategy;
            m_jsonConverter = jsonConverter;
        }

        public bool TryFromObject(BaristaContext context, object obj, out JsValue value)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.IsDisposed)
                throw new ObjectDisposedException(nameof(context));

            var valueFactory = (IBaristaValueFactory)context;

            //Well this is easy!
            if (obj == null)
            {
                value = valueFactory.Null;
                return true;
            }

            switch (obj)
            {
                case Undefined undefinedValue:
                    value = valueFactory.Undefined;
                    return true;
                case JsValue jsValue:
                    value = jsValue;
                    return true;
                case JavaScriptValueSafeHandle jsValueSafeHandle:
                    value = valueFactory.CreateValue(jsValueSafeHandle);
                    return true;
                case string stringValue:
                    value = valueFactory.CreateString(stringValue);
                    return true;
                case double doubleValue:
                case float floatValue:
                    value = valueFactory.CreateNumber((double)obj);
                    return true;
                case int intValue:
                case short shortValue:
                case ushort ushortValue:
                case byte byteValue:
                case sbyte sbyteValue:
                    value = valueFactory.CreateNumber((int)obj);
                    return true;
                case uint uintValue:
                    value = valueFactory.CreateNumber(uintValue);
                    return true;
                case long longValue:
                    value = valueFactory.CreateNumber(longValue);
                    return true;
                case bool boolValue:
                    value = boolValue ? valueFactory.True : valueFactory.False;
                    return true;
                case Array arrayValue:
                    var arr = valueFactory.CreateArray((uint)arrayValue.LongLength);
                    for (int i = 0; i < arrayValue.Length; i++)
                    {
                        if (TryFromObject(context, arrayValue.GetValue(i), out JsValue currentValue))
                        {
                            arr[i] = currentValue;
                        }
                        else
                        {
                            value = null;
                            return false;
                        }
                    }
                    value = arr;
                    return true;
                //Don't convert IEnumerators -- they'll be auto-created.
                //case IEnumerator enumeratorValue:
                //    value = valueFactory.CreateIterator(enumeratorValue);
                //    return true;
                case Delegate delegateValue:
                    value = valueFactory.CreateFunction(delegateValue);
                    return true;
                case Exception exValue:
                    value = valueFactory.CreateError(exValue.Message);
                    return true;
                case Task taskValue:
                    value = valueFactory.CreatePromise(taskValue);
                    return true;
                case Type typeValue:
                    if (m_typeConversionStrategy == null)
                    {
                        value = null;
                        return false;
                    }
                    var result = m_typeConversionStrategy.TryCreatePrototypeFunction(context, typeValue, out JsFunction fnValue);
                    value = fnValue;
                    return result;
            }

            var objType = obj.GetType();

            if (objType.IsValueType)
            {
                if (m_jsonConverter == null)
                {
                    value = null;
                    return false;
                }

                var jsonString = m_jsonConverter.Stringify(obj);
                value = context.JSON.Parse(valueFactory.CreateString(jsonString));
                return true;
            }

            //We've run out of options, convert the non-primitive object.
            return TryConvertFromNonPrimitiveObject(context, obj, out value);
        }

        private bool TryConvertFromNonPrimitiveObject(BaristaContext context, object obj, out JsValue value)
        {
            if (m_typeConversionStrategy == null)
            {
                //TODO: think about cheating with a JsonConversion
                value = null;
                return false;
            }
                
            Type typeToConvert = obj.GetType();
            if (m_typeConversionStrategy.TryCreatePrototypeFunction(context, typeToConvert, out JsFunction fnCtor))
            {
                var exObj = context.CreateExternalObject(obj);
                var resultValue = fnCtor.Construct(null, exObj);
                
                value = resultValue;
                return true;
            }
            
            value = null;
            return false;
        }

        public bool TryToObject(BaristaContext context, JsValue value, out object obj)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.IsDisposed)
                throw new ObjectDisposedException(nameof(context));

            var valueFactory = (IBaristaValueFactory)context;

            if (value == null)
            {
                obj = null;
                return true;
            }

            switch (value)
            {
                case JsNull jsNull:
                    obj = null;
                    return true;
                case JsUndefined jsUndefined:
                    obj = Undefined.Value;
                    return true;
                case JsBoolean jsBoolean:
                    obj = jsBoolean.ToBoolean();
                    return true;
                case JsNumber jsNumber:
                    obj = jsNumber.ToDouble();
                    return true;
                case JsSymbol jsSymbol:
                case JsFunction jsFunction:
                case JsString jsString:
                    obj = value.ToString();
                    return true;
                case JsArray jsArray:
                    var successful = true;
                    var arrayResult = jsArray.Select(v =>
                    {
                        if (TryToObject(context, v, out object currentValue))
                        {
                            return currentValue;
                        }
                        successful = false;
                        return null;
                    }).ToArray();

                    if (successful)
                    {
                        obj = arrayResult;
                        return true;
                    }
                    obj = null;
                    return false;
                case JsExternalArrayBuffer jsExternalArrayBuffer:
                    //TODO: Add this implementation.
                    obj = null;
                    return false;
                case JsArrayBuffer jsArrayBuffer:
                    obj = jsArrayBuffer.GetArrayBufferStorage();
                    return true;
                case JsDataView jsDataView:
                    obj = jsDataView.GetDataViewStorage();
                    return true;
                case JsTypedArray jsTypedArray:
                    obj = jsTypedArray.GetTypedArrayStorage();
                    return true;
                case JsError jsError:
                    obj = new JsScriptException(JsErrorCode.ScriptException, jsError.Handle);
                    return true;
                case JsExternalObject jsExternalObject:
                    obj = jsExternalObject.Target;
                    return true;
                case JsObject jsObject:
                    //TODO: we can cheat a bit here with Json converter
                    //Also, figure out how to convert other types, like the date built-in.
                    obj = jsObject;
                    return true;
                default:
                    obj = null;
                    return false;
            }
        }
    }
}
