namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;

    public class JsObject : JsValue
    {
        private readonly IBaristaValueService m_baristaValueService;

        public JsObject(IJavaScriptEngine engine, BaristaContext context, IBaristaValueService valueService, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
            m_baristaValueService = valueService ?? throw new ArgumentNullException(nameof(valueService));
        }

        #region Indexers
        public JsValue this[int index]
        {
            get
            {
                return GetProperty(index);
            }
            set
            {
                SetProperty(index, value);
            }
        }

        public JsValue this[string propertyName]
        {
            get
            {
                return GetProperty(propertyName);
            }
            set
            {
                SetProperty(propertyName, value);
            }
        }

        public JsValue this[JsSymbol symbol]
        {
            get
            {
                return GetProperty(symbol);
            }
            set
            {
                SetProperty(symbol, value);
            }
        }

        public JsValue this[JsValue index]
        {
            get
            {
                return GetProperty(index);
            }
            set
            {
                SetProperty(index, value);
            }
        }
        #endregion

        #region Properties
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public JsArray Keys
        {
            get
            {
                var fn = Context.GlobalObject.SelectValue<JsFunction>("Object.keys");
                return fn.Call(new JsValue[] { Context.Undefined, this }) as JsArray;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsExtensible
        {
            get
            {
                return Engine.JsGetExtensionAllowed(Handle);
            }
        }

        public override JavaScriptValueType Type
        {
            get { return JavaScriptValueType.Object; }
        }

        /// <summary>
        /// Gets the value service associated with the value.
        /// </summary>
        public IBaristaValueService ValueService
        {
            get { return m_baristaValueService; }
        }
        #endregion

        #region Delete Property Methods
        /// <summary>
        /// Delete the value at the specified index of the object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="useStrictRules"></param>
        public void DeleteProperty(int index)
        {
            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            var indexHandle = ValueService.CreateNumber(index);
            Engine.JsDeleteIndexedProperty(Handle, indexHandle.Handle);
        }

        /// <summary>
        /// Delete the specified property name of the object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="useStrictRules"></param>
        public void DeleteProperty(string propertyName, bool useStrictRules = false)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            using (var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length))
            {
                Engine.JsDeleteProperty(Handle, propertyIdHandle, useStrictRules);
            }
        }

        /// <summary>
        /// Delete the specified property name of the object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="useStrictRules"></param>
        public void DeleteProperty(JsSymbol symbol, bool useStrictRules = false)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            using (var propertyIdHandle = Engine.JsGetPropertyIdFromSymbol(symbol.Handle))
            {
                Engine.JsDeleteProperty(Handle, propertyIdHandle, useStrictRules);
            }
        }

        /// <summary>
        /// Delete the value at the specified index of the object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="useStrictRules"></param>
        public void DeleteProperty(JsValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            Engine.JsDeleteIndexedProperty(Handle, value.Handle);
        }
        #endregion

        #region Get Property Methods
        /// <summary>
        /// Returns the value of the specified JavaScript property. An active execution scope is required.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public JsValue GetProperty(string propertyName)
        {
            var valueHandle = GetPropertyByNameInternal(propertyName);
            return ValueService.CreateValue(valueHandle);
        }

        public T GetProperty<T>(string propertyName)
            where T : JsValue
        {
            var valueHandle = GetPropertyByNameInternal(propertyName);
            return ValueService.CreateValue<T>(valueHandle);
        }

        private JavaScriptValueSafeHandle GetPropertyByNameInternal(string propertyName)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            using (var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length))
            {
                return Engine.JsGetProperty(Handle, propertyIdHandle);
            }
        }

        /// <summary>
        /// Returns the value of the specified JavaScript property. An active execution scope is required.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public JsValue GetProperty(int index)
        {
            var valueHandle = GetPropertyByIndexInternal(index);
            return ValueService.CreateValue(valueHandle);
        }

        public T GetProperty<T>(int index)
            where T : JsValue
        {
            var valueHandle = GetPropertyByIndexInternal(index);
            return ValueService.CreateValue<T>(valueHandle);
        }

        private JavaScriptValueSafeHandle GetPropertyByIndexInternal(int index)
        {
            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            var indexHandle = ValueService.CreateNumber(index);
            return Engine.JsGetIndexedProperty(Handle, indexHandle.Handle);
        }

        /// <summary>
        /// Returns the value of the specified JavaScript property using the specified symbol. An active execution scope is required.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public JsValue GetProperty(JsSymbol symbol)
        {
            var valueHandle = GetPropertyBySymbolInternal(symbol);
            return ValueService.CreateValue(valueHandle);
        }

        public T GetProperty<T>(JsSymbol symbol)
            where T : JsValue
        {
            var valueHandle = GetPropertyBySymbolInternal(symbol);
            return ValueService.CreateValue<T>(valueHandle);
        }

        private JavaScriptValueSafeHandle GetPropertyBySymbolInternal(JsSymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            using (var propertyIdHandle = Engine.JsGetPropertyIdFromSymbol(symbol.Handle))
            {
                return Engine.JsGetProperty(Handle, propertyIdHandle);
            }
        }

        /// <summary>
        /// Returns the value of the specified JavaScript property using the specified value. An active execution scope is required.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public JsValue GetProperty(JsValue value)
        {
            var valueHandle = GetPropertyByValueInternal(value);
            return ValueService.CreateValue(valueHandle);
        }

        public T GetProperty<T>(JsValue value)
            where T : JsValue
        {
            var valueHandle = GetPropertyByValueInternal(value);
            return ValueService.CreateValue<T>(valueHandle);
        }

        private JavaScriptValueSafeHandle GetPropertyByValueInternal(JsValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            return Engine.JsGetIndexedProperty(Handle, value.Handle);
        }
        #endregion

        #region HasProperty
        public bool HasProperty(string propertyName)
        {
            using (var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length))
            {
                return Engine.JsHasProperty(Handle, propertyIdHandle);
            }
        }

        public bool HasOwnProperty(string propertyName)
        {
            using (var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length))
            {
                return Engine.JsHasOwnProperty(Handle, propertyIdHandle);
            }
        }

        #endregion

        #region Select Value Methods
        public JsValue SelectValue(string path, bool errorWhenNoMatch = false)
        {
            var valueHandle = SelectValueInternal(path, errorWhenNoMatch);
            return ValueService.CreateValue(valueHandle);
        }

        public T SelectValue<T>(string path, bool errorWhenNoMatch = false)
            where T : JsValue
        {
            var valueHandle = SelectValueInternal(path, errorWhenNoMatch);
            return ValueService.CreateValue<T>(valueHandle);
        }

        private JavaScriptValueSafeHandle SelectValueInternal(string path, bool errorWhenNoMatch)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            var currentObject = this;
            var segments = path.Split('.');
            for (int i = 0; i < segments.Length - 1; i++)
            {
                var propertyValue = currentObject.GetProperty(segments[i]);

                if (typeof(JsObject).IsSameOrSubclass(propertyValue.GetType()) == false)
                {
                    if (errorWhenNoMatch == true)
                    {
                        throw new BaristaException($"Index {segments[i]} not valid on {currentObject.ToString()}.");
                    }
                    return null;
                }

                currentObject = (JsObject)propertyValue;
            }

            return currentObject.GetPropertyByNameInternal(segments[segments.Length - 1]);
        }
        #endregion

        #region Set Property Methods
        public void SetProperty<T>(string propertyName, T value, bool useStrictRules = false)
            where T : JsValue
        {
            using (var propertyIdHandle = Engine.JsCreatePropertyId(propertyName, (ulong)propertyName.Length))
            {
                var propertyHandle = Engine.JsGetProperty(Handle, propertyIdHandle);
                Engine.JsSetProperty(Handle, propertyIdHandle, value.Handle, useStrictRules);
            }
        }

        public void SetProperty<T>(int index, T value)
            where T : JsValue
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            var indexHandle = ValueService.CreateNumber(index);
            Engine.JsSetIndexedProperty(Handle, indexHandle.Handle, value.Handle);
        }

        public void SetProperty<T>(JsSymbol symbol, T value, bool useStrictRules = false)
            where T : JsValue
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            using (var propertyIdHandle = Engine.JsGetPropertyIdFromSymbol(symbol.Handle))
            {
                var propertyHandle = Engine.JsGetProperty(Handle, propertyIdHandle);
                Engine.JsSetProperty(Handle, propertyIdHandle, value.Handle, useStrictRules);
            }
        }

        public void SetProperty<T>(JsValue index, T propertyValue)
            where T : JsValue
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            if (propertyValue == null)
                throw new ArgumentNullException(nameof(propertyValue));

            if (!Context.HasCurrentScope)
                throw new InvalidOperationException("An active execution scope is required.");

            Engine.JsSetIndexedProperty(Handle, index.Handle, propertyValue.Handle);
        }
        #endregion

        #region Dynamic Object Overrides
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Keys.Select(propertyName => propertyName.ToString());
        }

        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            if (indexes.Length != 1)
                return base.TryDeleteIndex(binder, indexes);

            if (indexes[0] is string propertyName)
            {
                DeleteProperty(propertyName);
                return true;
            }
            else if (indexes[0] is int index)
            {
                DeleteProperty(index);
                return true;
            }
            else if (indexes[0] is JsSymbol symbol)
            {
                DeleteProperty(symbol);
                return true;
            }
            else if (Context.Converter.TryFromObject(ValueService, indexes[0], out JsValue jsIndex))
            {
                DeleteProperty(jsIndex);
                return true;
            }

            return base.TryDeleteIndex(binder, indexes);
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            DeleteProperty(binder.Name);
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length != 1)
                return base.TryGetIndex(binder, indexes, out result);

            if (indexes[0] is string propertyName)
            {
                result = GetProperty(propertyName);
                return true;
            }
            else if (indexes[0] is int index)
            {
                result = GetProperty(index);
                return true;
            }
            else if (indexes[0] is JsSymbol symbol)
            {
                result = GetProperty(symbol);
                return true;
            }
            else if (Context.Converter.TryFromObject(ValueService, indexes[0], out JsValue jsIndex))
            {
                result = GetProperty(jsIndex);
                return true;
            }

            return base.TryGetIndex(binder, indexes, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetProperty(binder.Name);
            return result is JsUndefined ? false : true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (GetProperty(binder.Name) is JsFunction fn)
            {
                if (Context.Converter.TryFromObject(ValueService, args, out JsValue jsArguments))
                {
                    result = fn.Call(jsArguments);
                    return true;
                }
            }

            return base.TryInvokeMember(binder, args, out result);
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length != 1)
                return base.TrySetIndex(binder, indexes, value);

            if (Context.Converter.TryFromObject(ValueService, value, out JsValue jsValue))
            {

                if (indexes[0] is string propertyName)
                {
                    SetProperty(propertyName, jsValue);
                    return true;
                }
                else if (indexes[0] is int index)
                {
                    SetProperty(index, jsValue);
                    return true;
                }
                else if (indexes[0] is JsSymbol symbol)
                {
                    SetProperty(symbol, jsValue);
                    return true;
                }
                else if (Context.Converter.TryFromObject(ValueService, indexes[0], out JsValue jsIndex))
                {
                    SetProperty(jsIndex, jsValue);
                    return true;
                }
            }

            return base.TrySetIndex(binder, indexes, value);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (Context.Converter.TryFromObject(ValueService, value, out JsValue jsValue))
            {
                SetProperty(binder.Name, jsValue);
                return true;
            }
            return base.TrySetMember(binder, value);
        }
        #endregion
    }
}
