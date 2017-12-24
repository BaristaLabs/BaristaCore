namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System.Runtime.InteropServices;

    public class JsDataView : JsObject
    {
        public JsDataView(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
        }

        public override JsValueType Type
        {
            get { return JsValueType.DataView; }
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
