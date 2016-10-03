namespace BaristaLabs.BaristaCore.JavaScript
{
    using SafeHandles;
    using System;

    public class JavaScriptSymbol : JavaScriptValue
    {
        internal JavaScriptSymbol(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptEngine engine) :
            base(handle, type, engine)
        {

        }

        public string Description
        {
            get
            {
                throw new NotImplementedException("Converting a symbol to a string in the host is not currently supported.  To get the Symbol's description, if there is one, request it via toString in script.");
            }
        }
    }
}
