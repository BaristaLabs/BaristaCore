namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System.Runtime.InteropServices;

    public class JsArrayBuffer : JsObject
    {
        public JsArrayBuffer(IJavaScriptEngine engine, BaristaContext context, IBaristaValueFactory valueFactory, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueFactory, valueHandle)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.ArrayBuffer; }
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
