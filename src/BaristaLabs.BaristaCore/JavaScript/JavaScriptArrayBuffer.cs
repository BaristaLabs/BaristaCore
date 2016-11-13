namespace BaristaLabs.BaristaCore.JavaScript
{
    using SafeHandles;
    using System;
    using System.IO;

    public sealed class JavaScriptArrayBuffer : JavaScriptObject
    {
        private Lazy<uint> m_length;

        internal JavaScriptArrayBuffer(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptContext context) :
            base(handle, type, context)
        {
            m_length = new Lazy<uint>(GetLength);
        }

        private uint GetLength()
        {
            var eng = GetContext();
            byte[] buffer;
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

        public Stream GetUnderlyingMemory()
        {
            var eng = GetContext();
            byte[] buffer;
            uint len;
            Errors.ThrowIfIs(m_api.JsGetArrayBufferStorage(m_handle, out buffer, out len));
            if (len > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum buffer length.");

            return new MemoryStream(buffer, 0, (int)len);
        }

        internal Tuple<byte[], uint> GetUnderlyingMemoryInfo()
        {
            var eng = GetContext();
            byte[] buffer;
            uint len;
            Errors.ThrowIfIs(m_api.JsGetArrayBufferStorage(m_handle, out buffer, out len));

            return Tuple.Create(buffer, len);
        }
    }
}
