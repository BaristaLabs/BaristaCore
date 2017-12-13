namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a JavaScript Function
    /// </summary>
    /// <remarks>
    /// <see cref="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Functions"/>
    /// </remarks>
    public class JsFunction : JsObject
    {
        public JsFunction(IJavaScriptEngine engine, BaristaContext context, IBaristaValueService valueService, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueService, valueHandle)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.Function; }
        }

        /// <summary>
        /// Invokes the function with the specified arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public JsValue Invoke(params JsValue[] args)
        {
            var result = InvokeInternal(args);
            return ValueService.CreateValue(result);
        }

        /// <summary>
        /// Invokes the function with the specified arguments expecting a return value of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public T Invoke<T>(params JsValue[] args)
            where T : JsValue
        {
            var result = InvokeInternal(args);
            return ValueService.CreateValue<T>(result);
        }

        private JavaScriptValueSafeHandle InvokeInternal(params JsValue[] args)
        {
            var argPtrs = args.Select(a => a == null ? Context.Undefined.Handle.DangerousGetHandle() : a.Handle.DangerousGetHandle()).ToArray();

            return Engine.JsCallFunction(Handle, argPtrs, (ushort)argPtrs.Length);
        }

        private const string toStringPropertyName = "toString";

        public override string ToString()
        {
            var fnToString = GetProperty<JsFunction>(toStringPropertyName);
            var fnAsAString = fnToString.Invoke<JsString>(this);
            return fnAsAString.ToString();
        }
    }
}
