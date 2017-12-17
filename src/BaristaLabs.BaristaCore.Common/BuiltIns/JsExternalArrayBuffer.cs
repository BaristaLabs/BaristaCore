namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    /// <summary>
    /// Represents a external array buffer.
    /// </summary>
    public class JsExternalArrayBuffer : JsArrayBuffer
    {
        protected JsExternalArrayBuffer(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
        }
    }

    /// <summary>
    /// Represents an external array buffer that is backed by memory managed by the application.
    /// </summary>
    public sealed class JsManagedExternalArrayBuffer : JsExternalArrayBuffer
    {
        private IntPtr m_bufferHandle;
        private readonly Action<IntPtr> m_releaseBufferHandle;

        public JsManagedExternalArrayBuffer(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle, IntPtr bufferHandle, Action<IntPtr> releaseBufferHandle)
            : base(engine, context, valueHandle)
        {
            if (bufferHandle == default(IntPtr) || bufferHandle == null)
                throw new ArgumentNullException(nameof(bufferHandle));
            m_bufferHandle = bufferHandle;
            m_releaseBufferHandle = releaseBufferHandle ?? throw new ArgumentNullException(nameof(releaseBufferHandle));
        }

        /// <summary>
        /// Gets the underlying GCHandle.
        /// </summary>
        public IntPtr BufferHandle
        {
            get { return m_bufferHandle; }
        }

        protected override void Dispose(bool disposing)
        {
            if (m_bufferHandle != default(IntPtr))
            {
                m_releaseBufferHandle?.Invoke(m_bufferHandle);
                m_bufferHandle = default(IntPtr);
            }

            base.Dispose(disposing);
        }
    }
}
