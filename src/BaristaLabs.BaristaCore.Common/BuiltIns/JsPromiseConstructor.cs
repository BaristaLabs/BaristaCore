namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    /// <summary>
    /// Represents the built-in Promise Object.
    /// </summary>
    public class JsPromiseConstructor : JsObject
    {
        public JsPromiseConstructor(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {

        }

        public JsPromiseConstructor All(params JsValue[] values)
        {
            if(values == null)
                throw new ArgumentNullException(nameof(values));

            var arr = ValueFactory.CreateArray(0);
            foreach (var val in values)
            {
                arr.Push(val);
            }

            var fnAll = GetProperty<JsFunction>("all");
            var result = fnAll.Call<JsPromiseConstructor>(this, arr);
            Context.CurrentScope.ResolvePendingPromises();
            return result;
        }

        public JsPromiseConstructor Race(params JsValue[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var arr = ValueFactory.CreateArray(0);
            foreach(var val in values)
            {
                arr.Push(val);
            }
            var fnRace = GetProperty<JsFunction>("race");
            var result = fnRace.Call<JsPromiseConstructor>(this, arr);
            Context.CurrentScope.ResolvePendingPromises();
            return result;
        }

        public JsPromiseConstructor Reject(JsValue reason)
        {
            if (reason == null)
                throw new ArgumentNullException(nameof(reason));

            var fnReject = GetProperty<JsFunction>("reject");
            var result = fnReject.Call<JsPromiseConstructor>(this, reason);
            Context.CurrentScope.ResolvePendingPromises();
            return result;
        }

        public JsPromiseConstructor Resolve(JsValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var fnResolve = GetProperty<JsFunction>("resolve");
            var result = fnResolve.Call<JsPromiseConstructor>(this, value);
            Context.CurrentScope.ResolvePendingPromises();
            return result;
        }

        /// <summary>
        /// Unwraps the specified promise.
        /// </summary>
        /// <param name="promise"></param>
        /// <returns></returns>
        public JsValue Wait(JsObject promise)
        {
            var promiseResultHandle = Engine.JsGetPromiseResult(promise.Handle);
            var state = Engine.JsGetPromiseState(promise.Handle);

            if (state == JavaScriptPromiseState.Rejected)
            {
                throw new JsScriptException(JsErrorCode.ScriptException, promiseResultHandle);
            }

            var result = Context.CreateValue(promiseResultHandle);
            return result;
        }

        /// <summary>
        /// Unwraps the specified promise.
        /// </summary>
        /// <param name="promise"></param>
        /// <returns></returns>
        public T Wait<T>(JsObject promise)
            where T : JsValue
        {
            var promiseResultHandle = Engine.JsGetPromiseResult(promise.Handle);
            var state = Engine.JsGetPromiseState(promise.Handle);

            if (state == JavaScriptPromiseState.Rejected)
            {
                throw new JsScriptException(JsErrorCode.ScriptException, promiseResultHandle);
            }

            var result = Context.CreateValue<T>(promiseResultHandle);
            return result;
        }
    }
}
