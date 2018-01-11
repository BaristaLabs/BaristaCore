namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Collections;
    using System.Threading.Tasks;

    public interface IBaristaValueFactory : IDisposable
    {

        #region Properties
        /// <summary>
        /// Gets the value of false.
        /// </summary>
        /// <returns></returns>
        JsBoolean False
        {
            get;
        }

        /// <summary>
        /// Gets the global object.
        /// </summary>
        /// <returns></returns>
        JsObject GlobalObject
        {
            get;
        }

        /// <summary>
        /// Gets the value of null
        /// </summary>
        /// <returns></returns>
        JsNull Null
        {
            get;
        }

        /// <summary>
        /// Gets the value of true
        /// </summary>
        /// <returns></returns>
        JsBoolean True
        {
            get;
        }

        /// <summary>
        /// Gets the value of undefined
        /// </summary>
        /// <returns></returns>
        JsUndefined Undefined
        {
            get;
        }
        #endregion

        JsArray CreateArray(uint length);

        /// <summary>
        /// Creates a unicode (UTF-16) based ExternalArrayBuffer from the specified string.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        JsArrayBuffer CreateArrayBuffer(string data);

        /// <summary>
        /// Creates a utf-8 based ExternalArrayBuffer from the specified string.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        JsArrayBuffer CreateArrayBufferUtf8(string data);

        JsArrayBuffer CreateArrayBuffer(byte[] data);

        JsError CreateError(string message);

        JsExternalObject CreateExternalObject(object obj);

        JsFunction CreateFunction(Delegate func, string name = null);

        JsIterator CreateIterator(IEnumerator enumerator);

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
        JsValue CreateValue(JavaScriptValueSafeHandle valueHandle, JsValueType? valueType = null);

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
    }
}