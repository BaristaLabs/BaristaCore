namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a external array buffer.
    /// </summary>
    public class JavaScriptExternalArrayBuffer : JavaScriptArrayBuffer
    {
        protected JavaScriptExternalArrayBuffer(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle value)
            : base(engine, context, value)
        {
        }
    }

    /// <summary>
    /// Represents an external array buffer that is backed by memory managed by the application.
    /// </summary>
    public sealed class JavaScriptManagedExternalArrayBuffer : JavaScriptExternalArrayBuffer
    {
        private IntPtr m_bufferHandle;
        private readonly Action<IntPtr> m_releaseBufferHandle;

        internal JavaScriptManagedExternalArrayBuffer(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle value, IntPtr bufferHandle, Action<IntPtr> releaseBufferHandle)
            : base(engine, context, value)
        {
            if (bufferHandle == default(IntPtr) || bufferHandle == null)
                throw new ArgumentNullException(nameof(bufferHandle));

            if (releaseBufferHandle == null)
                throw new ArgumentNullException(nameof(releaseBufferHandle));

            m_bufferHandle = bufferHandle;
            m_releaseBufferHandle = releaseBufferHandle;
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
