namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    public interface IBaristaValueFactory : IDisposable
    {
        JavaScriptString CreateString(BaristaContext context, string str);

        JavaScriptExternalArrayBuffer CreateExternalArrayBufferFromString(BaristaContext context, string data);

        JavaScriptValue CreateValue(BaristaContext context, JavaScriptValueSafeHandle valueHandle);

        T CreateValue<T>(BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            where T : JavaScriptValue;

        JavaScriptBoolean GetFalseValue(BaristaContext context);

        JavaScriptNull GetNullValue(BaristaContext context);

        JavaScriptBoolean GetTrueValue(BaristaContext context);

        JavaScriptUndefined GetUndefinedValue(BaristaContext context);
    }
}