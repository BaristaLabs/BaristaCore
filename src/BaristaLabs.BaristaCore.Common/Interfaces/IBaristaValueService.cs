﻿namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    public interface IBaristaValueService : IDisposable
    {
        /// <summary>
        /// Gets or sets the context associated with the Value Service.
        /// </summary>
        BaristaContext Context
        {
            get;
        }

        JsArray CreateArray(uint length);

        JsArrayBuffer CreateArrayBuffer(string data);

        JsError CreateError(string message);

        JsNumber CreateNumber(double number);

        JsNumber CreateNumber(int number);

        JsObject CreateObject();

        JsObject CreatePromise(out JsFunction resolve, out JsFunction reject);

        JsString CreateString(string str);

        /// <summary>
        /// Returns a new JsValue for the specified handle using the specified type information. If no type information is provided, the object will be queried for its type.
        /// </summary>
        /// <param name="valueHandle"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        JsValue CreateValue(JavaScriptValueSafeHandle valueHandle, JavaScriptValueType? valueType = null);

        /// <summary>
        /// Returns a new JavaScriptValue for the specified handle using the supplied type information.
        /// </summary>
        /// <returns>The JavaScript Value that represents the Handle</returns>
        JsValue CreateValue(Type targetType, JavaScriptValueSafeHandle valueHandle);

        /// <summary>
        /// Returns a new JavaScriptValue for the specified handle using the supplied type information.
        /// </summary>
        /// <returns>The JavaScript Value that represents the Handle</returns>
        T CreateValue<T>(JavaScriptValueSafeHandle valueHandle)
            where T : JsValue;

        /// <summary>
        /// Gets the value of false in the specified script context.
        /// </summary>
        /// <returns></returns>
        JsBoolean GetFalseValue();

        /// <summary>
        /// Gets the global object in the specified script context.
        /// </summary>
        /// <returns></returns>
        JsObject GetGlobalObject();

        /// <summary>
        /// Gets the value of null in the specified script context.
        /// </summary>
        /// <returns></returns>
        JsNull GetNullValue();

        /// <summary>
        /// Gets the value of true in the specified script context.
        /// </summary>
        /// <returns></returns>
        JsBoolean GetTrueValue();

        /// <summary>
        /// Gets the value of undefined in the specified script context.
        /// </summary>
        /// <returns></returns>
        JsUndefined GetUndefinedValue();
    }
}