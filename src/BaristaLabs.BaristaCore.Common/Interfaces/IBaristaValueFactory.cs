namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Threading.Tasks;

    public interface IBaristaValueFactory : IDisposable
    {
        JsArray CreateArray(uint length);

        JsArrayBuffer CreateArrayBuffer(string data);

        JsError CreateError(string message);

        JsExternalObject CreateExternalObject(object obj);

        JsFunction CreateFunction(Delegate func, string name = null);

        JsNumber CreateNumber(double number);

        JsNumber CreateNumber(int number);

        JsObject CreateObject();
        
        /// <summary>
        /// Returns a new Promise that will resolve/reject via the corresponding out parameters.
        /// </summary>
        /// <param name="resolve"></param>
        /// <param name="reject"></param>
        /// <returns></returns>
        JsObject CreatePromise(out JsFunction resolve, out JsFunction reject);

        /// <summary>
        /// Returns a new Promise created from the specified task. The task must be created using the TaskFactory on the context.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        JsObject CreatePromise(Task task);

        JsString CreateString(string str);

        JsSymbol CreateSymbol(string description);

        /// <summary>
        /// Creates a new JavaScript TypeError object.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        JsError CreateTypeError(string message);

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