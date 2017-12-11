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
        public JsFunction(IJavaScriptEngine engine, BaristaContext context, IBaristaValueFactory valueFactory, JavaScriptValueSafeHandle value)
            : base(engine, context, valueFactory, value)
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
            return ValueFactory.CreateValue(result);
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
            return ValueFactory.CreateValue<T>(result);
        }

        private JavaScriptValueSafeHandle InvokeInternal(params JsValue[] args)
        {
            var argPtrs = args.Select(a => a.Handle.DangerousGetHandle()).ToArray();

            return Engine.JsCallFunction(Handle, argPtrs, (ushort)argPtrs.Length);
        }

        private const string toStringPropertyName = "toString";

        public override string ToString()
        {
            var toStringFunctionPropertyIdHandle = Engine.JsCreatePropertyId(toStringPropertyName, (ulong)toStringPropertyName.Length);
            var toStringFnHandle = Engine.JsGetProperty(Handle, toStringFunctionPropertyIdHandle);

            var resultHandle = Engine.JsCallFunction(toStringFnHandle, new IntPtr[] { Handle.DangerousGetHandle() }, 1);

            var size = Engine.JsCopyString(resultHandle, null, 0);
            if ((int)size > int.MaxValue)
                throw new OutOfMemoryException("Exceeded maximum string length.");

            byte[] result = new byte[(int)size];
            var written = Engine.JsCopyString(resultHandle, result, size);
            return Encoding.UTF8.GetString(result, 0, result.Length);
        }
    }
}
