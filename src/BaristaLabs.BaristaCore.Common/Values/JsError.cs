namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public sealed class JsError : JsObject
    {
        public JsError(IJavaScriptEngine engine, BaristaContext context, IBaristaValueFactory valueFactory, JavaScriptValueSafeHandle value)
            : base(engine, context, valueFactory, value)
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
                return GetPropertyByName<string>("message");
            }
        }

        public int Line
        {
            get
            {
                return GetPropertyByName<int>("line");
            }
        }

        public int Column
        {
            get
            {
                return GetPropertyByName<int>("column");
            }
        }

        public int Length
        {
            get
            {
                return GetPropertyByName<int>("length");
            }
        }

        public string Source
        {
            get
            {
                return GetPropertyByName<string>("source");
            }
        }
    }
}
