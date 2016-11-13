namespace BaristaLabs.BaristaCore.JavaScript
{
    using Interop;
    using Interop.SafeHandles;
    using System;

    public class JavaScriptSymbol : JavaScriptValue
    {
        internal JavaScriptSymbol(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptContext context) :
            base(handle, type, context)
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
