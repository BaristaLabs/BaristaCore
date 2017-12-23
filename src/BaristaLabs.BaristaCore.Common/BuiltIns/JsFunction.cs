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
        public JsFunction(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.Function; }
        }

        /// <summary>
        /// Calls the function with the specified arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public JsValue Call(JsObject thisObj = null, params JsValue[] args)
        {
            var result = CallInternal(thisObj, args);
            return ValueFactory.CreateValue(result);
        }

        /// <summary>
        /// Calls the function with the specified arguments expecting a return value of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public T Call<T>(JsObject thisObj = null, params JsValue[] args)
            where T : JsValue
        {
            var result = CallInternal(thisObj, args);
            return ValueFactory.CreateValue<T>(result);
        }

        private JavaScriptValueSafeHandle CallInternal(JsObject thisObj = null, params JsValue[] args)
        {
            //Must at least have a 'thisObject'
            if (thisObj == null)
                thisObj = Context.GlobalObject;

            var argPtrs = args
                .Select(a => a == null ? Context.Undefined.Handle.DangerousGetHandle() : a.Handle.DangerousGetHandle())
                .Prepend(thisObj.Handle.DangerousGetHandle())
                .ToArray();

            return Engine.JsCallFunction(Handle, argPtrs, (ushort)argPtrs.Length);
        }

        /// <summary>
        /// Invokes function as a constructor.
        /// </summary>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public JsObject Construct(JsObject thisObj = null, params JsValue[] args)
        {
            //Must at least have a 'thisObject'
            if (thisObj == null)
                thisObj = Context.GlobalObject;

            var argPtrs = args
                .Select(a => a == null ? Context.Undefined.Handle.DangerousGetHandle() : a.Handle.DangerousGetHandle())
                .Prepend(thisObj.Handle.DangerousGetHandle())
                .ToArray();

            var newObjHandle = Engine.JsConstructObject(Handle, argPtrs, (ushort)argPtrs.Length);
            return ValueFactory.CreateValue<JsObject>(newObjHandle);
        }

        private const string toStringPropertyName = "toString";

        public override string ToString()
        {
            var fnToString = GetProperty<JsFunction>(toStringPropertyName);
            var fnAsAString = fnToString.Call<JsString>(this);
            return fnAsAString.ToString();
        }
    }

    /// <summary>
    /// Represents a .net delegate that can be invoked as a JsFunction
    /// </summary>
    public sealed class JsNativeFunction : JsFunction
    {
        private readonly GCHandle m_delegateHandle;

        internal JsNativeFunction(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle, Delegate fnDelegate)
            : base(engine, context, valueHandle)
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
