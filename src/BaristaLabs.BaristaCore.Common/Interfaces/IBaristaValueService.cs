namespace BaristaLabs.BaristaCore
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

        JsNumber CreateNumber(int number);

        JsObject CreateObject();

        JsString CreateString(string str);

        JsValue CreateValue(JavaScriptValueSafeHandle valueHandle, JavaScriptValueType? valueType = null);

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