﻿namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    public interface IBaristaValueFactory : IDisposable
    {
        JsString CreateString(BaristaContext context, string str);

        JsExternalArrayBuffer CreateExternalArrayBufferFromString(BaristaContext context, string data);

        JsError CreateError(BaristaContext context, JavaScriptValueSafeHandle valueHandle);

        JsFunction CreateFunction(BaristaContext context, JavaScriptValueSafeHandle valueHandle);

        JsNumber CreateNumber(BaristaContext context, int number);

        JsValue CreateValue(BaristaContext context, JavaScriptValueSafeHandle valueHandle);

        T CreateValue<T>(BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            where T : JsValue;

        JsBoolean GetFalseValue(BaristaContext context);

        JsNull GetNullValue(BaristaContext context);

        JsBoolean GetTrueValue(BaristaContext context);

        JsUndefined GetUndefinedValue(BaristaContext context);
    }
}