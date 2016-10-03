namespace BaristaLabs.BaristaCore.JavaScript
{
    using SafeHandles;
    using System;
    using System.IO;

    public sealed class JavaScriptArrayBuffer : JavaScriptObject
    {
        private Lazy<uint> m_length;

        internal JavaScriptArrayBuffer(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptEngine engine) :
            base(handle, type, engine)
        {
            m_length = new Lazy<uint>(GetLength);
        }

        private uint GetLength()
        {
            var eng = GetEngine();
            IntPtr buffer;
            uint len;
            Errors.ThrowIfIs(m_api.JsGetArrayBufferStorage(m_handle, out buffer, out len));

            return len;
        }

        public uint ByteLength
        {
            get
            {
                return m_length.Value;
            }
        }

        public unsafe Stream GetUnderlyingMemory()
        {
            var eng = GetEngine();
            IntPtr buffer;
            uint len;
            Errors.ThrowIfIs(m_api.JsGetArrayBufferStorage(m_handle, out buffer, out len));

            return new UnmanagedMemoryStream((byte*)buffer.ToPointer(), len);
        }

        internal unsafe Tuple<IntPtr, uint> GetUnderlyingMemoryInfo()
        {
            var eng = GetEngine();
            IntPtr buffer;
            uint len;
            Errors.ThrowIfIs(m_api.JsGetArrayBufferStorage(m_handle, out buffer, out len));

            return Tuple.Create(buffer, len);
        }
    }
}
