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

        public BaristaConversionStrategy()
        {
        }

        public BaristaConversionStrategy(IJsonConverter jsonConverter)
        {
            //JsonConverter is not required.
            m_jsonConverter = jsonConverter;
        }

        public bool TryFromObject(IBaristaValueFactory valueFactory, object obj, out JsValue value)
        {
            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            var context = valueFactory.Context;
            if (context.IsDisposed)
                throw new ObjectDisposedException(nameof(context));

            //Well this is easy!
            if (obj == null)
            {
                value = valueFactory.GetNullValue();
                return true;
            }

            switch (obj)
            {
                case Undefined undefinedValue:
                    value = valueFactory.GetUndefinedValue();
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
                    value = boolValue ? valueFactory.GetTrueValue() : valueFactory.GetFalseValue();
                    return true;
                case IEnumerable enumerableValue:
                    var arrayValue = enumerableValue.OfType<object>().ToArray();
                    var arr = valueFactory.CreateArray((uint)arrayValue.LongLength);
                    for(int i = 0; i < arrayValue.Length; i++)
                    {
                        if (TryFromObject(valueFactory, arrayValue.GetValue(i), out JsValue currentValue))
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
                case Delegate delegateValue:
                    value = valueFactory.CreateFunction(delegateValue);
                    return true;
                case Exception exValue:
                    //Create an error.
                    var error = valueFactory.CreateError(exValue.Message);
                    //TODO: Potentially populate error properties.
                    value = error;
                    return true;
                case Task taskValue:
                    value = valueFactory.CreatePromise(taskValue);
                    return true;
            }

            var objType = obj.GetType();

            if (objType.IsValueType)
            {
                if (m_jsonConverter == null)
                {
                    //throw new ArgumentException("Non-primitive value types require that a json converter func be specified.");
                    value = null;
                    return false;
                }

                var jsonString = m_jsonConverter.Stringify(obj);
                value = context.JSON.Parse(valueFactory.CreateString(jsonString));
                return true;
            }

            //We've run out of options, convert the non-primitive object.
            return TryConvertFromNonPrimitiveObject(valueFactory, obj, out value);
        }

        private bool TryConvertFromNonPrimitiveObject(IBaristaValueFactory valueFactory, object obj, out JsValue value)
        {
            value = null;
            return false;
        }

        public bool TryToObject(BaristaContext context, JsValue value, out object obj)
        {
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
                    throw new NotImplementedException();
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
                    obj = new BaristaScriptException(jsError);
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
