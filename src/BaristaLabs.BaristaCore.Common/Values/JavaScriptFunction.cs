namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Linq;
    using System.Text;

    public sealed class JavaScriptFunction : JavaScriptObject
    {
        public JavaScriptFunction(IJavaScriptEngine engine, BaristaContext context, IBaristaValueFactory valueFactory, JavaScriptValueSafeHandle value)
            : base(engine, context, valueFactory, value)
        {
        }

        public JavaScriptValue Invoke(params JavaScriptValue[] args)
        {
            var argPtrs = args.Select(a => a.Handle.DangerousGetHandle()).Prepend(Handle.DangerousGetHandle()).ToArray();

            var result = Engine.JsCallFunction(Handle, argPtrs, (ushort)argPtrs.Length);
            return ValueFactory.CreateValue(Context, result);
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
