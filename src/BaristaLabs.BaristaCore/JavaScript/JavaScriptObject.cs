namespace BaristaLabs.BaristaCore.JavaScript
{
    using Interop;
    using Interop.Interfaces;
    using Interop.SafeHandles;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;

    public class JavaScriptObject : JavaScriptValue
    {
        internal JavaScriptObject(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptContext context) :
            base(handle, type, context)
        {

        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public JavaScriptArray Keys
        {
            get
            {
                var eng = GetContext();
                var fn = GetObjectBuiltinFunction("keys", "Object.keys");
                return fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this }) as JavaScriptArray;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsExtensible
        {
            get
            {
                var eng = GetContext();

                bool result;
                Errors.ThrowIfIs(m_api.JsGetExtensionAllowed(m_handle, out result));

                return result;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public JavaScriptObject Prototype
        {
            get
            {
                var eng = GetContext();

                JavaScriptValueSafeHandle handle;
                Errors.ThrowIfIs(m_api.JsGetPrototype(m_handle, out handle));

                return eng.CreateObjectFromHandle(handle);
            }
            set
            {
                var eng = GetContext();
                if (value == null)
                    value = eng.NullValue;

                Errors.ThrowIfIs(m_api.JsSetPrototype(m_handle, value.m_handle));
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public object ExternalObject
        {
            get
            {
                var eng = GetContext();
                return eng.GetExternalObjectFrom(this);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsSealed
        {
            get
            {
                var eng = GetContext();
                var fn = GetObjectBuiltinFunction("isSealed", "Object.isSealed");

                return eng.Converter.ToBoolean(fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this }));
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsFrozen
        {
            get
            {
                var eng = GetContext();
                var fn = GetObjectBuiltinFunction("isFrozen", "Object.isFrozen");

                return eng.Converter.ToBoolean(fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this }));
            }
        }

        internal JavaScriptFunction GetBuiltinFunctionProperty(string functionName, string nameIfNotFound)
        {
            var fn = GetPropertyByName(functionName) as JavaScriptFunction;
            if (fn == null)
                Errors.ThrowIOEFmt(Errors.DefaultFnOverwritten, nameIfNotFound);

            return fn;
        }

        internal JavaScriptFunction GetObjectBuiltinFunction(string functionName, string nameIfNotFound)
        {
            var eng = GetContext();
            var obj = eng.GlobalObject.GetPropertyByName("Object") as JavaScriptFunction;
            if (obj == null)
                Errors.ThrowIOEFmt(Errors.DefaultFnOverwritten, "Object");
            var fn = obj.GetPropertyByName(functionName) as JavaScriptFunction;
            if (fn == null)
                Errors.ThrowIOEFmt(Errors.DefaultFnOverwritten, nameIfNotFound);

            return fn;
        }

        public bool IsPrototypeOf(JavaScriptObject other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var eng = GetContext();
            var fn = GetBuiltinFunctionProperty("isPrototypeOf", "Object.prototype.isPrototypeOf");

            var args = new List<JavaScriptValue>() { this, other };

            return eng.Converter.ToBoolean(fn.Invoke(args));
        }

        public bool PropertyIsEnumerable(string propertyName)
        {
            var eng = GetContext();
            var fn = GetBuiltinFunctionProperty("propertyIsEnumerable", "Object.prototype.propertyIsEnumerable");
            using (var jsPropName = eng.Converter.FromString(propertyName))
            {
                var args = new List<JavaScriptValue>() { this, jsPropName };
                return eng.Converter.ToBoolean(fn.Invoke(args));
            }
        }

        public JavaScriptValue GetPropertyByName(string propertyName)
        {
            var eng = GetContext();

            using (var propertyId = JavaScriptPropertyIdSafeHandle.FromString(propertyName))
            {
                JavaScriptValueSafeHandle propertyHandle;
                Errors.ThrowIfIs(m_api.JsGetProperty(m_handle, propertyId, out propertyHandle));

                return eng.CreateValueFromHandle(propertyHandle);
            }
        }

        public void SetPropertyByName(string propertyName, JavaScriptValue value)
        {
            var eng = GetContext();

            using (var propertyId = JavaScriptPropertyIdSafeHandle.FromString(propertyName))
            { 
                Errors.ThrowIfIs(m_api.JsSetProperty(m_handle, propertyId, value.m_handle, false));
            }
        }


        public bool DeletePropertyByName(string propertyName)
        {
            var eng = GetContext();

            using (var propertyId = JavaScriptPropertyIdSafeHandle.FromString(propertyName))
            {
                JavaScriptValueSafeHandle result;
                Errors.ThrowIfIs(m_api.JsDeleteProperty(m_handle, propertyId, false, out result));

                try
                {
                    bool bResult;
                    Errors.ThrowIfIs(m_api.JsBooleanToBool(result, out bResult));
                    return bResult;
                }
                finally
                {
                    result.Dispose();
                }
            }
        }

        public JavaScriptValue this[string name]
        {
            get
            {
                return GetPropertyByName(name);
            }
            set
            {
                SetPropertyByName(name, value);
            }
        }

        public JavaScriptValue GetPropertyBySymbol(JavaScriptSymbol symbol)
        {
            var eng = GetContext();

            JavaScriptPropertyIdSafeHandle propId;
            Errors.ThrowIfIs(m_api.JsGetPropertyIdFromSymbol(symbol.m_handle, out propId));

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(m_api.JsGetProperty(m_handle, propId, out resultHandle));

            return eng.CreateValueFromHandle(resultHandle);
        }

        public void SetPropertyBySymbol(JavaScriptSymbol symbol, JavaScriptValue value)
        {
            var eng = GetContext();

            JavaScriptPropertyIdSafeHandle propId;
            Errors.ThrowIfIs(m_api.JsGetPropertyIdFromSymbol(symbol.m_handle, out propId));
            Errors.ThrowIfIs(m_api.JsSetProperty(m_handle, propId, value.m_handle, false));
        }

        public void DeletePropertyBySymbol(JavaScriptSymbol symbol)
        {
            var eng = GetContext();

            JavaScriptPropertyIdSafeHandle propId;
            Errors.ThrowIfIs(m_api.JsGetPropertyIdFromSymbol(symbol.m_handle, out propId));

            JavaScriptValueSafeHandle tmpResult;
            Errors.ThrowIfIs(m_api.JsDeleteProperty(m_handle, propId, false, out tmpResult));
            tmpResult.Dispose();
        }

        public JavaScriptValue this[JavaScriptSymbol symbol]
        {
            get
            {
                return GetPropertyBySymbol(symbol);
            }
            set
            {
                SetPropertyBySymbol(symbol, value);
            }
        }

        public JavaScriptValue GetValueAtIndex(JavaScriptValue index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            var eng = GetContext();
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfIs(m_api.JsGetIndexedProperty(m_handle, index.m_handle, out result));

            return eng.CreateValueFromHandle(result);
        }

        public void SetValueAtIndex(JavaScriptValue index, JavaScriptValue value)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            var eng = GetContext();
            if (value == null)
                value = eng.NullValue;

            Errors.ThrowIfIs(m_api.JsSetIndexedProperty(m_handle, index.m_handle, value.m_handle));
        }

        public void DeleteValueAtIndex(JavaScriptValue index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            Errors.ThrowIfIs(m_api.JsDeleteIndexedProperty(m_handle, index.m_handle));
        }

        public JavaScriptValue this[JavaScriptValue index]
        {
            get
            {
                return GetValueAtIndex(index);
            }
            set
            {
                SetValueAtIndex(index, value);
            }
        }

        public bool HasOwnProperty(string propertyName)
        {
            var eng = GetContext();
            var fn = GetBuiltinFunctionProperty("hasOwnProperty", "Object.prototype.hasOwnProperty");

            return eng.Converter.ToBoolean(fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromString(propertyName) }));
        }

        public bool HasProperty(string propertyName)
        {
            var eng = GetContext();

            using (var propertyId = JavaScriptPropertyIdSafeHandle.FromString(propertyName))
            {
                bool result;
                Errors.ThrowIfIs(m_api.JsHasProperty(m_handle, propertyId, out result));
                return result;
            }
        }

        public JavaScriptObject GetOwnPropertyDescriptor(string propertyName)
        {
            var m_apiWin = m_api as IChakraCommonWindows;
            if (m_apiWin == null)
                throw new InvalidOperationException("This operation only works on windows.");

            var eng = GetContext();
            JavaScriptPropertyIdSafeHandle propId;
            Errors.ThrowIfIs(m_apiWin.JsGetPropertyIdFromName(propertyName, out propId));
            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(m_api.JsGetOwnPropertyDescriptor(m_handle, propId, out resultHandle));

            return eng.CreateObjectFromHandle(resultHandle);
        }

        public void DefineProperty(string propertyName, JavaScriptObject descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            var m_apiWin = m_api as IChakraCommonWindows;
            if (m_apiWin == null)
                throw new InvalidOperationException("This operation only works on windows.");

            var eng = GetContext();

            JavaScriptPropertyIdSafeHandle propId;
            Errors.ThrowIfIs(m_apiWin.JsGetPropertyIdFromName(propertyName, out propId));

            bool wasSet;
            Errors.CheckForScriptExceptionOrThrow(m_api.JsDefineProperty(m_handle, propId, descriptor.m_handle, out wasSet), eng);
        }

        public void DefineProperties(JavaScriptObject propertiesContainer)
        {
            var eng = GetContext();
            var fnDP = GetObjectBuiltinFunction("defineProperties", "Object.defineProperties");

            fnDP.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this, propertiesContainer });
        }

        public JavaScriptArray GetOwnPropertyNames()
        {
            var eng = GetContext();

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(m_api.JsGetOwnPropertyNames(m_handle, out resultHandle));

            return eng.CreateArrayFromHandle(resultHandle);
        }

        public JavaScriptArray GetOwnPropertySymbols()
        {
            var eng = GetContext();

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(m_api.JsGetOwnPropertySymbols(m_handle, out resultHandle));

            return eng.CreateArrayFromHandle(resultHandle);
        }

        public void PreventExtensions()
        {
            Errors.ThrowIfIs(m_api.JsPreventExtension(m_handle));
        }

        public void Seal()
        {
            var eng = GetContext();
            var fn = GetObjectBuiltinFunction("seal", "Object.seal");

            fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this });
        }

        public void Freeze()
        {
            var eng = GetContext();
            var fn = GetObjectBuiltinFunction("freeze", "Object.freeze");

            fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this });
        }

        #region DynamicObject overrides
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var fn = GetPropertyByName(binder.Name) as JavaScriptFunction;
            var eng = GetContext();
            var c = eng.Converter;

            if (fn != null)
            {
                result = fn.Invoke(args.Select(a => c.FromObject(a)));
                return true;
            }
            return base.TryInvokeMember(binder, args, out result);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length > 1)
                return base.TryGetIndex(binder, indexes, out result);

            var eng = GetContext();
            var jsIndex = eng.Converter.FromObject(indexes[0]);
            result = GetValueAtIndex(jsIndex);
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length > 1)
                return base.TrySetIndex(binder, indexes, value);

            var eng = GetContext();
            var jsIndex = eng.Converter.FromObject(indexes[0]);
            var jsVal = eng.Converter.FromObject(value);
            SetValueAtIndex(jsIndex, jsVal);

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetPropertyByName(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var jsVal = GetContext().Converter.FromObject(value);
            SetPropertyByName(binder.Name, jsVal);

            return true;
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            DeletePropertyByName(binder.Name);
            return true;
        }

        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            if (indexes.Length > 1)
                return base.TryDeleteIndex(binder, indexes);

            var jsIndex = GetContext().Converter.FromObject(indexes[0]);
            DeleteValueAtIndex(jsIndex);

            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Keys.Select(v => v.ToString());
        }
        #endregion

    }
}
