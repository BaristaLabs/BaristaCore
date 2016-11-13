namespace BaristaLabs.BaristaCore.JavaScript
{
    using SafeHandles;
    using System;
    using System.Diagnostics;
    using System.IO;

    public sealed class JavaScriptTypedArray : JavaScriptObject
    {
        private Lazy<JavaScriptTypedArrayType> m_arrayType;
        internal JavaScriptTypedArray(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptContext context) :
            base(handle, type, context)
        {
            m_arrayType = new Lazy<JavaScriptTypedArrayType>(GetArrayType);
        }

        public JavaScriptArrayBuffer Buffer
        {
            get
            {
                return GetPropertyByName("buffer") as JavaScriptArrayBuffer;
            }
        }

        public uint ByteLength
        {
            get
            {
                var eng = GetContext();
                var val = GetPropertyByName("byteLength");
                return (uint)eng.Converter.ToDouble(val);
            }
        }

        public uint ByteOffset
        {
            get
            {
                var eng = GetContext();
                var val = GetPropertyByName("byteOffset");
                return (uint)eng.Converter.ToDouble(val);
            }
        }

        public Stream GetUnderlyingMemory()
        {
            var buf = Buffer;
            Debug.Assert(buf != null);

            var mem = buf.GetUnderlyingMemoryInfo();
            if (mem.Item2 > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum buffer length.");
            
            return new MemoryStream(mem.Item1, 0, (int)mem.Item2);
        }

        public uint Length
        {
            get
            {
                var eng = GetContext();
                var val = GetPropertyByName("length");
                return (uint)eng.Converter.ToDouble(val);
            }
        }

        public JavaScriptTypedArrayType ArrayType
        {
            get
            {
                return m_arrayType.Value;
            }
        }

        private JavaScriptTypedArrayType GetArrayType()
        {
            GetContext();
            byte[] buf;
            uint len;
            JavaScriptTypedArrayType type;
            int elemSize;

            Errors.ThrowIfIs(m_api.JsGetTypedArrayStorage(m_handle, out buf, out len, out type, out elemSize));

            return type;
        }
    }
}
