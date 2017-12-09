namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    public sealed class JsArray : JsValue
    {
        private readonly IBaristaValueFactory m_valueFactory;

        public JsArray(IJavaScriptEngine engine, BaristaContext context, IBaristaValueFactory valueFactory, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
            m_valueFactory = valueFactory;
        }

        public JsValue this[int i]
        {
            get
            {
                using (var indexHandle = m_valueFactory.CreateNumber(Context, i))
                {
                    var valueHandle = Engine.JsGetIndexedProperty(Handle, indexHandle.Handle);
                    return m_valueFactory.CreateValue(Context, valueHandle);
                }
            }
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.Array; }
        }
    }
}
