namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System.Runtime.InteropServices;

    public class JsDataView : JsObject
    {
        public JsDataView(IJavaScriptEngine engine, BaristaContext context, IBaristaValueService valueService, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueService, valueHandle)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.DataView; }
        }

        public byte[] GetDataViewStorage()
        {
            var ptrBuffer = Engine.JsGetDataViewStorage(Handle, out uint bufferLength);
            byte[] buffer = new byte[bufferLength];
            Marshal.Copy(ptrBuffer, buffer, 0, (int)bufferLength);
            return buffer;
        }
    }
}
