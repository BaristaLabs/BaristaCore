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
                    try
                    {
                        PromiseContinuationCallback(taskHandle, callbackState);
                    }
                    catch
                    {
                        //Do Nothing.
                    }
                };
                m_promiseContinuationCallbackDelegateHandle = GCHandle.Alloc(promiseContinuationCallback);
                m_context.Engine.JsSetPromiseContinuationCallback(promiseContinuationCallback, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Returns a value that indicates if the current context is in an exception state.
        /// </summary>
        /// <returns></returns>
        public bool HasException()
        {
            return m_context.Engine.JsHasException();
        }

        /// <summary>
        /// Returns the exception that caused the runtime of the current context to be in the exception state and resets the exception state for the runtime.
        /// </summary>
        /// <returns></returns>
        public JsValue GetAndClearException()
        {
            var valueHandle = m_context.Engine.JsGetAndClearException();
            return m_context.CreateValue(valueHandle);
        }

        /// <summary>
        /// Returns metadata relating to the exception that caused the runtime of the current context to be in the exception state and resets the exception state for the runtime.
        /// </summary>
        /// <returns></returns>
        public JsValue GetAndClearExceptionWithMetadata()
        {
            var valueHandle = m_context.Engine.JsGetAndClearExceptionWithMetadata();
            return m_context.CreateValue(valueHandle);
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
                if (promise == null || promise.IsDisposed)
                {
                    continue;
                }
                try
                {
                    //All results from this are undefined.
                    m_context.Engine.JsCallFunction(promise.Handle, args, (ushort)args.Length);
                }
                finally
                {
                    m_context.Engine.JsRelease(promise.Handle);
                    promise.Dispose();
                }
            }
        }


        /// <summary>
        /// Sets the runtime of the current context to an exception state.
        /// </summary>
        /// <param name="error"></param>
        public void SetException(JsError error)
        {
            m_context.Engine.JsSetException(error.Handle);
        }

        private void PromiseContinuationCallback(IntPtr taskHandle, IntPtr callbackState)
        {
            if (m_promiseTaskQueue == null)
            {
                return;
            }
            var task = new JavaScriptValueSafeHandle(taskHandle);
            //Ensure that the object doesn't get disposed as we're processing items in the queue.
            m_context.Engine.JsAddRef(task);
            var promise = m_context.CreateValue<JsFunction>(task);
            m_promiseTaskQueue.Enqueue(promise);
        }

        #region Disposable
        private bool m_isDisposed = false;

        private void Dispose(bool disposing)
        {
            if (!m_isDisposed)
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
                m_isDisposed = true;
            }
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
