namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;
    using System.Linq;

    /// <summary>
    /// Contains the methods of the object constructor
    /// </summary>
    public class JsObjectConstructor : JsObject
    {
        public JsObjectConstructor(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
        }

        /// <summary>
        /// Has a value of 1.
        /// </summary>
        public int Length
        {
            get
            {
                var pLength = GetProperty<JsNumber>("length");
                return pLength.ToInt32();
            }
        }

        /// <summary>
        /// Copies the values of all enumerable own properties from one or more source objects to a target object.
        /// </summary>
        /// <remarks>
        /// <see cref="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Object/assign"/>
        /// </remarks>
        /// <param name="target">The target object.</param>
        /// <param name="sourceObjects">The source object(s).</param>
        /// <returns>The target object.</returns>
        public JsObject Assign(JsObject target, params JsObject[] sourceObjects)
        {
            var fnAssign = GetProperty<JsFunction>("assign");
            var args = sourceObjects.Prepend(target).ToArray();
            return fnAssign.Call<JsObject>(this, args);
        }

        /// <summary>
        /// Returns a new object with the specified prototype object and properties.
        /// </summary>
        /// <remarks>
        /// <see cref="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Object/create"/>
        /// </remarks>
        /// <param name="proto">The object which should be the prototype of the newly-created object.</param>
        /// <param name="propertiesObject">Optional. If specified and not undefined, an object whose enumerable own properties (that is, those properties defined upon itself and not enumerable properties along its prototype chain) specify property descriptors to be added to the newly-created object, with the corresponding property names. These properties correspond to the second argument of Object.defineProperties().</param>
        /// <returns>A new object with the specified prototype object and properties.</returns>
        public JsObject Create(JsObject proto, JsObject propertiesObject = null)
        {
            var fnCreate = GetProperty<JsFunction>("create");
            return fnCreate.Call<JsObject>(this, proto, propertiesObject);
        }

        /// <summary>
        /// Defines a new property directly on an object, or modifies an existing property on an object, and returns the object.
        /// </summary>
        /// <remarks>
        /// <see cref="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Object/defineProperty"/>
        /// </remarks>
        /// <param name="obj">The object on which to define the property.</param>
        /// <param name="prop">The name of the property to be defined or modified.</param>
        /// <param name="descriptor">The descriptor for the property being defined or modified.</param>
        /// <returns></returns>
        public JsObject DefineProperty(JsObject obj, JsValue prop, JsObject descriptor)
        {
            var fnDefineProperty = GetProperty<JsFunction>("defineProperty");
            return fnDefineProperty.Call<JsObject>(this, obj, prop, descriptor);
        }

        public bool DefineProperty(JsObject obj, JsSymbol symbol, JsObject descriptor)
        {
            using (var propertyIdHandle = Engine.JsGetPropertyIdFromSymbol(symbol.Handle))
            {
                return Engine.JsDefineProperty(obj.Handle, propertyIdHandle, descriptor.Handle);
            }
        }

        public JsObject DefineProperty(JsObject obj, string prop, JsPropertyDescriptor descriptor)
        {
            var objDescriptor = descriptor.GetDescriptorObject(Context);
            var objProp = Context.CreateString(prop);
            return DefineProperty(obj, objProp, objDescriptor);
        }

        public bool DefineProperty(JsObject obj, JsSymbol symbol, JsPropertyDescriptor descriptor)
        {
            var objDescriptor = descriptor.GetDescriptorObject(Context);
            return DefineProperty(obj, symbol, objDescriptor);
        }

        /// <summary>
        /// Defines new or modifies existing properties directly on an object, returning the object.
        /// </summary>
        /// <remarks>
        /// <see cref="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Object/defineProperties"/>
        /// </remarks>
        /// <param name="obj">The object on which to define or modify properties.</param>
        /// <param name="props">An object whose own enumerable properties constitute descriptors for the properties to be defined or modified.</param>
        /// <returns>The object that was passed to the function.</returns>
        public JsObject DefineProperties(JsObject obj, JsObject props)
        {
            var fnDefineProperties = GetProperty<JsFunction>("defineProperties");
            return fnDefineProperties.Call<JsObject>(this, obj, props);
        }

        /// <summary>
        /// Returns the specified object in a frozen state.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JsObject Freeze(JsObject obj)
        {
            var fnFreeze = GetProperty<JsFunction>("freeze");
            return fnFreeze.Call<JsObject>(this, obj);
        }

        /// <summary>
        /// Returns all own property descriptors of a given object.
        /// <remarks>
        /// <see cref="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Object/getOwnPropertyDescriptors"/>
        /// </remarks>
        /// </summary>
        /// <param name="obj">The object for which to get all own property descriptors.</param>
        /// <returns>An object containing all own property descriptors of an object. Might be an empty object, if there are no properties.</returns>
        public JsObject GetOwnPropertyDescriptors(JsObject obj)
        {
            var fnGetOwnPropertyDescriptors = GetProperty<JsFunction>("getOwnPropertyDescriptors");
            return fnGetOwnPropertyDescriptors.Call<JsObject>(this, obj);
        }

        /// <summary>
        /// Gets the prototype of an object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JsObject GetPrototypeOf(JsObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var protoHandle = Engine.JsGetPrototype(obj.Handle);
            return ValueFactory.CreateValue<JsObject>(protoHandle);
        }

        /// <summary>
        /// Tests whether the prototype property of a constructor appears anywhere in the prototype chain of an object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public bool InstanceOf(JsObject obj, JsFunction constructor)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(BaristaContext));

            return Engine.JsInstanceOf(obj.Handle, constructor.Handle);
        }

        /// <summary>
        /// Sets the prototype of an object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prototype"></param>
        public void SetPrototypeOf(JsObject obj, JsObject prototype)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var prototypeHandle = JavaScriptValueSafeHandle.Invalid;
            if (prototype != null)
                prototypeHandle = prototype.Handle;

            Engine.JsSetPrototype(obj.Handle, prototypeHandle);
        }
    }
}
