namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Threading.Tasks;
    using BaristaLabs.BaristaCore.JavaScript;
    using Extensions;

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

        public bool TryFromObject(IBaristaValueService valueService, object obj, out JsValue value)
        {
            if (valueService == null)
                throw new ArgumentNullException(nameof(valueService));

            var context = valueService.Context;
            if (context.IsDisposed)
                throw new ObjectDisposedException(nameof(context));

            //Well this is easy!
            if (obj == null)
            {
                value = valueService.GetNullValue();
                return true;
            }

            switch (obj)
            {
                case Undefined undefinedValue:
                    value = valueService.GetUndefinedValue();
                    return true;
                case JsValue jsValue:
                    value = jsValue;
                    return true;
                case JavaScriptValueSafeHandle jsValueSafeHandle:
                    value = valueService.CreateValue(jsValueSafeHandle);
                    return true;
                case string stringValue:
                    value = valueService.CreateString(stringValue);
                    return true;
                case double doubleValue:
                case float floatValue:
                    value = valueService.CreateNumber((double)obj);
                    return true;
                case int intValue:
                case short shortValue:
                case ushort ushortValue:
                case byte byteValue:
                case sbyte sbyteValue:
                    value = valueService.CreateNumber((int)obj);
                    return true;
                case uint uintValue:
                    value = valueService.CreateNumber(uintValue);
                    return true;
                case long longValue:
                    value = valueService.CreateNumber(longValue);
                    return true;
                case bool boolValue:
                    value = boolValue ? valueService.GetTrueValue() : valueService.GetFalseValue();
                    return true;
                case IEnumerable enumerableValue:
                    var arrayValue = enumerableValue.OfType<object>().ToArray();
                    var arr = valueService.CreateArray((uint)arrayValue.LongLength);
                    for(int i = 0; i < arrayValue.Length; i++)
                    {
                        if (TryFromObject(valueService, arrayValue.GetValue(i), out JsValue currentValue))
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
                    //TODO: create a JsFunction and provide the parameters -- probably will be in a method below.
                    value = null;
                    return false;
                case Exception exValue:
                    //Create an error.
                    var error = valueService.CreateError(exValue.Message);
                    //TODO: Potentially populate error properties.
                    value = error;
                    return true;
                case Task taskValue:
                    return TryConvertFromTask(valueService, taskValue, out value);
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
                value = context.JSON.Parse(valueService.CreateString(jsonString));
                return true;
            }

            //We've run out of options, convert the non-primitive object.
            return TryConvertFromNonPrimitiveObject(valueService, obj, out value);
        }

        private bool TryConvertFromTask(IBaristaValueService valueService, Task task, out JsValue value)
        {
            //Create a promise
            var promise = valueService.CreatePromise(out JsFunction resolve, out JsFunction reject);
            task.ContinueWith((t) =>
            {
                if (t.IsCanceled || t.IsFaulted)
                {
                    if (TryFromObject(valueService, t.Exception, out JsValue rejectValue))
                    {
                        reject.Invoke(rejectValue);
                    }
                    else
                    {
                        reject.Invoke(valueService.GetUndefinedValue());
                    }
                }

                var resultType = t.GetType();
                var resultProperty = resultType.GetProperty("Result");
                if (resultProperty == null)
                {
                    resolve.Invoke(valueService.GetNullValue());
                    return;
                }

                var result = resultProperty.GetValue(t);
                if (TryFromObject(valueService, result, out JsValue resolveValue))
                {
                    resolve.Invoke(resolveValue);
                }
                else
                {
                    resolve.Invoke(valueService.GetUndefinedValue());
                }
                
            });

            task.Start();
            value = promise;
            return true;
        }

        private bool TryConvertFromNonPrimitiveObject(IBaristaValueService valueService, object obj, out JsValue value)
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
