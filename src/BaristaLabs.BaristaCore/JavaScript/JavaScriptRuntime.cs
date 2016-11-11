namespace BaristaLabs.BaristaCore.JavaScript
{
    using Callbacks;
    using Interfaces;
    using SafeHandles;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public sealed class JavaScriptRuntime : IDisposable
    {
        private JavaScriptRuntimeSettings m_settings;
        private JavaScriptRuntimeSafeHandle m_handle;
        private IChakraApi m_api = ChakraApi.Instance;
        private List<WeakReference<JavaScriptContext>> m_childEngines;

        public JavaScriptRuntime() : this(null)
        {
        }

        public JavaScriptRuntime(JavaScriptRuntimeSettings settings = null)
        {
            if (settings == null)
                settings = new JavaScriptRuntimeSettings();

            m_childEngines = new List<WeakReference<JavaScriptContext>>();
            m_settings = settings;
            var attrs = settings.GetRuntimeAttributes();

            var errorCode = m_api.JsCreateRuntime(attrs, null, out m_handle);
            if (errorCode != JsErrorCode.JsNoError)
                Errors.ThrowFor(errorCode);

            settings.Used = true;

            GCHandle handle = GCHandle.Alloc(this, GCHandleType.Weak);
            errorCode = m_api.JsSetRuntimeMemoryAllocationCallback(m_handle, GCHandle.ToIntPtr(handle), MemoryCallbackThunk);
            if (errorCode != JsErrorCode.JsNoError)
                Errors.ThrowFor(errorCode);
        }

        public void CollectGarbage()
        {
            if (m_handle == null)
                throw new ObjectDisposedException(nameof(JavaScriptRuntime));

            var error = m_api.JsCollectGarbage(m_handle);
            Errors.ThrowIfIs(error);
        }

        public JavaScriptContext CreateContext()
        {
            if (m_handle == null)
                throw new ObjectDisposedException(nameof(JavaScriptRuntime));

            JavaScriptContextSafeHandle context;
            var error = m_api.JsCreateContext(m_handle, out context);
            Errors.ThrowIfIs(error);

            return new JavaScriptContext(context, this, m_api);
        }

        public void EnableExecution()
        {
            if (m_handle == null)
                throw new ObjectDisposedException(nameof(JavaScriptRuntime));

            var error = m_api.JsEnableRuntimeExecution(m_handle);
            Errors.ThrowIfIs(error);
        }

        public void DisableExecution()
        {
            if (m_handle == null)
                throw new ObjectDisposedException(nameof(JavaScriptRuntime));

            var error = m_api.JsDisableRuntimeExecution(m_handle);
            Errors.ThrowIfIs(error);
        }

        public ulong RuntimeMemoryUsage
        {
            get
            {
                if (m_handle == null)
                    throw new ObjectDisposedException(nameof(JavaScriptRuntime));

                UIntPtr result;
                var error = m_api.JsGetRuntimeMemoryUsage(m_handle, out result);
                Errors.ThrowIfIs(error);

                return result.ToUInt64();
            }
        }

        public bool IsExecutionEnabled
        {
            get
            {
                if (m_handle == null)
                    throw new ObjectDisposedException(nameof(JavaScriptRuntime));

                bool result;
                var error = m_api.JsIsRuntimeExecutionDisabled(m_handle, out result);
                Errors.ThrowIfIs(error);

                return !result;
            }
        }

        public event EventHandler<JavaScriptMemoryEventArgs> MemoryChanging;
        private void OnMemoryChanging(JavaScriptMemoryEventArgs args)
        {
            MemoryChanging?.Invoke(this, args);
        }

        public JavaScriptRuntimeSettings Settings
        {
            get { return m_settings; }
        }

        #region Disposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_api.JsSetCurrentContext(JavaScriptContextSafeHandle.Invalid);
                m_api.JsSetRuntimeMemoryAllocationCallback(m_handle, IntPtr.Zero, null);
                if (m_childEngines != null)
                {
                    foreach (var engineRef in m_childEngines)
                    {
                        JavaScriptContext engine;
                        if (engineRef.TryGetTarget(out engine))
                        {
                            engine.Dispose();
                        }
                    }
                    m_childEngines = null;
                }

                if (m_handle != null && !m_handle.IsClosed)
                {
                    m_handle.Dispose();
                    m_handle = null;
                }
            }
        }

        ~JavaScriptRuntime()
        {
            Dispose(false);
        }
        #endregion

        #region Memory callback implementation
        static JavaScriptRuntime()
        {
            MemoryCallbackThunkDelegate = MemoryCallbackThunk;
        }

        private static bool MemoryCallbackThunk(IntPtr callbackState, JavaScriptMemoryEventType allocationEvent, UIntPtr allocationSize)
        {
            GCHandle handle = GCHandle.FromIntPtr(callbackState);
            JavaScriptRuntime runtime = handle.Target as JavaScriptRuntime;
            if (runtime == null)
            {
                Debug.Assert(false, "Runtime has been freed.");
                return false;
            }

            var args = new JavaScriptMemoryEventArgs(allocationSize, allocationEvent);
            runtime.OnMemoryChanging(args);

            if (args.IsCancelable && args.Cancel)
                return false;

            return true;
        }

        private static IntPtr MemoryCallbackThunkPtr;
        private static JavaScriptMemoryAllocationCallback MemoryCallbackThunkDelegate;
        #endregion
    }
}
