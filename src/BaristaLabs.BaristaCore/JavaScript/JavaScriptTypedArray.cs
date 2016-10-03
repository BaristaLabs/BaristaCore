namespace BaristaLabs.BaristaCore.JavaScript
{
    using SafeHandles;
    using System;
    using System.Diagnostics;
    using System.IO;

    public sealed class JavaScriptTypedArray : JavaScriptObject
    {
        private Lazy<JavaScriptTypedArrayType> m_arrayType;
        internal JavaScriptTypedArray(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptEngine engine) :
            base(handle, type, engine)
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
                var eng = GetEngine();
                var val = GetPropertyByName("byteLength");
                return (uint)eng.Converter.ToDouble(val);
            }
        }

        public uint ByteOffset
        {
            get
            {
                var eng = GetEngine();
                var val = GetPropertyByName("byteOffset");
                return (uint)eng.Converter.ToDouble(val);
            }
        }

        public unsafe Stream GetUnderlyingMemory()
        {
            var buf = Buffer;
            Debug.Assert(buf != null);

            var mem = buf.GetUnderlyingMemoryInfo();
            byte* pMem = (byte*)mem.Item1.ToPointer();

            return new UnmanagedMemoryStream(pMem + ByteOffset, ByteLength);
        }

        public uint Length
        {
            get
            {
                var eng = GetEngine();
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
            GetEngine();
            IntPtr buf;
            uint len;
            JavaScriptTypedArrayType type;
            int elemSize;

            Errors.ThrowIfIs(m_api.JsGetTypedArrayStorage(m_handle, out buf, out len, out type, out elemSize));

            return type;
        }
    }
}
