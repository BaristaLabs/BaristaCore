namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Runtime.InteropServices;

    public class JsExternalObject : JsValue
    {
        private readonly GCHandle m_objHandle;

        public JsExternalObject(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle, GCHandle objHandle)
            : base(engine, context, valueHandle)
        {
            m_objHandle = objHandle;
        }

        public object Target
        {
            get { return m_objHandle.Target; }
        }

        public override JsValueType Type
        {
            get { return JsValueType.Object; }
        }

        /// <summary>
        /// Returns a value that indicates if the current object has external data associated with it.
        /// </summary>
        /// <returns></returns>
        public bool HasExternalData()
        {
            return Engine.JsHasExternalData(Handle);
        }

        /// <summary>
        /// Retrieves the external data associated with the current object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetExternalData<T>()
            where T : class
        {
            var ptrData = Engine.JsGetExternalData(Handle);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the external data associated with the current object.
        /// </summary>
        /// <param name="externalData"></param>
        /// <returns></returns>
        public void SetExternalData<T>(T externalData)
            where T : class
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (m_objHandle.IsAllocated)
                m_objHandle.Free();

            base.Dispose(disposing);
        }
    }
}
