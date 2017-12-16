namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Runtime.InteropServices;

    public sealed class BaristaExecutionScope : IDisposable
    {
        private readonly BaristaContext m_context;
        private readonly IPromiseTaskQueue m_promiseTaskQueue;
        private readonly GCHandle m_promiseContinuationCallbackDelegateHandle;
        private readonly Action m_release;

        internal BaristaExecutionScope(BaristaContext context, IPromiseTaskQueue taskQueue, Action release)
        {
            m_context = context;
            m_promiseTaskQueue = taskQueue;
            m_release = release;

            //Clear and set the task queue if specified
            m_promiseContinuationCallbackDelegateHandle = default(GCHandle);
            if (m_promiseTaskQueue != null)
            {
                m_promiseTaskQueue.Clear();
                JavaScriptPromiseContinuationCallback promiseContinuationCallback = (IntPtr taskHandle, IntPtr callbackState) =>
                {
                    PromiseContinuationCallback(taskHandle, callbackState);
                };
                m_promiseContinuationCallbackDelegateHandle = GCHandle.Alloc(promiseContinuationCallback);
                m_context.Engine.JsSetPromiseContinuationCallback(promiseContinuationCallback, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Resolves any pending promises queued in the execution scope.
        /// </summary>
        /// <remarks>
        /// Pending promises are resolved when the execution scope is 
        /// disposed, but can be invoked directly to resolve pending values.
        /// </remarks>
        public void ResolvePendingPromises()
        {
            if (m_promiseTaskQueue == null || m_promiseTaskQueue.Count <= 0)
                return;

            var args = new IntPtr[] { m_context.Undefined.Handle.DangerousGetHandle() };
            while (m_promiseTaskQueue.Count > 0)
            {
                var promise = m_promiseTaskQueue.Dequeue();
                try
                {
                    m_context.Engine.JsCallFunction(promise.Handle, args, (ushort)args.Length);
                }
                finally
                {
                    promise.Dispose();
                }
            }
        }

        private void PromiseContinuationCallback(IntPtr taskHandle, IntPtr callbackState)
        {
            if (m_promiseTaskQueue == null)
            {
                return;
            }

            var task = new JavaScriptValueSafeHandle(taskHandle);
            var promise = m_context.ValueFactory.CreateValue<JsFunction>(task);
            m_promiseTaskQueue.Enqueue(promise);
        }

        #region Disposable
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                ResolvePendingPromises();
            }

            //Unset the Promise callback.
            if (m_promiseTaskQueue != null)
            {
                m_context.Engine.JsSetPromiseContinuationCallback(null, IntPtr.Zero);
            }

            //Free the delegate.
            if (m_promiseContinuationCallbackDelegateHandle != default(GCHandle) && m_promiseContinuationCallbackDelegateHandle.IsAllocated)
            {
                m_promiseContinuationCallbackDelegateHandle.Free();
            }

            m_release();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BaristaExecutionScope()
        {
            Dispose(false);
        }

        #endregion
    }
}
