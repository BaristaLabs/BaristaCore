namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using BaristaLabs.BaristaCore.JavaScript.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents the built-in Promise Object.
    /// </summary>
    public class JsPromise : JsObject
    {
        public JsPromise(IJavaScriptEngine engine, BaristaContext context, IBaristaValueFactory valueFactory, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueFactory, valueHandle)
        {

        }

        public JsPromise All(JsValue iterable)
        {
            if (iterable == null)
                throw new ArgumentNullException(nameof(iterable));

            var fnAll = GetProperty<JsFunction>("all");
            return fnAll.Call<JsPromise>(fnAll, iterable);
        }

        public JsPromise Race(params JsValue[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var arr = ValueFactory.CreateArray(0);
            foreach(var val in values)
            {
                arr.Push(val);
            }
            var fnRace = GetProperty<JsFunction>("race");
            return fnRace.Call<JsPromise>(new JsValue[] { this, arr });
        }

        public JsPromise Reject(JsValue reason)
        {
            if (reason == null)
                throw new ArgumentNullException(nameof(reason));

            var fnReject = GetProperty<JsFunction>("reject");
            return fnReject.Call<JsPromise>(fnReject, reason);
        }

        public JsPromise Resolve(JsValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var fnResolve = GetProperty<JsFunction>("resolve");
            return fnResolve.Call<JsPromise>(fnResolve, value);
        }

        /// <summary>
        /// Unwraps the specified promise.
        /// </summary>
        /// <param name="promise"></param>
        /// <returns></returns>
        public JsValue Wait(JsObject promise)
        {
            var globalName = WaitInternal(promise);
            return Context.GlobalObject.GetProperty(globalName);
        }

        /// <summary>
        /// Unwraps the specified promise.
        /// </summary>
        /// <param name="promise"></param>
        /// <returns></returns>
        public T Wait<T>(JsObject promise)
            where T : JsValue
        {
            var globalName = WaitInternal(promise);
            return Context.GlobalObject.GetProperty<T>(globalName);
        }

        private string WaitInternal(JsObject promise)
        {
            const string waitScript = @"(async () => { this.$EXPORTS = await this.$PROMISE; })();";
            Context.GlobalObject["$PROMISE"] = promise;

            var promiseTaskQueue = new PromiseTaskQueue();
            JavaScriptPromiseContinuationCallback promiseContinuationCallback = (IntPtr taskHandle, IntPtr callbackState) =>
            {
                var task = new JavaScriptValueSafeHandle(taskHandle);
                var promiseTask = ValueFactory.CreateValue<JsFunction>(task);
                promiseTaskQueue.Enqueue(promiseTask);
            };
            var promiseContinuationCallbackDelegateHandle = GCHandle.Alloc(promiseContinuationCallback);
            Engine.JsSetPromiseContinuationCallback(promiseContinuationCallback, IntPtr.Zero);

            try
            {
                var resultHandle = Engine.JsRunScript(waitScript, sourceUrl: "[eval wait]");

                //Evaluate any pending promises.
                var args = new IntPtr[] { Context.Undefined.Handle.DangerousGetHandle() };
                while (promiseTaskQueue.Count > 0)
                {
                    var promiseTask = promiseTaskQueue.Dequeue();
                    try
                    {
                        var promiseResult = Engine.JsCallFunction(promiseTask.Handle, args, (ushort)args.Length);
                        promiseResult.Dispose();
                    }
                    finally
                    {
                        promiseTask.Dispose();
                    }
                }
                return "$EXPORTS";
            }
            finally
            {
                Engine.JsSetPromiseContinuationCallback(null, IntPtr.Zero);
                promiseContinuationCallbackDelegateHandle.Free();
            }
        }
    }
}
