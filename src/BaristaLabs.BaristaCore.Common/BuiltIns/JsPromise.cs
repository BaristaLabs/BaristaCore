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
            var result = fnRace.Call<JsPromise>(this, arr);
            Context.CurrentScope.ResolvePendingPromises();
            return result;
        }

        public JsPromise Reject(JsValue reason)
        {
            if (reason == null)
                throw new ArgumentNullException(nameof(reason));

            var fnReject = GetProperty<JsFunction>("reject");
            return fnReject.Call<JsPromise>(this, reason);
        }

        public JsPromise Resolve(JsValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var fnResolve = GetProperty<JsFunction>("resolve");
            return fnResolve.Call<JsPromise>(this, value);
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
            const string waitScript = @"
(async () => await Promise.resolve(this.$PROMISE))().then((result) => { this.$EXPORTS = result }, (reject) => { this.$ERROR = reject; });";
            
            Context.GlobalObject["$PROMISE"] = promise;
            Context.GlobalObject.DeleteProperty("$ERROR");
            var res = Engine.JsRunScript(waitScript, sourceUrl: "[eval wait]");
            Context.CurrentScope.ResolvePendingPromises();

            if (Context.GlobalObject.HasOwnProperty("$ERROR"))
            {
                throw new BaristaScriptException(Context.GlobalObject.GetProperty<JsObject>("$ERROR"));
            }

            return "$EXPORTS";
        }
    }
}
