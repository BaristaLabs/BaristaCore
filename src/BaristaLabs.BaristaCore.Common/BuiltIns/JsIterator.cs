namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System.Collections;
    using System;

    /// <summary>
    /// Represents a object that implements the JavaScript iterator protocol, encapsulating an IEnumerator
    /// </summary>
    /// <remarks>
    /// <see cref="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Iteration_protocols"/>
    /// </remarks>
    public sealed class JsIterator : JsObject
    {
        private readonly IEnumerator m_enumerator;

        public JsIterator(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle, IEnumerator enumerator)
            : base(engine, context, valueHandle)
        {
            m_enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));

            var fnNext = context.CreateFunction(new Func<JsObject, JsObject>((thisObj) =>
            {
                return Next();
            }));

            SetProperty("next", fnNext);
        }

        public JsObject Next()
        {
            var isDone = !m_enumerator.MoveNext();
            JsValue value = Context.Undefined;

            if (!isDone)
            {
                if (Context.Converter.TryFromObject(Context, m_enumerator.Current, out JsValue currentValue))
                {
                    value = currentValue;
                }
            }

            var resultObj = Context.CreateObject();
            resultObj.SetProperty("done", isDone ? Context.True : Context.False);
            resultObj.SetProperty("value", value);

            return resultObj;
        }
    }
}
