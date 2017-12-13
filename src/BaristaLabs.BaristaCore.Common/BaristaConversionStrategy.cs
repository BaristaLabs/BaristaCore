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
        public JsValue FromObject(IBaristaValueService valueService, object obj, Func<object, string> jsonConverter = null)
        {
            if (valueService == null)
                throw new ArgumentNullException(nameof(valueService));

            var context = valueService.Context;
            if (context.IsDisposed)
                throw new ObjectDisposedException(nameof(context));

            //Well this is easy!
            if (obj == null)
            {
                return valueService.GetNullValue();
            }

            if (obj is JsValue jsValue)
            {
                return jsValue;
            }

            if (obj is JavaScriptValueSafeHandle jsValueSafeHandle)
            {
                return valueService.CreateValue(jsValueSafeHandle);
            }

            switch(obj)
            {
                case string stringValue:
                    return valueService.CreateString(stringValue);
                case double doubleValue:
                case float floatValue:
                    return valueService.CreateNumber((double)obj);
                case int intValue:
                case short shortValue:
                case ushort ushortValue:
                case byte byteValue:
                case sbyte sbyteValue:
                    return valueService.CreateNumber((int)obj);
                case uint uintValue:
                    return valueService.CreateNumber(uintValue);
                case long longValue:
                    return valueService.CreateNumber(longValue);
                case bool boolValue:
                    return boolValue ? valueService.GetTrueValue() : valueService.GetFalseValue();
                case IEnumerable enumerableValue:
                    var arrayValue = enumerableValue.OfType<object>().ToArray();
                    var arr = valueService.CreateArray((uint)arrayValue.LongLength);
                    for(int i = 0; i < arrayValue.Length; i++)
                    {
                        arr[i] = FromObject(valueService, arrayValue.GetValue(i), jsonConverter);
                    }
                    return arr;
                case Delegate delegateValue:
                    //TODO: create a JsFunction and provide the parameters -- probably will be in a method below.
                    throw new NotImplementedException();
                case Exception exValue:
                    //Create an error.
                    var error = valueService.CreateError(exValue.Message);
                    //TODO: Potentially populate error properties.
                    return error;
                case Task taskValue:
                    return ConvertFromTask(valueService, taskValue, jsonConverter);
            }

            var objType = obj.GetType();

            if (objType.IsValueType)
            {
                if (jsonConverter == null)
                    throw new ArgumentException("Non-primitive value types require that a json converter func be specified.");

                var jsonString = jsonConverter(obj);
                return context.JSON.Parse(valueService.CreateString(jsonString));
            }

            //We've run out of options, convert the non-primitive object.
            return ConvertFromNonPrimitiveObject(valueService, obj, jsonConverter);
        }

        private JsObject ConvertFromTask(IBaristaValueService valueService, Task task, Func<object, string> jsonConverter = null)
        {
            //Create a promise
            var promise = valueService.CreatePromise(out JsFunction resolve, out JsFunction reject);
            task.ContinueWith((t) =>
            {
                if (t.IsCanceled || t.IsFaulted)
                {
                    reject.Invoke(FromObject(valueService, t.Exception, jsonConverter));
                    return;
                }

                var resultType = t.GetType();
                var resultProperty = resultType.GetProperty("Result");
                if (resultProperty == null)
                {
                    resolve.Invoke(valueService.GetNullValue());
                    return;
                }

                var result = resultProperty.GetValue(t);
                resolve.Invoke(FromObject(valueService, result, jsonConverter));
            });

            task.Start();
            return promise;
        }

        private JsObject ConvertFromNonPrimitiveObject(IBaristaValueService valueService, object obj, Func<object, string> jsonConverter = null)
        {
            throw new NotImplementedException();
        }

        public object ToObject(BaristaContext context, JsValue value)
        {
            if (value == null)
                return null;

            switch (value)
            {
                case JsNull jsNull:
                    return null;
                case JsUndefined jsUndefined:
                    return Undefined.Value;
                case JsBoolean jsBoolean:
                    return jsBoolean.ToBoolean();
                case JsNumber jsNumber:
                    return jsNumber.ToDouble();
                case JsSymbol jsSymbol:
                case JsFunction jsFunction:
                case JsString jsString:
                    return value.ToString();
                case JsArray jsArray:
                    return jsArray.Select(v => ToObject(context, v)).ToArray();
                case JsExternalArrayBuffer jsExternalArrayBuffer:
                    throw new NotImplementedException();
                case JsArrayBuffer jsArrayBuffer:
                    return jsArrayBuffer.GetArrayBufferStorage();
                case JsDataView jsDataView:
                    return jsDataView.GetDataViewStorage();
                case JsTypedArray jsTypedArray:
                    return jsTypedArray.GetTypedArrayStorage();
                case JsError jsError:
                    return new BaristaScriptException(jsError);
                case JsObject jsObject:
                    //TODO: we can cheat a bit here with Json converter
                    //Also, figure out how to convert other types, like the date built-in.
                    return jsObject;
                default:
                    throw new NotImplementedException($"Conversion of a {value} to a .net type is not implemented.");
            }
        }
    }
}
