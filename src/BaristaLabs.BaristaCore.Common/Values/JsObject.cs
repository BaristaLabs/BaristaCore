namespace BaristaLabs.BaristaCore
{
    using System;
    using BaristaLabs.BaristaCore.JavaScript;

    public class JsObject : JsValue
    {
        private readonly IBaristaValueFactory m_baristaValueFactory;

        public JsObject(IJavaScriptEngine engine, BaristaContext context, IBaristaValueFactory valueFactory, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
            m_baristaValueFactory = valueFactory;
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.Object; }
        }

        public IBaristaValueFactory ValueFactory
        {
            get { return m_baristaValueFactory; }
        }

        /// <summary>
        /// Returns the value of the specified JavaScript property. An active execution scope is required.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public JsValue GetPropertyByName(string propertyName)
        {
            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length);

            try
            {
                var propertyHandle = Engine.JsGetProperty(Handle, propertyIdHandle);
                return m_baristaValueFactory.CreateValue(Context, propertyHandle);
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
