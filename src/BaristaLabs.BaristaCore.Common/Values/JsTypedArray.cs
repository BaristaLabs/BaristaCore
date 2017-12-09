namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    /// <summary>
    /// Represents a TypedArray
    /// </summary>
    /// <remarks>
    /// See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/TypedArray
    /// </remarks>
    public sealed class JsTypedArray : JsObject
    {
        private Lazy<JavaScriptTypedArrayInfo> m_arrayInfo;

        #region Properties

        public JsArrayBuffer Buffer
        {
            get
            {
                return m_arrayInfo.Value.Buffer;
            }
        }

        public uint ByteLength
        {
            get
            {
                return m_arrayInfo.Value.ByteLength;
            }
        }

        public uint ByteOffset
        {
            get
            {
                return m_arrayInfo.Value.ByteOffset;
            }
        }

        public uint Length
        {
            get
            {
                return GetPropertyByName<uint>("length");
            }
        }

        public JavaScriptTypedArrayType ArrayType
        {
            get
            {
                return m_arrayInfo.Value.Type;
            }
        }
        #endregion

        public JsTypedArray(IJavaScriptEngine engine, BaristaContext context, BaristaValueFactory valueFactory, JavaScriptValueSafeHandle value)
            : base(engine, context, valueFactory, value)
        {
            m_arrayInfo = new Lazy<JavaScriptTypedArrayInfo>(GetTypedArrayInfo);
        }

        private JavaScriptTypedArrayInfo GetTypedArrayInfo()
        {
            var result = new JavaScriptTypedArrayInfo();

            uint byteOffset, byteLength;
            JavaScriptValueSafeHandle arrayBufferHandle;

            result.Type = Engine.JsGetTypedArrayInfo(Handle, out arrayBufferHandle, out byteOffset, out byteLength);
            result.ByteOffset = byteOffset;
            result.ByteLength = byteLength;
            result.Buffer = new JsArrayBuffer(Engine, Context, arrayBufferHandle);

            return result;
        }

        private struct JavaScriptTypedArrayInfo
        {
            public uint ByteOffset
            {
                get;
                set;
            }

            public uint ByteLength
            {
                get;
                set;
            }

            public JsArrayBuffer Buffer
            {
                get;
                set;
            }

            public JavaScriptTypedArrayType Type
            {
                get;
                set;
            }
        }
    }
}
