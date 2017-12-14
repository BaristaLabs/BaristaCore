namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;

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
            var argPtrs = args
                .Select(a => a == null ? Context.Undefined.Handle.DangerousGetHandle() : a.Handle.DangerousGetHandle())
                .ToArray();

            if (argPtrs.Length == 0)
                argPtrs = new IntPtr[] { Handle.DangerousGetHandle() };

            try
            {
                return Engine.JsCallFunction(Handle, argPtrs, (ushort)argPtrs.Length);
            }
            catch(JavaScriptException jsEx)
            {
                //TODO: This logic needs to be reused... somewhere.

                switch (jsEx.ErrorCode)
                {
                    case JavaScriptErrorCode.ScriptException:
                        var exceptionHandle = Engine.JsGetAndClearException();
                        JsError jsError = ValueService.CreateValue<JsError>(exceptionHandle);
                        throw new BaristaScriptException(jsError.Message);
                    default:
                        throw new NotImplementedException("Exception type has not been implemented.");
                }
            }
        }

        private const string toStringPropertyName = "toString";

        public override string ToString()
        {
            var fnToString = GetProperty<JsFunction>(toStringPropertyName);
            var fnAsAString = fnToString.Invoke<JsString>(this);
            return fnAsAString.ToString();
        }
    }

    /// <summary>
    /// Represents a .net delegate that can be invoked as a JsFunction
    /// </summary>
    public sealed class JsNativeFunction : JsFunction
    {
        private readonly GCHandle m_delegateHandle;

        internal JsNativeFunction(IJavaScriptEngine engine, BaristaContext context, IBaristaValueService valueService, JavaScriptValueSafeHandle valueHandle, Delegate fnDelegate)
            : base(engine, context, valueService, valueHandle)
        {
            m_delegateHandle = GCHandle.Alloc(fnDelegate);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            m_delegateHandle.Free();
        }
    }

}
