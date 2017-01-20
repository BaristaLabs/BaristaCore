namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using Microsoft.CSharp;

    public class JavaScriptObject : JavaScriptValue
    {
        internal JavaScriptObject(IJavaScriptEngine engine, JavaScriptContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }

        /// <summary>
        /// Returns the value of the specified JavaScript property. An active execution scope is required.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public JavaScriptValue GetPropertyByName(string propertyName)
        {
            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length);

            try
            {
                var propertyHandle = Engine.JsGetProperty(Handle, propertyIdHandle);
                return Context.ValuePool.GetOrAdd(propertyHandle);
            }
            finally
            {
                propertyIdHandle.Dispose();
            }
        }

        public T GetPropertyByName<T>(string propertyName)
        {
            dynamic result = GetPropertyByName(propertyName);
            return (T)result;
        }
    }
}
