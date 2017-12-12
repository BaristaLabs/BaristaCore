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
    public class JsTypedArray : JsObject
    {
        private Lazy<JavaScriptTypedArrayInfo> m_arrayInfo;

        public JsTypedArray(IJavaScriptEngine engine, BaristaContext context, BaristaValueService valueService, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueService, valueHandle)
        {
            m_arrayInfo = new Lazy<JavaScriptTypedArrayInfo>(GetTypedArrayInfo);
        }

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
                dynamic result = GetPropertyByName<JsNumber>("length");
                return (uint)result;
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

        private JavaScriptTypedArrayInfo GetTypedArrayInfo()
        {
            var result = new JavaScriptTypedArrayInfo
            {
                Type = Engine.JsGetTypedArrayInfo(Handle, out JavaScriptValueSafeHandle arrayBufferHandle, out uint byteOffset, out uint byteLength),
                ByteOffset = byteOffset,
                ByteLength = byteLength,
                Buffer = ValueService.CreateValue<JsArrayBuffer>(arrayBufferHandle)
            };

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
