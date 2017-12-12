namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public class JsError : JsObject
    {
        public JsError(IJavaScriptEngine engine, BaristaContext context, IBaristaValueService valueService, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueService, valueHandle)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.Error; }
        }

        public string Message
        {
            get
            {
                dynamic result = GetPropertyByName<JsString>("message");
                return (string)result;
            }
        }

        public int Line
        {
            get
            {
                dynamic result = GetPropertyByName<JsNumber>("line");
                return (int)result;
            }
        }

        public int Column
        {
            get
            {
                dynamic result = GetPropertyByName<JsNumber>("column");
                return (int)result;
            }
        }

        public int Length
        {
            get
            {
                dynamic result = GetPropertyByName<JsNumber>("length");
                return (int)result;
            }
        }

        public string Source
        {
            get
            {
                dynamic result = GetPropertyByName<JsString>("source");
                return (string)result;
            }
        }
    }
}
