namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System.Runtime.InteropServices;

    public class JsArrayBuffer : JsObject
    {
        public JsArrayBuffer(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
        }

        public override JsValueType Type
        {
            get { return JsValueType.ArrayBuffer; }
        }

        public byte[] GetArrayBufferStorage()
        {
            var ptrBuffer = Engine.JsGetArrayBufferStorage(Handle, out uint bufferLength);
            byte[] buffer = new byte[bufferLength];
            Marshal.Copy(ptrBuffer, buffer, 0, (int)bufferLength);
            return buffer;
        }
    }
}
