namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    /// <summary>
    /// Represents the built-in JSON Object.
    /// </summary>
    public class JsJSON : JsObject
    {
        public JsJSON(IJavaScriptEngine engine, BaristaContext context, IBaristaValueService valueService, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueService, valueHandle)
        {

        }

        public string Stringify(JsValue value, JsValue replacer = null, JsValue space = null)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var fnStringify = GetProperty<JsFunction>("stringify");
            var resultHandle = fnStringify.Invoke<JsString>(fnStringify, value, replacer, space);
            if (resultHandle == null)
                return null;

            return resultHandle.ToString();
        }

        public JsValue Parse(JsValue value, JsValue reviver = null)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var fnParse = GetProperty<JsFunction>("parse");
            return fnParse.Invoke(fnParse, value, reviver);
        }
    }
}
